using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms;
using System.Diagnostics;
using IWshRuntimeLibrary;
using System.Runtime.InteropServices;
using MahApps.Metro;
using ProjectManager.Properties;
using System.Windows.Media.Imaging;

namespace ProjectManager
{
    public static class Globals
    {
        public static MainWindow Main;
        public static CMessageBox MsgBox;
        public static FolderBrowser FolderBrowser;
        public static SettingsWindow SettingsWindow;
        public static CustomWindow CWindow;

        public static Log Logger;
        public static Theme CurrentTheme;
        public static Accent CurrentAccent;

        public static bool DirectoryEntered, Locked;
        public static List<FileInfo> SearchFiles;

        public static string RootDirectory, ProjectsPath, MapName;

        public static void Initialize(MainWindow main)
        {
            // Set up all windows
            Main = main;
            MsgBox = new CMessageBox();
            FolderBrowser = new FolderBrowser();
            SettingsWindow = new SettingsWindow();
            CWindow = new CustomWindow();

            SearchFiles = new List<FileInfo>();
            DirectoryEntered = Locked = false;

            // Set theme and accent from settings
            CurrentTheme = Properties.Settings.Default.Theme;
            if (Properties.Settings.Default.Accent != "")
                ChangeTheme(Properties.Settings.Default.Accent, CurrentTheme);

            else
                ChangeTheme("Red", CurrentTheme);

            // Set up and configure logger
            Logger = new Log();
            Logger.ConfigureLogger();
            Logger.Info("Application starting...");

            System.Windows.Application.Current.DispatcherUnhandledException += (sender, args) =>
            {
                Logger.Error(args.Exception);
            };
        }

        public static void NoRoot()
        {
            MsgBox.Show(Main, "No UDK root folder found, go to settings to set one.", "Warning", MessageBoxButton.OK, MessageBoxIconType.Warning);
        }

        private static List<Accent> _accents;
        public static List<Accent> Accents
        {
            get
            {
                return _accents ?? (_accents =
                    new List<Accent>{
                        new Accent("Red", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Red.xaml")),
                        new Accent("Green", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Green.xaml")),
                        new Accent("Blue", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml")),
                        new Accent("Purple", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Purple.xaml")),
                        new Accent("Orange", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Orange.xaml")),
                    });
            }
        }

        /// <summary>
        /// Get an accent color
        /// </summary>
        /// <param name="name">Name of the color</param>
        public static Accent GetAccent(string name)
        {
            for (int i = 0; i < Accents.Count; i++)
                if (Accents[i].Name == name)
                    return Accents[i];

            return null;
        }

        /// <summary>
        /// Change the overall theme and accent of the program
        /// </summary>
        /// <param name="accent">Color name for accent</param>
        /// <param name="theme">Theme type</param>
        public static void ChangeTheme(string accent, Theme theme)
        {
            CurrentAccent = GetAccent(accent);
            CurrentTheme = theme;

            ThemeManager.ChangeTheme(FolderBrowser, CurrentAccent, CurrentTheme);
            ThemeManager.ChangeTheme(SettingsWindow, CurrentAccent, CurrentTheme);
            ThemeManager.ChangeTheme(CWindow, CurrentAccent, CurrentTheme);
            ThemeManager.ChangeTheme(MsgBox, CurrentAccent, CurrentTheme);
            ThemeManager.ChangeTheme(Main, CurrentAccent, CurrentTheme);
        }

        public static CustomItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is CustomItem))
                source = VisualTreeHelper.GetParent(source);

            return source as CustomItem;
        }

        /// <summary>
        /// Returns a ready-to-use Bitmap Image
        /// </summary>
        /// <param name="name">Name of the resource in program resources</param>
        public static BitmapImage GetBitmapImage(string resourceName)
        {
            Uri uri = new Uri("pack://application:,,,/Resources/" + resourceName + ".png");
            BitmapImage source = new BitmapImage(uri);

            return source;
        }

        /// <summary>
        /// Checks directories and makes sure they exist
        /// </summary>
        public static void StartupCheck()
        {
            if (Settings.Default.UDKDirectory != null)
                if (Directory.Exists(Settings.Default.UDKDirectory))
                {
                    Globals.DirectoryEntered = true;
                    Globals.RootDirectory = Settings.Default.UDKDirectory;
                    Globals.ProjectsPath = Globals.RootDirectory + "/Development/Src/";
                    SettingsWindow.rootTextBox.Text = RootDirectory;
                }

                else
                    SetRootFolder();
        }

        /// <summary>
        /// Lets the user set the root folder, and makes sure it exists
        /// </summary>
        public static void SetRootFolder()
        {
            if (FolderBrowser.Show(Main, "Specify your UDK install directory.") == FolderBrowserResult.OK)
            {
                if (Directory.Exists(FolderBrowser.SelectedPath + "/UDKGame"))
                {
                    RootDirectory = FolderBrowser.SelectedPath;
                    Globals.ProjectsPath = Globals.RootDirectory + "/Development/Src/";
                    DirectoryEntered = true;
                    Settings.Default.UDKDirectory = RootDirectory;
                    Settings.Default.Save();
                    Globals.Logger.Info("UDK directory has been set to " + RootDirectory);
                    SettingsWindow.rootTextBox.Text = RootDirectory;
                    Main.UpdateBrowseList();
                }

                else
                {
                    Logger.Error("Did not specify the right folder!");
                    MsgBox.Show(Main, "This is not the right folder!\nMake sure it has the 4 folder 'Binaries', 'Development', 'Engine', 'UDKGame' in it.", "", MessageBoxButton.OK, MessageBoxIconType.Warning);
                    SetRootFolder();
                }
            }

            else
                DirectoryEntered = false;
        }

        /// <summary>
        /// Create in new file in a specified path
        /// </summary>
        /// <param name="path">The path of the new file</param>
        /// <param name="text">If a text document, put any text you want in your new file here</param>
        public static void CreateFile(string path, string text)
        {
            if (text == null)
                text = "";

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (TextWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(text);
                }
            }
        }

        private static ProcessStartInfo startInfo = new ProcessStartInfo();
        /// <summary>
        /// Opens a file or program in a specified path
        /// </summary>
        /// <param name="path">The path of the file/program</param>
        public static void Open(string path)
        {
            // Use ProcessStartInfo class
            startInfo.FileName = path;

            try
            {
                Process process = Process.Start(startInfo);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }

        /// <summary>
        /// Search for a file in the treeview
        /// </summary>
        /// <param name="fileName">File name to search for</param>
        public static void FileSearch(string fileName)
        {
            try
            {
                // Clear the list and treeview in preparation for a new search
                Main.searchTreeView.Items.Clear();
                SearchFiles.Clear();

                DirectoryInfo newCurrentDirectory;
                newCurrentDirectory = new DirectoryInfo(Globals.ProjectsPath);

                FileInfo[] fileArray;
                DirectoryInfo[] directoryArray = newCurrentDirectory.GetDirectories();

                Main.searchTreeView.IsEnabled = false;
                for (int i = 0; i < directoryArray.Length; i++)
                {
                    fileArray = directoryArray[i].GetFiles("*.uc", SearchOption.AllDirectories);
                    for (int j = 0; j < fileArray.Length; j++)
                        if (fileArray[j].Name.Contains(fileName))
                        {
                            CustomItem item = new CustomItem();
                            item.Header = fileArray[j].Name;
                            item.Tag = "c";
                            item.Path = fileArray[j].FullName;
                            Main.searchTreeView.Items.Add(item);
                            SearchFiles.Add(fileArray[j]);
                        }
                }
                Main.searchTreeView.IsEnabled = true;

                // If no items with that name, display a message
                if (Main.searchTreeView.Items.Count == 0)
                    MsgBox.Show(Main, "Cannot find any file with the name '" + fileName + "'.", "Info", MessageBoxButton.OK, MessageBoxIconType.Info);
            }
            catch (System.Exception excpt)
            {
                Logger.Error(excpt.Message);
            }
        }

        /// <summary>
        /// Create Windows Shortcut
        /// </summary>
        /// <param name="SourceFile">A file you want to make shortcut to</param>
        /// <param name="ShortcutFile">Path and shorcut file name including file extension (.lnk)</param>
        public static void CreateShortcut(string SourceFile, string ShortcutFile)
        {
            try
            {
                CreateShortcut(SourceFile, ShortcutFile, null, null, null, null);
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Create Windows Shorcut
        /// </summary>
        /// <param name="SourceFile">A file you want to make shortcut to</param>
        /// <param name="ShortcutFile">Path and shorcut file name including file extension (.lnk)</param>
        /// <param name="Description">Shortcut description</param>
        /// <param name="Arguments">Command line arguments</param>
        /// <param name="HotKey">Shortcut hot key as a string, for example "Ctrl+F"</param>
        /// <param name="WorkingDirectory">"Start in" shorcut parameter</param>
        public static void CreateShortcut(string SourceFile, string ShortcutFile, string Description,
           string Arguments, string HotKey, string WorkingDirectory)
        {
            // Check necessary parameters first:
            if (String.IsNullOrEmpty(SourceFile))
                throw new ArgumentNullException("SourceFile");
            if (String.IsNullOrEmpty(ShortcutFile))
                throw new ArgumentNullException("ShortcutFile");

            // Create WshShellClass instance:
            var wshShell = new WshShellClass();

            // Create shortcut object:
            IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(ShortcutFile);

            // Assign shortcut properties:
            shortcut.TargetPath = SourceFile;
            shortcut.Description = Description;
            if (!String.IsNullOrEmpty(Arguments))
                shortcut.Arguments = Arguments;
            if (!String.IsNullOrEmpty(HotKey))
                shortcut.Hotkey = HotKey;
            if (!String.IsNullOrEmpty(WorkingDirectory))
                shortcut.WorkingDirectory = WorkingDirectory;

            // Save the shortcut:
            if (System.IO.File.Exists(SourceFile))
                shortcut.Save();
            else
                MsgBox.Show(Main, "The file " + SourceFile + " doesn't exist!", "Error", MessageBoxButton.OK, MessageBoxIconType.Error);
        }
    }
}
