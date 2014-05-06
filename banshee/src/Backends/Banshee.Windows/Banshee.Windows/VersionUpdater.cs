//
// VersionUpdater.cs
//
// Authors:
//   Ján Sokoly <cruster@gmail.com>
//
// Copyright 2011 Ján Sokoly
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Banshee.Gui;
using Banshee.ServiceStack;

using Gtk;
using Hyena.Downloader;
using Mono.Unix;

namespace Banshee.Windows
{
    public class VersionUpdater : DownloadManager
    {
        private const string download_url  = "http://ftp.gnome.org/pub/GNOME/sources/banshee/";
        private const string installer_url = "http://ftp.gnome.org/pub/GNOME/binaries/win32/banshee/";
        private const string doap_filename = "banshee.doap";
        private string temp_doap_path = Path.GetTempPath () + doap_filename;
        private string unstable_version;
        private string temp_installer_path;
        private bool verbose;
        private DownloadManagerJob job;

        public void CheckForUpdates (bool verbose)
        {
            this.verbose = verbose;

            HttpFileDownloader downloader = new HttpFileDownloader ();
            downloader.Uri = new Uri (download_url + doap_filename);
            downloader.TempPathRoot = Path.GetTempPath ();
            downloader.Finished += OnDoapDownloaderFinished;
            downloader.Start ();

            temp_doap_path = downloader.LocalPath;
        }

        void OnDoapDownloaderFinished (HttpDownloader obj)
        {
            if (obj.State.FailureException != null) {
                if (verbose) {
                    DisplayMessage (Catalog.GetString ("Can't check for updates"),
                        Catalog.GetString ("We're currently not able to check if there's a new version available. Please try again later."), MessageType.Error);
                }
            } else {
                var doap = XDocument.Load (temp_doap_path);
                var ns_doap = XNamespace.Get ("http://usefulinc.com/ns/doap#");
                unstable_version = doap.Descendants (ns_doap + "Version")
                                       .Where (p => p.Element (ns_doap + "branch").Value.StartsWith ("master"))
                                       .Select (p => new { Revision = p.Element (ns_doap + "revision").Value })
                                       .First ()
                                       .Revision;

                // once we have the version information, the temporary local copy of doap may be deleted
                File.Delete (temp_doap_path);

                if (!String.IsNullOrEmpty (unstable_version) &&
                    Banshee.ServiceStack.Application.Version != "Unknown" &&
                    new Version (Banshee.ServiceStack.Application.Version) < new Version (unstable_version)) {
                    Gtk.Application.Invoke (delegate {
                        DisplayUpdateAvailableDialog ();
                    });
                } else {
                    if (verbose) {
                        DisplayMessage (Catalog.GetString ("No update available"), Catalog.GetString ("You already have the latest version of Banshee installed."), MessageType.Info);
                    }
                }
            }
        }

        public void DisplayUpdateAvailableDialog ()
        {
            bool update;
            using (var message_dialog = new MessageDialog (ServiceManager.Get<GtkElementsService> ().PrimaryWindow, 0,
                    MessageType.Question, ButtonsType.YesNo, String.Format (
                    Catalog.GetString ("A new version of Banshee ({0}) is available.{1}Do you want to update?"), unstable_version, Environment.NewLine))) {
                message_dialog.WindowPosition = WindowPosition.CenterOnParent;
                message_dialog.Title = Catalog.GetString ("Banshee update available");
                update = (message_dialog.Run () == (int)ResponseType.Yes);
                message_dialog.Destroy ();
            }

            if (update) {
                string downloadUrl = String.Format ("{0}/Banshee-{1}.msi", installer_url, unstable_version);

                var downloader = new HttpFileDownloader () {
                    Uri = new Uri (downloadUrl),
                    TempPathRoot = Path.GetTempPath (),
                    FileExtension = "msi"
                };
                downloader.Progress += OnInstallerDownloaderProgress;
                downloader.Finished += OnInstallerDownloaderFinished;
                downloader.Start ();

                temp_installer_path = downloader.LocalPath;

                job = new DownloadManagerJob (this) {
                    // Translators: {0} is the filename, eg Banshee-1.9.5.msi
                    Title = String.Format (Catalog.GetString ("Downloading {0}"), String.Format ("Banshee-{0}.msi", unstable_version)),
                    CanCancel = false
                };

                ServiceManager.Get<JobScheduler> ().Add (job);
            }
        }

        void OnInstallerDownloaderProgress (HttpDownloader obj)
        {
            string downloaded = Math.Round (obj.State.TotalBytesRead / 1024d / 1024d, 1).ToString ("F1");
            string total = Math.Round (obj.State.TotalBytesExpected / 1024d / 1024d, 1).ToString ();
            string rate = Math.Round (obj.State.TransferRate / 1024d, 1).ToString ("F1");

            job.Progress = obj.State.PercentComplete;
            job.Status = String.Format (Catalog.GetString ("{0} MB / {1} MB ({2} KB/s)"), downloaded, total, rate);
        }

        void OnInstallerDownloaderFinished (HttpDownloader obj)
        {
            base.OnDownloaderFinished (obj);

            if (obj.State.FailureException != null) {
                DisplayMessage (Catalog.GetString ("Update download failed"), Catalog.GetString ("The download failed. Please try again later."), MessageType.Error);
            } else {
                Gtk.Application.Invoke (delegate {
                    DisplayUpdateFinishedDownloadingDialog ();
                });
            }
        }

        private void DisplayUpdateFinishedDownloadingDialog ()
        {
            bool update;
            using (var message_dialog = new MessageDialog (ServiceManager.Get<GtkElementsService> ().PrimaryWindow, 0,
                        MessageType.Question, ButtonsType.YesNo, String.Format (
                            Catalog.GetString ("The update finished downloading.{0}Do you want to shutdown Banshee and run the installer?"), Environment.NewLine))) {
                message_dialog.WindowPosition = WindowPosition.CenterOnParent;
                message_dialog.Title = Catalog.GetString ("Update finished downloading");
                update = (message_dialog.Run () == (int)ResponseType.Yes);
                message_dialog.Destroy ();
            }

            if (update) {
                // run the downloaded installer and shutdown the running instance of Banshee
                Process msiProcess = new Process ();
                msiProcess.StartInfo.FileName = "msiexec";
                msiProcess.StartInfo.Arguments = "/i " + temp_installer_path;
                msiProcess.Start ();

                Banshee.ServiceStack.Application.Shutdown ();
            } else {
                // delete the downloaded installer as the user does not want to update now
                File.Delete (temp_installer_path);
            }
        }

        // helper which makes it possible to show Gtk's MessageDialog from a thread
        private void DisplayMessage (string title, string message, MessageType type)
        {
            Gtk.Application.Invoke (delegate {
                using (var message_dialog = new MessageDialog (ServiceManager.Get<GtkElementsService> ().PrimaryWindow, 0, type, ButtonsType.Ok, message)) {
                    message_dialog.WindowPosition = WindowPosition.CenterOnParent;
                    message_dialog.Title = title;
                    message_dialog.Run ();
                    message_dialog.Destroy ();
                }
            });
        }
    }
}
