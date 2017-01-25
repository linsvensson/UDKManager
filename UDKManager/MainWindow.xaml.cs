using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ScintillaNET;
using Xceed.Wpf.AvalonDock.Layout;
using ZerO.Helpers;
using ZerO.Properties;
using ZerO.UserControls;
using ZerO.Windows;
using Action = System.Action;
using TabControl = System.Windows.Forms.TabControl;
using ThreadState = System.Threading.ThreadState;

namespace ZerO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly string[] _zoomFactors = { "-6", "-4", "-2", "0", "2", "4", "6" };
        private readonly string[] _functionStrings = { "defaultproperties", "function", "simulated", "static", "exec", "state" };
        private readonly List<TreeViewItem> _expandedFolders = new List<TreeViewItem>();

        private List<Range> _functions = new List<Range>();
        private List<Range> _events = new List<Range>();
        private List<Range> _states = new List<Range>();
        private List<Range> _localVars = new List<Range>();
        private List<Range> _vars = new List<Range>();
        private Thread _resizeThread;
        private TextDocument _lastDocument;

        public Updater Updater { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.LowQuality);
            RenderOptions.SetCachingHint(this, CachingHint.Cache);
        }

        /// <summary>
        /// MainWindow Loaded
        /// </summary>
        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            try { TextEditor.Initialize(); Globals.Initialize(this); }
            catch (Exception ex) { Globals.Logger.Error("Error initializing: " + ex.Message); }

            try { Globals.SetupSettings(); Globals.SetupWindows(); }
            catch (Exception ex) { Globals.Logger.Error("Error setting up windows: " + ex.Message); }

            Dispatcher.BeginInvoke(new Action(MainWindow_Rendered), DispatcherPriority.ContextIdle, null);
        }

        private void MainWindow_Rendered()
        {
            try { Globals.StartupCheck(); }
            catch (Exception ex) { Globals.Logger.Error("Error performing startup check: " + ex.Message); }

            try { UpdateMapList(); }
            catch (Exception ex) { Globals.Logger.Error("Error updating map list: " + ex.Message); }

            try
            {
                Updater = new Updater("https://dl.dropboxusercontent.com/u/41918503/PinkPoo/AppCast.xml");
                Updater.CheckForUpdate();
            }
            catch (Exception ex) { Globals.Logger.Warn("Could not connect to update server: " + ex.Message); Globals.MsgBox.Show(this, "Could not connect to update server!", "Update Failed", MessageBoxButton.OK, MessageBoxIconType.Error); }

            ZoomComboBox.ItemsSource = _zoomFactors;
            ZoomComboBox.SelectedValue = "0";

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            PreviewKeyDown += main_PreviewKeyDown;
            KeyDown += main_KeyDown;

            Keyboard.Focus(LogRichTextBox);
            LogRichTextBox.Focus();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (Width == e.NewSize.Width && Height == e.NewSize.Height) return;
                if (_resizeThread == null)
                    _resizeThread = new Thread(() => ResizeThread(e));

                if (_resizeThread.ThreadState == ThreadState.Unstarted)
                    _resizeThread.Start();
                else if (_resizeThread.ThreadState == ThreadState.Stopped ||
                         _resizeThread.ThreadState == ThreadState.Aborted)
                {
                    _resizeThread = new Thread(() => ResizeThread(e));
                    _resizeThread.Start();
                }
            }

            catch (Exception ex) { Globals.Logger.Warn("Resize thread error " + ex.Message); }
        }

        private void ResizeThread(SizeChangedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    if (Globals.Main == null) return;

                    foreach (var item in Globals.Main.DocumentPane.Children.OfType<LayoutDocument>())
                    {
                        var panel = item.Content as StackPanel;
                        StackPanel boxPanel = null;
                        TextDocument doc = null;

                        for (int i = 0; i < panel.Children.Count; i++)
                        {
                            var child = panel.Children[i] as StackPanel;
                            if (child != null)
                                boxPanel = child;
                            else if (panel.Children[i] is TextDocument)
                                doc = (TextDocument)panel.Children[i];
                        }

                        if (boxPanel == null) continue;
                        boxPanel.Width = e.NewSize.Width;
                        var cbOne = boxPanel.Children[0] as ComboBox;
                        var cbTwo = boxPanel.Children[1] as ComboBox;
                        if (cbOne != null) cbOne.Width = e.NewSize.Width / 2 - 3;
                        if (cbTwo != null) cbTwo.Width = e.NewSize.Width / 2 - 3;

                        panel.Height = e.NewSize.Height;

                        if (doc != null)
                            doc.Height = e.NewSize.Height - boxPanel.Height * 2f;
                    }

                    _resizeThread.Abort();
                    _resizeThread.Join();
                 }));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        /// <summary>
        /// MainWindow KeyDown
        /// </summary>
        private static void main_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.F5))
            {
                try
                {
                    if (Globals.DirectoryEntered)
                    {
                        if (!string.IsNullOrWhiteSpace(Globals.ExeFile))
                        {
                            Globals.Open(Globals.ExeFile, "make");
                            Globals.Logger.Init("Compiling scripts...");
                        }

                        else
                            Globals.Logger.Error("Exe file not found, cannot compile");

                    }
                    else
                        Globals.NoRoot();
                }

                catch (Exception ex) { Globals.Logger.Error("Error when trying to compile: " + ex.Message); }
            }

            // If Ctrl is down
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (Keyboard.IsKeyDown(Key.N))
                {
                    try { TextEditor.OpenNewDocument(); }
                    catch (Exception ex) { Globals.Logger.Error("Error when opening a new document: " + ex.Message); }
                }

                else if (Keyboard.IsKeyDown(Key.O))
                {
                    try { TextEditor.Open(); }
                    catch (Exception ex) { Globals.Logger.Error("Error when opening an existing document: " + ex.Message); }
                }

                else if (Keyboard.IsKeyDown(Key.W))
                {
                    try { TextEditor.Close(TextEditor.GetActiveTab); }
                    catch (Exception ex) { Globals.Logger.Error("Close error: " + ex.Message); }
                }

                else if (Keyboard.IsKeyDown(Key.S))
                {
                    try
                    {
                        if (Globals.DirectoryEntered)
                            TextEditor.Save();
                    }

                    catch (Exception ex) { Globals.Logger.Error("Error when using command Ctrl+S: " + ex.Message); }
                }

                else if (Keyboard.IsKeyDown(Key.F))
                {
                    try 
                    {
                        if (TextEditor.GetActiveTab == null) return;
                        var content = TextEditor.GetActiveTab.Content as TextDocument;

                        content?.Scintilla.FindReplace.ShowFind();
                    }

                    catch (Exception ex) { Globals.Logger.Error("Error when using command Ctrl+F: " + ex.Message); }
                }
            }

            // If Alt is down
            else if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                if (!Keyboard.IsKeyDown(Key.S)) return;
                try
                {
                    if (Globals.DirectoryEntered)
                        TextEditor.SaveAll();
                }

                catch (Exception ex) { Globals.Logger.Error("Error when using command Alt+S: " + ex.Message); }
            }
        }

        /// <summary>
        /// MainWindow PreviewKeyDown
        /// </summary>
        private void main_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
            {
                switch (e.SystemKey)
                {
                    case Key.W:
                        try { TextEditor.CloseAllTabs(); }
                        catch (Exception ex) { Globals.Logger.Error("CloseAll error: " + ex.Message); }
                        break;
                    case Key.F:
                        try
                        {
                            if (Globals.DirectoryEntered)
                                Globals.CWindow.ShowInput(this, null, CustomType.Other, UserControls.Action.Search);
                        }
                        catch (Exception ex) { Globals.Logger.Error("Error when using command Alt+F: " + ex.Message); }
                        break;
                    case Key.S:
                        try
                        {
                            if (Globals.DirectoryEntered)
                                TextEditor.SaveAll();
                        }
                        catch (Exception ex) { Globals.Logger.Error("Error when using command Alt+S: " + ex.Message); }
                        break;
                    default:
                        e.Handled = true;
                        break;
                }

                e.Handled = true;
            }

            if (e.Key != Key.Tab) return;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                var current = 0;

                // get selected tab
                var selectedTab = TextEditor.GetActiveTab;

                for (var i = 0; i < DocumentPane.ChildrenCount; i++)
                    if (DocumentPane.Children[i].Equals(selectedTab))
                        current = i;

                if (current == DocumentPane.ChildrenCount - 1)
                    current = 0;
                else
                    current++;

                var doc = DocumentPane.Children[current] as LayoutDocument;
                DockManager.ActiveContent = doc;

                Focus();
            }

            e.Handled = true;
        }

        private void typeTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// DockManager ActiveContent Changed
        /// </summary>
        private void dockManager_ActiveContentChanged(object sender, EventArgs e)
        {
            var isVisible = false;

            var selectedIndex = 0;

            var mruFind = new List<string>();
            var mruReplace = new List<string>();

            if (_lastDocument != null)
            {
                if (!(Equals(_lastDocument, DockManager.ActiveContent)))
                {
                    isVisible = _lastDocument != null && _lastDocument.Scintilla.FindReplace.Window.Visible;
                    if (isVisible)
                    {
                        // Save the selected tabIndex
                        var tabControl = _lastDocument.Scintilla.FindReplace.Window.Controls.Find("tabAll", true)[0] as TabControl;
                        if (tabControl != null) selectedIndex = tabControl.SelectedIndex;

                        // Save the search text, if there is any
                        var searchTbOne = _lastDocument.Scintilla.FindReplace.Window.Controls.Find("cboFindF", true)[0] as System.Windows.Forms.ComboBox;
                        if (searchTbOne != null && !mruFind.Contains(searchTbOne.Text) && !string.IsNullOrEmpty(searchTbOne.Text)) mruFind.Add(searchTbOne.Text);

                        var searchTbTwo = _lastDocument.Scintilla.FindReplace.Window.Controls.Find("cboFindR", true)[0] as System.Windows.Forms.ComboBox;
                        if (searchTbTwo != null && !mruFind.Contains(searchTbTwo.Text) && !string.IsNullOrEmpty(searchTbTwo.Text)) mruFind.Add(searchTbTwo.Text);

                        // Save the replace text, if there is any
                        var replaceTb = _lastDocument.Scintilla.FindReplace.Window.Controls.Find("cboReplace", true)[0] as System.Windows.Forms.ComboBox;
                        if (replaceTb != null && !mruReplace.Contains(replaceTb.Text) && !string.IsNullOrEmpty(replaceTb.Text)) mruReplace.Add(replaceTb.Text);

                        _lastDocument.Scintilla.FindReplace.Window.Close();
                    }
                }
            }

            var text = DockManager.ActiveContent as TextDocument;
            if (text != null)
                _lastDocument = text;

            if (text?.Content != null)
            {
                if (isVisible)
                {
                    if (text.Scintilla.FindReplace.Window == null || text.Scintilla.FindReplace.Window.Scintilla == null || text.Scintilla.FindReplace.Window.Equals(_lastDocument.Scintilla.FindReplace.Window))
                        text.Scintilla.FindReplace.Window = new FindReplaceDialog { Scintilla = text.Scintilla };

                    // Set the saved tabIndex
                    var tabControl = _lastDocument.Scintilla.FindReplace.Window.Controls.Find("tabAll", true)[0] as TabControl;
                    if (tabControl != null) tabControl.SelectedIndex = selectedIndex;

                    if (mruFind.Count != 0)
                    {
                        text.Scintilla.FindReplace.Window.MruFind = mruFind;
                    }

                    if (mruReplace.Count != 0)
                        text.Scintilla.FindReplace.Window.MruReplace = mruReplace;

                    text.Scintilla.FindReplace.Window.Scintilla = text.Scintilla;
                    text.Scintilla.FindReplace.Window.Show();
                    if (!text.Scintilla.FindReplace.Window.Focused)
                        text.Scintilla.FindReplace.Window.Focus();
                }
                text.ResetInfo();
                text.Scintilla.Capture = false;
            }

            ExploreScript();
        }

        /// <summary>
        /// MainWindow Closing
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                if (Settings.Default.LastTextFiles != null)
                    Settings.Default.LastTextFiles.Clear();

                if (Settings.Default.LastTextFiles == null)
                    Settings.Default.LastTextFiles = new StringCollection();

                foreach (var t in TextEditor.AllOpenDocs.Where(t => !string.IsNullOrEmpty(t.FilePath)))
                    Settings.Default.LastTextFiles.Add(t.FilePath);

                Settings.Default.Save();
            }
            catch (Exception ex) { Globals.Logger.Error("Error when saving current session: " + ex.Message); }

            try { TextEditor.CloseAllTabs(); }
            catch (Exception ex) { Globals.Logger.Error("Error when shutting down (you can probably ignore this if it terminated correctly: " + ex.Message); }

            base.OnClosing(e);
            Shutdown();
        }

        /// <summary>
        /// MainWindow ÜBERSHUTDOWN
        /// </summary>
        public void Shutdown()
        {
            Globals.Logger.Init("Shutting down...");

            if (Globals.MsgBox != null) Globals.MsgBox.Close();
            if (Globals.FolderBrowser != null) Globals.FolderBrowser.Close();
            if (Globals.CWindow != null) Globals.CWindow.Close();
            if (Globals.SettingsWindow != null) Globals.SettingsWindow.Close();

            Application.Current.Shutdown();
        }

        private void compileButton_Click(object sender, RoutedEventArgs e)
        {
            //TextEditor.AutoFormatCode();
            try
            {
                if (Globals.DirectoryEntered)
                {
                    if (!string.IsNullOrWhiteSpace(Globals.ExeFile))
                    {
                        Globals.Open(Globals.ExeFile, "make");
                        Globals.Logger.Init("Compiling scripts...");
                    }

                    else
                        Globals.Logger.Error("Exe file not found, cannot compile");

                }
                else
                    Globals.NoRoot();
            }

            catch (Exception ex) { Globals.Logger.Error("Error when trying to compile: " + ex.Message); }
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            try { Globals.SettingsWindow.Show(this); }
            catch (Exception ex) { Globals.Logger.Error("Error opening settings window: " + ex.Message); }
        }

        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            try { Globals.CWindow.ShowAbout(this); }
            catch (Exception ex) { Globals.Logger.Error("Error opening about window: " + ex.Message); }
        }

        /// <summary>
        /// Checks the active TextDocument for functions and events
        /// </summary>
        public void ExploreScript()
        {
            try
            {
                if (TextEditor.GetActiveTab == null) return;
                var activePanel = DockManager.ActiveContent as StackPanel;
                if (activePanel == null) return;

                TextDocument activeDoc = null;
                for (int i = 0; i < activePanel.Children.Count; i++)
                {
                    var doc = activePanel.Children[i] as TextDocument;
                    if (doc != null)
                        activeDoc = doc;
                }

                if (activeDoc == null) return;

                //exploreTreeView.Items.Clear();
                activeDoc.FuncComboBox.Items.Clear();

                _events = activeDoc.Scintilla.FindReplace.FindAll("event ", SearchFlags.WordStart);
                _functions = activeDoc.Scintilla.FindReplace.FindAll("function ", SearchFlags.WordStart);
                _states = activeDoc.Scintilla.FindReplace.FindAll("state ", SearchFlags.WordStart);

                string id;
                foreach (var function in _functions)
                    if (function.StartingLine.Text.StartsWith("simulated") ||
                        function.StartingLine.Text.TrimStart().StartsWith("static") || function.StartingLine.Text.TrimStart().StartsWith("exec") ||
                            function.StartingLine.Text.TrimStart().StartsWith("function") || function.StartingLine.Text.TrimStart().StartsWith(" function"))

                {
                    var func = new Function
                    {
                        Content = function.StartingLine.Text.TrimStart(),
                        Pos = function.StartingLine.Number,
                        Tag = "function"
                    };
                    func.Initialize();
                    id = func.Content;

                    id = _functionStrings.Aggregate(id, (current, t) => current.Replace(t, ""));

                    func.Id = id.TrimStart();
                    activeDoc.FuncComboBox.Items.Add(func);
                }

                foreach (var func in from t in _events where t.StartingLine.Text.StartsWith("simulated") ||
                                         t.StartingLine.Text.TrimStart().StartsWith("static") || t.StartingLine.Text.TrimStart().StartsWith("exec") ||
                                         t.StartingLine.Text.TrimStart().StartsWith("event") || t.StartingLine.Text.TrimStart().StartsWith(" event")
                                     select new Function { Content = t.StartingLine.Text.TrimStart(), Pos = t.StartingLine.Number, Tag = "event" })
                {
                    func.Initialize();
                    id = _functionStrings.Aggregate(func.Content, (current, t) => current.Replace(t, ""));

                    func.Id = id.TrimStart();

                    activeDoc.FuncComboBox.Items.Add(func);
                }

                string[] split;
                foreach (var func in from t in _states
                                     where t.StartingLine.Text.TrimStart().StartsWith("state") || t.StartingLine.Text.TrimStart().StartsWith(" state") ||
                                     t.StartingLine.Text.TrimStart().StartsWith("auto")
                                     select new Function { Content = t.StartingLine.Text.TrimStart(), Pos = t.StartingLine.Number, Tag = "state" })
                {
                    func.Initialize();
                    split = func.Content.Split(' ');
                    func.Id = split[split.Length - 1];

                    activeDoc.FuncComboBox.Items.Add(func);
                }

                var defaultproperties = activeDoc.Scintilla.FindReplace.FindAll("defaultproperties", SearchFlags.Posix);
                foreach (var func in from t in defaultproperties
                                     where t.StartingLine.Text.TrimStart().StartsWith("defaultproperties", StringComparison.OrdinalIgnoreCase)
                                     select new Function { Content = t.StartingLine.Text, Pos = t.StartingLine.Number, Tag = "defaultProperties" })
                {
                    func.Initialize();
                    split = func.Content.Split(' ');
                    func.Id = split[split.Length - 1];

                    activeDoc.FuncComboBox.Items.Add(func);
                }

                activeDoc.FuncComboBox.SelectedIndex = 0;
                activeDoc.FuncComboBox.Items.Refresh();
                activeDoc.FuncComboBox.Items.SortDescriptions.Clear();
                activeDoc.FuncComboBox.Items.SortDescriptions.Add(new SortDescription("Tag", ListSortDirection.Ascending));
                activeDoc.FuncComboBox.Items.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Ascending));


                activeDoc.VarComboBox.Items.Clear();
                _localVars = activeDoc.Scintilla.FindReplace.FindAll("local ", SearchFlags.WordStart);
                _vars = activeDoc.Scintilla.FindReplace.FindAll("var ", SearchFlags.WordStart);
                foreach (var func in from t in _localVars
                                     where t.StartingLine.Text.StartsWith("local") || t.StartingLine.Text.TrimStart().StartsWith("local ")
                                     select new Function { Content = t.StartingLine.Text.TrimStart(), Pos = t.StartingLine.Number, Tag = "localVariable" })
                {
                    func.Initialize();
                    func.Id = func.Content;

                    activeDoc.VarComboBox.Items.Add(func);
                }

                foreach (var func in from t in _vars
                                     where t.StartingLine.Text.TrimStart().StartsWith("var") || t.StartingLine.Text.TrimStart().StartsWith("var ") ||
                                     t.StartingLine.Text.TrimStart().StartsWith("var() ")
                                     select new Function { Content = t.StartingLine.Text.TrimStart(), Pos = t.StartingLine.Number, Tag = "variable" })
                {
                    func.Initialize();
                    func.Id = func.Content;

                    activeDoc.VarComboBox.Items.Add(func);
                }

                activeDoc.VarComboBox.SelectedIndex = 0;
                activeDoc.VarComboBox.Items.Refresh();
                activeDoc.VarComboBox.Items.SortDescriptions.Clear();
                activeDoc.VarComboBox.Items.SortDescriptions.Add(new SortDescription("Tag", ListSortDirection.Ascending));
                activeDoc.VarComboBox.Items.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Ascending));
            }

            catch (Exception ex) { Globals.Logger.Error("Cannot load projects and/or classes: " + ex.Message); }
        }

        /// <summary>
        /// Repopulates the Test Map TreeView
        /// </summary>
        public void UpdateMapList()
        {
            try
            {
                TestMapTreeView.Items.Clear();

                if (!string.IsNullOrEmpty(Globals.MapsFolder))
                {
                    var dir = Globals.MapsFolder + "\\TestMaps";

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);


                    var newCurrentDirectory = new DirectoryInfo(dir);

                    var fileArray = newCurrentDirectory.GetFiles("*.bat", SearchOption.AllDirectories);

                    TestMapTreeView.IsEnabled = false;
                    foreach (var tempItem in fileArray.Select(t => new CustomItem {Header = t.Name, Path = t.FullName, Tag = "testMap"}))
                    {
                        tempItem.SetImage();
                        TestMapTreeView.Items.Add(tempItem);
                    }
                    TestMapTreeView.IsEnabled = true;
                }

                else
                    Globals.Logger.Warn("Maps folder not set.");
            }

            catch (Exception ex) { Globals.Logger.Error("Cannot load testmaps: " + ex.Message); }
        }

        /// <summary>
        /// Repopulates the Browse TreeView
        /// </summary>
        public void UpdateBrowseList(string lastSelected = "")
        {
            _expandedFolders.Clear();
            foreach (var node in BrowseTreeView.Items.Cast<object>().OfType<TreeViewItem>().Where(node => node.IsExpanded))
            {
                _expandedFolders.Add(node);
            }

            CustomItem item;

            if (BrowseTreeView.SelectedItem != null)
            {
                item = BrowseTreeView.SelectedItem as CustomItem;

                if (lastSelected == "")
                    if (item != null) lastSelected = item.Header.ToString();
            }

            try
            {
                BrowseTreeView.Items.Clear();

                var newCurrentDirectory = new DirectoryInfo(Globals.ProjectsPath);
                var directoryArray = newCurrentDirectory.GetDirectories();
                DirectoryInfo iniFolder = null;
                string[] iniSplit = null;

                if (Globals.MapsFolder != null)
                    iniSplit = Globals.MapsFolder.Split('\\');
                var iniPath = "";

                if (iniSplit != null)
                {
                    for (int i = 0; i < iniSplit.Length - 2; i++)
                        iniPath += iniSplit[i] + "\\";
                    iniPath = iniPath + "Config";

                    iniFolder = new DirectoryInfo(iniPath);
                }

                BrowseTreeView.IsEnabled = false;
                foreach (var t in directoryArray)
                {
                    var projectDir = Globals.ProjectsPath + "\\" + t.Name + "\\Classes\\";

                    if (!Directory.Exists(projectDir)) continue;
                    var newCurrentDirectory0 = new DirectoryInfo(projectDir);
                    var fileArray = newCurrentDirectory0.GetFiles("*uc");

                    var tempItem = new CustomItem {Header = t.Name, Path = t.FullName};

                    foreach (var subItem in fileArray.Select(t1 => new CustomItem
                    {
                        Header = t1.Name,
                        Path = t1.FullName,
                        Tag = "class",
                        ParentFolder = tempItem
                    }))
                    {
                        subItem.SetImage();
                        tempItem.Items.Add(subItem);
                    }

                    tempItem.Tag = "project";
                    tempItem.SetImage();
                    BrowseTreeView.Items.Add(tempItem);
                }

                // Sort out ini files
                if (iniSplit != null)
                {
                    var iniArray = iniFolder.GetFiles("*ini");

                    foreach (var i in iniArray)
                    {
                        var tempItem = new CustomItem
                        {
                            Header = i.Name,
                            Path = i.FullName,
                            Tag = "ini"
                        };
                        tempItem.SetImage();
                        BrowseTreeView.Items.Add(tempItem);
                    }
                }

                BrowseTreeView.IsEnabled = true;
            }

            catch (Exception ex) 
            { 
                Globals.Logger.Error("Cannot load projects/classes: " + ex.Message);
                BrowseTreeView.IsEnabled = true;
            }

            if (_expandedFolders.Count != 0)
            {
                foreach (var node in from object t in BrowseTreeView.Items from t1 in _expandedFolders let node = t as CustomItem
                                     where node != null && node.Header.ToString() == t1.Header.ToString() select node)
                {
                    node.IsExpanded = true;
                    node.BringIntoView();
                }
            }

            if (string.IsNullOrWhiteSpace(lastSelected)) return;

            foreach (var t in BrowseTreeView.Items)
            {
                item = t as CustomItem;
                var last = lastSelected.Split('.');

                var tItem = t as CustomItem;

                if (item != null && item.Header.ToString() == last[0])
                {
                    if (!Equals(item, tItem)) continue;

                    tItem.IsSelected = true;
                    ScrollToItem(tItem);

                    break;
                }

                if (tItem == null) continue;
                foreach (var t1 in tItem.Items)
                {
                    item = t1 as CustomItem;
                    var subItem = t1 as CustomItem;

                    if (item != null && item.Header.ToString() != lastSelected) continue;
                    if (!Equals(item, subItem)) continue;
                    if (subItem == null) continue;

                    subItem.IsSelected = true;
                    ScrollToItem(subItem);

                    break; 
                }
            }
        }

        /// <summary>
        /// Scroll a TreeView to a specific item
        /// </summary>
        /// <param name="item"></param>
        private void ScrollToItem(DependencyObject item)
        {
            // Allow UI Rendering to Refresh
            DispatcherHelper.WaitForPriority();

            // Get the TreeView's ScrollViewer by going going up in the parent tree until you reach it
            var scrollParent = VisualTreeHelper.GetParent(item);
            while (scrollParent != null && !(scrollParent is ScrollViewer))
            {
                scrollParent = VisualTreeHelper.GetParent(scrollParent);
            }

            var scroll = scrollParent as ScrollViewer;

            // Get the selected item as a CustomItem
            var tvi = BrowseTreeView.SelectedItem as CustomItem;

            if (scroll == null) return;
            if (tvi == null) return;

            // Scroll to selected Item
            var offset = tvi.TransformToAncestor(scroll).Transform(new Point(0, 0));
            scroll.ScrollToVerticalOffset(offset.Y);
        }

        #region Browse
        private void browseTreeView_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void browseTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!Globals.DirectoryEntered) return;
            try
            {
                var item = Globals.VisualUpwardSearch(e.OriginalSource as DependencyObject);
                if (item == null) return;
                if (item.Tag.ToString() != "class" && item.Tag.ToString() != "ini") return;

                TextEditor.OpenFile(item.Path);

                ExploreScript();
            }

            catch (Exception ex) { Globals.Logger.Error(ex.Message); }
        }

        private void browseTreeView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BrowseTreeView.Focus();
        }

        private void browseTreeView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!Globals.DirectoryEntered) return;

                var item = Globals.VisualUpwardSearch(e.OriginalSource as DependencyObject);
                if (item != null)
                {
                    item.Focus();
                    e.Handled = true;
                }

                else
                    e.Handled = false;

                if (BrowseTreeView.Items.Count == 0) return;
                if (item != null)
                {
                    switch (item.Tag.ToString())
                    {
                        case "project":
                            BrowseTreeView.ContextMenu = BrowseTreeView.Resources["onProjectItem"] as ContextMenu;
                            break;
                        case "class":
                            BrowseTreeView.ContextMenu = BrowseTreeView.Resources["onClassItem"] as ContextMenu;
                            break;
                    }
                }

                else
                    BrowseTreeView.ContextMenu = BrowseTreeView.Resources["offItem"] as ContextMenu;
            }

            catch (Exception ex) { Globals.Logger.Error(ex.Message); }
        }

        private void openProject_Click(object sender, RoutedEventArgs e)
        {
            var item = BrowseTreeView.SelectedItem as CustomItem;

            try { if (item != null) Globals.Open(item.Path, null); }
            catch (Exception ex) { Globals.Logger.Error("Error opening project: " + ex.Message); }
        }

        private void deleteProject_Click(object sender, RoutedEventArgs e)
        {
            var item = BrowseTreeView.SelectedItem as CustomItem;
            var delete = false;

            try
            {
                if (item != null && Globals.MsgBox.Show(this, "Are you sure you want to delete " + item.Header + "?", "", MessageBoxButton.YesNo, MessageBoxIconType.Question) == MessageBoxResult.Yes)
                {
                    if (Directory.Exists(item.Path) && Directory.Exists(item.Path + "/Classes"))
                    {
                        var classPath = item.Path + "/Classes/";
                        var newCurrentDirectory = new DirectoryInfo(classPath);

                        if (Directory.GetFiles(classPath).Length != 0)
                        {
                            var fileArray = newCurrentDirectory.GetFiles();

                            foreach (var t in fileArray)
                                t.Delete();
                            Directory.Delete(item.Path + "/Classes");
                            Directory.Delete(item.Path);
                        }
                            
                        else
                        {
                            Directory.Delete(item.Path + "/Classes");
                            Directory.Delete(item.Path);
                        }

                        delete = true;
                    }

                    else
                    {
                        Globals.MsgBox.Show(this, "Cannot delete project '" + item.Header + "', it doesn't exist!", "", MessageBoxButton.OK, MessageBoxIconType.Error);
                    }
                }

                if (!delete) return;

                IniFileManager.RemoveValue("+NonNativePackages=" + item.Header);
                IniFileManager.RemoveValue("+EditPackages=" + item.Header);

                //if (Globals.MsgBox.Show(this, "Successfully deleted project '" + item.Header +
                //                              "'.\nHere, don't forget to remove it from the [UnrealEd.EditorEngine]", "", MessageBoxButton.OK, MessageBoxIconType.Info) == MessageBoxResult.OK)
                //    Globals.Open(Globals.DefaultEngine, null);

                Globals.Logger.Info("Successfully deleted  project '" + item.Header + "' and removed it from DefaultEngine.ini");
                BrowseTreeView.Items.Remove(item);
            }

            catch (Exception ex) { Globals.Logger.Error("Error when deleting project: " + ex.Message); }
        }

        private void openClass_Click(object sender, RoutedEventArgs e)
        {
            var item = BrowseTreeView.SelectedItem as CustomItem;

            try 
            {
                if (item != null) TextEditor.OpenFile(item.Path);
            }
            catch (Exception ex) { Globals.Logger.Error("Error opening class: " + ex.Message); }
        }

        private void deleteClass_Click(object sender, RoutedEventArgs e)
        {
            var item = BrowseTreeView.SelectedItem as CustomItem;
            var delete = false;

            if (item != null && Globals.MsgBox.Show(this, "Are you sure you want to delete " + item.Header + "?", "", MessageBoxButton.YesNo, MessageBoxIconType.Question) == MessageBoxResult.Yes)
            {
                if (File.Exists(item.Path))
                {
                    // Check all open tabs 
                    foreach (var t in TextEditor.AllOpenDocs)
                        if (!string.IsNullOrEmpty(t.FilePath))
                        {
                            var newFilePath = t.FilePath.Replace('/', '\\');

                            if (item.Path.Equals(newFilePath))
                            {
                                // Close the old file
                                t.ParentTab.IsSelected = true;
                                TextEditor.Close(TextEditor.GetActiveTab);
                                break;
                            }
                        }

                    // Delete File
                    File.Delete(item.Path);
                    delete = true;
                }

                // Class doesn't exist!
                else
                    Globals.MsgBox.Show(this, "Cannot delete class '" + item.Header + "', it doesn't exist!", "", MessageBoxButton.OK, MessageBoxIconType.Error);
            }

            if (!delete) return;

            Globals.Logger.Info("Successfully deleted class '" + item.Header + "'.");
            Globals.Main.UpdateBrowseList();
        }

        private void renameProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BrowseTreeView.SelectedItem == null) return;

                var item = BrowseTreeView.SelectedItem as CustomItem;

                Globals.CWindow.ShowInput(this, item, CustomType.Project, UserControls.Action.Rename);
            }

            catch (Exception ex) { Globals.Logger.Error("Error opening input window: " + ex.Message); }
        }

        private void renameClass_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BrowseTreeView.SelectedItem == null) return;

                var item = BrowseTreeView.SelectedItem as CustomItem;

                Globals.CWindow.ShowInput(this, item, CustomType.Class, UserControls.Action.Rename);
            }

            catch (Exception ex) { Globals.Logger.Error("Error opening input window: " + ex.Message); }
        }

        private void newClass_Click(object sender, RoutedEventArgs e)
        {
            if (!Globals.DirectoryEntered) return;

            try
            {
                var item = BrowseTreeView.SelectedItem as CustomItem;
                Globals.CWindow.ShowNewClass(this, item);
            }

            catch (Exception ex) { Globals.Logger.Error("Error opening input window: " + ex.Message); }
        }

        private void newProject_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.DirectoryEntered)
            {
                try { Globals.CWindow.ShowInput(this, null, CustomType.Project, UserControls.Action.New); }
                catch (Exception ex) { Globals.Logger.Error("Error opening input window: " + ex.Message); }
            }

            else
                Globals.NoRoot();
        }

        private void newProjectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.DirectoryEntered)
            {
                try { Globals.CWindow.ShowInput(this, null, CustomType.Project, UserControls.Action.New); }
                catch (Exception ex) { Globals.Logger.Error("Error opening input window: " + ex.Message); }
            }

            else
                Globals.NoRoot();
        }

        private void newClassMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.DirectoryEntered)
            {
                try { Globals.CWindow.ShowNewClass(this, null); }
                catch (Exception ex) { Globals.Logger.Error("Error opening choose window: " + ex.Message); }
            }

            else
                Globals.NoRoot();
        }

        private void newTestMapMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.DirectoryEntered)
            {
                try { Globals.CWindow.ShowTestMap(this); }
                catch (Exception ex) { Globals.Logger.Error("Error opening testmap window: " + ex.Message); }
            }

            else
                Globals.NoRoot();
        }
        #endregion

        #region Search
        private void searchTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!Globals.DirectoryEntered) return;

                var item = Globals.VisualUpwardSearch(e.OriginalSource as DependencyObject);
                if (item != null)
                {
                    TextEditor.OpenFile(item.Path);
                }
            }

            catch (Exception ex) { Globals.Logger.Error(ex.Message); }
        }

        private void searchTreeView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SearchTreeView.Focus();
        }

        #endregion

        #region TextEditButtons
        private void new_Click(object sender, RoutedEventArgs e)
        {
            try { TextEditor.OpenNewDocument(); }
            catch (Exception ex) { Globals.Logger.Error("Error when opening a new document: " + ex.Message); }
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            try { TextEditor.Open(); }
            catch (Exception ex) { Globals.Logger.Error("Error when opening an existing document: " + ex.Message); }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            try { TextEditor.Save(); ExploreScript(); }
            catch (Exception ex) { Globals.Logger.Error("Error when saving a document: " + ex.Message); }
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            try { TextEditor.Copy(); }
            catch (Exception ex) { Globals.Logger.Error("Copy error: " + ex.Message); }
        }

        private void cut_Click(object sender, RoutedEventArgs e)
        {
            try { TextEditor.Cut(); }
            catch (Exception ex) { Globals.Logger.Error("Cut error: " + ex.Message); }
        }

        private void paste_Click(object sender, RoutedEventArgs e)
        {
            try { TextEditor.Paste(); }
            catch (Exception ex) { Globals.Logger.Error("Paste error: " + ex.Message); }
        }

        private void saveAll_Click(object sender, RoutedEventArgs e)
        {
            try { TextEditor.SaveAll(); ExploreScript(); }
            catch (Exception ex) { Globals.Logger.Error("SaveAll error: " + ex.Message); }
        }

        private void undo_Click(object sender, RoutedEventArgs e)
        {
            try { TextEditor.Undo(); }
            catch (Exception ex) { Globals.Logger.Error("Undo error: " + ex.Message); }
        }

        private void redo_Click(object sender, RoutedEventArgs e)
        {
            try { TextEditor.Redo(); }
            catch (Exception ex) { Globals.Logger.Error("Redo error: " + ex.Message); }
        }

        private void replace_Click(object sender, RoutedEventArgs e)
        {
            try { TextEditor.Replace(); }
            catch (Exception ex) { Globals.Logger.Error("Replace error: " + ex.Message); }
        }

        private void find_Click(object sender, RoutedEventArgs e)
        {
            try { TextEditor.Find(); }
            catch (Exception ex) { Globals.Logger.Error("Find error: " + ex.Message); }
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            try { TextEditor.Close(TextEditor.GetActiveTab); }
            catch (Exception ex) { Globals.Logger.Error("Close error: " + ex.Message); }
        }

        private void closeAll_Click(object sender, RoutedEventArgs e)
        {
            try { TextEditor.CloseAllTabs(); }
            catch (Exception ex) { Globals.Logger.Error("CloseAll error: " + ex.Message); }
        }

        private void comment_Click(object sender, RoutedEventArgs e)
        {
            try { TextEditor.CommentOut(); }
            catch (Exception ex) { Globals.Logger.Error("CommentOut error: " + ex.Message); }
        }

        private void noComment_Click(object sender, RoutedEventArgs e)
        {
            try { TextEditor.CommentIn(); }
            catch (Exception ex) { Globals.Logger.Error("CommentIn error: " + ex.Message); }
        }
        #endregion

        #region MainMenu
        private void defaultEngMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.DirectoryEntered)
            {
                try { Globals.Open(Globals.DefaultEngine, null); }

                catch (Exception ex) { Globals.Logger.Error("Error when trying to open DefaultEngine.ini: " + ex.Message); }
            }
            else
                Globals.NoRoot();
        }

        private void runGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.DirectoryEntered)
            {
                if (Globals.ExeFile == "") return;

                try
                {
                    Globals.Open(Globals.ExeFile, "null");
                    Globals.Logger.Init("Launching UDK Game...");
                }

                catch (Exception ex) { Globals.Logger.Error("Error when trying to run UDK Game: " + ex.Message); }
            }

            else
                Globals.NoRoot();
        }

        private void runFrontendMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.DirectoryEntered)
            {
                Globals.Open(Globals.RootDirectory + "/Binaries/UnrealFrontend.exe", null);
                Globals.Logger.Init("Launching Unreal Frontend...");
            }

            else
                Globals.NoRoot();
        }

        private void udkRootMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.DirectoryEntered)
                Globals.Open(Globals.RootDirectory, null);

            else
                Globals.NoRoot();
        }

        private void runEditorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.DirectoryEntered)
            {
                if (Globals.ExeFile == "") return;
                try
                {
                    Globals.Open(Globals.ExeFile, "editor");
                    Globals.Logger.Init("Launching UDK Editor...");
                }

                catch (Exception ex) { Globals.Logger.Error("Error when trying to run UDK Editor: " + ex.Message); }
            }

            else
                Globals.NoRoot();
        }

        private void reloadAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Globals.DirectoryEntered)
                {
                    Main.UpdateBrowseList();
                    Main.UpdateMapList();
                    Globals.Logger.Info("All folders successfully reloaded");
                }

                else
                    Globals.NoRoot();
            }

            catch (Exception ex) { Globals.Logger.Error("Error reloading all folders: " + ex.Message); }
        }

        private void reloadProjectsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Globals.DirectoryEntered)
                {
                    Main.UpdateBrowseList();
                    Globals.Logger.Info("Projects successfully reloaded");
                }

                else
                    Globals.NoRoot();
            }

            catch (Exception ex) { Globals.Logger.Error("Error reloading projects: " + ex.Message); }
        }

        private void reloadTestMapsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Globals.DirectoryEntered)
                {
                    Main.UpdateMapList();
                    Globals.Logger.Info("TestMaps successfully reloaded");
                }

                else
                    Globals.NoRoot();
            }

            catch (Exception ex) { Globals.Logger.Error("Error reloading TestMaps: " + ex.Message); }
        }

        private void reloadScintillaMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (
                Globals.MsgBox.Show(this, "Doing this will save all your current open files, continue?", "",
                    MessageBoxButton.YesNo, MessageBoxIconType.Question) == MessageBoxResult.No)
                return;

            try
            {
                if (Globals.DirectoryEntered)
                {
                    TextEditor.ReloadScintilla();
                    Globals.Logger.Info("Unrealscript.xml successfully reloaded");
                }

                else
                    Globals.NoRoot();
            }

            catch (Exception ex) { Globals.Logger.Error("Error reloading Unrealscript.xml: " + ex.Message); }
        }
        #endregion

        #region Script Explorer
        private void explorerTreeView_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = Globals.VisualUpwardSearchTwo(e.OriginalSource as DependencyObject);

                if (item == null) return;
                TextEditor.GetActiveDoc.SetFocus();
                TextEditor.GetActiveDoc.Scintilla.GoTo.Line(item.Pos);
                TextEditor.GetActiveDoc.Scintilla.Scrolling.ScrollToLine(item.Pos);
            }

            catch (Exception ex) { Globals.Logger.Error(ex.Message + ", GetActiveDoc == null"); }
        }
        #endregion

        #region Test Map
        private void newTestMap_Click(object sender, RoutedEventArgs e)
        {
            try { Globals.CWindow.ShowTestMap(this); }
            catch (Exception ex) {
                if (ex.InnerException != null)
                    Globals.Logger.Error("Error opening testmap window: " + ex.InnerException.Message);
            }
        }

        private void testMapTreeView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TestMapTreeView.Focus();
        }

        private void testMapTreeView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!Globals.DirectoryEntered) return;

                var item = Globals.VisualUpwardSearch(e.OriginalSource as DependencyObject);
                if (item != null)
                {
                    item.Focus();
                    e.Handled = true;
                }

                else

                    e.Handled = false;

                if (item != null)
                    TestMapTreeView.ContextMenu = TestMapTreeView.Resources["onMapItem"] as ContextMenu;

                else
                    TestMapTreeView.ContextMenu = TestMapTreeView.Resources["offMapItem"] as ContextMenu;
            }

            catch (Exception ex) { Globals.Logger.Error(ex.Message); }
        }

        private void testMapTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!Globals.DirectoryEntered) return;

                var item = TestMapTreeView.SelectedItem as CustomItem;

                if (item == null) return;
                if (item.Tag.ToString() != "testMap") return;

                Globals.Logger.Init("Launching Test Map " + item.Header + "...");
                Globals.Open(item.Path, null);
            }

            catch (Exception ex) { Globals.Logger.Error(ex.Message); }
        }

        private void deleteTestMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = TestMapTreeView.SelectedItem as CustomItem;
                var delete = false;

                if (item != null && Globals.MsgBox.Show(this, "Are you sure you want to delete " + item.Header + "?", "", MessageBoxButton.YesNo, MessageBoxIconType.Question) == MessageBoxResult.Yes)
                {
                    if (File.Exists(item.Path))
                    {
                        File.Delete(item.Path);
                        delete = true;
                    }

                    else
                    {
                        Globals.MsgBox.Show(this, "Cannot delete test map '" + item.Header + "', it doesn't exist!", "", MessageBoxButton.OK, MessageBoxIconType.Error);
                    }
                }

                if (!delete) return;

                Globals.Logger.Info("Successfully deleted test map '" + item.Header + "'.");
                Globals.Main.UpdateMapList();
            }

            catch (Exception ex) { Globals.Logger.Error("Error trying to delete testmap: " + ex.Message); }
        }
        #endregion

        //private string time;
        private void lastLogMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!Globals.DirectoryEntered) return;

            try
            {
                Process.Start(Globals.RootDirectory + "/UDKGame/Logs/Launch.log");
                Globals.Logger.Init("Opening last log item...");
            }
            catch (Exception ex)
            {
                Globals.Logger.Error("Error opening last log item: " + ex.Message);
            }
        }

        private void browseTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete) return;
            try
            {
                if (!Globals.DirectoryEntered) return;

                var item = Globals.VisualUpwardSearch(e.OriginalSource as DependencyObject);

                if (BrowseTreeView.Items.Count == 0) return;
                if (item == null) return;

                switch (item.Tag.ToString())
                {
                    case "project":
                        deleteProject_Click(null, null);
                        break;
                    case "class":
                        deleteClass_Click(null, null);
                        break;
                }
            }

            catch (Exception ex) { Globals.Logger.Error(ex.Message); }
        }

        private void zoomCB_DropDownClosed(object sender, EventArgs e)
        {
            var size = 0;

            for (int i = 0; i < TextEditor.AllOpenDocs.Count; i++)
            {
                switch (ZoomComboBox.SelectedValue.ToString())
                {
                    case "-6":
                        size = -6;
                    break;

                    case "-4":
                        size = -4;
                        break;

                    case "-2":
                        size = -2;
                        break;

                    case "0":
                        size = 0;
                        break;

                    case "2":
                        size = 2;
                        break;

                    case "4":
                        size = 4;
                        break;

                    case "6":
                        size = 6;
                        break;
                }

                TextEditor.AllOpenDocs[i].Scintilla.ZoomFactor = size;

                Settings.Default.ZoomFactor = size;
                Settings.Default.Save();
            }
        }

        public void box_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                var box = sender as ComboBox;
                var item = box?.SelectedItem as Function;

                if (item == null) return;
                //box.Text = item.Content;

                if (!(bool)box.Tag) return;
                TextEditor.GetActiveDoc.SetFocus();
                TextEditor.GetActiveDoc.Scintilla.GoTo.Line(item.Pos);
                TextEditor.GetActiveDoc.Scintilla.Scrolling.ScrollToLine(item.Pos);
            }

            catch (Exception ex) { Globals.Logger.Error(ex.Message + ", GetActiveDoc == null"); }
        }
    }
}
