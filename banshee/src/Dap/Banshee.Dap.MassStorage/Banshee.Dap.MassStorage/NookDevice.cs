// 
// NookDevice.cs
// 
// Author:
//   Mark Saunders <mscoolnerd@gmail.com>
// 
// Copyright 2010 Mark Saunders
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
using Mono.Unix;

using Banshee.Base;
using Banshee.Hardware;
using Banshee.Library;
using Banshee.Collection;
using Banshee.Collection.Database;

namespace Banshee.Dap.MassStorage
{
    public class NookDevice : CustomMassStorageDevice
    {
        private static string [] playback_mime_types = new string [] {
            "audio/mpeg",
        };

        private static string [] audio_folders = new string [] {
            "my music/",
        };

        public override void SourceInitialize ()
        {
        }

        public override bool LoadDeviceConfiguration ()
        {
            LoadConfig ();
            return true;
        }

        protected override string [] DefaultAudioFolders {
            get { return audio_folders; }
        }

        protected override string [] DefaultVideoFolders {
            get { return new string[0]; }
        }

        protected override string [] DefaultPlaylistFormats {
            get { return new string[0]; }
        }

        protected override string DefaultPlaylistPath {
            get { return null; }
        }

        protected override string [] DefaultPlaybackMimeTypes {
            get { return playback_mime_types; }
        }

        protected override int DefaultFolderDepth {
            get { return 2; }
        }

        protected override string DefaultCoverArtFileName {
            get { return "AlbumArt.jpg"; }
        }

        protected override string DefaultCoverArtFileType {
            get { return "jpeg"; }
        }

        protected override int DefaultCoverArtSize {
            get { return 320; }
        }

        public override string [] GetIconNames ()
        {
            string [] icon_names = new string [] {
                null, DapSource.FallbackIcon
            };
            switch (Name) {
                case "Nook Classic":
                    icon_names[0] = "phone-nook";
                    break;
                default:
                    icon_names[0] = "phone-htc-g1-white";
                    break;
            }

            return icon_names;
        }

        public override bool GetTrackPath (TrackInfo track, out string path)
        {
            path = MusicLibrarySource.MusicFileNamePattern.CreateFromTrackInfo (
                "%artist%%path_sep%%album%%path_sep%{%track_number%. }%title%",
                track);
            return true;
        }

        public override bool DeleteTrackHook (DatabaseTrackInfo track)
        {
            return true;
        }
    }
}