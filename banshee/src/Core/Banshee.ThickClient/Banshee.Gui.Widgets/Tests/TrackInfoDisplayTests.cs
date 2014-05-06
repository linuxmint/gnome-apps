//
// TrackInfoDisplayTests.cs
//
// Author:
//   Bertrand Lorentz <bertrand.lorentz@gmail.com>
//
// Copyright 2012 Bertrand Lorentz
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

#if ENABLE_TESTS

using System;

using Cairo;
using Mono.Unix;
using NUnit.Framework;

using Hyena;
using Hyena.Tests;

using Banshee.Collection;
using Banshee.Gui.Widgets;

namespace Banshee.Gui.Widgets.Tests
{
    public class TextTrackInfoDisplay : TrackInfoDisplay
    {
        protected override void RenderTrackInfo (Context cr, TrackInfo track, bool render_track, bool render_artist_album)
        {
        }

        public string GetFirstLine (TrackInfo track)
        {
            return GetFirstLineText (track);
        }

        public string GetSecondLine (TrackInfo track)
        {
            return GetSecondLineText (track);
        }
    }

    [TestFixture]
    public class TrackInfoDisplayTests : TestBase
    {
        private TextTrackInfoDisplay display;

        [TestFixtureSetUp]
        public void Init ()
        {
            // TrackInfoDisplay is a widget, so we need to initialize GTK
            Gtk.Application.Init ();
            display = new TextTrackInfoDisplay ();
        }

        [Test]
        public void TestEmptyTrack ()
        {
            var track = new TrackInfo ();

            TestFirstLine (track, TrackInfo.UnknownTitle);
            TestSecondLine (track,
                            "<span color=\"#000000\"><span color=\"#000000\" size=\"small\">by</span> {0}</span>",
                            ArtistInfo.UnknownArtistName);
        }

        [Test]
        public void TestTrackArtist ()
        {
            var track = new TrackInfo ();
            track.TrackTitle = "The Title";
            track.ArtistName = "The Artist";

            TestFirstLine (track, track.TrackTitle);
            TestSecondLine (track,
                            "<span color=\"#000000\"><span color=\"#000000\" size=\"small\">by</span> {0}</span>",
                            track.ArtistName);
        }

        [Test]
        public void TestTrackAlbum ()
        {
            var track = new TrackInfo ();
            track.TrackTitle = "The Title";
            track.AlbumTitle = "The Album Title";

            TestFirstLine (track, track.TrackTitle);
            TestSecondLine (track,
                            "<span color=\"#000000\"><span color=\"#000000\" size=\"small\">from</span> {0}</span>",
                            track.AlbumTitle);
        }

        [Test]
        public void TestTrackFull ()
        {
            var track = new TrackInfo ();
            track.TrackTitle = "The Title";
            track.AlbumTitle = "The Album Title";
            track.ArtistName = "The Artist";

            TestFirstLine (track, track.TrackTitle);
            TestSecondLine (track,
                            "<span color=\"#000000\"><span color=\"#000000\" size=\"small\">by</span> {0} " +
                            "<span color=\"#000000\" size=\"small\">from</span> {1}</span>",
                            track.ArtistName, track.AlbumTitle);
        }

        [Test]
        public void TestPodcastTrack ()
        {
            var track = new TrackInfo ();
            track.TrackTitle = "The Title";
            track.AlbumTitle = "The Album Title";
            track.ArtistName = "The Artist";
            track.MediaAttributes |= TrackMediaAttributes.Podcast;
            track.ReleaseDate = new DateTime (2012, 02, 29);

            TestFirstLine (track, track.TrackTitle);
            TestSecondLine (track,
                            "<span color=\"#000000\"><span color=\"#000000\" size=\"small\">from</span> {0} " +
                            "<span color=\"#000000\" size=\"small\">published</span> {1}</span>",
                            track.AlbumTitle, track.ReleaseDate.ToShortDateString ());
        }

        [Test]
        public void TestRadioTrackEmpty ()
        {
            var station = new TrackInfo ();

            var track = new Banshee.Streaming.RadioTrackInfo (station);

            TestFirstLine (track, TrackInfo.UnknownTitle);
            TestSecondLine (track,
                            "<span color=\"#000000\">{0}</span>",
                            Banshee.Streaming.RadioTrackInfo.UnknownStream);
        }

        [Test]
        public void TestRadioTrackStationOnly ()
        {
            var station = new TrackInfo ();
            station.TrackTitle = "Station Title";
            station.AlbumTitle = "Station Album Title";
            station.ArtistName = "Station Artist";

            var track = new Banshee.Streaming.RadioTrackInfo (station);

            TestFirstLine (track, station.TrackTitle);
            TestSecondLine (track,
                            "<span color=\"#000000\">{0}</span>",
                            track.ParentTrack.ArtistName);
        }

        [Test]
        public void TestRadioTrackPartial ()
        {
            var station = new TrackInfo ();
            station.TrackTitle = "Station Title";

            var track = new Banshee.Streaming.RadioTrackInfo (station);
            track.TrackTitle = "The Title";
            track.ArtistName = "The Artist";

            TestFirstLine (track, track.TrackTitle);
            TestSecondLine (track,
                            "<span color=\"#000000\"><span color=\"#000000\" size=\"small\">by</span> {0} " +
                            "<span color=\"#000000\" size=\"small\">on</span> {1}</span>",
                            track.ArtistName, track.ParentTrack.TrackTitle);
        }

        [Test]
        public void TestRadioTrackFull ()
        {
            var station = new TrackInfo ();
            station.TrackTitle = "Station Title";
            station.AlbumTitle = "Station Album Title";
            station.ArtistName = "Station Artist";

            var track = new Banshee.Streaming.RadioTrackInfo (station);
            track.TrackTitle = "The Title";
            track.AlbumTitle = "The Album Title";
            track.ArtistName = "The Artist";

            TestFirstLine (track, track.TrackTitle);
            TestSecondLine (track,
                            "<span color=\"#000000\"><span color=\"#000000\" size=\"small\">by</span> {0} " +
                            "<span color=\"#000000\" size=\"small\">from</span> {1} " +
                            "<span color=\"#000000\" size=\"small\">on</span> {2}</span>",
                            track.ArtistName, track.AlbumTitle, track.ParentTrack.TrackTitle);
        }

        private void TestFirstLine (TrackInfo track, string content)
        {
            Assert.AreEqual (String.Format ("<b>{0}</b>", content), display.GetFirstLine (track));
        }

        private void TestSecondLine (TrackInfo track, string format, params object [] args)
        {
            Assert.AreEqual (String.Format (format, args), display.GetSecondLine (track));
        }
    }
}

#endif
