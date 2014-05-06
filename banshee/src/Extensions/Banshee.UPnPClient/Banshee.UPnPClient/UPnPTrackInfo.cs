//
// UPnPTrackInfo.cs
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

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV;

using Hyena;

using Banshee.Sources;
using Banshee.Collection.Database;

namespace Banshee.UPnPClient
{
    public class UPnPTrackInfo : DatabaseTrackInfo
    {
        static long id = 0;

        public UPnPTrackInfo (MusicTrack track, UPnPMusicSource source) : this (track as Item, source)
        {
            ArtistName = track.Artists.Count > 0 ? track.Artists[0].Name : "";
            AlbumTitle = track.Albums.Count > 0 ? track.Albums[0] : "";
            
            TrackNumber = track.OriginalTrackNumber.GetValueOrDefault ();

            Genre = track.Genres.Count > 0 ? track.Genres[0] : "";
        }

        public UPnPTrackInfo (VideoItem track, UPnPVideoSource source) : this (track as Item, source)
        {
            ArtistName = track.Producers.Count > 0 ? track.Producers[0] : "";

            Genre = track.Genres.Count > 0 ? track.Genres[0] : "";
        }

        public UPnPTrackInfo (Item track, PrimarySource source) : base ()
        {
            if (track == null) {
              throw new ArgumentNullException ("track");
            }

            if (source == null) {
              throw new ArgumentNullException ("source");
            }

            TrackTitle = track.Title;

            Resource resource = FindSuitableResource (track.Resources);

            if (resource != null) {
                BitRate = (int)resource.BitRate.GetValueOrDefault ();
                BitsPerSample = (int)resource.BitsPerSample.GetValueOrDefault ();
                Duration = resource.Duration.GetValueOrDefault ();
                SampleRate = (int)resource.SampleFrequency.GetValueOrDefault ();
                FileSize = (int)resource.Size.GetValueOrDefault ();

                Uri = new SafeUri (resource.Uri);
            } else {
                CanPlay = false;
            }

            ExternalId = ++id;

            PrimarySource = source;
        }

        private Resource FindSuitableResource (IList<Resource> resources)
        {
            foreach (Resource resource in resources) {
                if (resource.Uri != null && resource.Uri.Scheme == System.Uri.UriSchemeHttp) {
                    return resource;
                }
            }

            return null;
        }
    }
}
