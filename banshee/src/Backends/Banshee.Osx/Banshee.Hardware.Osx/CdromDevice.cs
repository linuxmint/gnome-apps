//
// CdromDevice.cs
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
using System.Collections;
using System.Collections.Generic;

using MonoMac.Foundation;

using Banshee.Hardware;
using Banshee.Hardware.Osx.LowLevel;

namespace Banshee.Hardware.Osx
{
    public class CdromDevice : BlockDevice, ICdromDevice
    {
        public CdromDevice (DeviceArguments arguments) : base (arguments)
        {
        }

        #region ICdromDevice implementation
        public bool LockDoor ()
        {
            return true;
        }

        public bool UnlockDoor ()
        {
            return true;
        }

        public bool IsDoorLocked {
            get {
                return false;
            }
        }
        #endregion
    }
    public class BlockDevice : Device, IBlockDevice
    {
        private IVolume v;

        public BlockDevice (DeviceArguments arguments) : base (arguments)
        {
            this.v = new DiscVolume (arguments, this);
        }

        #region IEnumerable implementation
        IEnumerator<IVolume> IEnumerable<IVolume>.GetEnumerator ()
        {
            yield return v;
        }
        #endregion

        #region IBlockDevice implementation
        public string DeviceNode {
            get {
                return "/dev/disk3";
            }
        }

        public IEnumerable<IVolume> Volumes {
            get {
                List<IVolume> l = new List<IVolume> ();
                l.Add (v);
                return l;
            }
        }

        public bool IsRemovable {
            get {
                return true;
            }
        }
        #endregion

        #region IEnumerable implementation
        public IEnumerator GetEnumerator ()
        {
            throw new System.NotImplementedException ();
        }
        #endregion
    }
}

