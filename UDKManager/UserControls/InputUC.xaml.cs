using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using ZerO.Helpers;
using ZerO.Windows;
using static System.String;

namespace ZerO.UserControls
{
    public enum CustomType
    {
        Project,
        Class,
        Map,
        Other
    }

    public enum Action
    {
        New,
        Rename,
        Search,
        None,
    }

    /// <summary>
    /// Interaction logic for InputUC.xaml
    /// </summary>
    public partial class InputUc
    {
        private Action _action;
        private CustomType _type;
        private CustomItem _currentItem;

        public CustomWindow ParentWindow;

        public InputUc()
        {
            InitializeComponent();
        }

        public void Initialize(CustomWindow parent, CustomItem item, CustomType cType, Action cAction)
        {
            IsEnabled = true;
            ParentWindow = parent;
            _action = cAction;
            _type = cType;

            if (item != null)
                _currentItem = item;

            Globals.ClearControls(this);

            switch (cAction)
            {
                case Action.Rename:
                    {
                        MessageLabel.Content = "Enter New Name:";
                        ParentWindow.Title = "Rename";

                        var name = _currentItem.Header.ToString().Split('.');
                        NameTextBox.Text = name[0];
                        NameTextBox.SelectAll();
                        break;
                    }

                case Action.Search:
                    {
                        MessageLabel.Content = "Enter filename to search for:";
                        ParentWindow.Title = "Search";
                        break;
                    }
            }

            switch (cType)
            {
                case CustomType.Project:
                    {
                        if (cAction == Action.New)
                        {
                            MessageLabel.Content = "Enter Project Name:";
                            ParentWindow.Title = "New Project";
                        }
                        break;
                    }

                case CustomType.Class:
                    {
                        if (cAction == Action.New)
                        {
                            MessageLabel.Content = "Enter Class Name:";
                            ParentWindow.Title = "New Class";
                        }
                        break;
                    }
            }

            FocusHelper.Focus(NameTextBox);
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.Text == "")
                Globals.MsgBox.Show(ParentWindow, "You forgot to enter a name!", "Error",
                                 MessageBoxButton.OK, MessageBoxIconType.Error);

            else
            {
                switch (_type)
                {
                    case CustomType.Project:
                        {
                            if (_action == Action.New)
                            {
                                if (!Directory.Exists(Globals.ProjectsPath + NameTextBox.Text))
                                {
                                    Directory.CreateDirectory(Globals.ProjectsPath + NameTextBox.Text);
                                    Directory.CreateDirectory(Globals.ProjectsPath + NameTextBox.Text + "/Classes");

                                    Globals.Logger.Info("Successfully created project '" + NameTextBox.Text + "'. and added it to DefaultEngine.ini");

                                    IniFileManager.AddValue("[Engine.ScriptPackages]", "+NonNativePackages=" + NameTextBox.Text);
                                    IniFileManager.AddValue("[UnrealEd.EditorEngine]", "+EditPackages=" + NameTextBox.Text);

                                        Globals.Main.UpdateBrowseList();
                                        IsEnabled = false;
                                        ParentWindow.HideWindow();
                                }

                                else
                                {
                                    Globals.MsgBox.Show(ParentWindow, "The project '" + NameTextBox.Text + "' already exists!", "Error",
                                     MessageBoxButton.OK, MessageBoxIconType.Error);
                                    NameTextBox.Clear();
                                    NameTextBox.Focus();
                                }

                            }

                            else if (_action == Action.Rename)
                            {
                                if (Directory.Exists(_currentItem.Path))
                                {
                                    Directory.Move(_currentItem.Path, Globals.ProjectsPath + "/" + NameTextBox.Text);

                                    IniFileManager.ReplaceValue("+NonNativePackages=" + _currentItem.Header, "+NonNativePackages=" + NameTextBox.Text);
                                    IniFileManager.ReplaceValue("+EditPackages=" + _currentItem.Header, "+EditPackages=" + NameTextBox.Text);

                                    Globals.Logger.Info("Successfully renamed project '" + _currentItem.Header + "' to '" + NameTextBox.Text + "', also changed the name in DefaultEngine.ini");
                                    Globals.Main.UpdateBrowseList();
                                    IsEnabled = false;
                                    ParentWindow.HideWindow();
                                }

                                else
                                {
                                    Globals.MsgBox.Show(ParentWindow, "The project '" + NameTextBox.Text + "' already exists!", "Error",
                                     MessageBoxButton.OK, MessageBoxIconType.Error);
                                    NameTextBox.Clear();
                                    NameTextBox.Focus();
                                }
                            }
                            break;
                        }

                    case CustomType.Class:
                        {
                            string classPath;

                            if (_currentItem.ParentFolder != null)
                                classPath = _currentItem.ParentFolder.Path + "/Classes/";
                            else
                                classPath = _currentItem.Path + "/Classes/";

                            if (_action == Action.New)
                            {
                                if (NameTextBox.Text == _currentItem.Header.ToString())
                                {
                                    IsEnabled = false;
                                    ParentWindow.HideWindow();
                                }

                                else if (!File.Exists(classPath + NameTextBox.Text + ".uc"))
                                {
                                    Globals.CreateFile(classPath + NameTextBox.Text + ".uc", "class " + NameTextBox.Text);
                                    Globals.Logger.Info("Successfully created class '" + NameTextBox.Text + "'.");
                                    Globals.Main.UpdateBrowseList();
                                    IsEnabled = false;
                                    ParentWindow.HideWindow();
                                }

                                else
                                {
                                    Globals.MsgBox.Show(ParentWindow, "The project '" + NameTextBox.Text + "' already exists!", "",
                                    MessageBoxButton.OK, MessageBoxIconType.Error);
                                    NameTextBox.Clear();
                                    NameTextBox.Focus();
                                }
                            }

                            if (_action == Action.Rename)
                            {
                                if (!File.Exists(classPath + NameTextBox.Text + ".uc"))
                                {
                                    var oldName = _currentItem.Header.ToString();

                                    // Check all open tabs 
                                    foreach (var t in TextEditor.AllOpenDocs)
                                    if (!IsNullOrEmpty(t.FilePath))
                                    {
                                        var newFilePath = t.FilePath.Replace('/', '\\');

                                        if (!_currentItem.Path.Equals(newFilePath)) continue;
                                        // Close the old file
                                        t.ParentTab.IsSelected = true;
                                        TextEditor.Close(TextEditor.GetActiveTab);
                                        break;
                                    }

                                    File.Move(classPath + _currentItem.Header, classPath + NameTextBox.Text + ".uc");
                                    if (File.Exists(_currentItem.Path))
                                        File.Delete(_currentItem.Path);

                                    // Change the class name inside the file
                                    var regex = new Regex(Regex.Escape(oldName.Split('.')[0]));
                                    File.WriteAllText(classPath + NameTextBox.Text + ".uc", regex.Replace(File.ReadAllText(classPath + NameTextBox.Text + ".uc"), NameTextBox.Text, 1));

                                    // Open the file in the editor
                                    TextEditor.OpenFile(classPath + NameTextBox.Text + ".uc");

                                    // Tell the user that the operation is done
                                    Globals.Logger.Info("Successfully renamed class '" + _currentItem.Header + "' to '" + NameTextBox.Text + ".uc'.");

                                    // Make sure the last selected item is not with the old name before updating the treeview
                                    Globals.Main.UpdateBrowseList(NameTextBox.Text + ".uc");

                                    IsEnabled = false;
                                    ParentWindow.HideWindow();
                                }

                                else
                                {
                                    Globals.MsgBox.Show(ParentWindow, "The class '" + NameTextBox.Text + "' already exists!", "",
                                    MessageBoxButton.OK, MessageBoxIconType.Error);
                                    NameTextBox.Clear();
                                    NameTextBox.Focus();
                                }
                            }

                            break;
                        }

                    case CustomType.Other:
                        {
                            if (_action == Action.Search)
                            {
                                Globals.FileSearch(NameTextBox.Text);
                                Globals.Main.SearchTabItem.Focus();
                                IsEnabled = false;
                                ParentWindow.HideWindow();
                            }

                            break;
                        }
                }
            }
        }

        private const string ReservedCharacters = "!*'();:@&=+$,/?%#[]-";

        public static string UrlEncode(string value)
        {
            if (IsNullOrEmpty(value))
                return Empty;

            var sb = new StringBuilder();

            foreach (char @char in value)
            {
                if (ReservedCharacters.IndexOf(@char) == -1)
                    sb.Append(@char);
                else
                    sb.AppendFormat("%{0:X2}", (int)@char);
            }
            return sb.ToString();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            ParentWindow.HideWindow();
        }
    }
}
