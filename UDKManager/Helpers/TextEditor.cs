using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using ScintillaNET;
using Xceed.Wpf.AvalonDock.Layout;
using ZerO.Helpers;
using ZerO.Windows;
using static System.String;
using Button = System.Windows.Controls.Button;
using ComboBox = System.Windows.Controls.ComboBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Orientation = System.Windows.Controls.Orientation;

namespace ZerO
{
    /// <summary>
    /// This class handles most of the actions for the text editor
    /// </summary>
    public static class TextEditor
    {
        private const string NewDocumentText = "new ";
        public static readonly List<KeyWord> KeywordsList = new List<KeyWord>();
        private static readonly List<LayoutDocument> TabsToDelete = new List<LayoutDocument>();

        private static int _newDocumentCount;
        private static Function _lastSelected;

        public static ImageSourceConverter ImgConvert = new ImageSourceConverter();
        public static List<TextDocument> AllOpenDocs = new List<TextDocument>();
        public static List<string> AutoCompleteList = new List<string>();
        public static bool InsertParenthesis, InsertBracket, InsertCurlyBracket, InsertQuotation, InsertSingleQuotation;

        public static OpenFileDialog OpenFileDialog { get; set; }
        public static SaveFileDialog SaveFileDialog { get; set; }

        /// <summary>
        /// Get a certain TextDocument using its parent tab
        /// </summary>
        /// <param name="tab">Parent tab to find content from</param>
        public static TextDocument GetCertainDoc(LayoutDocument tab)
        {
            var panel = tab.Content as StackPanel;
            TextDocument doc = null;

            for (int i = 0; i < panel.Children.Count; i++)
            {
                var child = panel.Children[i] as TextDocument;
                if (child == null) continue;
                doc = child;
                break;
            }

            return doc;
        }

        /// <summary>
        /// Get the active tab's TextDocument content
        /// </summary>
        public static TextDocument GetActiveDoc
        {
            get
            {
                var panel = GetActiveTab.Content as StackPanel;
                TextDocument doc = null;

                for (int i = 0; i < panel.Children.Count; i++)
                {
                    var child = panel.Children[i] as TextDocument;
                    if (child == null) continue;
                    doc = child;
                    break;
                }

                return doc;
            }
        }

        /// <summary>
        /// Get the active tab
        /// </summary>
        public static LayoutDocument GetActiveTab => Globals.Main.DocumentPane.SelectedContent as LayoutDocument;

        /// <summary>
        /// Initialization
        /// </summary>
        public static void Initialize()
        {
            SaveFileDialog = new SaveFileDialog();
            OpenFileDialog = new OpenFileDialog();

            // saveFileDialog
            SaveFileDialog.Filter = @"Uscript class files (*.uc*)|*.uc*";
            SaveFileDialog.OverwritePrompt = true;
            SaveFileDialog.AddExtension = true;
            SaveFileDialog.DefaultExt = "uc";

            // openFileDialog
            OpenFileDialog.Filter = @"Uscript class files (*.uc*)|*.uc*";
            OpenFileDialog.DefaultExt = "uc";

            LoadKeywordsFromXml();
        }

        /// <summary>
        /// Set up the tab's content
        /// </summary>
        /// <param name="doc">Textdocument to use</param>
        /// <param name="item">LayoutDocument to use</param>
        private static void SetupComboboxes(TextDocument doc, LayoutDocument item)
        {
            const int offset = 3;

            // Setup combobox bar
            var boxPanel = new StackPanel { Orientation = Orientation.Horizontal };
            var boxOne = new ComboBox
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = Globals.Main.DockManager.ActualWidth / 2 - offset,
                Height = 19,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.DimGray
            };
            boxOne.SelectionChanged += box_SelectionChanged;
            boxOne.PreviewMouseDown += box_LostFocus;
            boxOne.DropDownOpened += box_DropDownOpened;
            boxOne.VerticalAlignment = VerticalAlignment.Center;
            boxOne.DropDownClosed += Globals.Main.box_DropDownClosed;
            var boxTwo = new ComboBox
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = Globals.Main.DockManager.ActualWidth / 2 - offset,
                Height = 19,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.DimGray
            };
            boxTwo.SelectionChanged += box_SelectionChanged;
            boxTwo.PreviewMouseDown += box_LostFocus;
            boxTwo.DropDownOpened += box_DropDownOpened;
            boxTwo.VerticalAlignment = VerticalAlignment.Center;
            boxTwo.Tag = false;
            boxTwo.DropDownClosed += Globals.Main.box_DropDownClosed;
            boxPanel.Height = boxOne.Height + boxTwo.Height;
            boxPanel.Children.Add(boxOne);
            boxPanel.Children.Add(boxTwo);

            // Setup the final full stackpanel containing both comboboxes and texteditor combined
            var finalStack = new StackPanel { Orientation = Orientation.Vertical };
            finalStack.Children.Add(boxPanel);
            finalStack.Height = Globals.Main.DockManager.ActualHeight;
            doc.Height = Globals.Main.DockManager.ActualHeight - boxPanel.Height * 2.0f;
            finalStack.Children.Add(doc);

            // Finish it
            doc.VarComboBox = boxOne;
            doc.FuncComboBox = boxTwo;
            item.Content = finalStack;
        }

        private static void box_DropDownOpened(object sender, EventArgs e)
        {
            var box = sender as ComboBox;
            if (box == null) return;

            _lastSelected = box.Items[box.SelectedIndex] as Function;
        }

        public static void ToggleLineHighlight(bool bValue)
        {
            foreach (var t in AllOpenDocs)
                t.Scintilla.Caret.HighlightCurrentLine = bValue;
        }

        public static void ToggleLineNumbers(bool bValue)
        {
            foreach (var t in AllOpenDocs)
            {
                t.Scintilla.Margins[0].Width = bValue ? 20 : 0;
            }
        }

        private static void box_LostFocus(object sender, MouseButtonEventArgs e)
        {
            var box = sender as ComboBox;

            if (box == null) return;
            box.Tag = false;

            if (box.IsDropDownOpen && !box.IsMouseDirectlyOver && Equals(box.SelectedItem, _lastSelected))
                box_SelectionChanged(box, null);
        }

        private static void box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var box = sender as ComboBox;

            if (box == null) return;
            box.Tag = true;
        }

        /// <summary>
        /// Opens a new empty text document
        /// </summary>
        public static void OpenNewDocument()
        {
            if (Globals.Main.DocumentPane.Children.Count == 0)
                _newDocumentCount = 0;

            var title = Format(CultureInfo.CurrentCulture, "{0}{1}", NewDocumentText, ++_newDocumentCount);
            var doc = new TextDocument(false);
            var item = new LayoutDocument();

            SetupComboboxes(doc, item);

            doc.Title = title;
            item.Title = title;
            item.Closing += item_Closing;
            doc.ParentTab = item;
            item.IconSource = (ImageSource)ImgConvert.ConvertFrom(new Uri("pack://application:,,,/Resources/class.png"));
            AllOpenDocs.Add(doc);
            Globals.Main.DocumentPane.Children.Add(item);

            Globals.Main.DockManager.ActiveContent = item;
            doc.SetFocus();
        }

        /// <summary>
        /// Tab Closing event
        /// </summary>
        private static void item_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;

            var item = sender as LayoutDocument;
            Close(item);
        }

        /// <summary>
        /// Finalizes the closing of the tab
        /// </summary>
        /// <param name="tab">LayoutDocument tab to close</param>
        public static void Close(LayoutDocument tab)
        {
            if (tab == null) return;

            var doc = GetCertainDoc(tab);
            if (doc == null) return;

            // get selected tab
            var selectedTab = GetActiveTab;
            if (!doc.AttemptClose()) return;

            // select previously selected tab. if that is removed then select first tab
            if (selectedTab == null || selectedTab.Equals(tab))
                selectedTab = (LayoutDocument)Globals.Main.DocumentPane.Children[0];

            Globals.Main.DockManager.ActiveContent = selectedTab;

            tab.Closing -= item_Closing;
            AllOpenDocs.Remove(doc);
            Globals.Main.DocumentPane.Children.Remove(tab);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        /// <summary>
        /// Close a tab
        /// </summary>
        public static void CloseTab(object sender)
        {
            var button = sender as Button;
            if (button == null) return;
            var tabName = button.CommandParameter.ToString();

            var item = Globals.Main.DocumentPane.Children.Cast<LayoutDocument>().SingleOrDefault(i => i.Title.Equals(tabName));

            var tab = item;

            Close(tab);
        }

        /// <summary>
        /// Close all tabs
        /// </summary>
        public static void CloseAllTabs()
        {
            foreach (var item in Globals.Main.DocumentPane.Children.OfType<LayoutDocument>())
                TabsToDelete.Add(item);


            foreach (var t in TabsToDelete)
            {
                Close(t);
                _newDocumentCount = 0;
            }
        }

        /// <summary>
        /// Save all open documents
        /// </summary>
        public static void SaveAll()
        {
            bool bSaved = false;

            // Check if anything actually needs saving
            foreach (var t in AllOpenDocs)
                if (!t.Saved)
                    if (t.Save())
                    {
                        t.ParentTab.Title = t.Title;
                        bSaved = true;
                    }

            if (bSaved)
                Globals.Logger.Info("Saved all open documents");
        }

        /// <summary>
        /// Save all open documents WITH a filepath
        /// </summary>
        public static void SafeSaveAll()
        {
            foreach (var t in AllOpenDocs)
                if (!t.Saved && !IsNullOrEmpty(t.FilePath))
                    if (t.Save())
                        t.ParentTab.Title = t.Title;
        }

        /// <summary>
        /// Opens Find/Replace for the text editor
        /// </summary>
        public static void Find()
        {
            if (GetActiveDoc != null)
                GetActiveDoc.Scintilla.FindReplace.ShowFind();
        }

        /// <summary>
        /// Opens Find/Replace for the text editor
        /// </summary>
        public static void Replace()
        {
            if (GetActiveDoc != null)
                GetActiveDoc.Scintilla.FindReplace.ShowReplace();
        }

        /// <summary>
        /// Copy from the active document
        /// </summary>
        public static void Copy()
        {
            if (GetActiveDoc != null)
                GetActiveDoc.Scintilla.Clipboard.Copy();
        }

        /// <summary>
        /// Cut from the active document
        /// </summary>
        public static void Cut()
        {
            if (GetActiveDoc != null)
                GetActiveDoc.Scintilla.Clipboard.Cut();
        }

        /// <summary>
        /// Copy tothe active document
        /// </summary>
        public static void Paste()
        {
            if (GetActiveDoc != null)
                GetActiveDoc.Scintilla.Clipboard.Paste();
        }

        /// <summary>
        /// Redo the last action
        /// </summary>
        public static void Redo()
        {
            if (GetActiveDoc != null)
                GetActiveDoc.Scintilla.UndoRedo.Redo();
        }

        /// <summary>
        /// Undo the last action
        /// </summary>
        public static void Undo()
        {
            if (GetActiveDoc != null)
                GetActiveDoc.Scintilla.UndoRedo.Undo();
        }

        /// <summary>
        /// Save the current document
        /// </summary>
        public static void Save()
        {
            TextDocument doc = GetActiveDoc;
            if (doc != null && doc.ParentTab == null) return;
            if (doc != null && doc.Saved) return;
            if (doc == null || !doc.Save()) return;

            doc.ParentTab.Title = doc.Title;
            doc.ToolTip = doc.FilePath;
            Globals.Logger.Info("Saved " + doc.ParentTab.Title);
        }

        /// <summary>
        /// First step in opening a file
        /// </summary>
        public static void Open()
        {
            if (Globals.DirectoryEntered)
                OpenFileDialog.InitialDirectory = Globals.RootDirectory + "\\Development\\Src";

            if (OpenFileDialog.ShowDialog() != DialogResult.OK)
                return;

            foreach (var filePath in OpenFileDialog.FileNames)
            {
                // Ensure this file isn't already open
                var isOpen = false;

                // See if we can find it in the list of open documents
                var doc = AllOpenDocs.Find(x => x.FilePath.Equals(filePath, StringComparison.CurrentCultureIgnoreCase));
                if (doc != null)
                {
                    doc.ParentTab.IsSelected = true;
                    isOpen = true;
                }

                // Open the files
                if (!isOpen)
                    OpenFile(filePath);
            }
        }

        /// <summary>
        /// Actually open file
        /// </summary>
        /// <param name="filePath">Path of the file to open</param>
        public static void OpenFile(string filePath)
        {
            // Make sure the paths use the same char
            var newFilePath = filePath.Replace('/', '\\');

            // Make sure the file exists on the computer
            if (!File.Exists(newFilePath)) return;

            // Make sure it isn't already open
            var docOpen = AllOpenDocs.Find(x => x.FilePath.Equals(filePath, StringComparison.CurrentCultureIgnoreCase));
            if (docOpen != null)
            {
                docOpen.ParentTab.IsSelected = true;
                return;
            }

            var title = Path.GetFileNameWithoutExtension(filePath);
            var ext = Path.GetExtension(filePath);
            bool bIni = ext.Contains("ini");

            var doc = new TextDocument(bIni);
            var newTab = new LayoutDocument { Title = title, IsSelected = true };
            SetupComboboxes(doc, newTab);

            newTab.Closing += item_Closing;
            newTab.ToolTip = filePath;

            doc.ParentTab = newTab;
            doc.Title = title;
            doc.Scintilla.Text = File.ReadAllText(filePath);
            doc.Scintilla.UndoRedo.EmptyUndoBuffer();
            doc.Scintilla.Modified = false;
            doc.FilePath = filePath;
            doc.IsSaved(true);

            AllOpenDocs.Add(doc);
            newTab.IconSource = (ImageSource)ImgConvert.ConvertFrom(new Uri("pack://application:,,,/Resources/class.png"));

            Globals.Main.DocumentPane.Children.Add(newTab);
            Globals.Main.ExploreScript();

            Globals.Main.DockManager.ActiveContent = newTab;
            doc.SetFocus();
        }

        /// <summary>
        /// Comment in or out selected code
        /// </summary>
        public static void CommentInOut()
        {
            TextDocument doc = GetActiveDoc;
            if (doc?.ParentTab == null) return;

            // If selection is already commented, comment out and vice versa
            if (doc.Scintilla.Selection.Text.Length <= 0) return;

            if (!doc.Scintilla.Selection.Text.StartsWith("/*") && !doc.Scintilla.Selection.Text.StartsWith("//"))
                CommentOut();

            else
                CommentIn();
        }

        /// <summary>
        /// Comment out selected code
        /// </summary>
        public static void CommentOut()
        {
            TextDocument content = GetActiveDoc;
            if (content?.ParentTab == null || IsNullOrEmpty(content.Scintilla.Selection.Text)) return;

            content.Scintilla.UndoRedo.BeginUndoAction();

            var range = content.Scintilla.Selection.Range;
            var f = range.StartingLine.Number;
            var t = range.EndingLine.Number;

            for (var i = f; i <= t; i++)
                if (!IsNullOrWhiteSpace(content.Scintilla.Lines[i].Text))
                {
                    content.IsSaved(false);
                    content.Scintilla.InsertText(content.Scintilla.Lines[i].StartPosition, "//");
                }

            content.Scintilla.UndoRedo.EndUndoAction();

            content.Scintilla.Selection.Start = content.Scintilla.Lines[f].StartPosition;
            content.Scintilla.Selection.End = content.Scintilla.Lines[t].EndPosition;
            content.Scintilla.Focus();
        }

        /// <summary>
        /// Comment in selected code
        /// </summary>
        public static void CommentIn()
        {
            TextDocument content = GetActiveDoc;
            if (content?.ParentTab == null || IsNullOrEmpty(content.Scintilla.Selection.Text)) return;

            content.Scintilla.UndoRedo.BeginUndoAction();

            content.Scintilla.Lexing.LineUncomment();

            var range = content.Scintilla.Selection.Range;
            var f = range.StartingLine.Number;
            var t = range.EndingLine.Number;


            for (var i = f; i <= t; i++)
                if (!IsNullOrWhiteSpace(content.Scintilla.Lines[i].Text))
                {
                    content.IsSaved(false);
                }

            var lineRange = new List<Range>();
            if (content.Scintilla.Selection.Range.IsMultiLine)
                lineRange = content.Scintilla.FindReplace.FindAll(range, "//");
            else
                lineRange.Add(content.Scintilla.FindReplace.Find(range, "//"));

            foreach (var line in lineRange)
            {
                try
                {
                    content.Scintilla.FindReplace.ReplaceAll(line.Start,
                        line.End + line.Length,
                        "//", "", SearchFlags.WordStart);
                    content.Scintilla.FindReplace.ReplaceAll(line.Start,
                        line.End + line.Length,
                        "// ", "", SearchFlags.WordStart);
                }

                catch
                {
                    // ignored
                }
            }

            content.Scintilla.UndoRedo.EndUndoAction();

            content.Scintilla.Focus();
        }

        /// <summary>
        /// Loads keywords from xml file for a pre-defined lexer. 
        /// </summary>
        public static void LoadKeywordsFromXml()
        {
            var xmlFileName = Environment.CurrentDirectory + "/UnrealScript.xml";
            if (IsNullOrEmpty(xmlFileName) || !File.Exists(xmlFileName))
                return;

            var xmlDoc = XDocument.Load(xmlFileName);
            if (xmlDoc.Root == null) return;
            foreach (var rootElement in xmlDoc.Root.Elements("root"))
            {
                foreach (var element in rootElement.Elements("KeyWord"))
                {
                    try
                    {
                        var keyWord = new KeyWord { Name = (string)element.Attribute("name") };

                        if (!AutoCompleteList.Contains(keyWord.Name))
                            AutoCompleteList.Add(keyWord.Name);

                        var func = (string)element.Attribute("func");
                        if (func != null) keyWord.Func = func.Equals("yes");

                        // Check for parameters
                        foreach (var e in element.Elements("Overload"))
                        {
                            keyWord.Type = (string)e.Attribute("retVal");

                            foreach (var para in e.Elements("Param"))
                                keyWord.AddParam((string)para.Attribute("name"));
                        }

                        // Add the keyword to the list
                        KeywordsList.Add(keyWord);

                        AutoCompleteList.Sort();

                    }
                    catch (Exception ex)
                    {
                        Globals.Logger.Error("Could not load xml document 'uc.xml': " + ex.Message);
                    }
                }
            }
        }

        public static void ReloadScintilla()
        {
            SaveAll();
            Globals.Logger.Info("All open documents saved");

            //Save the list so that we can open it afterwards
            var openedDocs = new List<TextDocument>();
            openedDocs.AddRange(AllOpenDocs);

            //Close all open tabs
            CloseAllTabs();

            //Reopen them
            foreach (var doc in openedDocs)
            {
                OpenFile(doc.FilePath);

                doc.Scintilla.Lexing.Colorize();
            }
        }
    }
}
