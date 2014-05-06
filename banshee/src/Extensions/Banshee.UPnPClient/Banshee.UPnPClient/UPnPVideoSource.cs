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
    public class UPnPVideoSource : PrimarySource
    {
        const int sort_order = 190;
        private Dictionary<string, UPnPTrackInfo> video_tracks;

        public UPnPVideoSource (string udn) : base (Catalog.GetString ("Video"), Catalog.GetString ("Video"), udn + "-video", sort_order)
        {
            Properties.SetStringList ("Icon.Name", "video-x-generic", "video", "source-library");
            Properties.Set<string> ("SearchEntryDescription", Catalog.GetString ("Search your videos"));
            Properties.SetString ("TrackView.ColumnControllerXml", String.Format (@"
                <column-controller>
                  <add-all-defaults />
                  <remove-default column=""DiscColumn"" />
                  <remove-default column=""AlbumColumn"" />
                  <remove-default column=""ComposerColumn"" />
                  <remove-default column=""AlbumArtistColumn"" />
                  <remove-default column=""ConductorColumn"" />
                  <remove-default column=""ComposerColumn"" />
                  <remove-default column=""BpmColumn"" />
                  <sort-column direction=""asc"">track_title</sort-column>
                  <column modify-default=""ArtistColumn"">
                    <title>{0}</title>
                    <long-title>{0}</long-title>
                  </column>
                </column-controller>
            ", Catalog.GetString ("Produced By")));

            video_tracks = new Dictionary<string, UPnPTrackInfo> ();

            // Remove tracks previously associated with this source
            // we do this to be sure they are non-existant before we refresh.
            PurgeTracks ();
            AfterInitialized ();
            OnTracksRemoved ();
        }

        ~UPnPVideoSource ()
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

        public override bool ShowBrowser {
            get { return false; }
        }

        protected override bool HasArtistAlbum {
            get { return false; }
        }

        public override bool CanDeleteTracks {
            get { return false; }
        }

        public void AddTracks (List<VideoItem> tracks)
        {
            foreach (var track in tracks) {
                if (video_tracks.ContainsKey(track.Id)) {
                    continue;
                }

                UPnPTrackInfo track_info = new UPnPTrackInfo (track, this);
                track_info.Save (false);
            }

            OnTracksAdded ();
        }
    }
}
