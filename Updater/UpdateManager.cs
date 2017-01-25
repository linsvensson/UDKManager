using System.Linq;
using System.Reflection;
using System.Threading;
using Ionic.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;

namespace Updater
{
    public class UpdateManager
    {
        private string _basePath, _zipPath;

        public CustomAppCast AppCast { get; set; }

        /// <summary>
        /// Creates the string composing the theme from Tetris
        /// </summary>
        /// <returns>String containing note-octave-duration of song</returns>
        public static string Victory()
        {
            var song = string.Empty;

            song += "A-4-8,A-4-8,A-4-8,A-4-4,P-16,F-4-4,G-4-4,A-4-4,G-4-8,A-4-4";

            return song;
        }

        public static string Intro()
        {
            var song = string.Empty;

            song += "D-4-8,D-4-8,D-4-8,A-4-8-,P-4,A-4-8,B-4-8,A-4-8,G-4-8,A-4-8,P-8,D-5-4";

            return song;
        }

        // Function for animated chars
        public static void Animate(string text, int sleep)
        {
            foreach (var t in text)
            {
                Console.Write(t);
                Thread.Sleep(sleep);
            }
        }

        public void Install()
        {
            _basePath = AppDomain.CurrentDomain.BaseDirectory;
            _zipPath = Path.Combine(_basePath, AppCast.Title);

            var newCurrentDirectory = new DirectoryInfo(_basePath);
            var fileArray = newCurrentDirectory.GetFiles();

            Animate("basePath " + _basePath + "\n\n", 100);

            FileInfo file = null;

            if (!File.Exists(_basePath + @"UDKManager Editor.exe"))
            {
                Console.WriteLine(_basePath + @"UDKManager Editor.exe not found, downloading again");

                ReDownload(AppCast.MainDownloadLink, Path.Combine(_basePath + @"UDKManager Editor.zip"));

                var fs = File.OpenRead(Path.Combine(_basePath + @"UDKManager Editor.zip"));
                //extract 
                var zipFile = ZipFile.Read(fs);

                foreach (var entry in zipFile.Where(entry => entry.FileName != "Updater.exe" && entry.FileName != "Ionic.Zip.dll" && entry.FileName != "Ionic.Zip.xml"))
                {
                    entry.ZipErrorAction = ZipErrorAction.Retry;
                    entry.Extract(_basePath, ExtractExistingFileAction.OverwriteSilently);
                }

                fs.Flush();
                fs.Close();

                Thread.Sleep(4000);

                if (File.Exists(_basePath + @"UDKManager_Editor.zip"))
                    File.Delete(Path.Combine(_basePath + @"UDKManager_Editor.zip"));

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("\n-------------------------------------------------------\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    Animate("\n\nPress any key to exit . . .", 100);

                    Console.ReadKey(false);
                    Environment.Exit(0);
            }

            Animate("\n\nChecking versions . . .", 70);
            Animate("\n\nGetting version (" + AppCast.Version + ")", 70);

                if (!File.Exists(_zipPath))
                {
                    Console.WriteLine(_zipPath + " not found, downloading again");

                    ReDownload(AppCast.DownloadLink, _zipPath);
                }

                if (File.Exists(_zipPath))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Animate("\n\nUnzipping archive . . .", 60);

                    foreach (var t in fileArray)
                    {
                        File.SetAttributes(t.FullName, FileAttributes.Normal);

                        if (t.Name == "UDKManager Editor.exe")
                            file = t;
                    }

                    Animate("\n\nDeleting unnecessary files . . .", 60);

                    var fs = File.OpenRead(_zipPath);

                    //extract 
                    var zipFile = ZipFile.Read(fs);

                    foreach (
                        var entry in
                            zipFile.Where(
                                entry =>
                                    entry.FileName != "Updater.exe" && entry.FileName != "Ionic.Zip.dll" &&
                                    entry.FileName != "Ionic.Zip.xml"))
                    {
                        entry.ZipErrorAction = ZipErrorAction.Retry;
                        entry.Extract(_basePath, ExtractExistingFileAction.OverwriteSilently);
                    }

                    fs.Flush();
                    fs.Close();

                    Thread.Sleep(4000);

                    if (File.Exists(_zipPath))
                        File.Delete(_zipPath);
                    
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Animate("\n\nUpdate Complete!\n", 70);
                    Tones.PlaySong(Victory());

                    Animate(
                        file != null
                            ? "UDKManager Editor.exe found, will relaunch after termination."
                            : "UDKManager Editor.exe not found, cannot relaunch.", 70);

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("\n-------------------------------------------------------\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    Animate("\n\nPress any key to exit . . .", 100);

                    Console.ReadKey(false);

                    if (file != null)
                        Process.Start(file.FullName);
                    Environment.Exit(0);
                }

                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("-------------------------------------------------------\n");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Animate(
                        AppCast.Title +
                        " not found, remember you can only update through the\nmain 'UDKManager Editor.exe'!", 60);
                    Console.ForegroundColor = ConsoleColor.White;
                    Animate("\n\nPress any key to exit . . .", 100);

                    while (true)
                    {
                        Console.ReadKey();
                        break;
                    }

                    Environment.Exit(0);
            }
        }

        private static void ReDownload(string path, string fileName)
        {
            if (string.IsNullOrEmpty(path)) return;

            var downloadClient = new WebClient();//Declaring the webclient as Download_Client
            var dur = path.Trim();
            downloadClient.DownloadFileAsync(new Uri(dur), fileName);

            while (downloadClient.IsBusy)
                Thread.Sleep(2);

            if (!downloadClient.IsBusy)
                Animate("\n\nDownload finished", 60);
        }

        public UpdateManager(string appcastUrl)
        {
            // set the url
            AppCast = new CustomAppCast(appcastUrl);
            AppCast.CheckForUpdate();
        }
    }

    public class CustomAppCast
    {
        public string Title;
        public string AppVersionInstalled;

        public string Version;
        public string ReleaseNotesLink;
        public string DownloadLink;
        public string MainDownloadLink;

        private readonly string _castUrl;

        private const string TitleNode = "title";
        private const string EnclosureNode = "enclosure";
        private const string ReleaseNotesLinkNode = "releaseNotesLink";
        private const string VersionAttribute = "version";
        private const string UrlAttribute = "url";
        private const string MainUrlAttribute = "mainUrl";

        public CustomAppCast(string castUrl)
        {
            _castUrl = castUrl;
        }

        public void CheckForUpdate()
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
