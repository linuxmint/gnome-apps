//
// OsxService.cs
//
// Authors:
//   Aaron Bockover <abockover@novell.com>
//   Eoin Hennessy <eoin@randomrules.org>
//   Timo Dörr <timo@latecrew.de>
//
// Copyright 2012 Timo Dörr
// Copyright 2009-2010 Novell, Inc.
// Copyright 2008 Eoin Hennessy
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
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using Gtk;
using Mono.Unix;

using Banshee.ServiceStack;
using Banshee.Gui;

using OsxIntegration.GtkOsxApplication;
using Hyena;

using MonoMac.CoreFoundation;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreWlan;

namespace Banshee.OsxBackend
{
    public class OsxService : IExtensionService, IDisposable
    {
        private GtkElementsService elements_service;
        private InterfaceActionService interface_action_service;
        private string accel_map_filename = "osx_accel_map";

        private static bool is_nsapplication_initialized = false;
        public static void GlobalInit ()
        {
            // Nearly all MonoMac related functions require that NSApplication.Init () is called
            // before usage, however other Addins (like HardwareManager) might launch before
            // OsxService got started
            lock (typeof (OsxService)) {
                if (!is_nsapplication_initialized) {
                    NSApplication.Init ();
                    is_nsapplication_initialized = true;
                }
            }

            // Register event that handles openFile AppleEvent, i.e. when
            // opening a .mp3 file through Finder
            NSApplication.SharedApplication.OpenFiles += (sender, e) => {
                foreach (string file in e.Filenames) {
                    // Upon successfull start, we receive a openFile event with the Nereid.exe
                    // since mono passes that event - we ignore it here
                    if (file.ToLower ().EndsWith (".exe")) continue;

                    // Put the file on the bus - usually FileSystemQueueSource will
                    // pick this event and put it into a queue
                    ServiceManager.Get<DBusCommandService> ().PushFile (file);
                }
                // Immediately start playback of the enqueued files
                ServiceManager.Get<DBusCommandService> ().PushArgument ("play-enqueued", "");
            };
        }

        void IExtensionService.Initialize ()
        {
            elements_service = ServiceManager.Get<GtkElementsService> ();
            interface_action_service = ServiceManager.Get<InterfaceActionService> ();

            if (!ServiceStartup ()) {
                ServiceManager.ServiceStarted += OnServiceStarted;
            }
        }

        private void OnServiceStarted (ServiceStartedArgs args)
        {
            if (args.Service is Banshee.Gui.InterfaceActionService) {
                interface_action_service = (InterfaceActionService)args.Service;
            } else if (args.Service is GtkElementsService) {
                elements_service = (GtkElementsService)args.Service;
            }

            ServiceStartup ();
        }

        private bool ServiceStartup ()
        {
            if (elements_service == null || interface_action_service == null) {
                return false;
            }

            Initialize ();
            ServiceManager.ServiceStarted -= OnServiceStarted;

            return true;
        }
        private void Initialize ()
        {
            GlobalInit ();

            // load OS X specific key mappings, possibly overriding default mappings
            // set in GlobalActions or $HOME/.config/banshee-1/gtk_accel_map
            string accel_map = Paths.Combine (Paths.ApplicationData, accel_map_filename);
            if (!File.Exists (accel_map)) {
                // copy our template
                CopyAccelMapToDataDir ();
            }
            Gtk.AccelMap.Load (accel_map);

            ConfigureOsxMainMenu ();
        }

        public void Dispose ()
        {
        }

        private void ConfigureOsxMainMenu ()
        {
            var osx_app = new GtkOsxApplication ();

            // remove the "Quit" item as this is auto-added by gtk-mac-integration to the AppMenu
            var quit_item = ((MenuItem)interface_action_service.UIManager.GetWidget ( "/MainMenu/MediaMenu/Quit"));
            if(quit_item != null) {
                quit_item.Hide ();
            }

            MenuShell shell = (MenuShell) interface_action_service.UIManager.GetWidget ("/MainMenu");
            if (shell != null) {
                osx_app.SetMenu (shell);
            }

            // place the "about" and "preferences" menu items into the OS X application menu
            // as every OS X app uses this convention
            var about_item = interface_action_service.UIManager.GetWidget ("/MainMenu/HelpMenu/About") as MenuItem;
            if (about_item != null) {
                osx_app.InsertIntoAppMenu (about_item, 0);
            }

            // place a separator between the About and the Preferences dialog
            var separator = new SeparatorMenuItem ();
            osx_app.InsertIntoAppMenu (separator, 1);

            var preferences_item = interface_action_service.UIManager.GetWidget ("/MainMenu/EditMenu/Preferences") as MenuItem;
            if (preferences_item != null) {
                osx_app.InsertIntoAppMenu (preferences_item, 2);
            }

            // remove unnecessary separator as we have moved the preferences item
            var preferences_seperator = interface_action_service.UIManager.GetWidget ("/MainMenu/EditMenu/PreferencesSeparator") as SeparatorMenuItem;
            if (preferences_seperator != null) {
                preferences_seperator.Destroy ();
            }

            // actually performs the menu binding
            osx_app.Ready ();
        }

        /// <summary>
        /// Copies the OSX specific accel map from embedded resource
        /// to the user's data dir for future loading
        /// </summary>
        public void CopyAccelMapToDataDir ()
        {
            byte[] buffer = new byte[1024];
            var assembly = Assembly.GetExecutingAssembly ();
            var accel_map = Paths.Combine (Paths.ApplicationData, accel_map_filename);

            // perform the copy
            using (Stream output = File.OpenWrite(accel_map)) {
                using (Stream resource_stream = assembly.GetManifestResourceStream (accel_map_filename)) {
                    int bytes = -1;
                    while ((bytes = resource_stream.Read(buffer, 0, buffer.Length)) > 0) {
                        output.Write(buffer, 0, bytes);
                    }
                }
             }
        }

        string IService.ServiceName {
            get { return "OsxService"; }
        }
    }
}
