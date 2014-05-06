//
// DvdService.cs
//
// Author:
//   Alex Launi <alex.launi@canonical.com>
//
// Copyright 2010 Alex Launi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

using Banshee.Hardware;
using Banshee.Gui;
using Banshee.ServiceStack;
using Mono.Unix;
using Hyena;

namespace Banshee.OpticalDisc.Dvd
{
    public class DvdService : DiscService, IService
    {
        private uint global_interface_id;
        
        public DvdService ()
        {
        }

        protected override bool DeviceCommandMatchesSource (DiscSource source, DeviceCommand command)
        {
            DvdSource dvdSource = source as DvdSource;

            if (dvdSource != null && command.DeviceId.StartsWith ("dvd:")) {
                try {
                    Uri uri = new Uri (command.DeviceId);
                    string match_device_node = String.Format ("{0}{1}", uri.Host,
                        uri.AbsolutePath).TrimEnd ('/', '\\');
                    string device_node = source.DiscModel.Volume.DeviceNode;
                    return device_node.EndsWith (match_device_node);
                } catch {
                }
            }

            return false;
        }

        public override void Initialize()
        {
            lock (this) {
                base.Initialize ();
                SetupActions ();
            }

        }

        public override void Dispose ()
        {
            lock (this) {
                base.Dispose ();
                DisposeActions ();
            }
        }

#region UI Actions

        private void SetupActions ()
        {
            InterfaceActionService uia_service = ServiceManager.Get<InterfaceActionService> ();
            if (uia_service == null) {
                return;
            }

            uia_service.GlobalActions.AddImportant (new Gtk.ActionEntry [] {
                new Gtk.ActionEntry ("GoToMenuAction", null,
                    Catalog.GetString ("Go to Menu"), null,
                    Catalog.GetString ("Navigate to menu"),
                    (object o, EventArgs args) => { ServiceManager.PlayerEngine.NavigateToMenu (); })
            });

            global_interface_id = uia_service.UIManager.AddUiFromResource ("GlobalUI_Dvd.xml");
        }

        private void DisposeActions ()
        {
            InterfaceActionService uia_service = ServiceManager.Get<InterfaceActionService> ();
            if (uia_service == null) {
                return;
            }

            uia_service.GlobalActions.Remove ("GoToMenuAction");
            uia_service.UIManager.RemoveUi (global_interface_id);
        }

#endregion

#region implemented abstract members of Banshee.OpticalDisc.DiscService

        protected override DiscSource GetDiscSource (IDiscVolume volume)
        {
            if (volume.HasVideo) {
                Log.Debug ("Mapping dvd");
                return new DvdSource (this, new DvdModel (volume));
            } else {
                Log.Debug ("Can not map to dvd source.");
                return null;
            }
        }
        
#endregion

        string IService.ServiceName {
            get { return "DvdService"; }
        }
    }
}

