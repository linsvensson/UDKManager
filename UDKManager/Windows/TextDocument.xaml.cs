using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ScintillaNET;
using Xceed.Wpf.AvalonDock.Layout;
using ZerO.Helpers;
using ZerO.Properties;
using Color = System.Drawing.Color;
using ComboBox = System.Windows.Controls.ComboBox;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace ZerO.Windows
{
    /// <summary>
    /// Interaction logic for TextDocument.xaml
    /// </summary>
    public partial class TextDocument
    {
        private const uint SciSetextraascent = 2525;

        private int _lastMeasureLines = -1;
        private ToolStripMenuItem _menuItemCut;
        private ToolStripMenuItem _menuItemCopy;
        private ToolStripMenuItem _menuItemPaste;
        private ToolStripMenuItem _menuItemFind;
        private ToolStripMenuItem _menuItemFindReplace;
        private ToolStripMenuItem _menuItemGotoLine;
        private ToolStripMenuItem _menuItemInsert;
        private ToolStripItem[] _subMenuItemsInsert;
        private bool _inParameter;
        private bool _inComment;
        private KeyWord _keyword;
        private int _numCom;
        private char _lastChar;

        public ComboBox VarComboBox, FuncComboBox;
        public bool IsIni;
        public bool ShowLineNumbers;
        public string Title;
        public LayoutDocument ParentTab;
        public bool Saved = true;

        public string FilePath { get; set; }
        public Scintilla Scintilla { get; set; }

        public static int ConvertToIntFromRgba(byte red, byte green, byte blue, byte alpha)
        {
            return ((red << 24) | (green << 16) | (blue << 8) | (alpha));
        }

        public static int ConvertToIntFromRgb(byte red, byte green, byte blue)
        {
            return ((red << 24) | (green << 16) | (blue << 8));
        }

        public TextDocument(bool isIni)
        {
            IsIni = isIni;

            InitializeComponent();

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.LowQuality);
            RenderOptions.SetCachingHint(this, CachingHint.Cache);

            // Scintilla
            Scintilla = new Scintilla
            {
                BackColor = Color.FromArgb(20, 20, 20),
                BorderStyle = BorderStyle.None
            };

            Scintilla.LineWrapping.VisualFlags = LineWrappingVisualFlags.End;
            Scintilla.Margins.Margin1.AutoToggleMarkerNumber = 0;
            Scintilla.Margins.Margin1.IsClickable = true;
            Scintilla.Name = "scintilla";
            Scintilla.Selection.BackColor = Color.Pink;
            Scintilla.Selection.ForeColor = Color.Black;
            Scintilla.Selection.ForeColorUnfocused = Color.Black;

            if (Settings.Default.LineNumbers)
                Scintilla.Margins[0].Width = 25;
            else
                Scintilla.Margins[0].Width = 0;

            if (!isIni)
            {
                Scintilla.Lexing.LexerLanguageMap["Unrealscript"] = "cpp";
                Scintilla.Lexing.LexerName = "Unrealscript";
                Scintilla.ConfigurationManager.CustomLocation = Environment.CurrentDirectory;
                Scintilla.ConfigurationManager.Language = "Unrealscript";
                Scintilla.ConfigurationManager.Configure();

                Scintilla.MatchBraces = true;
                Scintilla.Styles.BraceLight.ForeColor = Color.Yellow;
                Scintilla.IsBraceMatching = true;

                Scintilla.Margins.Margin2.Width = 20;

                if (!string.IsNullOrEmpty(Globals.Main.ZoomComboBox.Text))
                    Scintilla.ZoomFactor = Int32.Parse(Globals.Main.ZoomComboBox.Text);

                Scintilla.Folding.IsEnabled = true;
                Scintilla.ZoomFactorChanged += scintilla_ZoomFactorChanged;
                Scintilla.Folding.UseCompactFolding = true;
                Scintilla.Folding.Flags = FoldFlag.Box;
                Scintilla.Folding.MarkerScheme = FoldMarkerScheme.BoxPlusMinus;

                Scintilla.CallTip.ForeColor = Color.DimGray;
                Scintilla.CallTip.BackColor = Color.AntiqueWhite;
                Scintilla.CallTip.HighlightTextColor = Color.AntiqueWhite;

                Scintilla.Lexing.Keywords[4] += " " + Convert.ToChar("@") + " " + (char)96;

                // LINE SPACING
                Scintilla.NativeInterface.SendMessageDirect(SciSetextraascent, 2);

                Scintilla.Indentation.SmartIndentType = SmartIndent.CPP2;
                Scintilla.CharAdded += scintilla_CharAdded;
                Scintilla.KeyDown += scintilla_KeyDown;
                Scintilla.TextInserted += scintilla_TextInserted;
                Scintilla.Caret.Color = Color.White;
                Scintilla.SelectionChanged += scintilla_SelectionChanged;
                Scintilla.TextChanged += scintilla_TextChanged;
                Scintilla.MouseDown += scintilla_MouseDown;

                if (Settings.Default.AutoComplete)
                {
                    Scintilla.AutoComplete.DropRestOfWord = false;
                    Scintilla.AutoComplete.SingleLineAccept = false;

                    foreach (var t in Scintilla.AutoComplete.List.Where(t => !TextEditor.AutoCompleteList.Contains(t)))
                        TextEditor.AutoCompleteList.Add(t);

                    TextEditor.AutoCompleteList.Sort();
                }

                SetupContextMenu();

                if (Settings.Default.LineHighlight)
                {
                    Scintilla.Caret.HighlightCurrentLine = true;
                    Scintilla.Caret.CurrentLineBackgroundColor = ColorTranslator.FromHtml("#292929");
                }

                else
                    Scintilla.Caret.HighlightCurrentLine = false;

                Scintilla.NativeInterface.SetFoldMarginColour(true, ConvertToIntFromRgba(255, 255, 255, 100));
                Scintilla.NativeInterface.SetFoldMarginHiColour(true, ConvertToIntFromRgba(255, 255, 255, 10));

                Scintilla.Styles[Scintilla.Lexing.StyleNameMap["STRING"]].ForeColor = Color.Yellow;
                Scintilla.Styles[Scintilla.Lexing.StyleNameMap["CHARACTER"]].ForeColor = Color.DarkOliveGreen;
                Scintilla.Styles[Scintilla.Lexing.StyleNameMap["DEFAULT"]].ForeColor = Color.Purple;
                Scintilla.Styles[Scintilla.Lexing.StyleNameMap["DOCUMENT_DEFAULT"]].ForeColor = Color.Yellow;
            }

            else
            {
                Scintilla.NativeInterface.SendMessageDirect(SciSetextraascent, 2);

                Scintilla.Margins.Margin2.Width = 20;

                Scintilla.Folding.IsEnabled = true;
                Scintilla.ZoomFactorChanged += scintilla_ZoomFactorChanged;
                Scintilla.Folding.UseCompactFolding = true;
                Scintilla.Folding.Flags = FoldFlag.Box;
                Scintilla.Folding.MarkerScheme = FoldMarkerScheme.BoxPlusMinus;

                Scintilla.SelectionChanged += scintilla_SelectionChanged;
                Scintilla.TextChanged += scintilla_TextChanged;
                Scintilla.MouseDown += scintilla_MouseDown;
                Scintilla.CharAdded += scintilla_CharAdded;
                Scintilla.KeyDown += scintilla_KeyDown;
                Scintilla.TextInserted += scintilla_TextInserted;

                Scintilla.NativeInterface.SetFoldMarginColour(true, ConvertToIntFromRgba(255, 255, 255, 100));
                Scintilla.NativeInterface.SetFoldMarginHiColour(true, ConvertToIntFromRgba(255, 255, 255, 10));

                Scintilla.Caret.Color = Color.White;
                Scintilla.ForeColor = Color.White;

                Scintilla.Caret.HighlightCurrentLine = true;
                Scintilla.Caret.CurrentLineBackgroundColor = ColorTranslator.FromHtml("#292929");
                Scintilla.NativeInterface.SetFoldMarginColour(true, ConvertToIntFromRgba(255, 255, 255, 100));
                Scintilla.NativeInterface.SetFoldMarginHiColour(true, ConvertToIntFromRgba(255, 255, 255, 10));
            }

            Scintilla.Focus();

            Host.Child = Scintilla;
        }

        private void scintilla_ZoomFactorChanged(object sender, EventArgs e)
        {
            Globals.Main.ZoomComboBox.Text = Scintilla.ZoomFactor.ToString();

            foreach (TextDocument t in TextEditor.AllOpenDocs)
            {
                if (t.Scintilla.ZoomFactor == Scintilla.ZoomFactor) continue;

                t.Scintilla.ZoomFactor = Scintilla.ZoomFactor;
            }

            Settings.Default.ZoomFactor = Scintilla.ZoomFactor;
            Settings.Default.Save();
        }

        public void ToggleLineNumbers(bool bShow)
        {
            Scintilla.Margins[0].Width = bShow ? 25 : 0;
        }

        #region ContextMenu
        private void SetupContextMenu()
        {
            var contextMenu = Scintilla.ContextMenuStrip = new ContextMenuStrip();

            _menuItemCut = new ToolStripMenuItem("Cut", null, (s, ea) => Scintilla.Clipboard.Cut());
            contextMenu.Items.Add(_menuItemCut);

            _menuItemCopy = new ToolStripMenuItem("Copy", null, (s, ea) => Scintilla.Clipboard.Copy());
            contextMenu.Items.Add(_menuItemCopy);

            _menuItemPaste = new ToolStripMenuItem("Paste", null, (s, ea) => Scintilla.Clipboard.Paste());
            contextMenu.Items.Add(_menuItemPaste);

            contextMenu.Items.Add(new ToolStripSeparator());

            _menuItemFind = new ToolStripMenuItem("Find", null, (s, ea) => Scintilla.FindReplace.ShowFind()) { ShortcutKeys = Keys.Control | Keys.F };
            contextMenu.Items.Add(_menuItemFind);

            _menuItemFindReplace = new ToolStripMenuItem("Find and Replace", null, (s, ea) => Scintilla.FindReplace.ShowReplace()) { ShortcutKeys = Keys.Control | Keys.H };

            contextMenu.Items.Add(_menuItemFindReplace);
            _menuItemGotoLine = new ToolStripMenuItem("Go To Line", null, (s, ea) => Scintilla.GoTo.ShowGoToDialog()) { ShortcutKeys = Keys.Control | Keys.G };

            contextMenu.Items.Add(_menuItemGotoLine);
            contextMenu.Items.Add(new ToolStripSeparator());

            _subMenuItemsInsert = new ToolStripItem[4];
            _subMenuItemsInsert[0] = new ToolStripMenuItem("Variable Section Title", null, (s, ea) =>
            {
                if (!string.IsNullOrEmpty(Scintilla.Selection.Text))
                    Scintilla.Selection.Text = "";

                var pos = Scintilla.Caret.Position;

                Scintilla.InsertText(pos,
                    "/*****************************************************************/\n" +
                    "/* Title */");


            })
            { ShortcutKeys = Keys.Control | Keys.D1 };
            _subMenuItemsInsert[1] = new ToolStripMenuItem("Function Title", null, (s, ea) =>
            {
                if (!string.IsNullOrEmpty(Scintilla.Selection.Text))
                    Scintilla.Selection.Text = "";

                var pos = Scintilla.Caret.Position;

                Scintilla.InsertText(pos,
                    "/**\n" +
                    "* Description\n" +
                    "*/");

            })
            { ShortcutKeys = Keys.Control | Keys.D2 };
            _subMenuItemsInsert[2] = new ToolStripMenuItem("Function Title /w params", null, (s, ea) =>
            {
                if (!string.IsNullOrEmpty(Scintilla.Selection.Text))
                    Scintilla.Selection.Text = "";

                var pos = Scintilla.Caret.Position;

                Scintilla.InsertText(pos,
                    "/**\n" +
                    "* Description\n" +
                    "<param name='name'>  <param>" +
                    "\n*/");

            })
            { ShortcutKeys = Keys.Control | Keys.D3 };
            _subMenuItemsInsert[3] = new ToolStripMenuItem("Region Title", null, (s, ea) =>
            {
                if (!string.IsNullOrEmpty(Scintilla.Selection.Text))
                    Scintilla.Selection.Text = "";

                var pos = Scintilla.Caret.Position;

                Scintilla.InsertText(pos,
                    "/******************************************************************\n" +
                    "*  TITLE - Description\n" +
                    "******************************************************************/");

            })
            { ShortcutKeys = Keys.Control | Keys.D4 };
            _menuItemInsert = new ToolStripMenuItem("Insert Comment", null, _subMenuItemsInsert);
            contextMenu.Items.Add(_menuItemInsert);
        }

        void scintilla_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                _inParameter = false;

                if (e.Button != MouseButtons.Right) return;
                _menuItemCut.Enabled = Scintilla.Clipboard.CanCut;
                _menuItemCopy.Enabled = Scintilla.Clipboard.CanCopy;
                _menuItemPaste.Enabled = Scintilla.Clipboard.CanPaste;
            }

            catch
            {
                // ignored
            }
        }
        #endregion

        private void scintilla_TextInserted(object sender, TextModifiedEventArgs e)
        {
            IsSaved(false);
        }

        private void scintilla_TextChanged(object sender, EventArgs e)
        {
            Globals.Main.LengthTextBlock.Text = "length: " + Scintilla.TextLength;
            Globals.Main.LinesTextBlock.Text = "lines: " + Scintilla.Lines.Count;

            // Automatically adjust the left margin to be wide enough to show the largest line number.
            var lines = Scintilla.Lines.Count;
            if (lines == _lastMeasureLines) return;
            _lastMeasureLines = lines;
            Scintilla.Margins[0].Width = TextRenderer.MeasureText(lines.ToString(CultureInfo.InvariantCulture), Scintilla.Font).Width;

            IsSaved(false);
        }

        private void scintilla_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
                IsSaved(false);

            if (!IsIni)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Enter)
                    _inParameter = false;

                if (e.KeyCode == Keys.F1)
                {
                    if (Settings.Default.AutoComplete)
                        Scintilla.AutoComplete.Show();
                }

                else if (Keyboard.IsKeyDown(Key.Space))
                {
                    if (Settings.Default.AutoComplete)
                    {
                        Scintilla.AutoComplete.Cancel();
                        Scintilla.AutoComplete.SelectedText = "";
                    }
                }
            }

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (Keyboard.IsKeyDown(Key.S))
                {
                    try
                    {
                        if (Globals.DirectoryEntered)
                            TextEditor.Save();
                    }

                    catch (Exception ex) { Globals.Logger.Error("Error when using command Ctrl+S: " + ex.Message); }
                }

                else if (Keyboard.IsKeyDown(Key.K))
                {
                    try
                    {
                        if (!IsIni)
                            TextEditor.CommentInOut();
                    }
                    catch (Exception ex) { Globals.Logger.Error("Error when using command Ctrl+K: " + ex.Message); }
                }

                else if (Keyboard.IsKeyDown(Key.W))
                {
                    try { TextEditor.Close(TextEditor.GetActiveTab); }
                    catch (Exception ex) { Globals.Logger.Error("Close error: " + ex.Message); }
                }
            }

            else if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                if (!Keyboard.IsKeyDown(Key.S)) return;
                try
                {
                    if (Globals.DirectoryEntered)
                        TextEditor.SaveAll();
                }

                catch (Exception ex) { Globals.Logger.Error("Error when using command Alt+S: " + ex.Message); }
            }

            else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                if (!Keyboard.IsKeyDown(Key.Tab)) return;
                var current = 0;

                // get selected tab
                var selectedTab = TextEditor.GetActiveTab;

                for (var i = 0; i < Globals.Main.DocumentPane.ChildrenCount; i++)
                    if (Globals.Main.DocumentPane.Children[i].Equals(selectedTab))
                        current = i;

                if (current == Globals.Main.DocumentPane.ChildrenCount - 1)
                    current = 0;
                else
                    current++;

                var doc = Globals.Main.DocumentPane.Children[current] as LayoutDocument;
                Globals.Main.DockManager.ActiveContent = doc;
            }
        }

        private void scintilla_CharAdded(object sender, CharAddedEventArgs e)
        {
            if (e.Ch == ' ')
                return;

            var pos = Scintilla.NativeInterface.GetCurrentPos();
            var word = Scintilla.GetWordFromPosition(pos);

            if (e.Ch == '/' || e.Ch == '*')
            {
                if (_lastChar == '/')
                    _inComment = true;
                _lastChar = e.Ch;
            }

            else if (e.Ch == '(')
            {
                if (TextEditor.InsertParenthesis)
                    Scintilla.InsertText(")");

                if (Settings.Default.FunctionHint)
                {
                    var startfWord = 0;

                    // Count backwards to find the empty space, where the word started
                    for (var i = pos; i > 0; i--)
                        if (string.IsNullOrWhiteSpace(Scintilla.GetWordFromPosition(i)))
                        {
                            startfWord = i;
                            break;
                        }

                    // Get the the word using a position
                    var startObj = Scintilla.NativeInterface.WordStartPosition(startfWord, false);
                    var prevWord = Scintilla.GetWordFromPosition(startObj);

                    // Check the list of keywords to see if it matches any of them, if so display the calltip
                    foreach (var t in TextEditor.KeywordsList)
                    {
                        if (t.Name.Equals(prevWord))
                        {
                            if (string.IsNullOrEmpty(t.CallTipText)) continue;
                            Scintilla.CallTip.Show(t.CallTipText);
                            _keyword = t;
                            _inParameter = true;
                            break;
                        }

                        _inParameter = false;
                    }
                }
            }

            else if (e.Ch == '[')
            {
                if (TextEditor.InsertBracket)
                    Scintilla.InsertText("]");
            }

            else if (e.Ch == '{')
            {
                if (TextEditor.InsertCurlyBracket)
                    Scintilla.InsertText("}");
            }

            else if (e.Ch == '"')
            {
                if (TextEditor.InsertQuotation)
                    Scintilla.InsertText("&quot");
            }

            else if (e.Ch == '\'')
            {
                if (TextEditor.InsertSingleQuotation)
                    Scintilla.InsertText("'");
            }

            if (Settings.Default.FunctionHint)
            {
                if (_inParameter)
                {
                    // Check if user wrote one too many commas, in that case, change the calltip
                    if (e.Ch == ',')
                    {
                        _numCom += 1;
                        if (_numCom >= _keyword?.ParamList.Count)
                        {
                            Scintilla.CallTip.Hide();
                            Scintilla.CallTip.Show("Only " + _keyword.ParamList.Count + " parameters allowed");
                            _inParameter = false;
                        }
                    }

                    // Parameter closed
                    if (e.Ch == ')')
                    {
                        _numCom = 0;
                        _inParameter = false;
                        Scintilla.CallTip.Hide();
                    }
                }

                if (!_inParameter)
                    _numCom = 0;

            }

            if (word == string.Empty)
                return;

            if (Settings.Default.AutoComplete && !_inParameter)
            {
                var list = TextEditor.AutoCompleteList.FindAll(item => item.StartsWith(word));
                if (list.Count > 0)
                    Scintilla.AutoComplete.Show(list);
            }

            IsSaved(false);
        }

        public void ResetInfo()
        {
            if (Scintilla == null) return;
            var line = Scintilla.Lines.Current.Number + 1;
            var col = Scintilla.GetColumn(Scintilla.Caret.Position) + 1;

            Globals.Main.LengthTextBlock.Text = "length: " + Scintilla.TextLength;
            Globals.Main.LinesTextBlock.Text = "lines: " + Scintilla.Lines.Count;

            Globals.Main.LineTextBlock.Text = "line: " + line;
            Globals.Main.ColumnTextBlock.Text = "col: " + col;
            Globals.Main.SelTextBlock.Text = Scintilla.Selection.Length.ToString(CultureInfo.InvariantCulture);
        }

        private void scintilla_SelectionChanged(object sender, EventArgs e)
        {
            var line = Scintilla.Lines.Current.Number + 1;
            var col = Scintilla.GetColumn(Scintilla.Caret.Position) + 1;

            Globals.Main.LineTextBlock.Text = "line: " + line;
            Globals.Main.ColumnTextBlock.Text = "col: " + col;
            Globals.Main.SelTextBlock.Text = Scintilla.Selection.Length.ToString(CultureInfo.InvariantCulture);

            // Highlight matched words
            Scintilla.Indicators[1].Style = IndicatorStyle.RoundBox;
            Scintilla.Indicators[1].Color = Color.LightPink;

            var range = Scintilla.GetRange(0, Scintilla.Text.Length - 1);
            range.ClearIndicator(1);

            if (Scintilla.Selection.Start == Scintilla.Selection.End) return;

            var selectedWord = Scintilla.Selection.Text;

            // Make sure the selection isn't null or a whitespace, and isn't an Alt+Drag selection
            if (string.IsNullOrWhiteSpace(selectedWord) || selectedWord.Contains("\0")) return;

            Scintilla.FindReplace.Flags = SearchFlags.WholeWord;
            IList<Range> ranges = Scintilla.FindReplace.FindAll(selectedWord);

            foreach (var r in ranges)
            {
                r.SetIndicator(1);
            }
        }

        public bool AttemptClose()
        {
            Globals.Main.LineTextBlock.Text = "line: 0";
            Globals.Main.ColumnTextBlock.Text = "col: 0";
            Globals.Main.SelTextBlock.Text = "0";

            Globals.Main.LengthTextBlock.Text = "length: 0";
            Globals.Main.LinesTextBlock.Text = "lines: 0";

            if (Saved) return true;
            string newTitle;

            if (Title.Length > 17)
            {
                newTitle = Title.Remove(17, Title.Length - 17);
                newTitle = newTitle + "...";
            }

            else
                newTitle = Title;

            // Prompt if not saved
            var title = String.Format(CultureInfo.CurrentCulture, "The text in {0} has changed.", newTitle.TrimEnd(' ', '*'));

            var dr = Globals.MsgBox.Show(Globals.Main, "Do you want to save the changes?", title, MessageBoxButton.YesNo, MessageBoxIconType.Warning);
            switch (dr)
            {
                case MessageBoxResult.No:
                    return true;
                case MessageBoxResult.Yes:
                    Save();
                    return true;
                default:
                    return false;
            }
        }

        public bool Save()
        {
            return string.IsNullOrEmpty(FilePath) ? SaveAs() : Save(FilePath);
        }

        public static ImageSourceConverter ImgConvert = new ImageSourceConverter();
        public void IsSaved(bool saved)
        {
            if (!saved)
                ParentTab.IconSource = (ImageSource)ImgConvert.ConvertFrom(new Uri("pack://application:,,,/Resources/classNoSave.png"));

            else
                ParentTab.IconSource = (ImageSource)ImgConvert.ConvertFrom(new Uri("pack://application:,,,/Resources/class.png"));

            Saved = saved;
        }

        public bool Save(string path)
        {
            using (var fs = File.Create(path))
            using (var bw = new BinaryWriter(fs))
                bw.Write(Scintilla.RawText, 0, Scintilla.RawText.Length - 1); // Omit trailing NULL

            Scintilla.Modified = false;
            IsSaved(true);

            return true;
        }

        public bool SaveAs()
        {
            TextEditor.SaveFileDialog.FileName = Title;
            if (TextEditor.SaveFileDialog.ShowDialog() != DialogResult.OK) return false;

            FilePath = TextEditor.SaveFileDialog.FileName;
            Title = Path.GetFileNameWithoutExtension(FilePath);
            Globals.Logger.Info("Successfully saved to file " + Path.GetFileNameWithoutExtension(FilePath) + " at " + FilePath);

            return Save(FilePath);
        }

        private void Document_Loaded(object sender, RoutedEventArgs e)
        {
            SetFocus();
        }

        public void SetFocus()
        {
            Scintilla.Focus();
        }
    }
}

public class KeyWord
{
    public string Name;
    public bool Func;
    public string Type;
    public string CallTipText;

    public List<string> ParamList = new List<string>();

    public KeyWord()
    {
    }
    public KeyWord(string name, bool func)
    {
        Name = name;
        Func = func;
    }

    public void AddParam(string name)
    {
        if (CallTipText == null)
            CallTipText = Type + " " + Name + " (" + name + ")";
        else
        {
            CallTipText = CallTipText.TrimEnd(')');
            CallTipText += ", " + name + ")";
        }

        if (!ParamList.Contains(name))
            ParamList.Add(name);
    }

    public void RemoveParam(string name)
    {
        if (ParamList.Contains(name))
            ParamList.Remove(name);

        CallTipText = Type + " " + Name + " (";

        if (ParamList.Count == 0) return;
        foreach (var t in ParamList)
        {
            CallTipText = CallTipText.TrimEnd(')');
            CallTipText += ", " + t + ")";
        }
    }
}
