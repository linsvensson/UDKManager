using System.Linq;
using System.Windows;
using ZerO.Helpers;
using ZerO.Windows;

namespace ZerO.UserControls
{
    /// <summary>
    /// Interaction logic for ProjectChooser.xaml
    /// </summary>
    public partial class ChooserUc
    {
        private CustomType _type;
        public CustomWindow ParentWindow;

        public ChooserUc()
        {
            InitializeComponent();
        }

        public void Initialize(CustomWindow parent, CustomType cType)
        {
            IsEnabled = true;
            ParentWindow = parent;
            _type = cType;

            Globals.ClearControls(this);
            CComboBox.Items.Clear();

            if (cType == CustomType.Project)
            {
                MessageLabel.Content = "Choose a project:";

                for (var i = 0; i < Globals.Main.BrowseTreeView.Items.Count; i++)
                {
                    var item = Globals.Main.BrowseTreeView.Items[i] as CustomItem;
                    if (item != null) CComboBox.Items.Add(item.Header);
                }
            }

            CComboBox.Focus();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (CComboBox.Text == "")
                Globals.MsgBox.Show(ParentWindow, "You forgot to enter a name!", "Error",
                                 MessageBoxButton.OK, MessageBoxIconType.Error);
            else
            {
                switch (_type)
                {
                    case CustomType.Project:
                    {
                        foreach (var item in Globals.Main.BrowseTreeView.Items.Cast<object>().OfType<CustomItem>().Where(item => item.Header.ToString() == CComboBox.Text))
                        {
                            IsEnabled = false;
                            ParentWindow.HideWindow();
                            Globals.CWindow.ShowInput(Globals.Main, item, CustomType.Class, Action.New);
                        }
                        break;
                    }
                }
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            ParentWindow.HideWindow();
        }
    }
}
