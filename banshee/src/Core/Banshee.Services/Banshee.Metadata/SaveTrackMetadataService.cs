//
// SaveTrackMetadataService.cs
//
// Author:
//   Gabriel Burt <gburt@novell.com>
//
// Copyright (C) 2009 Novell, Inc.
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

using Hyena.Jobs;
using Hyena.Data.Sqlite;

using Banshee.Streaming;
using Banshee.Collection.Database;
using Banshee.Sources;
using Banshee.ServiceStack;
using Banshee.Configuration.Schema;
using Banshee.Preferences;

namespace Banshee.Metadata
{
    public class SaveTrackMetadataService : IService
    {
        public static SchemaPreference<bool> WriteMetadataEnabled = new SchemaPreference<bool> (
                LibrarySchema.WriteMetadata,
                Catalog.GetString ("Sync _metadata between library and files"),
                Catalog.GetString ("Enable this option to have metadata in sync between your library and supported media files")
        );

        public static SchemaPreference<bool> WriteRatingsEnabled = new SchemaPreference<bool> (
                LibrarySchema.WriteRatings,
                Catalog.GetString ("Sync _ratings between library and files"),
                Catalog.GetString ("Enable this option to have rating metadata synced between your library and supported audio files")
        );

        public static SchemaPreference<bool> WritePlayCountsEnabled = new SchemaPreference<bool> (
                LibrarySchema.WritePlayCounts,
                Catalog.GetString ("Sync play counts between library and files"),
                Catalog.GetString ("Enable this option to have playcount metadata synced between your library and supported audio files")
        );

        private SaveTrackMetadataJob job;
        private object sync = new object ();
        private bool inited = false;
        private List<PrimarySource> sources = new List<PrimarySource> ();
        public IEnumerable<PrimarySource> Sources {
            get { return sources.AsReadOnly (); }
        }

        public SaveTrackMetadataService ()
        {
            Banshee.ServiceStack.Application.RunTimeout (10000, delegate {
                WriteMetadataEnabled.ValueChanged += OnEnabledChanged;
                WriteRatingsEnabled.ValueChanged += OnEnabledChanged;
                WritePlayCountsEnabled.ValueChanged += OnEnabledChanged;

                foreach (var source in ServiceManager.SourceManager.Sources) {
                    AddPrimarySource (source);
                }

                ServiceManager.SourceManager.SourceAdded += (a) => AddPrimarySource (a.Source);
                ServiceManager.SourceManager.SourceRemoved += (a) => RemovePrimarySource (a.Source);
                Save ();

                inited = true;
                return false;
            });
        }

        private void AddPrimarySource (Source s)
        {
            PrimarySource p = s as PrimarySource;
            if (p != null && p.HasEditableTrackProperties) {
                sources.Add (p);
                p.TracksChanged += OnTracksChanged;
            }
        }

        private void RemovePrimarySource (Source s)
        {
            PrimarySource p = s as PrimarySource;
            if (p != null && sources.Remove (p)) {
                p.TracksChanged -= OnTracksChanged;
            }
        }

        private void RemovePrimarySources ()
        {
            foreach (var source in sources) {
                source.TracksChanged -= OnTracksChanged;
            }
            sources.Clear ();
        }

        public void Dispose ()
        {
            if (inited) {
                RemovePrimarySources ();

                if (job != null) {
                    ServiceManager.JobScheduler.Cancel (job);
                }
            }
        }

        private void Save ()
        {
            if (!(WriteMetadataEnabled.Value || WriteRatingsEnabled.Value || WritePlayCountsEnabled.Value))
                return;

            lock (sync) {
                if (job != null) {
                    job.WriteMetadataEnabled = WriteMetadataEnabled.Value;
                    job.WriteRatingsEnabled = WriteRatingsEnabled.Value;
                    job.WritePlayCountsEnabled = WritePlayCountsEnabled.Value;
                } else {
                    var new_job = new SaveTrackMetadataJob () {
                        WriteMetadataEnabled = WriteMetadataEnabled.Value,
                        WriteRatingsEnabled = WriteRatingsEnabled.Value,
                        WritePlayCountsEnabled = WritePlayCountsEnabled.Value,
                    };
                    new_job.Finished += delegate { lock (sync) { job = null; } };
                    job = new_job;
                    job.Register ();
                }
            }
        }

        private void OnTracksChanged (Source sender, TrackEventArgs args)
        {
            Save ();
        }

        private void OnEnabledChanged (Root pref)
        {
            if (WriteMetadataEnabled.Value || WriteRatingsEnabled.Value || WritePlayCountsEnabled.Value) {
                Save ();
            } else {
                if (job != null) {
                    ServiceManager.JobScheduler.Cancel (job);
                }
            }
        }

        string IService.ServiceName {
            get { return "SaveTrackMetadataService"; }
        }

    }
}
