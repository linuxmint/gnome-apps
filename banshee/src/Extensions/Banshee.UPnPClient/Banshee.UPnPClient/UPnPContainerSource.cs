//
// UPnPContainerSource.cs
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
using Mono.Unix;

using Banshee.Configuration;
using Banshee.Sources;

namespace Banshee.UPnPClient
{
    public class UPnPContainerSource : Source
    {
        public UPnPContainerSource () :  base (Catalog.GetString ("Shared Media"), Catalog.GetString ("Shared Media"), 300)
        {
            Properties.SetStringList ("Icon.Name", "applications-internet", "network-server");
            TypeUniqueId = "upnp-container";
        }

        public override bool? AutoExpand {
            get { return ExpandedSchema.Get (); }
        }

        public override bool Expanded {
            get { return ExpandedSchema.Get (); }
            set { ExpandedSchema.Set (value); }
        }

        public override bool CanActivate {
            get { return false; }
        }

        public override bool CanRename {
            get { return false; }
        }

        public static readonly SchemaEntry<bool> ExpandedSchema = new SchemaEntry<bool> (
            "plugins.upnp", "expanded", true, "Shared Media expanded", "Shared Media expanded"
        );
    }
}
