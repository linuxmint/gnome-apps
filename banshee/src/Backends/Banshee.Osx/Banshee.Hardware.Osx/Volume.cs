// 
// Volume.cs
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
using System.IO;

using MonoMac.Foundation;

using Banshee.Hardware.Osx.LowLevel;

namespace Banshee.Hardware.Osx
{
    public class Volume : Device, IVolume
    {
        private IBlockDevice block_parent;
        public Volume (DeviceArguments arguments):base (arguments)
        {
        }

        public Volume (DeviceArguments arguments, IBlockDevice blockdevice) : base (arguments)
        {
            block_parent = blockdevice;
        }

        #region IVolume implementation
        public void Eject ()
        {
            Unmount ();
        }

        public void Mount ()
        {
            return;
        }

        public void Unmount ()
        {
            string volume_url = deviceArguments.DeviceProperties.GetStringValue ("DAVolumePath");
            if (string.IsNullOrEmpty (volume_url)) {
                return;
            }
            deviceArguments.DiskArbiter.UnmountDisk (volume_url);

            // remove this from the devices list
        }

        public string DeviceNode {
            get {
                if (deviceArguments.DeviceProperties.HasKey ("DAMediaBSDName")) {
                    return "/dev/" + deviceArguments.DeviceProperties.GetStringValue ("DAMediaBSDName");
                } else {
                    return null;
                }
            }
        }

        public string MountPoint {
            get {
                var volume_url = deviceArguments.DeviceProperties.GetStringValue ("DAVolumePath");
                if (volume_url == null) {
                    Hyena.Log.Error ("Trying to access device without valid DAVolumePath, aborting!");
                    throw new Exception ();
                }
                var mountpoint = OsxDiskArbiter.UrlToFileSystemPath (volume_url);
                return mountpoint;
            }
        }

        public bool IsMounted {
            get {
                // when a device is unmounted, it triggers a DiskDisappear so we
                // will never see unmounted devices
                return true;
            }
        }

        public bool IsReadOnly {
            get {
                bool isWriteable;
                if (deviceArguments.DeviceProperties.HasKey ("DAMediaWritable") &&
                    bool.TryParse (deviceArguments.DeviceProperties.GetStringValue ("DAMediaWritable"), out isWriteable)) {

                    return isWriteable;
                }
                return false;
            }
        }

        public ulong Capacity {
            get {
                ulong capacity;
                var size = deviceArguments.DeviceProperties.GetStringValue ("DAMediaSize");
                if (size != null && ulong.TryParse (size, out capacity)) {
                    return capacity;
                } else {
                    // try the .NET  way
                    DriveInfo info = new DriveInfo (MountPoint);
                    return (ulong)info.TotalSize;
                }
            }
        }

        public long Available {
            get {
                // this is unreliable
                DriveInfo info = new DriveInfo (MountPoint);
                return info.AvailableFreeSpace;
            }
        }

        public IBlockDevice Parent {
            get { return block_parent; }
        }

        public bool ShouldIgnore {
            get { return false; }
        }

        public string FileSystem {
            get {
                string fs = deviceArguments.DeviceProperties.GetStringValue ("DAMediaContent");
                if (!string.IsNullOrEmpty (fs)) {
                    return fs;
                }

                // Defaulting to MSDOS
                return "MSDOS";
            }
        }

        public bool CanEject {
            get {
                // for now, we can determine if it's ejectable, but no code to actually eject it
                // so return false because pressing the eject button wont work
                return true;
                /*
                bool isEjectable;
                if (deviceProperties.HasKey ("DAMediaEjectable"))
                    if (bool.TryParse (deviceProperties.GetStringValue("DAMediaEjectable"), out isEjectable))
                        return isEjectable;
                // default - not ejectable
                return false;
                */
            }
        }

        public bool CanMount {
            get { return false; }
        }

        public bool CanUnmount {
            get { return true; }
        }
        #endregion
    }
}
