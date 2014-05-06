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

using Mono.Unix;

using Mono.Upnp;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV;

using Banshee.Sources;
using Banshee.Collection.Database;
using Banshee.ServiceStack;

namespace Banshee.UPnPClient
{
    public class UPnPMusicSource : PrimarySource
    {
        const int sort_order = 190;
        private Dictionary<string, UPnPTrackInfo> music_tracks;

        public UPnPMusicSource (string udn) : base (Catalog.GetString ("Music"), Catalog.GetString ("Music"), udn + "-music", sort_order)
        {
            Properties.SetStringList ("Icon.Name", "audio-x-generic", "source-library");
            Properties.Set<string> ("SearchEntryDescription", Catalog.GetString ("Search your music"));

            music_tracks = new Dictionary<string, UPnPTrackInfo> ();

            // Remove tracks previously associated with this source
            // we do this to be sure they are non-existant before we refresh.
            PurgeTracks ();
            AfterInitialized ();
            OnTracksRemoved ();
        }

        ~UPnPMusicSource ()
        {
            Dispose ();
        }

        public override void Dispose ()
        {
            Disconnect ();
            base.Dispose ();
        }

        public void Disconnect ()
        {
            // Stop currently playing track if its from us.
            try {
                if (ServiceManager.PlayerEngine.CurrentState == Banshee.MediaEngine.PlayerState.Playing) {
                    DatabaseTrackInfo track = ServiceManager.PlayerEngine.CurrentTrack as DatabaseTrackInfo;
                    if (track != null && track.PrimarySource == this) {
                        ServiceManager.PlayerEngine.Close ();
                    }
                }
            } catch {}

            // Remove tracks associated with this source, we will refetch them on next connect
            PurgeTracks ();
        }

        public override bool CanDeleteTracks {
            get { return false; }
        }

        public void AddTracks (List<MusicTrack> tracks)
        {
            foreach (var track in tracks) {
                if (music_tracks.ContainsKey(track.Id)) {
                    continue;
                }

                UPnPTrackInfo track_info = new UPnPTrackInfo (track, this);
                track_info.Save (false);
            }

            OnTracksAdded ();
        }
    }
}
