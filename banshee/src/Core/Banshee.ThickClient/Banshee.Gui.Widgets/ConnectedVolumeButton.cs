//
// ConnectedVolumeButton.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2007 Novell, Inc.
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

using Banshee.MediaEngine;
using Banshee.ServiceStack;

namespace Banshee.Gui.Widgets
{
    public class ConnectedVolumeButton : Gtk.VolumeButton
    {
        public ConnectedVolumeButton () : base()
        {
            // PlayerEngine uses a volume range of 0 to 100
            Adjustment.SetBounds (0.0, 100.0, 5.0, 20.0, 0.0);

            var player = ServiceManager.PlayerEngine;

            if (player.ActiveEngine != null && player.ActiveEngine.IsInitialized) {
                SetVolume ();
            } else {
                Sensitive = false;
                player.EngineAfterInitialize += (e) => {
                    Hyena.ThreadAssist.ProxyToMain (delegate {
                        SetVolume ();
                        Sensitive = true;
                    });
                };
            }

            player.ConnectEvent (OnPlayerEvent, PlayerEvent.Volume);

            // On OS X we have no hardware mixer but use a software volume mixer in
            // the gstreamer pipeline which resets the volume to 1.0 (max) upon each
            // uri/track change, so we have to set the volume from the widget
            if (Hyena.PlatformDetection.IsMac) {
                player.ConnectEvent ((args) => {
                    ServiceManager.PlayerEngine.Volume = (ushort)Value;
                }, PlayerEvent.StartOfStream);
            }

            this.ValueChanged += (o, args) => {
                ServiceManager.PlayerEngine.Volume = (ushort)Value;
            };

            // We need to know whether the volume scale is being used
            this.Pressed += (o, args) => { active = true; };
            this.Released += (o, args) => { active = false; };
        }

        public void Disconnect ()
        {
            ServiceManager.PlayerEngine.DisconnectEvent (OnPlayerEvent);
        }

        private bool active = false;
        public bool Active { get { return active; } }

        private void OnPlayerEvent (PlayerEventArgs args)
        {
            SetVolume ();
        }

        private void SetVolume ()
        {
            Value = ServiceManager.PlayerEngine.Volume;
        }
    }
}
