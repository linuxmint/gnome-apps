//
// VideoManager.cs
//
// Author:
//  Olivier Dufour <olivier.duff@gmail.com>
//
// Copyright (C) 2011 Olivier Dufour
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
using System.Threading;
using System.Runtime.InteropServices;
using Mono.Unix;

using Hyena;
using Banshee.Base;
using Banshee.Collection;
using Banshee.ServiceStack;
using Banshee.MediaEngine;
using Banshee.MediaProfiles;
using Banshee.Configuration.Schema;

using Gst;
using Gst.BasePlugins;
using Gst.Interfaces;

namespace Banshee.GStreamerSharp
{
    public class VideoManager
    {
        PlayBin2 playbin;
        VideoDisplayContextType video_display_context_type;
        IntPtr video_window;
        ulong video_window_xid;
        XOverlay xoverlay;
        object video_mutex = new object ();

        public VideoManager (PlayBin2 playbin)
        {
            this.playbin = playbin;
        }

        public void Initialize ()
        {
            Element videosink;
            
            if (VideoPipelineSetup != null) {
                videosink = VideoPipelineSetup ();
                if (videosink != null && videosink is Element) {
                    playbin ["video-sink"] = videosink;
                    video_display_context_type = VideoDisplayContextType.Custom;
                    return;
                }
            }

            video_display_context_type = VideoDisplayContextType.GdkWindow;
            
            videosink = ElementFactory.Make ("gconfvideosink", "videosink");
            if (videosink == null) {
                videosink = ElementFactory.Make ("autovideosink", "videosink");
                if (videosink == null) {
                    video_display_context_type = VideoDisplayContextType.Unsupported;
                    videosink = ElementFactory.Make ("fakesink", "videosink");
                    if (videosink != null) {
                        videosink ["sync"] = true;
                    }
                }
            }
            
            playbin ["video-sink"] = videosink;
            
            playbin.Bus.SyncHandler = (bus, message) => {return bus.SyncSignalHandler (message); };
            playbin.Bus.SyncMessage += OnSyncMessage;
                
            if (videosink is Bin) {
                ((Bin)videosink).ElementAdded += OnVideoSinkElementAdded;
            }
            
            if (PrepareWindow != null) {
                PrepareWindow ();
            }
        }
        public delegate void PrepareWindowHandler ();
        public event PrepareWindowHandler PrepareWindow;
        
        public delegate Element VideoPipelineSetupHandler ();
        public event VideoPipelineSetupHandler VideoPipelineSetup;

        public delegate void VideoGeometryHandler (int width, int height, int fps_n, int fps_d, int par_n, int par_d);
        public event VideoGeometryHandler VideoGeometry;

        private void OnSyncMessage (object o, SyncMessageArgs args)
        {
            Message message = args.Message;

            if (message.Type != MessageType.Element)
                return;
            
            if (message.Structure == null || message.Structure.Name != "prepare-xwindow-id") {
                return;
            }

            bool found_xoverlay = FindXOverlay ();

            if (found_xoverlay) {
                xoverlay.XwindowId = video_window_xid;
            }
        }

        private void OnVideoSinkElementAdded (object o, ElementAddedArgs args)
        {
            FindXOverlay ();
        }

        private bool FindXOverlay ()
        {
            Element video_sink = null;
            Element xoverlay_element;
            bool    found_xoverlay;

            video_sink = playbin.VideoSink;

            Monitor.Enter (video_mutex);

            if (video_sink == null) {
                xoverlay = null;
                Monitor.Exit (video_mutex);
                return false;
            }
           
            xoverlay_element = video_sink is Bin
                ? ((Bin)video_sink).GetByInterface (typeof(XOverlay))
                : video_sink;
            
            xoverlay = xoverlay_element as XOverlay;

            if (!PlatformDetection.IsWindows) {
                // We can't rely on aspect ratio from dshowvideosink
                if (xoverlay != null && xoverlay_element.HasProperty ("force-aspect-ratio")) {
                    xoverlay_element ["force-aspect-ratio"] = true;
                }
            }

            if (xoverlay != null && xoverlay_element.HasProperty ("handle-events")) {
                xoverlay_element ["handle-events"] = false;
            }

            found_xoverlay = (xoverlay != null) ? true : false;

            Monitor.Exit (video_mutex);
            return found_xoverlay;
        }

        public void ParseStreamInfo ()
        {
            //int audios_streams;
            int video_streams;
            //int text_streams;
            Pad vpad = null;

            //audios_streams = playbin.NAudio;
            video_streams = playbin.NVideo;
            //text_streams = playbin.NText;

            if (video_streams > 0) {
                int i;
                /* Try to obtain a video pad */
                for (i = 0; i < video_streams && vpad == null; i++) {
                    vpad = playbin.GetVideoPad (i);
                }
            }

            if (vpad != null) {
                Caps caps = vpad.NegotiatedCaps;
                if (caps != null) {
                    OnCapsSet (vpad, null);
                }
                vpad.AddNotification ("caps", OnCapsSet);
            }
        }

        private void OnCapsSet (object o, Gst.GLib.NotifyArgs args)
        {
            Structure s = null;
            int width, height, fps_n, fps_d, par_n, par_d;
            Caps caps = ((Pad)o).NegotiatedCaps;

            width = height = fps_n = fps_d = 0;
            if (caps == null) {
                return;
            }

            /* Get video decoder caps */
            s = caps [0];
            if (s != null) {
                /* We need at least width/height and framerate */
                if (!(s.HasField ("framerate") && s.HasField ("width") && s.HasField ("height"))) {
                    return;
                }
                Fraction f = new Fraction (s.GetValue ("framerate"));
                fps_n = f.Numerator;
                fps_d = f.Denominator;
                Gst.GLib.Value val;
                width = (int)s.GetValue ("width");
                height = (int)s.GetValue ("height");
                /* Get the PAR if available */
                val = s.GetValue ("pixel-aspect-ratio");
                if (!val.Equals (Gst.GLib.Value.Empty)) {
                    Fraction par = new Fraction (val);
                    par_n = par.Numerator;
                    par_d = par.Denominator;
                }
                else { /* Square pixels */
                    par_n = 1;
                    par_d = 1;
                }

                /* Notify PlayerEngine if a callback was set */
                RaiseVideoGeometry (width, height, fps_n, fps_d, par_n, par_d);
            }
        }

        private void RaiseVideoGeometry (int width, int height, int fps_n, int fps_d, int par_n, int par_d)
        {
            if (VideoGeometry != null)
                VideoGeometry (width, height, fps_n, fps_d, par_n, par_d);
        }

        public void WindowExpose (IntPtr window, bool direct)
        {
            if (direct && xoverlay != null) {
                xoverlay.Expose ();
                return;
            }

            if (xoverlay == null && !FindXOverlay ()) {
                return;
            }

            xoverlay.XwindowId = video_window_xid;
            xoverlay.Expose ();
        }

        public void WindowRealize (IntPtr window)
        {
            switch (System.Environment.OSVersion.Platform) {
                case PlatformID.Unix:
                    video_window_xid = (ulong)gdk_x11_drawable_get_xid (window);
                break;
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    video_window_xid = (ulong)gdk_win32_drawable_get_handle (window);
                break;
            }
        }

        [DllImport ("libgdk-x11-2.0.so.0")]
        private static extern IntPtr gdk_x11_drawable_get_xid (IntPtr drawable);
        [DllImport ("libgdk-win32-2.0-0.dll")]
        private static extern IntPtr gdk_win32_drawable_get_handle (IntPtr drawable);

        public VideoDisplayContextType VideoDisplayContextType {
            get { return video_display_context_type; }
        }

        public IntPtr VideoDisplayContext {
            set { 
                if (VideoDisplayContextType == VideoDisplayContextType.GdkWindow) {
                    video_window = value;
                }
            }
            get { 
                if (VideoDisplayContextType == VideoDisplayContextType.GdkWindow) {
                        return video_window;
                    }
                return IntPtr.Zero;
            }
        }
    }
}
