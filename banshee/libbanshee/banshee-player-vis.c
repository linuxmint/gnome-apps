//
// banshee-player-vis.c
//
// Author:
//   Chris Howie <cdhowie@gmail.com>
//
// Copyright (C) 2008 Chris Howie
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#include <math.h>
#include <gst/audio/audio.h>

#include "banshee-player-vis.h"

#define SLICE_SIZE 735

static GstStaticCaps vis_data_sink_caps = GST_STATIC_CAPS (
    "audio/x-raw, "
    "format = (string) " GST_AUDIO_NE(F32) ", "
    "rate = (int) 44100, "
    "channels = (int) 2"
);

// ---------------------------------------------------------------------------
// Private Functions
// ---------------------------------------------------------------------------

static void
bp_vis_pcm_handoff (GstElement *sink, GstBuffer *buffer, GstPad *pad, gpointer userdata)
{
    BansheePlayer *player = (BansheePlayer*)userdata;
    GstCaps *caps;
    GstStructure *structure;
    gint channels, wanted_size;
    gfloat *data;
    BansheePlayerVisDataCallback vis_data_cb;
    
    g_return_if_fail (IS_BANSHEE_PLAYER (player));
    
    vis_data_cb = player->vis_data_cb;

    if (vis_data_cb == NULL) {
        return;
    }

    if (player->vis_thawing) {
        // Flush our buffers out.
        gst_adapter_clear (player->vis_buffer);
        memset (player->vis_fft_sample_buffer, 0, sizeof(gfloat) * SLICE_SIZE);

        player->vis_thawing = FALSE;
    }
    
    caps = gst_pad_get_current_caps (pad);
    structure = gst_caps_get_structure (caps, 0);
    gst_structure_get_int (structure, "channels", &channels);
    gst_caps_unref (caps);
    
    wanted_size = channels * SLICE_SIZE * sizeof (gfloat);

    gst_adapter_push (player->vis_buffer, gst_buffer_ref (buffer));
    
    while ((data = (gfloat *)gst_adapter_map (player->vis_buffer, wanted_size)) != NULL) {
        gfloat *deinterlaced = g_malloc (wanted_size);
        gfloat *specbuf = g_new (gfloat, SLICE_SIZE * 2);

        gint i, j;

        memcpy (specbuf, player->vis_fft_sample_buffer, SLICE_SIZE * sizeof(gfloat));
        
        for (i = 0; i < SLICE_SIZE; i++) {
            gfloat avg = 0.0f;

            for (j = 0; j < channels; j++) {
                gfloat sample = data[i * channels + j];

                deinterlaced[j * SLICE_SIZE + i] = sample;
                avg += sample;
            }

            avg /= channels;
            specbuf[i + SLICE_SIZE] = avg;
        }

        memcpy (player->vis_fft_sample_buffer, &specbuf[SLICE_SIZE], SLICE_SIZE * sizeof(gfloat));

        gst_fft_f32_window (player->vis_fft, specbuf, GST_FFT_WINDOW_HAMMING);
        gst_fft_f32_fft (player->vis_fft, specbuf, player->vis_fft_buffer);

        for (i = 0; i < SLICE_SIZE; i++) {
            gfloat val;

            GstFFTF32Complex cplx = player->vis_fft_buffer[i];

            val = cplx.r * cplx.r + cplx.i * cplx.i;
            val /= SLICE_SIZE * SLICE_SIZE;
            val = 10.0f * log10f(val);

            val = (val + 60.0f) / 60.0f;
            if (val < 0.0f)
                val = 0.0f;

            specbuf[i] = val;
        }

        vis_data_cb (player, channels, SLICE_SIZE, deinterlaced, SLICE_SIZE, specbuf);
        
        g_free (deinterlaced);
        g_free (specbuf);

        gst_adapter_unmap (player->vis_buffer);
        gst_adapter_flush (player->vis_buffer, wanted_size);
    }
}

// ---------------------------------------------------------------------------
// Internal Functions
// ---------------------------------------------------------------------------

static GstPadProbeReturn
_bp_vis_pipeline_event_probe (GstPad *pad, GstPadProbeInfo *info, gpointer data)
{
    BansheePlayer *player = (BansheePlayer *) data;
    GstEvent *event;

    if ((info->type & GST_PAD_PROBE_TYPE_EVENT_DOWNSTREAM) == 0)
        return GST_PAD_PROBE_PASS;

    event = GST_EVENT (info->data);
    switch (GST_EVENT_TYPE (event)) {
        case GST_EVENT_FLUSH_START:
        case GST_EVENT_FLUSH_STOP:
        case GST_EVENT_SEEK:
        case GST_EVENT_SEGMENT:
        case GST_EVENT_CUSTOM_DOWNSTREAM:
            player->vis_thawing = TRUE;

        default: break;
    }

    return GST_PAD_PROBE_PASS;
}

void
_bp_vis_pipeline_setup (BansheePlayer *player)
{
    // The basic pipeline we're constructing is:
    // .audiotee ! queue ! audioresample ! audioconvert ! fakesink

    GstElement *fakesink, *converter, *resampler, *audiosinkqueue;
    GstCaps *caps;
    GstPad *teepad;
    GstPad *pad;

    player->vis_buffer = NULL;
    player->vis_fft = gst_fft_f32_new (SLICE_SIZE * 2, FALSE);
    player->vis_fft_buffer = g_new (GstFFTF32Complex, SLICE_SIZE + 1);
    player->vis_fft_sample_buffer = g_new0 (gfloat, SLICE_SIZE);
    
    // Core elements, if something fails here, it's the end of the world
    audiosinkqueue = gst_element_factory_make ("queue", "vis-queue");

    player->vis_event_probe_pad = gst_element_get_static_pad (audiosinkqueue, "sink");
    player->vis_event_probe_id = gst_pad_add_probe (player->vis_event_probe_pad, GST_PAD_PROBE_TYPE_EVENT_DOWNSTREAM, _bp_vis_pipeline_event_probe, player, NULL);

    resampler = gst_element_factory_make ("audioresample", "vis-resample");
    converter = gst_element_factory_make ("audioconvert", "vis-convert");
    fakesink = gst_element_factory_make ("fakesink", "vis-sink");

    if (audiosinkqueue == NULL || resampler == NULL || converter == NULL || fakesink == NULL) {
        bp_debug ("Could not construct visualization pipeline, a fundamental element could not be created");
        return;
    }

    // Keep around the 5 most recent seconds of audio so that when resuming
    // visualization we have something to show right away.
    g_object_set (G_OBJECT (audiosinkqueue),
            "leaky", 2,
            "max-size-buffers", 0,
            "max-size-bytes", 0,
            "max-size-time", GST_SECOND * 5,
            NULL);
    
    g_signal_connect (G_OBJECT (fakesink), "handoff", G_CALLBACK (bp_vis_pcm_handoff), player);

    g_object_set (G_OBJECT (fakesink),
            // This enables the handoff signal.
            "signal-handoffs", TRUE,
            // Synchronize so we see vis at the same time as we hear it.
            "sync", TRUE,
            // Drop buffers if they come in too late.  This is mainly used when
            // thawing the vis pipeline.
            "max-lateness", GST_SECOND / 120,
            // Deliver buffers one frame early.  This allows for rendering
            // time.  (TODO: It would be great to calculate this on-the-fly so
            // we match the rendering time.
            "ts-offset", -GST_SECOND / 60,
            // Don't go to PAUSED when we freeze the pipeline.
            "async", FALSE, NULL);
    
    gst_bin_add_many (GST_BIN (player->audiobin), audiosinkqueue, resampler,
                      converter, fakesink, NULL);
    
    pad = gst_element_get_static_pad (audiosinkqueue, "sink");
    teepad = gst_element_get_request_pad (player->audiotee, "src_%u");
    gst_pad_link (teepad, pad);
    gst_object_unref (GST_OBJECT (teepad));
    gst_object_unref (GST_OBJECT (pad));
    
    gst_element_link_many (audiosinkqueue, resampler, converter, NULL);
    
    caps = gst_static_caps_get (&vis_data_sink_caps);
    gst_element_link_filtered (converter, fakesink, caps);
    gst_caps_unref (caps);
    
    player->vis_buffer = gst_adapter_new ();
    player->vis_resampler = resampler;
    player->vis_thawing = FALSE;
    player->vis_enabled = FALSE;
}

void
_bp_vis_pipeline_destroy (BansheePlayer *player)
{
    if (player->vis_event_probe_pad) {
        gst_pad_remove_probe (player->vis_event_probe_pad, player->vis_event_probe_id);
        gst_object_unref (GST_OBJECT (player->vis_event_probe_pad));
        player->vis_event_probe_pad = NULL;
    }

    if (player->vis_buffer != NULL) {
        gst_object_unref (player->vis_buffer);
        player->vis_buffer = NULL;
    }

    if (player->vis_fft != NULL) {
        gst_fft_f32_free (player->vis_fft);
        player->vis_fft = NULL;
    }

    if (player->vis_fft_buffer != NULL) {
        g_free (player->vis_fft_buffer);
        player->vis_fft_buffer = NULL;
    }

    if (player->vis_fft_sample_buffer != NULL) {
        g_free (player->vis_fft_sample_buffer);
        player->vis_fft_sample_buffer = NULL;
    }

    player->vis_resampler = NULL;
    player->vis_enabled = FALSE;
    player->vis_thawing = FALSE;
}

// ---------------------------------------------------------------------------
// Public Functions
// ---------------------------------------------------------------------------

P_INVOKE void
bp_set_vis_data_callback (BansheePlayer *player, BansheePlayerVisDataCallback cb)
{
    if (player == NULL)
        return;

    player->vis_data_cb = cb;

    player->vis_enabled = cb != NULL;
}
