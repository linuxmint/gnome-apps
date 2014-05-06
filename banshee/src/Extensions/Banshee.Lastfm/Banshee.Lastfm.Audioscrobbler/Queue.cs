//
// Queue.cs
//
// Author:
//   Chris Toshok <toshok@ximian.com>
//   Alexander Hixon <hixon.alexander@mediati.org>
//   Phil Trimble <philtrimble@gmail.com>
//
// Copyright (C) 2005-2008 Novell, Inc.
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
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Web;
using System.Xml;

using Hyena;

using Banshee.Base;
using Banshee.Collection;
using Banshee.ServiceStack;
using Banshee.Sources;

using Lastfm;
using Lastfm.Data;

namespace Banshee.Lastfm.Audioscrobbler
{
    class Queue : IQueue
    {
        private readonly TimeSpan MAXIMUM_TRACK_STARTTIME_IN_FUTURE = TimeSpan.FromDays (180);

        internal class QueuedTrack : IQueuedTrack
        {
            private static DateTime epoch = DateTimeUtil.LocalUnixEpoch.ToUniversalTime ();

            public QueuedTrack (TrackInfo track, DateTime start_time)
            {
                this.artist = track.ArtistName;
                this.album = track.AlbumTitle;
                this.title = track.TrackTitle;
                this.track_number = (int) track.TrackNumber;
                this.duration = (int) track.Duration.TotalSeconds;
                // Idealy would use Hyena's DateTimeUtil, but it is broken since the "unix epoch" it uses is
                // not UTC, so depending on whether jan 1 1970 was in day-light savings and whether the user's
                // current timezone is in DLS, we'll be an hour off.
                this.start_time = (long) (start_time.ToUniversalTime () - epoch).TotalSeconds;
                // TODO
                //this.musicbrainzid = track.MusicBrainzId;

                this.musicbrainzid = "";

                // set trackauth value, otherwise empty string is default
                if (track is ILastfmInfo) {
                    this.track_auth = (track as ILastfmInfo).TrackAuth;
                }
            }

            public QueuedTrack (string artist, string album,
                                string title, int track_number, int duration, long start_time,
                                string musicbrainzid, string track_auth)
            {
                this.artist = artist;
                this.album = album;
                this.title = title;
                this.track_number = track_number;
                this.duration = duration;
                this.start_time = start_time;
                this.musicbrainzid = musicbrainzid;
                this.track_auth = track_auth;
            }

            public long StartTime {
                get { return start_time; }
            }

            public string Artist {
                get { return artist; }
            }

            public string Album {
                get { return album; }
            }

            public string Title {
                get { return title; }
            }

            public int TrackNumber {
                get { return track_number; }
            }

            public int Duration {
                get { return duration; }
            }

            public string MusicBrainzId {
                get { return musicbrainzid; }
            }

            public string TrackAuth {
                get { return track_auth; }
            }

            public override string ToString ()
            {
                return String.Format (
                    "{0} - {1} (on {2} - track {3}) <duration: {4}sec, start time: {5}sec>",
                    Artist, Title, Album, TrackNumber, Duration, StartTime
                );
            }

            string artist;
            string album;
            string title;
            int track_number;
            int duration;
            string musicbrainzid;
            long start_time;
            string track_auth = String.Empty;
        }

        List<IQueuedTrack> queue;
        string xml_path;
        bool dirty;

        public event EventHandler TrackAdded;

        public Queue ()
        {
            string xml_dir_path = Path.Combine (Hyena.Paths.ExtensionCacheRoot, "lastfm");
            xml_path = Path.Combine (xml_dir_path, "audioscrobbler-queue.xml");
            queue = new List<IQueuedTrack> ();

            if (!Directory.Exists(xml_dir_path)) {
                Directory.CreateDirectory (xml_dir_path);
            }

            MigrateQueueFile ();

            Load ();
        }

        private void MigrateQueueFile ()
        {
            string old_xml_dir_path = Path.Combine (Hyena.Paths.ExtensionCacheRoot, "last.fm");
            string old_xml_path = Path.Combine (old_xml_dir_path, "audioscrobbler-queue.xml");

            if (Banshee.IO.Directory.Exists (old_xml_dir_path)) {
                var old_file = new SafeUri (old_xml_path);
                var file = new SafeUri (xml_path);
                if (Banshee.IO.File.Exists (old_file)) {
                    Banshee.IO.File.Copy (old_file, file, true);
                    Banshee.IO.File.Delete (old_file);
                }
                Banshee.IO.Directory.Delete (old_xml_dir_path, true);
            }
        }

        public void Save ()
        {
            if (!dirty)
                return;

            XmlTextWriter writer = new XmlTextWriter (xml_path, Encoding.Default);

            writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;
            writer.IndentChar = ' ';

            writer.WriteStartDocument (true);

            writer.WriteStartElement ("AudioscrobblerQueue");
            foreach (QueuedTrack track in queue) {
                writer.WriteStartElement ("QueuedTrack");
                writer.WriteElementString ("Artist", track.Artist);
                writer.WriteElementString ("Album", track.Album);
                writer.WriteElementString ("Title", track.Title);
                writer.WriteElementString ("TrackNumber", track.TrackNumber.ToString());
                writer.WriteElementString ("Duration", track.Duration.ToString());
                writer.WriteElementString ("StartTime", track.StartTime.ToString());
                writer.WriteElementString ("MusicBrainzId", track.MusicBrainzId);
                writer.WriteElementString ("TrackAuth", track.TrackAuth);
                writer.WriteEndElement (); // Track
            }
            writer.WriteEndElement (); // AudioscrobblerQueue
            writer.WriteEndDocument ();
            writer.Close ();
        }

        public void Load ()
        {
            queue.Clear ();
            try {
                string query = "//AudioscrobblerQueue/QueuedTrack";
                XmlDocument doc = new XmlDocument ();

                doc.Load (xml_path);
                XmlNodeList nodes = doc.SelectNodes (query);

                foreach (XmlNode node in nodes) {
                    string artist = "";
                    string album = "";
                    string title = "";
                    int track_number = 0;
                    int duration = 0;
                    long start_time = 0;
                    string musicbrainzid = "";
                    string track_auth = "";

                    foreach (XmlNode child in node.ChildNodes) {
                        if (child.Name == "Artist" && child.ChildNodes.Count != 0) {
                            artist = child.ChildNodes [0].Value;
                        } else if (child.Name == "Album" && child.ChildNodes.Count != 0) {
                            album = child.ChildNodes [0].Value;
                        } else if (child.Name == "Title" && child.ChildNodes.Count != 0) {
                            title = child.ChildNodes [0].Value;
                        } else if (child.Name == "TrackNumber" && child.ChildNodes.Count != 0) {
                            track_number = Convert.ToInt32 (child.ChildNodes [0].Value);
                        } else if (child.Name == "Duration" && child.ChildNodes.Count != 0) {
                            duration = Convert.ToInt32 (child.ChildNodes [0].Value);
                        } else if (child.Name == "StartTime" && child.ChildNodes.Count != 0) {
                            start_time = Convert.ToInt64 (child.ChildNodes [0].Value);
                        } else if (child.Name == "MusicBrainzId" && child.ChildNodes.Count != 0) {
                            musicbrainzid = child.ChildNodes [0].Value;
                        } else if (child.Name == "TrackAuth" && child.ChildNodes.Count != 0) {
                            track_auth = child.ChildNodes [0].Value;
                        }
                    }

                    queue.Add (new QueuedTrack (artist, album, title, track_number, duration,
                        start_time, musicbrainzid, track_auth));
                }
            } catch {
            }
        }

        public List<IQueuedTrack> GetTracks ()
        {
            // Last.fm can technically handle up to 50 songs in one request
            // but seems to throw errors if our submission is too long.
            return queue.GetRange (0, Math.Min (queue.Count, 30));
        }

        public void Add (object track, DateTime started_at)
        {
            TrackInfo t = (track as TrackInfo);
            if (t != null) {
                QueuedTrack new_queued_track = new QueuedTrack (t, started_at);

                //FIXME Just log invalid tracks until we determine the root cause
                if (IsInvalidQueuedTrack (new_queued_track)) {
                    Log.WarningFormat (
                        "Invalid data detected while adding to audioscrobbler queue for " +
                        "track '{0}', original start time: '{1}'", new_queued_track, started_at
                    );
                }

                queue.Add (new_queued_track);

                dirty = true;
                RaiseTrackAdded (this, new EventArgs ());
            }
        }

        public void RemoveRange (int first, int count)
        {
            queue.RemoveRange (first, count);
            dirty = true;
        }

        public int Count {
            get { return queue.Count; }
        }

        private void RaiseTrackAdded (object o, EventArgs args)
        {
            EventHandler handler = TrackAdded;
            if (handler != null)
                handler (o, args);
        }

        public void RemoveInvalidTracks ()
        {
            int removed_track_count = queue.RemoveAll (IsInvalidQueuedTrack);
            if (removed_track_count > 0) {
                Log.WarningFormat (
                    "{0} invalid track(s) removed from audioscrobbler queue",
                    removed_track_count
                );
                dirty = true;
                Save ();
            }
        }

        private bool IsInvalidQueuedTrack (IQueuedTrack track)
        {
            DateTime trackStartTime = DateTimeUtil.FromTimeT (track.StartTime);

            return (
                String.IsNullOrEmpty (track.Artist) ||
                String.IsNullOrEmpty (track.Title) ||
                trackStartTime < DateTimeUtil.LocalUnixEpoch ||
                trackStartTime > DateTime.Now.Add (MAXIMUM_TRACK_STARTTIME_IN_FUTURE)
            );
        }
    }
}
