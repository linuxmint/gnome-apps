//
// RockBoxDevice.cs
//
// Authors:
//   Jack Deslippe <jdeslip@gmail.com>
//   Andrés G. Aragoneses <knocte@gmail.com>
//
// Copyright (C) 2008 Jack Deslippe
// Copyright (C) 2012 Andres G. Aragoneses
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

namespace Banshee.Dap.MassStorage
{
    public class RockBoxDevice : MassStorageDevice
    {
        public RockBoxDevice (MassStorageSource source) : base (source)
        {
        }

        private static string name = Catalog.GetString ("Rockbox Device");

        private static string[] playback_mime_types = new string [] {
            "application/ogg",
            "audio/x-ms-wma",
            "audio/mpeg",
            "audio/mp4",
            "audio/flac",
            "audio/aac",
            "audio/mp4",
            "audio/x-wav"
        };

        private static string[] audio_folders = new string [] {
            "Music/",
            "Videos/"
        };

        private static string[] video_folders = new string [] {
            "Videos/"
        };

        private static string[] playlist_formats = new string [] {
            "audio/x-mpegurl"
        };

        private static string playlists_path = "Music/Playlists/";

        private static int folder_depth = 2;

        private static string cover_art_file_name = "cover.jpg";

        private static string cover_art_file_type = "jpeg";

        private static int cover_art_size = 320;

        public override bool LoadDeviceConfiguration ()
        {
            Hyena.Log.DebugFormat ("Found RockBox Device");

            LoadConfig (null);

            return true;
        }

        protected override string DefaultName {
            get { return name; }
        }

        protected override string [] DefaultAudioFolders {
            get { return audio_folders; }
        }

        protected override string [] DefaultVideoFolders {
            get { return video_folders; }
        }

        protected override string [] DefaultPlaylistFormats {
            get { return playlist_formats; }
        }

        protected override string DefaultPlaylistPath {
            get { return playlists_path; }
        }

        protected override string [] DefaultPlaybackMimeTypes {
            get { return playback_mime_types; }
        }

        protected override int DefaultFolderDepth {
            get { return folder_depth; }
        }

        protected override string DefaultCoverArtFileName {
            get { return cover_art_file_name; }
        }

        protected override string DefaultCoverArtFileType {
            get { return cover_art_file_type; }
        }

        protected override int DefaultCoverArtSize {
            get { return cover_art_size; }
        }
    }
}

