//
// AudioscrobblerService.cs
//
// Authors:
//   Alexander Hixon <hixon.alexander@mediati.org>
//   Chris Toshok <toshok@ximian.com>
//   Ruben Vermeersch <ruben@savanne.be>
//   Aaron Bockover <aaron@abock.org>
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
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using Gtk;
using Mono.Unix;

using Hyena;
using Hyena.Jobs;

using Lastfm;

using Banshee.MediaEngine;
using Banshee.Base;
using Banshee.Configuration;
using Banshee.ServiceStack;
using Banshee.Gui;
using Banshee.Networking;
using Banshee.Sources;

using Banshee.Collection;

using Browser = Lastfm.Browser;

namespace Banshee.Lastfm.Audioscrobbler
{
    public class AudioscrobblerService : IExtensionService, IDisposable
    {
        private AudioscrobblerConnection connection;
        private ActionGroup actions;
        private uint ui_manager_id;
        private InterfaceActionService action_service;
        private Queue queue;
        private Account account;

        private bool queued; /* if current_track has been queued */
        private bool now_playing_sent = false; /* self-explanatory :) */
        private int iterate_countdown = 4 * 4; /* number of times to wait for iterate event before sending now playing */

        private DateTime song_start_time;
        private TrackInfo last_track;

        private readonly TimeSpan MINIMUM_TRACK_DURATION = TimeSpan.FromSeconds (30);
        private readonly TimeSpan MINIMUM_TRACK_PLAYTIME = TimeSpan.FromSeconds (240);

        public AudioscrobblerService ()
        {
        }

        void IExtensionService.Initialize ()
        {
            account = LastfmCore.Account;

            if (account.UserName == null) {
                account.UserName = LastUserSchema.Get ();
                account.SessionKey = LastSessionKeySchema.Get ();
                account.ScrobbleUrl = LastScrobbleUrlSchema.Get ();
            }

            if (LastfmCore.UserAgent == null) {
                LastfmCore.UserAgent = Banshee.Web.Browser.UserAgent;
            }

            Browser.Open = Banshee.Web.Browser.Open;

            queue = new Queue ();
            LastfmCore.AudioscrobblerQueue = queue;
            connection = LastfmCore.Audioscrobbler;

            // Initialize with a reasonable value in case we miss the first StartOfStream event
            song_start_time = DateTime.Now;

            Network network = ServiceManager.Get<Network> ();
            connection.UpdateNetworkState (network.Connected);
            network.StateChanged += HandleNetworkStateChanged;

            // Update the Visit action menu item if we update our account info
            LastfmCore.Account.Updated += delegate (object o, EventArgs args) {
                actions["AudioscrobblerVisitAction"].Sensitive = String.IsNullOrEmpty (LastfmCore.Account.UserName);
            };

            ServiceManager.PlayerEngine.ConnectEvent (OnPlayerEvent,
                PlayerEvent.StartOfStream |
                PlayerEvent.EndOfStream |
                PlayerEvent.Seek |
                PlayerEvent.Iterate);

            if (DeviceEnabled) {
                SubscribeForDeviceEvents ();
            }

            action_service = ServiceManager.Get<InterfaceActionService> ();
            InterfaceInitialize ();
        }

        public void InterfaceInitialize ()
        {
            actions = new ActionGroup ("Audioscrobbler");

            actions.Add (new ActionEntry [] {
                new ActionEntry ("AudioscrobblerAction", null,
                    Catalog.GetString ("_Last.fm"), null,
                    Catalog.GetString ("Configure the Audioscrobbler plugin"), null),

                new ActionEntry ("AudioscrobblerVisitAction", null,
                    Catalog.GetString ("Visit _User Profile Page"), null,
                    Catalog.GetString ("Visit Your Last.fm Profile Page"), OnVisitOwnProfile)
            });

            actions.Add (new ToggleActionEntry [] {
                new ToggleActionEntry ("AudioscrobblerEnableAction", null,
                    Catalog.GetString ("_Enable Song Reporting From Banshee"), null,
                    Catalog.GetString ("Enable song reporting From Banshee"), OnToggleEnabled, Enabled)
            });

            actions.Add (new ToggleActionEntry [] {
                new ToggleActionEntry ("AudioscrobblerDeviceEnableAction", null,
                    Catalog.GetString ("_Enable Song Reporting From Device"), null,
                    Catalog.GetString ("Enable song reporting From Device"), OnToggleDeviceEnabled, DeviceEnabled)
            });

            action_service.UIManager.InsertActionGroup (actions, 0);
            ui_manager_id = action_service.UIManager.AddUiFromResource ("AudioscrobblerMenu.xml");

            actions["AudioscrobblerVisitAction"].Sensitive = account.UserName != null && account.UserName != String.Empty;
        }


        public void Dispose ()
        {
            // Try and queue the currently playing track just in case it's queueable
            // but the user hasn't hit next yet and quit/disposed the service.
            if (ServiceManager.PlayerEngine.CurrentTrack != null) {
                Queue (ServiceManager.PlayerEngine.CurrentTrack);
            }

            ServiceManager.PlayerEngine.DisconnectEvent (OnPlayerEvent);

            ServiceManager.Get<Network> ().StateChanged -= HandleNetworkStateChanged;

            if (DeviceEnabled) {
                UnsubscribeForDeviceEvents ();
            }

            // When we stop the connection, queue ends up getting saved too, so the
            // track we queued earlier should stay until next session.
            connection.Stop ();

            action_service.UIManager.RemoveUi (ui_manager_id);
            action_service.UIManager.RemoveActionGroup (actions);
            actions = null;
        }

        List<IBatchScrobblerSource> sources_watched;

        private void SubscribeForDeviceEvents ()
        {
            sources_watched = new List<IBatchScrobblerSource> ();
            ServiceManager.SourceManager.SourceAdded += OnSourceAdded;
            ServiceManager.SourceManager.SourceRemoved += OnSourceRemoved;
        }

        private void UnsubscribeForDeviceEvents ()
        {
            ServiceManager.SourceManager.SourceAdded -= OnSourceAdded;
            ServiceManager.SourceManager.SourceRemoved -= OnSourceRemoved;
            foreach (var source in sources_watched) {
                source.ReadyToScrobble -= OnReadyToScrobble;
            }
            sources_watched.Clear ();
            sources_watched = null;
        }

        private void HandleNetworkStateChanged (object o, NetworkStateChangedArgs args)
        {
            connection.UpdateNetworkState (args.Connected);
        }

        // We need to time how long the song has played
        internal class SongTimer
        {
            private long playtime = 0;  // number of msecs played
            public long PlayTime {
                get { return playtime; }
            }

            private long previouspos = 0;

            // number of events to ignore to get sync (since events may be fired in wrong order)
            private int ignorenext = 0;

            public void IncreasePosition ()
            {
                long increase = 0;

                if (ignorenext == 0) {
                    increase = (ServiceManager.PlayerEngine.Position - previouspos);
                    if (increase > 0) {
                        playtime += increase;
                    }
                } else {
                    ignorenext--;
                }

                previouspos = ServiceManager.PlayerEngine.Position;
            }

            public void SkipPosition ()
            {
                // Set newly seeked position
                previouspos = ServiceManager.PlayerEngine.Position;
                ignorenext = 2; // allow 2 iterates to sync
            }

            public void Reset ()
            {
                playtime = 0;
                previouspos = 0;
                ignorenext = 0;
            }
        }

        SongTimer st = new SongTimer ();

        private bool IsValidForSubmission (TrackInfo track)
        {
            return (track.Duration > MINIMUM_TRACK_DURATION &&
                    (track.MediaAttributes & TrackMediaAttributes.Music) != 0 &&
                    !String.IsNullOrEmpty (track.ArtistName) &&
                    !String.IsNullOrEmpty (track.TrackTitle));
        }

        private void Queue (TrackInfo track) {
            if (track == null || st.PlayTime == 0 ||
                !((ToggleAction) actions["AudioscrobblerEnableAction"]).Active) {

                return;
            }

            Log.DebugFormat ("Track {3} had playtime of {0} msec ({4}sec), duration {1} msec, queued: {2}",
                st.PlayTime, track.Duration.TotalMilliseconds, queued, track, st.PlayTime / 1000);

            if (!queued && IsValidForSubmission (track) &&
                (st.PlayTime > track.Duration.TotalMilliseconds / 2 ||
                 st.PlayTime > MINIMUM_TRACK_PLAYTIME.TotalMilliseconds)) {
                    if (!connection.Started) {
                        connection.Start ();
                    }

                    queue.Add (track, song_start_time);
                    queued = true;
            }
        }

        private void OnPlayerEvent (PlayerEventArgs args)
        {
            switch (args.Event) {
                case PlayerEvent.StartOfStream:
                    // Queue the previous track in case of a skip
                    Queue (last_track);

                    st.Reset ();
                    song_start_time = DateTime.Now;
                    last_track = ServiceManager.PlayerEngine.CurrentTrack;
                    queued = false;
                    now_playing_sent = false;
                    iterate_countdown = 4 * 4;  /* we get roughly 4 events/sec */

                    break;

                case PlayerEvent.Seek:
                    st.SkipPosition ();
                    break;

                case PlayerEvent.Iterate:
                    // Queue as now playing
                    if (!now_playing_sent && iterate_countdown == 0) {
                        if (last_track != null &&
                            IsValidForSubmission (last_track) &&
                            ((ToggleAction) actions["AudioscrobblerEnableAction"]).Active) {

                            connection.NowPlaying (last_track.ArtistName, last_track.TrackTitle,
                                last_track.AlbumTitle, last_track.Duration.TotalSeconds, last_track.TrackNumber);
                        }

                        now_playing_sent = true;
                    } else if (iterate_countdown > 0) {
                        iterate_countdown --;
                    }

                    st.IncreasePosition ();
                    break;

                case PlayerEvent.EndOfStream:
                    Queue (last_track);
                    last_track = null;
                    iterate_countdown = 4 * 4;
                    break;
            }
        }

        private void OnVisitOwnProfile (object o, EventArgs args)
        {
            account.VisitUserProfile (account.UserName);
        }

        private void OnToggleEnabled (object o, EventArgs args)
        {
            Enabled = ((ToggleAction) o).Active;
        }

        private void OnToggleDeviceEnabled (object o, EventArgs args)
        {
            DeviceEnabled = ((ToggleAction) o).Active;
        }

        internal bool Enabled {
            get { return EngineEnabledSchema.Get (); }
            set {
                EngineEnabledSchema.Set (value);
                ((ToggleAction) actions["AudioscrobblerEnableAction"]).Active = value;
            }
        }

        internal bool DeviceEnabled {
            get { return DeviceEngineEnabledSchema.Get (); }
            set {
                if (DeviceEnabled == value)
                    return;

                DeviceEngineEnabledSchema.Set (value);
                ((ToggleAction) actions["AudioscrobblerDeviceEnableAction"]).Active = value;

                if (value) {
                    SubscribeForDeviceEvents ();
                } else {
                    UnsubscribeForDeviceEvents ();
                }
            }
        }

#region scrobbling

        private void OnSourceAdded (SourceEventArgs args)
        {
            var scrobbler_source = args.Source as IBatchScrobblerSource;
            if (scrobbler_source == null) {
                return;
            }

            scrobbler_source.ReadyToScrobble += OnReadyToScrobble;
            sources_watched.Add (scrobbler_source);
        }

        private void OnSourceRemoved (SourceEventArgs args)
        {
            var scrobbler_source = args.Source as IBatchScrobblerSource;
            if (scrobbler_source == null) {
                return;
            }

            sources_watched.Remove (scrobbler_source);
            scrobbler_source.ReadyToScrobble -= OnReadyToScrobble;
        }

        private void OnReadyToScrobble (object source, ScrobblingBatchEventArgs args)
        {
            var scrobble_job = new UserJob (Catalog.GetString ("Scrobbling from device"),
                                            Catalog.GetString ("Scrobbling from device..."));

            scrobble_job.PriorityHints = PriorityHints.DataLossIfStopped;
            scrobble_job.Register ();

            try {
                if (!connection.Started) {
                    connection.Start ();
                }
    
                int added_track_count = 0, processed_track_count = 0;
                string message = Catalog.GetString ("Processing track {0} of {1} ...");
                var batchCount = args.ScrobblingBatch.Count;
    
                foreach (var track_entry in args.ScrobblingBatch) {
                    TrackInfo track = track_entry.Key;
    
                    if (IsValidForSubmission (track)) {
                        IList<DateTime> playtimes = track_entry.Value;
    
                        foreach (DateTime playtime in playtimes) {
                            queue.Add (track, playtime);
                            added_track_count++;
                        }
                        Log.DebugFormat ("Added to Last.fm queue: {0} - Number of plays: {1}", track, playtimes.Count);
                    } else {
                        Log.DebugFormat ("Track {0} failed validation check for Last.fm submission, skipping...",
                                         track);
                    }
    
                    scrobble_job.Status = String.Format (message, ++processed_track_count, batchCount);
                    scrobble_job.Progress = processed_track_count / (double) batchCount;
                }
    
                Log.InformationFormat ("Number of played tracks from device added to Last.fm queue: {0}", added_track_count);

            } finally {
                scrobble_job.Finish ();
            }
        }

#endregion

        public static readonly SchemaEntry<string> LastUserSchema = new SchemaEntry<string> (
            "plugins.lastfm", "username", "", "Last.fm user", "Last.fm username"
        );

        public static readonly SchemaEntry<string> LastSessionKeySchema = new SchemaEntry<string> (
            "plugins.lastfm", "session_key", "", "Last.fm session key", "Last.fm sessions key used in authenticated calls"
        );

        public static readonly SchemaEntry<string> LastScrobbleUrlSchema = new SchemaEntry<string> (
            "plugins.audioscrobbler", "api_url",
            null,
            "AudioScrobbler API URL",
            "URL for the AudioScrobbler API (supports turtle.libre.fm, for instance)"
        );

        public static readonly SchemaEntry<bool> EngineEnabledSchema = new SchemaEntry<bool> (
            "plugins.audioscrobbler", "engine_enabled",
            false,
            "Engine enabled",
            "Audioscrobbler reporting engine enabled"
        );

        public static readonly SchemaEntry<bool> DeviceEngineEnabledSchema = new SchemaEntry<bool> (
            "plugins.audioscrobbler", "device_engine_enabled",
            false,
            "Device engine enabled",
            "Audioscrobbler device reporting engine enabled"
        );

        string IService.ServiceName {
            get { return "AudioscrobblerService"; }
        }
    }
}
