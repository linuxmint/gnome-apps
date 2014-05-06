//
// Device.cs
//
// Author:
//   Timo Dörr <timo@latecrew.de>
//
// Copyright 2012 Timo Dörr 
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
using System.Security.Cryptography;

using MonoMac.Foundation;

using Banshee.Hardware;
using Banshee.Hardware.Osx.LowLevel;

namespace Banshee.Hardware.Osx
{
    public class Device : IDevice, IComparable, IDisposable
    {
        // this is a low-level NSDictionary the OS X DiskArbitration framework
        // gives us back for any disk devices or volumes and holds ALL information
        // we need for a given device
        protected DeviceArguments deviceArguments;

        public Device (DeviceArguments arguments)
        {
            this.deviceArguments = arguments;

            // copy values from the NSDictionary so we don't rely on it later
            this.vendor = deviceArguments.DeviceProperties.GetStringValue("DADeviceVendor");
            this.uuid = GetUUIDFromProperties (deviceArguments.DeviceProperties);

            this.name = deviceArguments.DeviceProperties.GetStringValue ("DAVolumeName");
            if (string.IsNullOrEmpty (this.name)) {
                this.name = deviceArguments.DeviceProperties.GetStringValue ("DAMediaName");
            }
   
            this.product = deviceArguments.DeviceProperties.GetStringValue("DADeviceModel");
        }

        #region IDevice implementation
        public IUsbDevice ResolveRootUsbDevice ()
        {
            // TODO this should be refactored - devices don't need to be usb devices
            // There's also firewire, thunderbolt, etc.
            if ((this as IUsbDevice) != null)
                return (IUsbDevice) this;
            else {
                // return a fake usb device
                return new UsbDevice (deviceArguments) as IUsbDevice;
            }
        }

        public IUsbPortInfo ResolveUsbPortInfo ()
        {
            return null;
        }

        /// <summary>
        /// Dumps the device details. Mainly usefull for debugging.
        /// </summary>
        public void DumpDeviceDetails ()
        {
            foreach (var key in deviceArguments.DeviceProperties.Keys)
                Console.WriteLine ("{0} => {1}",
                    key.ToString (),
                    deviceArguments.DeviceProperties.GetStringValue (key.ToString ())
                );
        }

        public void Dispose ()
        {}

        protected static string GetUUIDFromProperties (NSDictionary properties)
        {
             // this is somewhat troublesome
             // some devices have a filesystem UUID (i.e. HFS+ formated ones), but most other devices don't.
             // As the different devices/volumes have not really always a key in common, we use different keys
             // depending on the device type, and generate a UUID conforming 16byte value out of it

             string uuid_src = 
                properties.GetStringValue ("DAMediaBSDName") ??
                properties.GetStringValue ("DADevicePath")  ??
                properties.GetStringValue ("DAVolumePath");

            if (string.IsNullOrEmpty (uuid_src)) {
                Hyena.Log.ErrorFormat ("Tried to create a device for which we can't determine a UUID");
                throw new ApplicationException ("Could not determine a UUID for the device");
            }

            // TODO actually transform into a real UUID
            return uuid_src;
        }

        protected string uuid;
        public string Uuid {
            get {
                return uuid;
            }
        }

        protected string serial;
        public string Serial {
            get {
                return "123456789";
            }
        }

        protected string name;
        public string Name {
            get {
                return name;
            }
        }

        protected string product;
        public string Product {
            get {
                return product;
            }
        }

        protected string vendor;
        public string Vendor {
            get {
                return vendor;
            }
        }

        public IDeviceMediaCapabilities MediaCapabilities {
            get {
                return null;
            }
        }
        #endregion

        #region IComparable implementation
        public int CompareTo (object device)
        {
            if (device is IDevice) {
                return this.Uuid.CompareTo (((IDevice) device).Uuid);
            } else {
                throw new ArgumentException ("object is not an IDevice");
            }
        }
        #endregion
    }
}
