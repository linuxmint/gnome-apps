// 
// BansheeQueryTests.cs
// 
// Author:
//   Andrés G. Aragoneses <knocte@gmail.com>
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
using System.Text.RegularExpressions;
using NUnit.Framework;
using Banshee.Configuration.Schema;

namespace Banshee.Query.Tests
{
    public class GetSortTests
    {
        protected static void AssertAreEquivalent (string expected, string actual)
        {
            Assert.AreEqual (FullTrim (expected), FullTrim (actual));
        }

        protected static string FullTrim (string str)
        {
            var r = new Regex (@"\s+");
            return r.Replace (str, " ").Trim ();
        }
    }

    [TestFixture]
    public class GetSortTestsWithAlbumYearOff : GetSortTests
    {

        bool original_sort_album_by_year;

        [TestFixtureSetUp]
        public void SetSortAlbumByYearOff ()
        {
            original_sort_album_by_year = LibrarySchema.SortByAlbumYear.Get ();
            if (original_sort_album_by_year) {
                LibrarySchema.SortByAlbumYear.Set (false);
            }
        }

        [TestFixtureTearDown]
        public void RecoverSortAlbumByYearSetting ()
        {
            LibrarySchema.SortByAlbumYear.Set (original_sort_album_by_year);
        }

        [Test]
        public void GetSortForAddedAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DateAddedField, true);
            AssertAreEquivalent (@"CoreTracks.DateAddedStamp ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForAddedDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DateAddedField, false);
            AssertAreEquivalent (@"CoreTracks.DateAddedStamp DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForAlbumArtistAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.AlbumArtistField, true);
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForAlbumArtistDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.AlbumArtistField, false);
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey DESC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForAlbumAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.AlbumField, true);
            AssertAreEquivalent (@"CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForAlbumDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.AlbumField, false);
            AssertAreEquivalent (@"CoreAlbums.TitleSortKey DESC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }


        [Test]
        public void GetSortForArtistAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ArtistField, true);
            AssertAreEquivalent (@"CoreArtists.NameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForArtistDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ArtistField, false);
            AssertAreEquivalent (@"CoreArtists.NameSortKey DESC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                  sort);
        }

        [Test]
        public void GetSortForBpmAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.BpmField, true);
            AssertAreEquivalent (@"CoreTracks.BPM ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                  sort);
        }

        [Test]
        public void GetSortForBpmDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.BpmField, false);
            AssertAreEquivalent (@"CoreTracks.BPM DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForBitRateAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.BitRateField, true);
            AssertAreEquivalent (@"CoreTracks.BitRate ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForBitRateDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.BitRateField, false);
            AssertAreEquivalent (@"CoreTracks.BitRate DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForBitsPerSampleAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.BitsPerSampleField, true);
            AssertAreEquivalent (@"CoreTracks.BitsPerSample ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForBitsPerSampleDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.BitsPerSampleField, false);
            AssertAreEquivalent (@"CoreTracks.BitsPerSample DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForCommentAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.CommentField, true);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Comment) ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForCommentDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.CommentField, false);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Comment) DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForComposerAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ComposerField, true);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Composer) ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForComposerDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ComposerField, false);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Composer) DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForConductorAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ConductorField, true);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Conductor) ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForConductorDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ConductorField, false);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Conductor) DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDateAddedStampAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DateAddedField, true);
            AssertAreEquivalent (@"CoreTracks.DateAddedStamp ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDateAddedStampDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DateAddedField, false);
            AssertAreEquivalent (@"CoreTracks.DateAddedStamp DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDiscAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DiscNumberField, true);
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDiscDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DiscNumberField, false);
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc DESC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDiscCountAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DiscCountField, true);
            AssertAreEquivalent (@"CoreTracks.DiscCount ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDiscCountDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DiscCountField, false);
            AssertAreEquivalent (@"CoreTracks.DiscCount DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDurationAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DurationField, true);
            AssertAreEquivalent (@"CoreTracks.Duration ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDurationDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DurationField, false);
            AssertAreEquivalent (@"CoreTracks.Duration DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForFileSizeAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.FileSizeField, true);
            AssertAreEquivalent (@"CoreTracks.FileSize ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForFileSizeDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.FileSizeField, false);
            AssertAreEquivalent (@"CoreTracks.FileSize DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForGenreAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.GenreField, true);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Genre) ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForGenreDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.GenreField, false);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Genre) DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForGroupingAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.GroupingField, true);
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForGroupingDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.GroupingField, false);
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber DESC",
                                  sort);
        }

        [Test]
        public void GetSortForLastPlayedAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastPlayedField, true);
            AssertAreEquivalent (@"CoreTracks.LastPlayedStamp ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLastPlayedDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastPlayedField, false);
            AssertAreEquivalent (@"CoreTracks.LastPlayedStamp DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLastPlayedStampAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastPlayedField, true);
            AssertAreEquivalent (@"CoreTracks.LastPlayedStamp ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLastPlayedStampDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastPlayedField, false);
            AssertAreEquivalent (@"CoreTracks.LastPlayedStamp DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLastSkippedAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastSkippedField, true);
            AssertAreEquivalent (@"CoreTracks.LastSkippedStamp ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLastSkippedDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastSkippedField, false);
            AssertAreEquivalent (@"CoreTracks.LastSkippedStamp DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLastSkippedStampAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastSkippedField, true);
            AssertAreEquivalent (@"CoreTracks.LastSkippedStamp ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLastSkippedStampDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastSkippedField, false);
            AssertAreEquivalent (@"CoreTracks.LastSkippedStamp DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLicenseUriAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LicenseUriField, true);
            AssertAreEquivalent (@"CoreTracks.LicenseUri ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLicenseUriDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LicenseUriField, false);
            AssertAreEquivalent (@"CoreTracks.LicenseUri DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForMimeTypeAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.MimeTypeField, true);
            AssertAreEquivalent (@"CoreTracks.MimeType ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForMimeTypeDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.MimeTypeField, false);
            AssertAreEquivalent (@"CoreTracks.MimeType DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForPlayCountAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.PlayCountField, true);
            AssertAreEquivalent (@"CoreTracks.PlayCount ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForPlayCountDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.PlayCountField, false);
            AssertAreEquivalent (@"CoreTracks.PlayCount DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForRandom ()
        {
            string sort = FullTrim (BansheeQuery.GetRandomSort ());
            AssertAreEquivalent ("RANDOM ()", sort);
        }

        [Test]
        public void GetSortForRatingAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.RatingField, true);
            AssertAreEquivalent (@"CoreTracks.Rating ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForRatingDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.RatingField, false);
            AssertAreEquivalent (@"CoreTracks.Rating DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForSampleRateAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.SampleRateField, true);
            AssertAreEquivalent (@"CoreTracks.SampleRate ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForSampleRateDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.SampleRateField, false);
            AssertAreEquivalent (@"CoreTracks.SampleRate DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }


        [Test]
        public void GetSortForScoreAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ScoreField, true);
            AssertAreEquivalent (@"CoreTracks.Score ASC,
                                   CoreTracks.PlayCount ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForScoreDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ScoreField, false);
            AssertAreEquivalent (@"CoreTracks.Score DESC,
                                   CoreTracks.PlayCount DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForSkipCountAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.SkipCountField, true);
            AssertAreEquivalent (@"CoreTracks.SkipCount ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForSkipCountDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.SkipCountField, false);
            AssertAreEquivalent (@"CoreTracks.SkipCount DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForTitleAsc ()
        {
            string sort = FullTrim (BansheeQuery.GetSort (BansheeQuery.TitleField, true));
            AssertAreEquivalent (@"CoreTracks.TitleSortKey ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC",
                                 sort);
        }

        [Test]
        public void GetSortForTitleDesc ()
        {
            string sort = FullTrim (BansheeQuery.GetSort (BansheeQuery.TitleField, false));
            AssertAreEquivalent (@"CoreTracks.TitleSortKey DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC",
                                 sort);
        }

        [Test]
        public void GetSortForTrackAsc ()
        {
            string sort = FullTrim (BansheeQuery.GetSort (BansheeQuery.TrackNumberField, true));
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForTrackDesc ()
        {
            string sort = FullTrim (BansheeQuery.GetSort (BansheeQuery.TrackNumberField, false));
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber DESC",
                                 sort);
        }

        [Test]
        public void GetSortForTrackCountAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.TrackCountField, true);
            AssertAreEquivalent (@"CoreTracks.TrackCount ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForTrackCountDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.TrackCountField, false);
            AssertAreEquivalent (@"CoreTracks.TrackCount DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForUriAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.UriField, true);
            AssertAreEquivalent (@"CoreTracks.Uri ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForUriDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.UriField, false);
            AssertAreEquivalent (@"CoreTracks.Uri DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForYearAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.YearField, true);
            AssertAreEquivalent (@"CoreTracks.Year ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForYearDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.YearField, false);
            AssertAreEquivalent (@"CoreTracks.Year DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDefault ()
        {
            string sort = BansheeQuery.GetSort (new UnknownQueryField (), false);
            Assert.IsNull (sort);
        }

    }

    [TestFixture]
    public class GetSortTestsWithAlbumYearOn : GetSortTests
    {
        bool original_sort_album_by_year;

        [TestFixtureSetUp]
        public void SetSortAlbumByYearOn ()
        {
            original_sort_album_by_year = LibrarySchema.SortByAlbumYear.Get ();
            if (!original_sort_album_by_year) {
                LibrarySchema.SortByAlbumYear.Set (true);
            }
        }

        [TestFixtureTearDown]
        public void RecoverSortAlbumByYearSetting ()
        {
            LibrarySchema.SortByAlbumYear.Set (original_sort_album_by_year);
        }

        [Test]
        public void GetSortForAddedAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DateAddedField, true);
            AssertAreEquivalent (@"CoreTracks.DateAddedStamp ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForAddedDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DateAddedField, false);
            AssertAreEquivalent (@"CoreTracks.DateAddedStamp DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForAlbumArtistAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.AlbumArtistField, true);
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForAlbumArtistDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.AlbumArtistField, false);
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey DESC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForAlbumAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.AlbumField, true);
            AssertAreEquivalent (@"CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForAlbumDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.AlbumField, false);
            AssertAreEquivalent (@"CoreAlbums.TitleSortKey DESC,
                                   CoreTracks.Year ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }


        [Test]
        public void GetSortForArtistAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ArtistField, true);
            AssertAreEquivalent (@"CoreArtists.NameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForArtistDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ArtistField, false);
            AssertAreEquivalent (@"CoreArtists.NameSortKey DESC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                  sort);
        }

        [Test]
        public void GetSortForBpmAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.BpmField, true);
            AssertAreEquivalent (@"CoreTracks.BPM ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                  sort);
        }

        [Test]
        public void GetSortForBpmDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.BpmField, false);
            AssertAreEquivalent (@"CoreTracks.BPM DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForBitRateAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.BitRateField, true);
            AssertAreEquivalent (@"CoreTracks.BitRate ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForBitRateDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.BitRateField, false);
            AssertAreEquivalent (@"CoreTracks.BitRate DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForBitsPerSampleAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.BitsPerSampleField, true);
            AssertAreEquivalent (@"CoreTracks.BitsPerSample ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForBitsPerSampleDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.BitsPerSampleField, false);
            AssertAreEquivalent (@"CoreTracks.BitsPerSample DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForCommentAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.CommentField, true);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Comment) ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForCommentDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.CommentField, false);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Comment) DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForComposerAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ComposerField, true);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Composer) ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForComposerDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ComposerField, false);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Composer) DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForConductorAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ConductorField, true);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Conductor) ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForConductorDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ConductorField, false);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Conductor) DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDateAddedStampAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DateAddedField, true);
            AssertAreEquivalent (@"CoreTracks.DateAddedStamp ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDateAddedStampDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DateAddedField, false);
            AssertAreEquivalent (@"CoreTracks.DateAddedStamp DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDiscAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DiscNumberField, true);
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDiscDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DiscNumberField, false);
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc DESC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDiscCountAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DiscCountField, true);
            AssertAreEquivalent (@"CoreTracks.DiscCount ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDiscCountDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DiscCountField, false);
            AssertAreEquivalent (@"CoreTracks.DiscCount DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDurationAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DurationField, true);
            AssertAreEquivalent (@"CoreTracks.Duration ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDurationDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.DurationField, false);
            AssertAreEquivalent (@"CoreTracks.Duration DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForFileSizeAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.FileSizeField, true);
            AssertAreEquivalent (@"CoreTracks.FileSize ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForFileSizeDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.FileSizeField, false);
            AssertAreEquivalent (@"CoreTracks.FileSize DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForGenreAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.GenreField, true);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Genre) ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForGenreDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.GenreField, false);
            AssertAreEquivalent (@"HYENA_COLLATION_KEY(CoreTracks.Genre) DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForGroupingAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.GroupingField, true);
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForGroupingDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.GroupingField, false);
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber DESC",
                                  sort);
        }

        [Test]
        public void GetSortForLastPlayedAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastPlayedField, true);
            AssertAreEquivalent (@"CoreTracks.LastPlayedStamp ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLastPlayedDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastPlayedField, false);
            AssertAreEquivalent (@"CoreTracks.LastPlayedStamp DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLastPlayedStampAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastPlayedField, true);
            AssertAreEquivalent (@"CoreTracks.LastPlayedStamp ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLastPlayedStampDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastPlayedField, false);
            AssertAreEquivalent (@"CoreTracks.LastPlayedStamp DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLastSkippedAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastSkippedField, true);
            AssertAreEquivalent (@"CoreTracks.LastSkippedStamp ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLastSkippedDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastSkippedField, false);
            AssertAreEquivalent (@"CoreTracks.LastSkippedStamp DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLastSkippedStampAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastSkippedField, true);
            AssertAreEquivalent (@"CoreTracks.LastSkippedStamp ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLastSkippedStampDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LastSkippedField, false);
            AssertAreEquivalent (@"CoreTracks.LastSkippedStamp DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLicenseUriAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LicenseUriField, true);
            AssertAreEquivalent (@"CoreTracks.LicenseUri ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForLicenseUriDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.LicenseUriField, false);
            AssertAreEquivalent (@"CoreTracks.LicenseUri DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForMimeTypeAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.MimeTypeField, true);
            AssertAreEquivalent (@"CoreTracks.MimeType ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForMimeTypeDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.MimeTypeField, false);
            AssertAreEquivalent (@"CoreTracks.MimeType DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForPlayCountAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.PlayCountField, true);
            AssertAreEquivalent (@"CoreTracks.PlayCount ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForPlayCountDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.PlayCountField, false);
            AssertAreEquivalent (@"CoreTracks.PlayCount DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForRandomAsc ()
        {
            string sort = FullTrim (BansheeQuery.GetRandomSort ());
            AssertAreEquivalent ("RANDOM ()", sort);
        }

        [Test]
        public void GetSortForRatingAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.RatingField, true);
            AssertAreEquivalent (@"CoreTracks.Rating ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForRatingDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.RatingField, false);
            AssertAreEquivalent (@"CoreTracks.Rating DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForSampleRateAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.SampleRateField, true);
            AssertAreEquivalent (@"CoreTracks.SampleRate ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForSampleRateDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.SampleRateField, false);
            AssertAreEquivalent (@"CoreTracks.SampleRate DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForScoreAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ScoreField, true);
            AssertAreEquivalent (@"CoreTracks.Score ASC,
                                   CoreTracks.PlayCount ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForScoreDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.ScoreField, false);
            AssertAreEquivalent (@"CoreTracks.Score DESC,
                                   CoreTracks.PlayCount DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForSkipCountAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.SkipCountField, true);
            AssertAreEquivalent (@"CoreTracks.SkipCount ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForSkipCountDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.SkipCountField, false);
            AssertAreEquivalent (@"CoreTracks.SkipCount DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForTitleAsc ()
        {
            string sort = FullTrim (BansheeQuery.GetSort (BansheeQuery.TitleField, true));
            AssertAreEquivalent (@"CoreTracks.TitleSortKey ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC",
                                 sort);
        }

        [Test]
        public void GetSortForTitleDesc ()
        {
            string sort = FullTrim (BansheeQuery.GetSort (BansheeQuery.TitleField, false));
            AssertAreEquivalent (@"CoreTracks.TitleSortKey DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC",
                                 sort);
        }

        [Test]
        public void GetSortForTrackAsc ()
        {
            string sort = FullTrim (BansheeQuery.GetSort (BansheeQuery.TrackNumberField, true));
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForTrackDesc ()
        {
            string sort = FullTrim (BansheeQuery.GetSort (BansheeQuery.TrackNumberField, false));
            AssertAreEquivalent (@"CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber DESC",
                                 sort);
        }

        [Test]
        public void GetSortForTrackCountAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.TrackCountField, true);
            AssertAreEquivalent (@"CoreTracks.TrackCount ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForTrackCountDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.TrackCountField, false);
            AssertAreEquivalent (@"CoreTracks.TrackCount DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForUriAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.UriField, true);
            AssertAreEquivalent (@"CoreTracks.Uri ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForUriDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.UriField, false);
            AssertAreEquivalent (@"CoreTracks.Uri DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForYearAsc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.YearField, true);
            AssertAreEquivalent (@"CoreTracks.Year ASC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForYearDesc ()
        {
            string sort = BansheeQuery.GetSort (BansheeQuery.YearField, false);
            AssertAreEquivalent (@"CoreTracks.Year DESC,
                                   CoreAlbums.ArtistNameSortKey ASC,
                                   CoreTracks.Year ASC,
                                   CoreAlbums.TitleSortKey ASC,
                                   CoreTracks.Disc ASC,
                                   CoreTracks.TrackNumber ASC",
                                 sort);
        }

        [Test]
        public void GetSortForDefault ()
        {
            string sort = BansheeQuery.GetSort (new UnknownQueryField (), false);
            Assert.IsNull (sort);
        }

    }

    internal class UnknownQueryField : Hyena.Query.QueryField
    {
        internal UnknownQueryField () : base ("UnknownFieldName", null, null, String.Empty)
        {
        }
    }

}

#endif
