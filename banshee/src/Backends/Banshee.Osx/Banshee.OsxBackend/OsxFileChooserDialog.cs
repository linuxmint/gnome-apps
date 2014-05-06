//
// OsxFileChooserDialog.cs
//
// Author:
//   Timo Dörr <timo@latecrew.de>
//
// Copyright (C) 2012 Timo Dörr
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
using System.Linq;
using System.Collections.Generic;

using Gtk;
using Mono.Addins;
using MonoMac.AppKit;

using Banshee.Gui.Dialogs;
using Banshee.Configuration;
using Banshee.ServiceStack;
using Hyena;

namespace Banshee.OsxBackend
{
    public class OsxFileChooserDialog : IBansheeFileChooser
    {
        private NSOpenPanel open_panel;

        // this ctor is required for Mono.Addins
        public OsxFileChooserDialog ()
        {
        }

        public IBansheeFileChooser CreateForImport (string title, bool files)
        {
            return new OsxFileChooserDialog (title, files);
        }

        public OsxFileChooserDialog (string title, bool files)
        {
            var panel = new NSOpenPanel () {
                Title = title,
                CanChooseDirectories = !files,
                CanChooseFiles = files,
                AllowsMultipleSelection = true,
                // Translators: verb
                Prompt = Mono.Unix.Catalog.GetString ("Import")
            };
            open_panel = panel;
        }

        #region IBansheeFileChooser implementation
        public string[] Filenames {
            get {
                return open_panel.Urls.Select (uri => SafeUri.UriToFilename (uri.ToString ())).ToArray<string> ();
            }
        }
        public string[] Uris {
            get {
                return open_panel.Urls.Select (uri => uri.ToString ()).ToArray<string> ();
            }
        }

        public void Destroy ()
        {
            open_panel.Close ();
        }

        public int Run ()
        {
            int ret = open_panel.RunModal ();
            // TODO someday MonoMac should provide NSOKButton constant
            if (ret == 1)
                return (int) Gtk.ResponseType.Ok;
            else
                return (int) Gtk.ResponseType.Cancel;
        }

        public void AddFilter (FileFilter filter)
        {
        }
        #endregion
    }
}
