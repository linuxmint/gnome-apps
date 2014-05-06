//
// BpmDetector.cs
//
// Author:
//   Gabriel Burt <gburt@novell.com>
//
// Copyright (C) 2008-2011 Novell, Inc.
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

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Mono.Unix;

using Gst;

using Hyena;
using Hyena.Data;

using Banshee.Base;
using Banshee.Streaming;
using Banshee.MediaEngine;
using Banshee.ServiceStack;
using Banshee.Configuration;
using Banshee.Preferences;
using Gst.CorePlugins;
using Gst.BasePlugins;

namespace Banshee.GStreamerSharp
{
    public class BpmDetector : IBpmDetector
    {
        SafeUri current_uri;
        Dictionary<int, int> bpm_histogram = new Dictionary<int, int> ();

        Pipeline pipeline;
        FileSrc filesrc;
        FakeSink fakesink;

        public event BpmEventHandler FileFinished;

        public BpmDetector ()
        {
            try {
                pipeline = new Pipeline ();
                filesrc          = new FileSrc ();
                var decodebin    = new DecodeBin2 ();
                var audioconvert = Make ("audioconvert");
                var bpmdetect    = Make ("bpmdetect");
                fakesink         = new FakeSink ();

                pipeline.Add (filesrc, decodebin, audioconvert, bpmdetect, fakesink);

                if (!filesrc.Link (decodebin)) {
                    Log.Error ("Could not link pipeline elements");
                    throw new Exception ();
                }

                // decodebin and audioconvert are linked dynamically when the decodebin creates a new pad
                decodebin.NewDecodedPad += delegate(object o, DecodeBin2.NewDecodedPadArgs args) {
                    var audiopad = audioconvert.GetStaticPad ("sink");
                    if (audiopad.IsLinked) {
                        return;
                    }

                    using (var caps = args.Pad.Caps) {
                        using (var str = caps[0]) {
                            if (!str.Name.Contains ("audio"))
                                return;
                        }
                    }

                    args.Pad.Link (audiopad);
                };

                if (!Element.Link (audioconvert, bpmdetect, fakesink)) {
                    Log.Error ("Could not link pipeline elements");
                    throw new Exception ();
                }

                pipeline.Bus.AddWatch (OnBusMessage);
                //gst_bus_add_watch (gst_pipeline_get_bus (GST_PIPELINE (detector->pipeline)), bbd_pipeline_bus_callback, detector);
            } catch (Exception e) {
                Log.Exception (e);
                throw new ApplicationException (Catalog.GetString ("Could not create BPM detection driver."), e);
            }
        }

        private bool OnBusMessage (Bus bus, Message msg)
        {
            switch (msg.Type) {
            case MessageType.Tag:
                Pad pad;
                TagList tag_list;
                msg.ParseTag (out pad, out tag_list);

                foreach (var name in tag_list.Tags) {
                    if (name == "beats-per-minute") {
                        if (tag_list.GetTagSize (name) < 1) continue;
                        var tag = tag_list.GetTag (name);
                        foreach (var val in tag) {
                            if (val is double) {
                                double bpm = (double)val;
                                int rounded = (int) Math.Round (bpm);
                                if (!bpm_histogram.ContainsKey(rounded)) {
                                    bpm_histogram[rounded] = 1;
                                } else {
                                    bpm_histogram[rounded]++;
                                }
                            }
                            break;
                        }
                    }
                }

                tag_list.Dispose ();
                break;

            case MessageType.Error:
                Enum error_type;
                string err_msg, debug;
                msg.ParseError (out error_type, out err_msg, out debug);

                IsDetecting = false;
                Log.ErrorFormat ("BPM Detection error", err_msg);
                break;

            case MessageType.Eos:
                IsDetecting = false;
                pipeline.SetState (State.Null);

                SafeUri uri = current_uri;
                int best_bpm = -1, best_bpm_count = 0;
                foreach (int bpm in bpm_histogram.Keys) {
                    int count = bpm_histogram[bpm];
                    if (count > best_bpm_count) {
                        best_bpm_count = count;
                        best_bpm = bpm;
                    }
                }

                Reset ();

                var handler = FileFinished;
                if (handler != null) {
                    handler (this, new BpmEventArgs (uri, best_bpm));
                }

                break;
            }

            return true;
        }

        public void Dispose ()
        {
            Reset ();

            if (pipeline != null) {
                pipeline.SetState (State.Null);
                pipeline.Dispose ();
                pipeline = null;
            }
        }

        public bool IsDetecting { get; private set; }

        public void ProcessFile (SafeUri uri)
        {
            Reset ();
            current_uri = uri;
            string path = uri.LocalPath;

            try {
                Log.DebugFormat ("GStreamer running beat detection on {0}", path);
                IsDetecting = true;
                fakesink.SetState (State.Null);
                filesrc.Location = path;
                pipeline.SetState (State.Playing);
            } catch (Exception e) {
                Log.Exception (e);
            }
        }

        private void Reset ()
        {
            current_uri = null;
            bpm_histogram.Clear ();
        }

        private static Gst.Element Make (string name)
        {
            var e = ElementFactory.Make (name);
            if (e == null) {
                Log.ErrorFormat ("BPM Detector unable to make element '{0}'", name);
                throw new Exception ();
            }
            return e;
        }
    }
}