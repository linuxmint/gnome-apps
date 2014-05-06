//
// CompositeTrackSourceContents.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//   Gabriel Burt <gburt@novell.com>
//
// Copyright (C) 2007 Novell, Inc.
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
using System.Reflection;
using System.Collections.Generic;

using Gtk;
using Mono.Unix;

using Hyena.Data;
using Hyena.Data.Gui;
using Hyena.Widgets;

using Banshee.Sources;
using Banshee.ServiceStack;
using Banshee.Collection;
using Banshee.Collection.Database;
using Banshee.Configuration;
using Banshee.Gui;
using Banshee.Collection.Gui;

using ScrolledWindow=Gtk.ScrolledWindow;

namespace Banshee.Sources.Gui
{
    public class CompositeTrackSourceContents : FilteredListSourceContents, ITrackModelSourceContents
    {
        private QueryFilterView<string> genre_view;
        private YearListView year_view;
        private ArtistListView artist_view;
        private ArtistListView albumartist_view;
        private AlbumListView album_view;
        private TrackListView track_view;

        private InterfaceActionService action_service;
        private ActionGroup configure_browser_actions;

        private static string menu_xml = @"
            <ui>
              <menubar name=""MainMenu"">
                <menu name=""ViewMenu"" action=""ViewMenuAction"">
                  <placeholder name=""BrowserViews"">
                    <menu name=""BrowserContentMenu"" action=""BrowserContentMenuAction"">
                        <menuitem name=""ShowArtistFilter"" action=""ShowArtistFilterAction"" />
                        <separator />
                        <menuitem name=""ShowTrackArtistFilter"" action=""ShowTrackArtistFilterAction"" />
                        <menuitem name=""ShowAlbumArtistFilter"" action=""ShowAlbumArtistFilterAction"" />
                        <separator />
                        <menuitem name=""ShowGenreFilter"" action=""ShowGenreFilterAction"" />
                        <menuitem name=""ShowYearFilter"" action=""ShowYearFilterAction"" />
                    </menu>
                    <separator />
                  </placeholder>
                </menu>
              </menubar>
            </ui>
        ";

        public CompositeTrackSourceContents () : base ("albumartist")
        {
            if (ServiceManager.Contains ("InterfaceActionService")) {
                action_service = ServiceManager.Get<InterfaceActionService> ();

                if (action_service.FindActionGroup ("BrowserConfiguration") == null) {
                    configure_browser_actions = new ActionGroup ("BrowserConfiguration");

                    configure_browser_actions.Add (new ActionEntry [] {
                        new ActionEntry ("BrowserContentMenuAction", null,
                            Catalog.GetString ("Browser Content"), null,
                            Catalog.GetString ("Configure the filters available in the browser"), null)
                    });

                    configure_browser_actions.Add (new ToggleActionEntry [] {
                        new ToggleActionEntry ("ShowArtistFilterAction", null,
                            Catalog.GetString ("Show Artist Filter"), null,
                            Catalog.GetString ("Show a list of artists to filter by"), null, ArtistFilterVisible.Get ())});

                    configure_browser_actions.Add (new RadioActionEntry [] {
                        new RadioActionEntry ("ShowTrackArtistFilterAction", null,
                            Catalog.GetString ("Show all Artists"), null,
                            Catalog.GetString ("Show all artists in the artist filter"), 0),

                        new RadioActionEntry ("ShowAlbumArtistFilterAction", null,
                            Catalog.GetString ("Show Album Artists"), null,
                            Catalog.GetString ("Show only album artists, not artists with only single tracks"), 1),
                    }, ArtistFilterType.Get ().Equals ("artist") ? 0 : 1 , null);

                    configure_browser_actions.Add (new ToggleActionEntry [] {
                        new ToggleActionEntry ("ShowGenreFilterAction", null,
                            Catalog.GetString ("Show Genre Filter"), null,
                            Catalog.GetString ("Show a list of genres to filter by"), null, GenreFilterVisible.Get ())});

                    configure_browser_actions.Add (new ToggleActionEntry [] {
                        new ToggleActionEntry ("ShowYearFilterAction", null,
                            Catalog.GetString ("Show Year Filter"), null,
                            Catalog.GetString ("Show a list of years to filter by"), null, YearFilterVisible.Get ())});

                    action_service.AddActionGroup (configure_browser_actions);
                    action_service.UIManager.AddUiFromString (menu_xml);
                }

                action_service.FindAction("BrowserConfiguration.ShowArtistFilterAction").Activated += OnArtistFilterVisibilityChanged;
                action_service.FindAction("BrowserConfiguration.ShowGenreFilterAction").Activated += OnGenreFilterChanged;;
                action_service.FindAction("BrowserConfiguration.ShowYearFilterAction").Activated += OnYearFilterChanged;;

                var artist_filter_action = action_service.FindAction("BrowserConfiguration.ShowTrackArtistFilterAction") as RadioAction;
                var albumartist_filter_action = action_service.FindAction("BrowserConfiguration.ShowAlbumArtistFilterAction") as RadioAction;
                artist_filter_action.Changed += OnArtistFilterChanged;
                artist_filter_action.Sensitive = ArtistFilterVisible.Get ();
                albumartist_filter_action.Changed += OnArtistFilterChanged;
                albumartist_filter_action.Sensitive = ArtistFilterVisible.Get ();
            }
        }

        private void OnArtistFilterVisibilityChanged (object o, EventArgs args)
        {
            ToggleAction action = (ToggleAction)o;

            ClearFilterSelections ();

            ArtistFilterVisible.Set (action.Active);

            if (artist_view !=null && artist_view.Parent !=null) {
                artist_view.Parent.Visible = ArtistFilterVisible.Get ();
            } else if (albumartist_view != null && albumartist_view.Parent != null) {
                albumartist_view.Parent.Visible = ArtistFilterVisible.Get ();
            }

            action_service.FindAction("BrowserConfiguration.ShowTrackArtistFilterAction").Sensitive = ArtistFilterVisible.Get ();
            action_service.FindAction("BrowserConfiguration.ShowAlbumArtistFilterAction").Sensitive = ArtistFilterVisible.Get ();
        }

        private void OnGenreFilterChanged (object o, EventArgs args)
        {
            ToggleAction action = (ToggleAction)o;

            ClearFilterSelections ();

            GenreFilterVisible.Set (action.Active);

            Widget genre_view_widget = (Widget)genre_view;
            genre_view_widget.Parent.Visible = GenreFilterVisible.Get ();
        }

        private void OnYearFilterChanged (object o, EventArgs args)
        {
            ToggleAction action = (ToggleAction)o;

            ClearFilterSelections ();

            YearFilterVisible.Set (action.Active);

            Widget year_view_widget = (Widget)year_view;
            year_view_widget.Parent.Visible = YearFilterVisible.Get ();
        }

        private void OnArtistFilterChanged (object o, ChangedArgs args)
        {
            Widget new_artist_view = args.Current.Value == 0 ? artist_view : albumartist_view;
            Widget old_artist_view = args.Current.Value == 1 ? artist_view : albumartist_view;

            List<ScrolledWindow> new_filter_list = new List<ScrolledWindow> ();
            List<ScrolledWindow> old_filter_list = new List<ScrolledWindow> (filter_scrolled_windows);

            foreach (ScrolledWindow fw in old_filter_list)
            {
                bool contains = false;
                foreach (Widget child in fw.AllChildren) {
                    if (child == old_artist_view) {
                        contains = true;
                    }
                }

                if (contains) {
                    Widget view_widget = (Widget)new_artist_view;
                    if (view_widget.Parent == null) {
                        SetupFilterView (new_artist_view as ArtistListView);
                    }

                    ScrolledWindow win = (ScrolledWindow)view_widget.Parent;

                    new_filter_list.Add (win);
                } else {
                    new_filter_list.Add (fw);
                }
            }

            filter_scrolled_windows = new_filter_list;

            ClearFilterSelections ();

            Layout ();

            ArtistFilterType.Set (args.Current.Value == 1 ? "albumartist" : "artist");
        }

        protected override void InitializeViews ()
        {
            SetupMainView (track_view = new TrackListView ());

            SetupFilterView (genre_view = new QueryFilterView<string> (Catalog.GetString ("Not Set")));
            Widget genre_view_widget = (Widget)genre_view;
            genre_view_widget.Parent.Shown += delegate {
                genre_view_widget.Parent.Visible = GenreFilterVisible.Get ();
            };

            if (ArtistFilterType.Get ().Equals ("artist")) {
                SetupFilterView (artist_view = new ArtistListView ());
                artist_view.Parent.Shown += delegate {
                    artist_view.Parent.Visible = ArtistFilterVisible.Get ();
                };
                albumartist_view = new ArtistListView ();
            } else {
                SetupFilterView (albumartist_view = new ArtistListView ());
                albumartist_view.Parent.Shown += delegate {
                    albumartist_view.Parent.Visible = ArtistFilterVisible.Get ();
                };
                artist_view = new ArtistListView ();
            }

            SetupFilterView (year_view = new YearListView ());
            Widget year_view_widget = (Widget)year_view;
            year_view_widget.Parent.Shown += delegate {
                year_view_widget.Parent.Visible = YearFilterVisible.Get ();
            };

            SetupFilterView (album_view = new AlbumListView ());
        }

        protected override void ClearFilterSelections ()
        {
            if (genre_view.Model != null) {
                genre_view.Selection.Clear ();
            }

            if (artist_view.Model != null) {
                artist_view.Selection.Clear ();
            }

            if (albumartist_view.Model != null) {
                albumartist_view.Selection.Clear ();
            }

            if (album_view.Model != null) {
                album_view.Selection.Clear ();
            }

            if (year_view.Model != null) {
                year_view.Selection.Clear ();
            }
        }

        public void SetModels (TrackListModel track, IListModel<ArtistInfo> artist, IListModel<AlbumInfo> album, IListModel<QueryFilterInfo<string>> genre)
        {
            SetModel (track);
            SetModel (artist);
            SetModel (album);
            SetModel (genre);
        }

        IListView<TrackInfo> ITrackModelSourceContents.TrackView {
            get { return track_view; }
        }

        public TrackListView TrackView {
            get { return track_view; }
        }

        public TrackListModel TrackModel {
            get { return (TrackListModel)track_view.Model; }
        }

        protected override bool ActiveSourceCanHasBrowser {
            get {
                if (!(ServiceManager.SourceManager.ActiveSource is ITrackModelSource)) {
                    return false;
                }

                return ((ITrackModelSource)ServiceManager.SourceManager.ActiveSource).ShowBrowser;
            }
        }

#region Implement ISourceContents

        public override bool SetSource (ISource source)
        {
            ITrackModelSource track_source = source as ITrackModelSource;
            IFilterableSource filterable_source = source as IFilterableSource;
            if (track_source == null) {
                return false;
            }

            this.source = source;

            SetModel (track_view, track_source.TrackModel);

            bool genre_view_model_set = false;

            if (filterable_source != null && filterable_source.CurrentFilters != null) {
                foreach (IListModel model in filterable_source.CurrentFilters) {
                    if (model is IListModel<ArtistInfo> && model is DatabaseArtistListModel)
                        SetModel (artist_view, (model as IListModel<ArtistInfo>));
                    else if (model is IListModel<ArtistInfo> && model is DatabaseAlbumArtistListModel)
                        SetModel (albumartist_view, (model as IListModel<ArtistInfo>));
                    else if (model is IListModel<AlbumInfo>)
                        SetModel (album_view, (model as IListModel<AlbumInfo>));
                    else if (model is IListModel<QueryFilterInfo<string>> && !genre_view_model_set) {
                        SetModel (genre_view, (model as IListModel<QueryFilterInfo<string>>));
                        genre_view_model_set = true;
                    } else if (model is DatabaseYearListModel)
                        SetModel (year_view, model as IListModel<YearInfo>);
                    // else
                    //    Hyena.Log.DebugFormat ("CompositeTrackSourceContents got non-album/artist filter model: {0}", model);
                }
            }

            track_view.HeaderVisible = true;
            return true;
        }

        public override void ResetSource ()
        {
            source = null;
            SetModel (track_view, null);
            SetModel (artist_view, null);
            SetModel (albumartist_view, null);
            SetModel (album_view, null);
            SetModel (year_view, null);
            SetModel (genre_view, null);
            track_view.HeaderVisible = false;
        }

#endregion

        public static readonly SchemaEntry<bool> ArtistFilterVisible = new SchemaEntry<bool> (
            "browser", "show_artist_filter",
            true,
            "Artist Filter Visibility",
            "Whether or not to show the Artist filter"
        );

        public static readonly SchemaEntry<string> ArtistFilterType = new SchemaEntry<string> (
            "browser", "artist_filter_type",
            "artist",
            "Artist/AlbumArtist Filter Type",
            "Whether to show all artists or just album artists in the artist filter; either 'artist' or 'albumartist'"
        );

        public static readonly SchemaEntry<bool> GenreFilterVisible = new SchemaEntry<bool> (
            "browser", "show_genre_filter",
            false,
            "Genre Filter Visibility",
            "Whether or not to show the Genre filter"
        );

        public static readonly SchemaEntry<bool> YearFilterVisible = new SchemaEntry<bool> (
            "browser", "show_year_filter",
            false,
            "Year Filter Visibility",
            "Whether or not to show the Year filter"
        );
    }
}
