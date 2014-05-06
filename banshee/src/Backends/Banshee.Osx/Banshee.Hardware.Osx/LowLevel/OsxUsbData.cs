//
// OsxUsbData.cs
//
// Author:
//   Timo Dörr <timo@latecrew.de>
//
// Copyright (C) 2012 Timo Dörr
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

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

using MonoMac;
using MonoMac.CoreFoundation;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace Banshee.Hardware.Osx.LowLevel
{
    public class OsxUsbData
    {
        public uint VendorId;
        public uint ProductId;
        public string ProductName;
        public string VendorName;

        // not to be confused with the iSerialNumber
        public string UsbSerial;

        public ulong LocationID;

        public OsxUsbData ()
        {}

        internal OsxUsbData (IntPtr registry_entry)
        {
            // 1st approach - get IODeviceTree's parent locationID, then find by location ID
            /*string path = properties.GetStringValue ("DAMediaPath");
            IntPtr entry = IORegistryEntryFromPath (IntPtr.Zero, path);
            CFString s = new CFString ("locationID");
            IntPtr plane = new CFString ("IODeviceTree").Handle;
            IntPtr parent = IntPtr.Zero;
            IORegistryEntryGetParentEntry (entry, "IODeviceTree", out parent);
            if (parent != IntPtr.Zero) {
                IntPtr ptr = IORegistryEntryCreateCFProperty (parent, s.Handle, IntPtr.Zero, 0);
                CFShow (ptr);
            }*/
            // TODO recursive find

            // 2nd approach - walk the tree (which one?) up until we find
            // a idVendor - at worst, up to the root
            IntPtr cf_ref;

            // populate properties from the usb device info

            cf_ref = IOKit.GetUsbProperty (registry_entry, "idVendor");
            if (cf_ref != IntPtr.Zero) {
                Int32 num;
                CoreFoundation.CFNumberGetValue (cf_ref, 3, out num);
                VendorId = (uint) num;
            }

            cf_ref = IOKit.GetUsbProperty (registry_entry, "idProduct");
            if (cf_ref != IntPtr.Zero) {
                Int32 num;
                CoreFoundation.CFNumberGetValue (cf_ref, 3, out num);
                ProductId = (uint) num;
            }

            cf_ref = IOKit.GetUsbProperty (registry_entry, "USB Vendor Name");
            if (cf_ref != IntPtr.Zero) {
                VendorName = new CFString (cf_ref).ToString ();
            }

            cf_ref = IOKit.GetUsbProperty (registry_entry, "USB Product Name");
            if (cf_ref != IntPtr.Zero) {
                ProductName = new CFString (cf_ref).ToString ();
            }

            cf_ref = IOKit.GetUsbProperty (registry_entry, "USB Serial Number");
            if (cf_ref != IntPtr.Zero) {
                UsbSerial = new CFString (cf_ref).ToString ();
            }
        }
    }
}
