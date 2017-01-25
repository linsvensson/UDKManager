using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using Ionic.Zip;
using ZerO.Helpers;
using ZerO.Windows;

namespace ZerO
{
    /// <summary>
    /// Class for managing updates
    /// </summary>
    public class Updater
    {
        private FileInfo[] _fileArray;
        private string _basePath, _zipPath;
        private FileInfo _updater;
        private Thread _task;
        private WebClient _downloadClient;
        private ConsoleWindow _consoleWindow;
        private int _progress;
        private string _text = string.Empty;

        public CustomAppCast AppCast { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Updater(string appcastUrl)
        {
            // set the url
            AppCast = new CustomAppCast(appcastUrl);
        }

        /// <summary>
        /// Start the actual update process
        /// </summary>
        public void StartUpdater()
        {
            if (_updater != null)
            {
                if (
                    Globals.MsgBox.Show(Globals.Main, "UDKManager Editor will now terminate itself and start the updater.",
                        "Preparing for update", MessageBoxButton.OK, MessageBoxIconType.Info) != MessageBoxResult.OK)
                    return;
                Globals.CWindow.HideWindow();
                _consoleWindow.Close();
                Process.Start(_updater.FullName);
                Globals.Main.Main.Shutdown();
            }

            else
            {
                Globals.MsgBox.Show(Globals.Main, "Could not find the file 'Updater.exe', has it been moved?", "Error", MessageBoxButton.OK, MessageBoxIconType.Error);
                _consoleWindow.Hide();
                _consoleWindow = null;
                Globals.CWindow.HideWindow();
            }
        }

        private void DownloadFinished()
        {
            _basePath = AppDomain.CurrentDomain.BaseDirectory;

            var newCurrentDirectory = new DirectoryInfo(_basePath);
            _fileArray = newCurrentDirectory.GetFiles();

            _updater = Array.Find(_fileArray, x => x.Name.Equals("Updater.exe"));

            UpdateUpdater();
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(StartUpdater));
        }

        /// <summary>
        /// Initial update check
        /// </summary>
        public void CheckForUpdate()
        {
            if (!AppCast.CheckForUpdate()) return;
            if (Globals.MsgBox.Show(Globals.Main, "Do you want to update?", "Update Found!", MessageBoxButton.YesNo, MessageBoxIconType.Question) == MessageBoxResult.Yes)
                Globals.CWindow.ShowUpdate(Globals.Main);
        }

        /// <summary>
        /// Check for updates
        /// </summary>
        public void ForceUpdateCheck()
        {
            if (AppCast.CheckForUpdate())
            {
                if (Globals.MsgBox.Show(Globals.Main, "Do you want to update?", "Update Found!", MessageBoxButton.YesNo, MessageBoxIconType.Question) == MessageBoxResult.Yes)
                    Globals.CWindow.ShowUpdate(Globals.Main);              
            }

            else
                Globals.MsgBox.Show(Globals.Main, "Your poo is up to date!", "Up to date", MessageBoxButton.OK, MessageBoxIconType.Info);
        }

        /// <summary>
        /// Download the update
        /// </summary>
        public void DownloadUpdate()
        {
            _consoleWindow = new ConsoleWindow();
            _consoleWindow.Show();

            Globals.CWindow.HideWindow();

            if (!string.IsNullOrEmpty(AppCast.DownloadLink))
            {
                _basePath = AppDomain.CurrentDomain.BaseDirectory;

                _zipPath = Path.Combine(_basePath, AppCast.Title);

                // Check if the file exists and is accessible
                if (File.Exists(_zipPath))
                {
                    Globals.Logger.Info("File " + _zipPath + " exists and is accessible");

                    Thread.Sleep(1000);

                    _consoleWindow.Cc.ConsoleOutput.Add("------------------------------------------------\n");
                    _consoleWindow.Cc.ConsoleOutput.Add("Fetching update from\n" + AppCast.DownloadLink);
                    Thread.Sleep(1000);

                    _consoleWindow.Cc.ConsoleOutput.Add("\n\nLoading . . .\n");
                    Thread.Sleep(2000);
                }
            }

            _downloadClient = new WebClient();
            _downloadClient.DownloadProgressChanged += ProgressChanged;
            _downloadClient.DownloadFileCompleted += Completed;

            _task = new Thread(() =>
            {
                try
                {
                    var dur = AppCast.DownloadLink.Trim();
                    _downloadClient.DownloadFileAsync(new Uri(dur), _zipPath);
                }

                catch (Exception ex)
                {
                    Globals.Logger.Error("Thread fail: " + ex.Message);
                    _task.Join();
                    if (Globals.CWindow.Visibility == Visibility.Visible)
                        Globals.CWindow.HideWindow();
                }
            });

            TaskScheduler.FromCurrentSynchronizationContext();

            _task.SetApartmentState(ApartmentState.STA);
            _task.Start();
        }

        /// <summary>
        /// Update Complete Event
        /// </summary>
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            _task.Abort();
            _task.Join();
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => _consoleWindow.Write("\n\nDownload Complete . . .\n")));
            Globals.Logger.Info("Download Complete...");
            DownloadFinished();
        }

        /// <summary>
        /// Progress Changed Event
        /// </summary>
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _progress = e.ProgressPercentage;

            if (_text != "")
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => 
                    _consoleWindow.Cc.ConsoleOutput.RemoveAt(_consoleWindow.Cc.ConsoleOutput.Count-1)));

            if (_progress <= 20)
                _text = "██ 20%";
            else if (_progress <= 40)
                _text = "███ 40%";
            else if (_progress <= 60)
                _text = "████ 60%";
            else if (_progress <= 80)
                _text = "█████ 80%";
            else if (_progress <= 100)
                _text = "██████ 100%";

            if (_text != "")
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => _consoleWindow.Cc.ConsoleOutput.Add(_text)));
        }

        /// <summary>
        /// Update the updater
        /// </summary>
        private void UpdateUpdater()
        {
            var fs = File.OpenRead(_zipPath);

            //extract 
            var zipFile = ZipFile.Read(fs);

            try
            {
                foreach (var entry in from entry in zipFile.Where(entry => entry.FileName == "Updater.exe" || 
                    entry.FileName == "Updater.svhost.exe"|| 
                    entry.FileName == "Updater.vshost.exe.manifest") from t in _fileArray where entry.LastModified < t.LastWriteTime select entry)
                {
                    entry.ZipErrorAction = ZipErrorAction.Retry;
                    entry.Extract(_basePath, ExtractExistingFileAction.OverwriteSilently);
                }

                Globals.Logger.Info("Successfully extracted the Updater");
            }

            catch (Exception ex) { Globals.Logger.Error("Error trying to update the Updater: " + ex.Message); }

            fs.Flush();
            fs.Close();
        }
    }

    /// <summary>
    /// Custom class for holding the appcast information
    /// </summary>
    public class CustomAppCast
    {
        private readonly string _castUrl;

        private const string TitleNode = "title";
        private const string EnclosureNode = "enclosure";
        private const string ReleaseNotesLinkNode = "releaseNotesLink";
        private const string VersionAttribute = "version";
        private const string UrlAttribute = "url";
        private const string MainUrlAttribute = "mainUrl";

        public string Title;
        public string AppVersionInstalled;
        public string Version;
        public string ReleaseNotesLink;
        public string DownloadLink;
        public string MainDownloadLink;

        /// <summary>
        /// Constructor
        /// </summary>
        public CustomAppCast(string castUrl)
        {
            _castUrl = castUrl;
        }

        /// <summary>
        /// Check appcast from the URL and compare to see if you need to update
        /// </summary>
        public bool CheckForUpdate()
        {
            // build a http web request stream
            var request = WebRequest.Create(_castUrl);

            // request the cast and build the stream
            var response = request.GetResponse();

            var inputstream = response.GetResponseStream();

            if (inputstream != null)
            {
                var reader = new XmlTextReader(inputstream);
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element) continue;
                    switch (reader.Name)
                    {
                        case ReleaseNotesLinkNode:
                        {
                            ReleaseNotesLink = reader.ReadString();
                            ReleaseNotesLink = ReleaseNotesLink.Trim('\n');
                            break;
                        }
                        case EnclosureNode:
                        {
                            Version = reader.GetAttribute(VersionAttribute);
                            DownloadLink = reader.GetAttribute(UrlAttribute);
                            MainDownloadLink = reader.GetAttribute(MainUrlAttribute);
                            Title = reader.GetAttribute(TitleNode);
                            break;
                        }
                    }
                }

                reader.Close();
            }

            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            AppVersionInstalled = fvi.FileVersion;

            return CompareVersions(AppVersionInstalled, Version) < 0;
        }

        /// <summary>
        /// Compare versions of form "1,2,3,4" or "1.2.3.4". Throws FormatException
        /// in case of invalid version.
        /// </summary>
        /// <param name="strA">the first version</param>
        /// <param name="strB">the second version</param>
        /// <returns>less than zero if strA is less than strB, equal to zero if
        /// strA equals strB, and greater than zero if strA is greater than strB</returns>
        public static int CompareVersions(string strA, string strB)
        {
            var vA = new Version(strA);
            var vB = new Version(strB);

            return vA.CompareTo(vB);
        }
    }
}
