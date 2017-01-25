using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using ZerO.Helpers;
using ZerO.Windows;

namespace ZerO.UserControls
{
    /// <summary>
    /// Interaction logic for TestMapUC.xaml
    /// </summary>
    public partial class TestMapUc
    {
        private string _map, _project, _mainGame;

        public CustomWindow ParentWindow;

        public TestMapUc()
        {
            InitializeComponent();
        }

        public void Initialize(CustomWindow parent)
        {
            IsEnabled = true;
            ParentWindow = parent;

            MainGameButton.IsEnabled = false;
            MainGameTextBox.IsEnabled = false;

            ProjectComboBox.Items.Clear();

            for (var i = 0; i < Globals.Main.BrowseTreeView.Items.Count; i++)
            {
                var item = Globals.Main.BrowseTreeView.Items[i] as TreeViewItem;

                if (item != null && ((string) item.Tag).Equals("project"))
                    ProjectComboBox.Items.Add(item.Header);
            }

            Globals.ClearControls(this);

            NameTextBox.Focus();
        }

        private void mapButton_Click(object sender, RoutedEventArgs e)
        {
            var path = Globals.MapsFolder;

            Globals.FileBrowserDialog.DefaultExt = "*udk";
            Globals.FileBrowserDialog.Filter = @"udk Files (*.udk)|*.udk";
            Globals.FileBrowserDialog.InitialDirectory = path;
            Globals.FileBrowserDialog.Title = @"Choose a map file";
            Globals.DialogResult = Globals.FileBrowserDialog.ShowDialog();

            if (Globals.DialogResult != DialogResult.OK) return;
            var separator = new[] { '.' };
            if (Globals.FileBrowserDialog.SafeFileName == null) return;
            var stringArr = Globals.FileBrowserDialog.SafeFileName.Split(separator);

            MapTextBox.Text = stringArr[0];
            _map = stringArr[0];
        }

        private void mainGameButton_Click(object sender, RoutedEventArgs e)
        {
            var path = Globals.RootDirectory + "\\Development\\Src\\" + _project + "\\Classes\\";

            Globals.FileBrowserDialog.DefaultExt = "*uc";
            Globals.FileBrowserDialog.Filter = @"uc Files (*.uc)|*.uc";
            Globals.FileBrowserDialog.InitialDirectory = path;
            Globals.FileBrowserDialog.Title = @"Choose a class file";
            Globals.DialogResult = Globals.FileBrowserDialog.ShowDialog();

            if (Globals.DialogResult != DialogResult.OK) return;
            var separator = new[] { '.' };
            if (Globals.FileBrowserDialog.SafeFileName == null) return;
            var stringArr = Globals.FileBrowserDialog.SafeFileName.Split(separator);

            MainGameTextBox.Text = stringArr[0];
            _mainGame = stringArr[0];
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.Text == "" && MapTextBox.Text == "" && ProjectComboBox.Text == "" && MainGameTextBox.Text == "")
                Globals.MsgBox.Show(ParentWindow, "You've left one or more controls empty.", "Missing information", MessageBoxButton.OK, MessageBoxIconType.Warning);

            else
            {
                Globals.CreateFile(Globals.MapsFolder + "\\TestMaps\\" + NameTextBox.Text + ".bat", "\"" + Globals.ExeFile + "\"" + " " + _map + "?Goalscore=0?TimeLimit=0?Game=" + _project + "." + _mainGame + " -log");
                Globals.Main.UpdateMapList();
                IsEnabled = false;
                ParentWindow.HideWindow();
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            ParentWindow.HideWindow();
        }

        private void projectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProjectComboBox.SelectedItem != null)
                _project = ProjectComboBox.SelectedItem.ToString();

            MainGameButton.IsEnabled = true;
            MainGameTextBox.IsEnabled = true;
        }
    }
}
