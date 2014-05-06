//
// DatabaseYearInfo.cs
//
// Author:
//   Frank Ziegler <funtastix@googlemail.com>
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

using Mono.Unix;

using Hyena.Data;
using Hyena.Data.Sqlite;

using Banshee.Database;
using Banshee.ServiceStack;

namespace Banshee.Collection.Database
{
    public class DatabaseYearInfo : YearInfo
    {
        private static BansheeModelProvider<DatabaseYearInfo> provider = new BansheeModelProvider<DatabaseYearInfo> (
            ServiceManager.DbConnection, "CoreTracks"
        );

        public static BansheeModelProvider<DatabaseYearInfo> Provider {
            get { return provider; }
        }

        public DatabaseYearInfo ()
        {
        }

        private int track_id;
        [DatabaseColumn ("TrackID", Constraints = DatabaseColumnConstraints.PrimaryKey)]
        public int TrackId {
            get { return track_id; }
            protected set { track_id = value; }
        }

        [DatabaseColumn ("Year")]
        private int year;
        public override int Year {
            get { return year; }
        }
    }
}
