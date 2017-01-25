using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using IWshRuntimeLibrary;
using MahApps.Metro;
using ZerO.Properties;
using ZerO.Windows;
using static System.String;
using Application = System.Windows.Application;
using ComboBox = System.Windows.Controls.ComboBox;
using File = System.IO.File;
using Orientation = System.Windows.Controls.Orientation;
using RadioButton = System.Windows.Controls.RadioButton;
using TextBox = System.Windows.Controls.TextBox;

namespace ZerO.Helpers
{
    /// <summary>
    /// Helper class to keep track of ALOT of handy functions
    /// </summary>
    public static class Globals
    {
        private static List<Accent> _accents;

        public static List<Accent> Accents => _accents ?? (_accents =
                                                  new List<Accent>
                                                  {
                                                      new Accent("Red",
                                                          new Uri(
                                                              "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Red.xaml")),
                                                      new Accent("Green",
                                                          new Uri(
                                                              "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Green.xaml")),
                                                      new Accent("Blue",
                                                          new Uri(
                                                              "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml")),
                                                      new Accent("Purple",
                                                          new Uri(
                                                              "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Purple.xaml")),
                                                      new Accent("Orange",
                                                          new Uri(
                                                              "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Orange.xaml"))
                                                  });

        public static MainWindow Main;
        public static CMessageBox MsgBox;
        public static FolderBrowser FolderBrowser;
        public static SettingsWindow SettingsWindow;
        public static CustomWindow CWindow;

        public static FolderBrowserDialog FolderBrowserDialog;
        public static OpenFileDialog FileBrowserDialog;
        public static DialogResult DialogResult;

        public static Log.Log Logger;
        public static Theme CurrentTheme;
        public static Accent CurrentAccent;

        public static List<string> RecentFiles;

        public static bool DirectoryEntered, Locked;
        public static List<FileInfo> SearchFiles;

        public static string RootDirectory,
            ProjectsPath,
            MapName,
            MainGamesDirectory,
            ExeFile,
            DefaultEngine,
            MapsFolder;

        public static void Initialize(MainWindow main)
        {
            // Set up main window
            Main = main;

            SearchFiles = new List<FileInfo>();
            DirectoryEntered = Locked = false;

            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            for (int i = 0; i < TextEditor.AllOpenDocs.Count; i++)
                TextEditor.AllOpenDocs[i].Scintilla.ZoomFactor = Settings.Default.ZoomFactor;

            // Only change theme on main window first
            CurrentTheme = Settings.Default.Theme;
            string accent = Settings.Default.Accent != "" ? Settings.Default.Accent : "Red";
            ThemeManager.ChangeTheme(Main, GetAccent(accent), CurrentTheme);

            // Set up and configure logger
            Logger = new Log.Log();
            Logger.ConfigureLogger();
            Logger.Init("Application starting...");

            if (IsNullOrWhiteSpace(UserPrincipal.Current.DisplayName))
                Logger.Init("User " + UserPrincipal.Current.DisplayName + " on machine " + Environment.MachineName);
            else
                Logger.Init("User " + Environment.UserName + " on machine " + Environment.MachineName);

            Application.Current.DispatcherUnhandledException +=
                (sender, args) => Logger.Error("UNHANDLED: " + args.Exception.Message);
        }

        #region Setup

        public static void SetupSettings()
        {
            SettingsWindow = new SettingsWindow();

            Main.ZoomComboBox.Text = Settings.Default.ZoomFactor.ToString();

            if (Settings.Default.LineNumbers) SettingsWindow.LineNumberCheckBox.IsChecked = true;
            if (Settings.Default.LineHighlight) SettingsWindow.LineHighlightCheckBox.IsChecked = true;

            SettingsWindow.RememberCheckBox.IsChecked = Settings.Default.SaveSession;
            SettingsWindow.AutoCompleteCheckBox.IsChecked = Settings.Default.AutoComplete;
            SettingsWindow.FunctionHintCheckBox.IsChecked = Settings.Default.FunctionHint;
            SettingsWindow.AutoUpdateCheckBox.IsChecked = Settings.Default.AutoUpdate;

            if (Settings.Default.AutoInsertList == null) Settings.Default.AutoInsertList = new StringCollection();

            for (int i = 0; i < Settings.Default.AutoInsertList.Count; i++)
            {
                if (Settings.Default.AutoInsertList[i].Equals("("))
                {
                    SettingsWindow.InsertParentCheckBox.IsChecked = true;
                    TextEditor.InsertParenthesis = true;
                }

                else if (Settings.Default.AutoInsertList[i].Equals("["))
                {
                    SettingsWindow.InsertBracketCheckBox.IsChecked = true;
                    TextEditor.InsertBracket = true;
                }

                else if (Settings.Default.AutoInsertList[i].Equals("{"))
                {
                    SettingsWindow.InsertCurlyBracketCheckBox.IsChecked = true;
                    TextEditor.InsertCurlyBracket = true;
                }

                else if (Settings.Default.AutoInsertList[i].Equals("&quot"))
                {
                    SettingsWindow.InsertQuotationCheckBox.IsChecked = true;
                    TextEditor.InsertQuotation = true;
                }

                else if (Settings.Default.AutoInsertList[i].Equals("'"))
                {
                    SettingsWindow.InsertSingleQuotationCheckBox.IsChecked = true;
                    TextEditor.InsertSingleQuotation = true;
                }
            }

            if (Settings.Default.LastTextFiles != null)
            {
                if (Settings.Default.LastTextFiles.Count == 0) return;
                foreach (var t in Settings.Default.LastTextFiles)
                    TextEditor.OpenFile(t);

                SettingsWindow.RememberCheckBox.IsChecked = true;
            }
        }

        public static void SetupWindows()
        {
            MsgBox = new CMessageBox();
            FolderBrowser = new FolderBrowser();
            CWindow = new CustomWindow();

            // Set theme and accent from settings
            ChangeTheme(Settings.Default.Accent != "" ? Settings.Default.Accent : "Red", CurrentTheme);

            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            CWindow.AboutUc.VersionLabel.Content = "UDKManager Editor Version " + fvi.FileVersion;

            FolderBrowserDialog = new FolderBrowserDialog();
            FileBrowserDialog = new OpenFileDialog();
            FolderBrowserDialog.ShowNewFolderButton = false;
        }

        /// <summary>
        /// Lets the user set the root folder, and makes sure it exists
        /// </summary>
        public static void SetRootFolder()
        {
            if (FolderBrowser.Show(Main, BrowseType.Folder, "Specify your UDK install directory.") ==
                FolderBrowserResult.Ok)
            {
                if (Directory.Exists(FolderBrowser.SelectedPath + "/Binaries"))
                {
                    RootDirectory = FolderBrowser.SelectedPath;
                    ProjectsPath = FindFolder("Src");
                    Logger.Info("Projects folder found at " + ProjectsPath);
                    DirectoryEntered = true;
                    Settings.Default.UDKDirectory = RootDirectory;
                    Settings.Default.Save();
                    Logger.Info("UDK directory has been set to " + RootDirectory);
                    SettingsWindow.RootTextBox.Text = RootDirectory;
                    Main.UpdateBrowseList();

                    if (Directory.Exists(RootDirectory + "/Binaries/Win32"))
                    {
                        MainGamesDirectory = RootDirectory + "/Binaries/Win32";
                        ExeFile = MainGamesDirectory + "/UDK.exe";
                        Settings.Default.ExeFile = ExeFile;
                        Settings.Default.Save();
                    }

                    else
                        SetExe();

                    Logger.Info("UDK .exe found at " + ExeFile);
                    SettingsWindow.ExeTextBox.Text = ExeFile;

                    DefaultEngine = FindFile("DefaultEngine.ini", "ini");
                    SetMaps();
                }

                else
                {
                    Logger.Warn("Did not specify the right UDK directory!");
                    MsgBox.Show(Main, "Make sure it has at least the 2 folders 'Binaries' and 'Development' in it.",
                        "This is not the right directory!", MessageBoxButton.OK, MessageBoxIconType.Warning);
                    SetRootFolder();
                }
            }

            else
            {
                if (RootDirectory == "")
                    DirectoryEntered = false;

                Logger.Warn("UDK Root folder not set.");
            }
        }

        /// <summary>
        /// Lets the user set the main .exe to run, and makes sure it exists
        /// </summary>
        public static void SetExe()
        {
            if (DirectoryEntered)
            {
                FolderBrowser.InitialDirectory = RootDirectory;
                FolderBrowser.Filter = ".exe";

                if (FolderBrowser.Show(Main, BrowseType.File, "Specify your UDK .exe file.") != FolderBrowserResult.Ok)
                    return;

                if (!File.Exists(FolderBrowser.SelectedPath)) return;

                ExeFile = FolderBrowser.SelectedPath;
                Settings.Default.ExeFile = ExeFile;
                Settings.Default.Save();
                Logger.Info("UDK .exe file has been set to " + ExeFile);
                SettingsWindow.ExeTextBox.Text = ExeFile;
            }

            else
                NoRoot();
        }

        /// <summary>
        /// Lets the user set the UDK Maps folder, and makes sure it exists
        /// </summary>
        public static void SetMaps()
        {
            if (DirectoryEntered)
            {
                FolderBrowser.InitialDirectory = RootDirectory;

                if (FolderBrowser.Show(Main, BrowseType.Folder, "Specify your UDK Maps folder") !=
                    FolderBrowserResult.Ok) return;
                if (!Directory.Exists(FolderBrowser.SelectedPath)) return;
                MapsFolder = FolderBrowser.SelectedPath;
                Settings.Default.MapsFolder = MapsFolder;
                Settings.Default.Save();
                Logger.Info("UDK Maps folder has been set to " + MapsFolder);
                SettingsWindow.MapsTextBox.Text = FolderBrowser.SelectedPath;
            }

            else
                NoRoot();
        }

        /// <summary>
        /// Checks directories and makes sure they exist
        /// </summary>
        public static void StartupCheck()
        {
            if (!IsNullOrEmpty(Settings.Default.UDKDirectory))
            {
                if (Directory.Exists(Settings.Default.UDKDirectory))
                {
                    Logger.Info("UDK directory found at " + Settings.Default.UDKDirectory);
                    DirectoryEntered = true;
                    RootDirectory = Settings.Default.UDKDirectory;
                    ProjectsPath = FindFolder("Src") + "\\";
                    Logger.Info("Projects folder found at " + ProjectsPath);

                    try
                    {
                        Main.UpdateBrowseList();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Cannot load browserlist: " + ex.Message);
                    }

                    if (!IsNullOrWhiteSpace(Settings.Default.ExeFile))
                    {
                        if (File.Exists(Settings.Default.ExeFile))
                        {
                            ExeFile = Settings.Default.ExeFile;
                            Logger.Info("UDK .exe found at " + Settings.Default.ExeFile);
                            SettingsWindow.ExeTextBox.Text = Settings.Default.ExeFile;
                            //var exeName = ExeFile.Split('/');
                            //SettingsWindow.exeTextBox.Text = exeName[exeName.Length - 1];
                        }
                        else
                            SetExe();
                    }
                    else
                        SetExe();

                    if (!IsNullOrWhiteSpace(Settings.Default.MapsFolder))
                    {
                        if (Directory.Exists(Settings.Default.MapsFolder))
                        {
                            MapsFolder = Settings.Default.MapsFolder;
                            Logger.Info("UDK Maps folder has been set to " + MapsFolder);
                            Main.UpdateBrowseList();
                            SettingsWindow.MapsTextBox.Text = MapsFolder;
                        }
                        else
                            SetMaps();
                    }
                    else
                        SetMaps();

                    SettingsWindow.RootTextBox.Text = RootDirectory;

                    DefaultEngine = FindFile("DefaultEngine.ini", "ini");

                    if (!IsNullOrEmpty(DefaultEngine))
                    {
                        IniFileManager.SetFile(DefaultEngine);
                    }
                }

                else
                    SetRootFolder();
            }

            else
                SetRootFolder();
        }

        #endregion

        public static void NoRoot()
        {
            Logger.Warn("No UDK root folder found, go to settings to set one.");
        }

        /// <summary>
        /// Returns a ready-to-use Bitmap Image
        /// </summary>
        /// <param name="resourceName">Name of the resource in program resources</param>
        public static BitmapImage GetBitmapImage(string resourceName)
        {
            var uri = new Uri("pack://application:,,,/Resources/" + resourceName + ".png");
            var source = new BitmapImage(uri);

            return source;
        }

        /// <summary>
        /// Attempt to clear all controls
        /// </summary>
        public static void ClearControls(DependencyObject obj)
        {
            var ccChildren = new ChildControls();

            foreach (var o in ccChildren.GetChildren(obj, 5))
            {
                if (o.GetType() == typeof(TextBox))
                {
                    var txt = (TextBox)o;
                    txt.Text = string.Empty;
                }

                if (o.GetType() == typeof(ComboBox))
                {
                    var cb = (ComboBox)o;
                    cb.Text = string.Empty;
                }

                if (o.GetType() != typeof(RadioButton)) continue;
                var rb = (RadioButton)o;
                rb.IsChecked = false;
            }
        }

        #region Theme

        /// <summary>
        /// Get an accent color
        /// </summary>
        /// <param name="name">Name of the color</param>
        public static Accent GetAccent(string name)
        {
            return Accents.FirstOrDefault(t => t.Name == name);
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

            Settings.Default.Accent = accent;
            Settings.Default.Theme = theme;
            Settings.Default.Save();
        }

        #endregion

        #region Text Helpers

        /// <summary>
        /// Find the first occurance of a string in text
        /// </summary>
        /// <param name="text">Text to be changed</param>
        /// <param name="search">String to search for</param>
        /// <param name="replace">Replacement string</param>
        public static string ReplaceFirst(string text, string search, string replace)
        {
            var pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        /// <summary>
        /// Iterates through a string to find the first whitespace or tab char 
        /// </summary>
        /// <param name="s">String to iterate through</param>
        public static int FindFirstWhiteSpaceOrTab(string s)
        {
            var indexToReturn = 0;
            var index = 0;
            s = s.TrimEnd('\t', '\n', '\r');
            var tabsCount = s.Split('\t').Length - 1;

            if (s.StartsWith("\t") || s.StartsWith(" "))
            {
                foreach (var c in s)
                {
                    index++;
                    if (c == '\t')
                        return index * tabsCount;

                    if (char.IsWhiteSpace(c))
                        indexToReturn++;
                }
            }

            else
                return index;

            if (indexToReturn != 0)
                return indexToReturn - 2;

            return -1;
        }

        public static string ReplaceString(string str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        #endregion

        #region Files & Folders

        /// <summary>
        /// Find a specific file in the root directory
        /// </summary>
        /// <param name="name">File name</param>
        /// <param name="extension">File extension</param>
        public static string FindFile(string name, string extension)
        {
            var files = Directory.GetFiles(RootDirectory, "*" + extension, SearchOption.AllDirectories);

            foreach (var info in files.Select(file => new FileInfo(file)).Where(info => info.Name == name))
            {
                return info.FullName;
            }

            return "";
        }

        /// <summary>
        /// Get a specific folder in the root directory
        /// </summary>
        public static DirectoryInfo GetFolder(string name)
        {
            var folders = Directory.GetDirectories(RootDirectory, "*", SearchOption.AllDirectories);

            return folders.Select(folder => new DirectoryInfo(folder)).FirstOrDefault(info => info.Name == name);
        }

        /// <summary>
        /// Find a specific folder in the root directory with its full name
        /// </summary>
        /// <param name="name">Folder name</param>
        public static string FindFolder(string name)
        {
            var folders = Directory.GetDirectories(RootDirectory, "*", SearchOption.AllDirectories);

            foreach (var info in folders.Select(folder => new DirectoryInfo(folder)).Where(info => info.Name == name))
            {
                return info.FullName;
            }

            return "";
        }

        /// <summary>
        /// Create in new file in a specified path
        /// </summary>
        /// <param name="path">The path of the new file</param>
        /// <param name="text">If a text document, put any text you want in your new file here</param>
        public static void CreateFile(string path, string text)
        {
            try
            {
                if (text == null)
                    text = "";

                using (var fs = new FileStream(path, FileMode.Create))
                {
                    using (TextWriter tw = new StreamWriter(fs))
                    {
                        tw.WriteLine(text);
                    }
                }

                Logger.Info("Successfully created file at path " + path);
            }

            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// Opens a file or program in a specified path
        /// </summary>
        /// <param name="path">The path of the file/program</param>
        /// <param name="arguments">Parameter arguments</param>
        public static void Open(string path, string arguments)
        {
            try
            {
                if (!IsNullOrEmpty(arguments))
                    Process.Start(path, @"" + arguments);
                else
                    Process.Start(path);
            }

            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
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
                Main.SearchTreeView.Items.Clear();
                SearchFiles.Clear();

                var newCurrentDirectory = new DirectoryInfo(ProjectsPath);

                var directoryArray = newCurrentDirectory.GetDirectories();

                Main.SearchTreeView.IsEnabled = false;
                foreach (var t1 in directoryArray)
                {
                    var fileArray = t1.GetFiles("*.uc", SearchOption.AllDirectories);
                    foreach (var t in fileArray)
                        if (t.Name.Contains(fileName, StringComparison.OrdinalIgnoreCase))
                        {
                            var item = new CustomItem
                            {
                                Header = t.Name,
                                Tag = "search",
                                Path = t.FullName
                            };
                            item.SetImage();

                            Main.SearchTreeView.Items.Add(item);
                            SearchFiles.Add(t);
                        }
                }
                Main.SearchTreeView.IsEnabled = true;

                // If no items with that name, display a message
                if (Main.SearchTreeView.Items.Count == 0)
                    MsgBox.Show(Main, "Cannot find any file with the name '" + fileName + "'.", "Info",
                        MessageBoxButton.OK, MessageBoxIconType.Info);
            }
            catch (Exception excpt)
            {
                Logger.Error(excpt.Message);
            }
        }

        /// <summary>
        /// Create Windows Shortcut
        /// </summary>
        /// <param name="sourceFile">A file you want to make shortcut to</param>
        /// <param name="shortcutFile">Path and shorcut file name including file extension (.lnk)</param>
        public static void CreateShortcut(string sourceFile, string shortcutFile)
        {
            try
            {
                CreateShortcut(sourceFile, shortcutFile, null, null, null, null);
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Create Windows Shorcut
        /// </summary>
        /// <param name="sourceFile">A file you want to make shortcut to</param>
        /// <param name="shortcutFile">Path and shorcut file name including file extension (.lnk)</param>
        /// <param name="description">Shortcut description</param>
        /// <param name="arguments">Command line arguments</param>
        /// <param name="hotKey">Shortcut hot key as a string, for example "Ctrl+F"</param>
        /// <param name="workingDirectory">"Start in" shorcut parameter</param>
        public static void CreateShortcut(string sourceFile, string shortcutFile, string description,
            string arguments, string hotKey, string workingDirectory)
        {
            // Check necessary parameters first:
            if (IsNullOrEmpty(sourceFile))
                throw new ArgumentNullException("sourceFile");
            if (IsNullOrEmpty(shortcutFile))
                throw new ArgumentNullException("shortcutFile");

            // Create WshShellClass instance:
            var wshShell = new WshShellClass();

            // Create shortcut object:
            var shortcut = (IWshShortcut)wshShell.CreateShortcut(shortcutFile);

            // Assign shortcut properties:
            shortcut.TargetPath = sourceFile;
            shortcut.Description = description;
            if (!IsNullOrEmpty(arguments))
                shortcut.Arguments = arguments;
            if (!IsNullOrEmpty(hotKey))
                shortcut.Hotkey = hotKey;
            if (!IsNullOrEmpty(workingDirectory))
                shortcut.WorkingDirectory = workingDirectory;

            // Save the shortcut:
            if (File.Exists(sourceFile))
                shortcut.Save();
            else
                MsgBox.Show(Main, "The file " + sourceFile + " doesn't exist!", "Error", MessageBoxButton.OK,
                    MessageBoxIconType.Error);
        }

        #endregion

        #region TreeView

        public static CustomItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is CustomItem))
                source = VisualTreeHelper.GetParent(source);

            return source as CustomItem;
        }

        public static Function VisualUpwardSearchTwo(DependencyObject source)
        {
            while (source != null && !(source is Function))
                source = VisualTreeHelper.GetParent(source);

            return source as Function;
        }

        // Gotta call the TreeView an ItemsControl to cast it between TreeView and TreeViewItem
        // as you recurse
        public static void SetSelectedItem(ItemsControl parentContainer, object item)
        {
            // check current level of tree
            foreach (var it in parentContainer.Items)
            {
                var currentContainer = (CustomItem)parentContainer.ItemContainerGenerator.ContainerFromItem(it);
                if ((currentContainer == null) || (it != item)) continue;
                currentContainer.IsSelected = true;
                currentContainer.BringIntoView();
            }
            // item is not found at current level, check the kids
            foreach (var it in parentContainer.Items)
            {
                var currentContainer = (CustomItem)parentContainer.ItemContainerGenerator.ContainerFromItem(it);
                if ((currentContainer == null) || (currentContainer.Items.Count <= 0)) continue;
                // Have to expand the currentContainer or you can't look at the children
                currentContainer.IsExpanded = true;
                currentContainer.UpdateLayout();
                {
                    // We found the thing
                    var selectMethod =
                        typeof(CustomItem).GetMethod("Select", BindingFlags.NonPublic | BindingFlags.Instance);

                    selectMethod.Invoke(currentContainer, new object[] { true });
                }
            }
        }

        #endregion
    }

    #region Function
    /// <summary>
    /// Extended Custom StackPanel class for holding more variables
    /// </summary>
    public class Function : StackPanel
    {
        public int Pos;
        public string Id { get; set; }
        public string Content { get; set; }
        public BitmapImage Image { get; set; }

        public void Initialize()
        {
            SetImage();

            Height = 19;
            Orientation = Orientation.Horizontal;

            Children.Add(new Image { Source = Image, Height = 12, Width = 12, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0, 3, 0, 0) });
            Children.Add(new TextBlock { Text = Content, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(3, 0, 0, 5) });
        }

        public void SetImage()
        {
            if (((string)Tag).Equals("function"))
            {
                var uri = new Uri("pack://application:,,,/Resources/function.png");
                Image = new BitmapImage(uri);
            }

            else if (((string)Tag).Equals("event"))
            {
                var uri = new Uri("pack://application:,,,/Resources/event.png");
                Image = new BitmapImage(uri);
            }

            else if (((string)Tag).Equals("state"))
            {
                var uri = new Uri("pack://application:,,,/Resources/state.png");
                Image = new BitmapImage(uri);
            }

            else if (((string)Tag).Equals("variable"))
            {
                var uri = new Uri("pack://application:,,,/Resources/variable.png");
                Image = new BitmapImage(uri);
            }

            else if (((string)Tag).Equals("localVariable"))
            {
                var uri = new Uri("pack://application:,,,/Resources/localVariable.png");
                Image = new BitmapImage(uri);
            }

            else
            {
                var uri = new Uri("pack://application:,,,/Resources/defaultproperties.png");
                Image = new BitmapImage(uri);
            }
        }
    }
    #endregion

    #region FocusHelper
    /// <summary>
    /// Force focus helper
    /// </summary>
    internal static class FocusHelper
    {
        private delegate void MethodInvoker();

        public static void Focus(UIElement element)
        {
            // Focus in a callback to run on another thread, ensuring the main UI thread is initialized by the
            // time focus is set
            ThreadPool.QueueUserWorkItem(delegate (object foo)
            {
                if (foo == null) throw new ArgumentNullException(nameof(foo));
                var elem = (UIElement)foo;
                elem.Dispatcher.Invoke(DispatcherPriority.Normal,
                    (MethodInvoker)delegate
                    {
                        elem.Focus();
                        Keyboard.Focus(elem);
                    });
            }, element);
        }
    }
    #endregion

    #region TagToImageConverter
    [ValueConversion(typeof(string), typeof(bool))]
    public class TagToImageConverter : IValueConverter
    {
        public static TagToImageConverter Instance = new TagToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (((string)value).Equals("project"))
            {
                var uri = new Uri("pack://application:,,,/Resources/project.png");
                var source = new BitmapImage(uri);
                return source;
            }

            if (((string)value).Equals("class"))
            {
                var uri = new Uri("pack://application:,,,/Resources/class.png");
                var source = new BitmapImage(uri);
                return source;
            }

            if (((string)value).Equals("testMap"))
            {
                var uri = new Uri("pack://application:,,,/Resources/testmap.png");
                var source = new BitmapImage(uri);
                return source;
            }

            if (((string)value).Equals("function"))
            {
                var uri = new Uri("pack://application:,,,/Resources/function.png");
                var source = new BitmapImage(uri);
                return source;
            }

            if (((string)value).Equals("event"))
            {
                var uri = new Uri("pack://application:,,,/Resources/event.png");
                var source = new BitmapImage(uri);
                return source;
            }

            if (((string)value).Equals("state"))
            {
                var uri = new Uri("pack://application:,,,/Resources/state.png");
                var source = new BitmapImage(uri);
                return source;
            }

            if (((string)value).Equals("search"))
            {
                var uri = new Uri("pack://application:,,,/Resources/searchResult.png");
                var source = new BitmapImage(uri);
                return source;
            }

            else
            {
                var uri = new Uri("pack://application:,,,/Resources/defaultproperties.png");
                var source = new BitmapImage(uri);
                return source;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
    #endregion

    #region ChildControls
    internal class ChildControls
    {
        private List<object> _lstChildren;

        public List<object> GetChildren(DependencyObject pVParent, int pNLevel)
        {
            if (pVParent == null)
            {
                throw new ArgumentNullException("pVParent is null");
            }

            _lstChildren = new List<object>();

            GetChildControls(pVParent, pNLevel);

            return _lstChildren;

        }

        private void GetChildControls(DependencyObject pVParent, int pNLevel)
        {
            var nChildCount = VisualTreeHelper.GetChildrenCount(pVParent);

            for (var i = 0; i <= nChildCount - 1; i++)
            {
                var v = (Visual)VisualTreeHelper.GetChild(pVParent, i);

                _lstChildren.Add(v);

                if (VisualTreeHelper.GetChildrenCount(v) > 0)
                {
                    GetChildControls(v, pNLevel + 1);
                }
            }
        }
    }
    #endregion
}
