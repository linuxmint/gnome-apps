//
// UsbVolume.cs
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
using System.Collections.Generic;

using MonoMac.Foundation;

using Banshee.Hardware;
using Banshee.Hardware.Osx.LowLevel;

namespace Banshee.Hardware.Osx
{
    public class UsbVolume : Volume, IUsbDevice, IBlockDevice
    {
        public UsbVolume (DeviceArguments arguments) : base (arguments)
        {
            return;
        }

        #region IUsbDevice implementation
        public int ProductId {
            get {
                if (deviceArguments.UsbInfo != null)
                    return (int) deviceArguments.UsbInfo.ProductId;
                else
                    return 0;
            }
        }

        public int VendorId {
            get {
                if (deviceArguments.UsbInfo != null)
                    return (int) deviceArguments.UsbInfo.VendorId;
                else
                    return 0;
            }
        }

        public double Speed {
            get { return 2.0; }
        }

        public double Version {
            get { return 1.0; }
        }
        #endregion

        #region IEnumerable implementation
        public System.Collections.IEnumerator GetEnumerator ()
        {
            throw new System.NotImplementedException ();
        }
        #endregion

        #region IEnumerable implementation
        IEnumerator<IVolume> IEnumerable<IVolume>.GetEnumerator ()
        {
            yield return this;
            throw new System.NotImplementedException ();
        }
        #endregion

        #region IBlockDevice implementation
        public IEnumerable<IVolume> Volumes {
            get {
                yield return this;
            }
        }

        public bool IsRemovable {
            get {
                throw new System.NotImplementedException ();
            }
        }
        #endregion
    }
}
