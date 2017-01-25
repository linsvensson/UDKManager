using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro;
using MahApps.Metro.Controls;
using ZerO.Helpers;
using ZerO.Properties;

namespace ZerO.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        public bool IsShowing;

        public SettingsWindow()
        {
            InitializeComponent();

            ThemeComboBox.Items.Add("Dark");
            ThemeComboBox.Items.Add("Light");
        }

        public void Show(MetroWindow owner)
        {
            if (owner != null || Owner == null)
                Owner = owner;

            IsShowing = true;

            ThemeComboBox.Text = Globals.CurrentTheme.ToString();

            IsEnabled = true;
            ShowDialog();
        }

        private void greenButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.ChangeTheme("Green", Globals.CurrentTheme);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            IsShowing = false;
            IsEnabled = false;
            Hide();
        }

        private void blueButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.ChangeTheme("Blue", Globals.CurrentTheme);
        }

        private void purpleButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.ChangeTheme("Purple", Globals.CurrentTheme);
        }

        private void orangeButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.ChangeTheme("Orange", Globals.CurrentTheme);
        }

        private void redButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.ChangeTheme("Red", Globals.CurrentTheme);
        }

        private void themeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            Globals.CurrentTheme = ThemeComboBox.Text == "Light" ? Theme.Light : Theme.Dark;

            Globals.ChangeTheme(Globals.CurrentAccent.Name, Globals.CurrentTheme);
        }

        private void changeRootFolder_Click(object sender, RoutedEventArgs e)
        {
            Globals.SetRootFolder();
        }

        private void setExeFile_Click(object sender, RoutedEventArgs e)
        {
            Globals.SetExe();
        }

        private void trayCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (TrayCheckBox.IsChecked == true)
            {
                // Enable "minimize to tray" behavior for main Window
                MinimizeToTray.Enable(Globals.Main);
            }

            else
            {
                // Disable "minimize to tray" behavior for main Window
                MinimizeToTray.Disable(Globals.Main);
            }
        }

        private void setMapsFolder_Click(object sender, RoutedEventArgs e)
        {
            Globals.SetMaps();
        }

        private void rememberCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (RememberCheckBox.IsChecked != null)
            {
                Settings.Default.SaveSession = RememberCheckBox.IsChecked.Value;

                if (RememberCheckBox.IsChecked == true)
                {
                    if (Settings.Default.LastTextFiles == null)
                        Settings.Default.LastTextFiles = new StringCollection();

                    foreach (var t in TextEditor.AllOpenDocs.Where(t => !string.IsNullOrEmpty(t.FilePath)))
                        Settings.Default.LastTextFiles.Add(t.FilePath);
                }

                else
                    Settings.Default.LastTextFiles.Clear();
            }

            Settings.Default.Save();
        }

        private void forceUpdate_Click(object sender, RoutedEventArgs e)
        {
            try { Globals.Main.Updater.ForceUpdateCheck(); }
            catch (Exception ex) { Globals.Logger.Warn("Could not connect to update server: " + ex.Message); Globals.MsgBox.Show(this, "Could not connect to update server!", "Update Failed", MessageBoxButton.OK, MessageBoxIconType.Error); }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null) return;

            switch (cb.Name)
            {
                case "autoUpdateCheckBox":
                    Settings.Default.AutoUpdate = AutoUpdateCheckBox.IsChecked == true;
                    break;

                case "functionHintCheckBox":
                    Settings.Default.FunctionHint = FunctionHintCheckBox.IsChecked == true;

                    if (FunctionHintCheckBox.IsChecked == true)
                    {
                        InsertParentCheckBox.IsChecked = false;
                        Settings.Default.AutoInsertList.Remove("(");
                    }
                    break;

                case "lineHighlightCB":
                    Settings.Default.LineHighlight = LineHighlightCheckBox.IsChecked.Value;

                    TextEditor.ToggleLineHighlight(LineHighlightCheckBox.IsChecked == true);
                    break;

                case "lineNumberCB":
                    Settings.Default.LineNumbers = LineNumberCheckBox.IsChecked.Value;

                    if (LineNumberCheckBox.IsChecked == true)
                        TextEditor.ToggleLineNumbers(true);
                    else
                        TextEditor.ToggleLineNumbers(false);
                    break;

                case "autoCompleteCheckBox":
                    Settings.Default.AutoComplete = AutoCompleteCheckBox.IsChecked.Value;
                    break;
            }

            Settings.Default.Save();
        }

        private void autoInsertCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null) return;

            if (Settings.Default.AutoInsertList.Contains(cb.Content.ToString()))
                Settings.Default.AutoInsertList.Remove(cb.Content.ToString());
            else
                Settings.Default.AutoInsertList.Add(cb.Content.ToString());

            Settings.Default.Save();

            // Reset them all
            TextEditor.InsertParenthesis = false;
            TextEditor.InsertBracket = false;
            TextEditor.InsertCurlyBracket = false;
            TextEditor.InsertQuotation = false;
            TextEditor.InsertSingleQuotation = false;

            foreach (string t in Settings.Default.AutoInsertList)
            {
                if (t.Equals("("))
                {
                    TextEditor.InsertParenthesis = true;
                    FunctionHintCheckBox.IsChecked = false;
                }

                else if (t.Equals("["))
                    TextEditor.InsertBracket = true;
                else if (t.Equals("{"))
                    TextEditor.InsertCurlyBracket = true;
                else if (t.Equals("&quot"))
                    TextEditor.InsertQuotation = true;
                else if (t.Equals("'"))
                    TextEditor.InsertSingleQuotation = true;
            }
        }
    }
}
