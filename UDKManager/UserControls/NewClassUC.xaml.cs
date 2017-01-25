using System.Windows;
using ZerO.Helpers;
using ZerO.Windows;

namespace ZerO.UserControls
{
    /// <summary>
    /// Interaction logic for NewClass.xaml
    /// </summary>
    public partial class NewClassUc
    {
        private CustomItem _currentItem;
        public CustomWindow ParentWindow;

        public NewClassUc()
        {
            InitializeComponent();
        }

        public void Initialize(CustomWindow parent, CustomItem item)
        {
            IsEnabled = true;
            ParentWindow = parent;

            if (item != null)
                _currentItem = item;

            Globals.ClearControls(this);

            EmptyRadioButton.IsChecked = true;

            PreBeginCheckBox.IsChecked = false;
            PostBeginCheckBox.IsChecked = false;
            TickCheckBox.IsChecked = false;
            DefaultPropertiesCheckBox.IsChecked = false;

            ExtendsTextBox.IsEnabled = false;
            PreBeginCheckBox.IsEnabled = false;
            PostBeginCheckBox.IsEnabled = false;
            TickCheckBox.IsEnabled = false;
            DefaultPropertiesCheckBox.IsEnabled = false;

            FocusHelper.Focus(NameTextBox);
        }

        private void emptyRB_Click(object sender, RoutedEventArgs e)
        {
            ExtendsTextBox.IsEnabled = false;
            PreBeginCheckBox.IsEnabled = false;
            PostBeginCheckBox.IsEnabled = false;
            TickCheckBox.IsEnabled = false;
            DefaultPropertiesCheckBox.IsEnabled = false;
        }

        private void EnableAll()
        {
            ExtendsTextBox.IsEnabled = true;
            PreBeginCheckBox.IsEnabled = true;
            PostBeginCheckBox.IsEnabled = true;
            TickCheckBox.IsEnabled = true;
            DefaultPropertiesCheckBox.IsEnabled = true;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            string classPath;

            if (_currentItem.ParentFolder != null)
                classPath = _currentItem.ParentFolder.Path + "/Classes/";
            else
                classPath = _currentItem.Path + "/Classes/";

            if (NameTextBox.Text == "")
                Globals.MsgBox.Show(ParentWindow, "You forgot to enter a name!", "Error",
                                 MessageBoxButton.OK, MessageBoxIconType.Error);

            if (EmptyRadioButton.IsChecked == true)
                Globals.CreateFile(classPath + NameTextBox.Text + ".uc", "class " + NameTextBox.Text);

            else
            {
                var text = "class " + NameTextBox.Text + " extends " + ExtendsTextBox.Text + ";";

                if (PreBeginCheckBox.IsChecked == true)
                    text += "\n\nsimulated function PreBeginPlay()\n{\n          super.PreBeginPlay();\n}";
                if (PostBeginCheckBox.IsChecked == true)
                    text += "\n\nsimulated function PostBeginPlay()\n{\n         super.PostBeginPlay();\n}";
                if (TickCheckBox.IsChecked == true)
                {
                    if (!ExtendsTextBox.Text.Contains("PlayerController"))
                        text += "\n\nfunction Tick(float deltaTime)\n{\n         super.Tick(deltaTime);\n}";
                    else
                        text += "\n\nsimulated function PlayerTick(float deltaTime)\n{\n         super.PlayerTick(deltaTime);\n}";
                }
                if (DefaultPropertiesCheckBox.IsChecked == true)
                    text += "\n\ndefaultproperties\n{\n}";

                Globals.CreateFile(classPath + NameTextBox.Text + ".uc", text);
            }

            Globals.Logger.Info("Successfully created class '" + NameTextBox.Text + "'.");
            Globals.Main.UpdateBrowseList();

            TextEditor.OpenFile(classPath + NameTextBox.Text + ".uc");

            IsEnabled = false;
            ParentWindow.HideWindow();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            ParentWindow.HideWindow();
        }

        private void classRB_Checked(object sender, RoutedEventArgs e)
        {
            ExtendsTextBox.Text = "Object";
            EnableAll();
        }

        private void weaponRB_Checked(object sender, RoutedEventArgs e)
        {
            ExtendsTextBox.Text = "UDKWeapon";
            EnableAll();
        }

        private void pawnRB_Checked(object sender, RoutedEventArgs e)
        {
            ExtendsTextBox.Text = "UDKPawn";
            EnableAll();
        }

        private void playerControllerRB_Checked(object sender, RoutedEventArgs e)
        {
            ExtendsTextBox.Text = "UDKPlayerController";
            EnableAll();
        }

        private void gameRB_Checked(object sender, RoutedEventArgs e)
        {
            ExtendsTextBox.Text = "UDKGame";
            EnableAll();
        }
    }
}
