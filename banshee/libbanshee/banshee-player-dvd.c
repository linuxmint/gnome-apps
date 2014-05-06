//
// banshee-player-dvd.c
//
// Author:
//   Alex Launi <alex.launi@canonical.com>
//
// Copyright (C) 2010 Alex Launi
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

#include "banshee-player-dvd.h"

// ---------------------------------------------------------------------------
// Private Functions
// ---------------------------------------------------------------------------

static void
bp_dvd_on_notify_source (GstElement *playbin, gpointer unknown, BansheePlayer *player)
{
    GstElement *dvd_src = NULL;

    g_return_if_fail (IS_BANSHEE_PLAYER (player));

    if (player->dvd_device == NULL) {
        return;
    }

    g_object_get (playbin, "source", &dvd_src, NULL);
    if (dvd_src == NULL) {
        return;
    }

    if (G_LIKELY (g_object_class_find_property (G_OBJECT_GET_CLASS (dvd_src), "device"))) {
        bp_debug2 ("bp_dvd: setting device property on source (%s)", player->dvd_device);
        g_object_set (dvd_src, "device", player->dvd_device, NULL);
    }

    g_object_unref (dvd_src);
}

// ---------------------------------------------------------------------------
// Internal Functions
// ---------------------------------------------------------------------------

void
_bp_dvd_pipeline_setup (BansheePlayer *player)
{
    if (player != NULL && player->playbin != NULL) {
        g_signal_connect (player->playbin, "notify::source", G_CALLBACK (bp_dvd_on_notify_source), player);
    }
}

void
_bp_dvd_elements_process_message (BansheePlayer *player, GstMessage *message)
{
    g_return_if_fail (IS_BANSHEE_PLAYER (player));
    g_return_if_fail (message != NULL);

    player->is_menu = FALSE;
    // Get available command to know if player is in menu
    GstQuery *query = gst_navigation_query_new_commands();

    guint n_cmds, i;
    if (!player->navigation) {
        _bp_dvd_find_navigation (player);
    }
    if (!(gst_element_query (GST_ELEMENT_CAST (player->navigation), query)
        && gst_navigation_query_parse_commands_length (query, &n_cmds))) {
        gst_query_unref (query);
        return;
    }

    for (i = 0; i < n_cmds; i++) {
        GstNavigationCommand cmd;
        if (gst_navigation_query_parse_commands_nth (query, i, &cmd)) {
            switch (cmd) {
                case GST_NAVIGATION_COMMAND_ACTIVATE:
                case GST_NAVIGATION_COMMAND_LEFT:
                case GST_NAVIGATION_COMMAND_RIGHT:
                case GST_NAVIGATION_COMMAND_UP:
                case GST_NAVIGATION_COMMAND_DOWN:
                    player->is_menu = TRUE;
                    break;
                default:
                    break;
            }
        }
    }

    gst_query_unref (query);
}

gboolean
_bp_dvd_handle_uri (BansheePlayer *player, const gchar *uri)
{
    // Processes URIs like dvd://<track-number>#<device-node> and overrides
    // track transitioning through playbin if playback was already happening
    // from the device node by seeking directly to the track since the disc
    // is already spinning; playbin doesn't handle DVD URIs with device nodes
    // so we have to handle setting the device property through the
    // notify::source signal on playbin

    const gchar *new_dvd_device;

    if (player == NULL || uri == NULL || !g_str_has_prefix (uri, "dvd://")) {
        // Something is hosed or the URI isn't actually DVD
        if (player->dvd_device != NULL) {
            bp_debug2 ("bp_dvd: finished using device (%s)", player->dvd_device);
            g_free (player->dvd_device);
            player->dvd_device = NULL;
        }

        return FALSE;
    }

    // 6 is the size of "dvd://"
    // so we skip this part to only get the device
    new_dvd_device = uri + 6;
    
    if (player->dvd_device == NULL) {
        // If we weren't already playing from a DVD, cache the
        // device and allow playbin to begin playing it
        player->dvd_device = g_strdup (new_dvd_device);
        bp_debug2 ("bp_dvd: storing device node (%s)", player->dvd_device);
        return FALSE;
    }

    if (strcmp (new_dvd_device, player->dvd_device) == 0) {
        bp_debug2 ("bp_dvd: Already playing device (%s)", player->dvd_device);
        return TRUE;
    }

    // We were already playing some DVD, but switched to a different device node, 
    // so unset and re-cache the new device node and allow playbin to do its thing
    bp_debug3 ("bp_dvd: switching devices for DVD playback (from %s, to %s)", player->dvd_device, new_dvd_device);
    g_free (player->dvd_device);
    player->dvd_device = g_strdup (new_dvd_device);

    return FALSE;
}

void _bp_dvd_find_navigation (BansheePlayer *player)
{
    GstElement *video_sink = NULL;
    GstElement *navigation = NULL;
    GstNavigation *previous_navigation;

    previous_navigation = player->navigation;
    g_object_get (player->playbin, "video-sink", &video_sink, NULL);

    if (video_sink == NULL) {
        player->navigation = NULL;
        if (previous_navigation != NULL) {
            gst_object_unref (previous_navigation);
        }
    }

    navigation = GST_IS_BIN (video_sink)
        ? gst_bin_get_by_interface (GST_BIN (video_sink), GST_TYPE_NAVIGATION)
        : video_sink;

    player->navigation = GST_IS_NAVIGATION (navigation) ? GST_NAVIGATION (navigation) : NULL;

    if (previous_navigation != NULL) {
        gst_object_unref (previous_navigation);
    }

    gst_object_unref (video_sink);
}

P_INVOKE gboolean
bp_dvd_is_menu (BansheePlayer *player)
{
    return player->is_menu;
}

P_INVOKE void
bp_dvd_mouse_move_notify (BansheePlayer *player, double x, double y)
{
    if (!player->navigation) {
        _bp_dvd_find_navigation (player);
    }
    if (player->navigation) {
        gst_navigation_send_mouse_event (player->navigation, "mouse-move", 0, x, y);
    }
}

P_INVOKE void
bp_dvd_mouse_button_pressed_notify (BansheePlayer *player, int button, double x, double y)
{
    if (!player->navigation) {
        _bp_dvd_find_navigation (player);
    }
    if (player->navigation) {
        gst_navigation_send_mouse_event (player->navigation, "mouse-button-press", button, x, y);
    }
}

P_INVOKE void
bp_dvd_mouse_button_released_notify (BansheePlayer *player, int button, double x, double y)
{
    if (!player->navigation) {
        _bp_dvd_find_navigation (player);
    }
    if (player->navigation) {
        gst_navigation_send_mouse_event (player->navigation, "mouse-button-release", button, x, y);
    }
}

P_INVOKE void
bp_dvd_left_notify (BansheePlayer *player)
{
    if (!player->navigation) {
        _bp_dvd_find_navigation (player);
    }
    if (player->navigation) {
        gst_navigation_send_command (player->navigation, GST_NAVIGATION_COMMAND_LEFT);
    }
}

P_INVOKE void
bp_dvd_right_notify (BansheePlayer *player)
{
    if (!player->navigation) {
        _bp_dvd_find_navigation (player);
    }
    if (player->navigation) {
        gst_navigation_send_command (player->navigation, GST_NAVIGATION_COMMAND_RIGHT);
    }
}

P_INVOKE void
bp_dvd_up_notify (BansheePlayer *player)
{
    if (!player->navigation) {
        _bp_dvd_find_navigation (player);
    }
    if (player->navigation) {
        gst_navigation_send_command (player->navigation, GST_NAVIGATION_COMMAND_UP);
    }
}

P_INVOKE void
bp_dvd_down_notify (BansheePlayer *player)
{
    if (!player->navigation) {
        _bp_dvd_find_navigation (player);
    }
    if (player->navigation) {
        gst_navigation_send_command (player->navigation, GST_NAVIGATION_COMMAND_DOWN);
    }
}

P_INVOKE void
bp_dvd_activate_notify (BansheePlayer *player)
{
    if (!player->navigation) {
        _bp_dvd_find_navigation (player);
    }
    if (player->navigation) {
        gst_navigation_send_command (player->navigation, GST_NAVIGATION_COMMAND_ACTIVATE);
    }
}

P_INVOKE void
bp_dvd_go_to_menu (BansheePlayer *player)
{
    if (!player->navigation) {
        _bp_dvd_find_navigation (player);
    }
    if (player->navigation) {
        gst_navigation_send_command (player->navigation, GST_NAVIGATION_COMMAND_DVD_MENU);
    }
}

P_INVOKE void
bp_dvd_go_to_next_chapter (BansheePlayer *player)
{
    gint64 index;
    GstFormat format = gst_format_get_by_nick ("chapter");
    gst_element_query_position (player->playbin, format, &index);
    gst_element_seek (player->playbin, 1.0, format, GST_SEEK_FLAG_FLUSH,
        GST_SEEK_TYPE_SET, index + 1, GST_SEEK_TYPE_NONE, 0);
}

P_INVOKE void
bp_dvd_go_to_previous_chapter (BansheePlayer *player)
{
    gint64 index;
    GstFormat format = gst_format_get_by_nick ("chapter");
    gst_element_query_position (player->playbin, format, &index);
    gst_element_seek (player->playbin, 1.0, format, GST_SEEK_FLAG_FLUSH,
        GST_SEEK_TYPE_SET, index - 1, GST_SEEK_TYPE_NONE, 0);
}
