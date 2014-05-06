//
// UPnPClientSource.cs
//
// Authors:
//   Tobias 'topfs2' Arrskog <tobias.arrskog@gmail.com>
//
// Copyright (C) 2011 Tobias 'topfs2' Arrskog
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
using System.Collections.Generic;

using Mono.Upnp;

using Hyena;

using Banshee.ServiceStack;

namespace Banshee.UPnPClient
{
    public class UPnPService : IExtensionService, IDisposable, IDelayedInitializeService
    {
        private Mono.Upnp.Client client;
        private UPnPContainerSource container;

        private Dictionary<string, UPnPServerSource> source_map;

        void IExtensionService.Initialize ()
        {
        }

        public void DelayedInitialize ()
        {
            source_map = new Dictionary<string, UPnPServerSource> ();
            container = new UPnPContainerSource ();

            client = new Mono.Upnp.Client ();
            client.DeviceAdded += DeviceAdded;
            client.DeviceRemoved += DeviceRemoved;

            client.BrowseAll ();
        }
    
        public void Dispose ()
        {
            if (client != null) {
                client.DeviceAdded -= DeviceAdded;
                client.DeviceRemoved -= DeviceRemoved;
                client.Dispose ();
            }

            if (source_map != null) {
                foreach (var kv in source_map) {
                    if (kv.Value != null) {
                        kv.Value.Disconnect ();
                    }
                }

                source_map.Clear ();
            }

            if (container != null) {
                foreach (UPnPServerSource source in container.Children) {
                    source.Disconnect ();
                }

                ServiceManager.SourceManager.RemoveSource (container);
                container = null;
            }
        }

        void DeviceAdded (object sender, DeviceEventArgs e)
        {
            Log.DebugFormat ("UPnPService.DeviceAdded ({0}) {1}", e.Device.Type, e.Device.Udn);
            Device device = e.Device.GetDevice ();

            if (device.Type.Type == "MediaServer") {
                Log.DebugFormat ("UPnPService MediaServer Found: {0} {1}", device.ModelName, device.ModelNumber);
                UPnPServerSource source = new UPnPServerSource (device);

                string key = device.Udn;
                if (source_map.Count == 0) {
                    ThreadAssist.ProxyToMain (delegate {
                        ServiceManager.SourceManager.AddSource (container);
                    });
                }

                if (source_map.ContainsKey (key)) {
                    // Received new connection info for service
                    container.RemoveChildSource (source_map [key]);
                    source_map [key] = source;
                } else {
                    // New service information
                    source_map.Add (key, source);
                }

                container.AddChildSource (source);
            }
        }

        void DeviceRemoved (object sender, DeviceEventArgs e)
        {
            Log.DebugFormat ("UPnPService.DeviceRemoved ({0}) {1}", e.Device.Type, e.Device.Udn);

            // We can't use e.Device.GetDevice () here, because the device might already be disposed
            if (e.Device.Type.Type == "MediaServer") {
                Log.DebugFormat ("UPnPService MediaServer Removed: {0} {1}", e.Device.Type, e.Device.Udn);
                String key = e.Device.Udn;
                UPnPServerSource source = source_map [key];

                source.Disconnect ();
                container.RemoveChildSource (source);
                source_map.Remove (key);

                if (source_map.Count == 0) {
                    ServiceManager.SourceManager.RemoveSource (container);
                }
            }
        }

        string IService.ServiceName {
            get { return "uPnP Client service"; }
        }
    }
}
