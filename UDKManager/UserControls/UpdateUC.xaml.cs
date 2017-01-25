using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using mshtml;
using ZerO.Helpers;
using ZerO.Windows;

namespace ZerO.UserControls
{
    /// <summary>
    /// Interaction logic for UpdateUC.xaml
    /// </summary>
    public partial class UpdateUc
    {
        public CustomWindow ParentWindow;

        public UpdateUc()
        {
            InitializeComponent();
        }

        public void Initialize(CustomWindow parent)
        {
            IsEnabled = true;
            ParentWindow = parent;

            Globals.ClearControls(this);

            if (!string.IsNullOrEmpty(Globals.Main.Updater.AppCast.ReleaseNotesLink))
                Browser.Source = new Uri(Globals.Main.Updater.AppCast.ReleaseNotesLink);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            ParentWindow.HideWindow();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            try { Globals.Main.Updater.DownloadUpdate(); }
            catch (Exception ex) { Globals.Logger.Error("Unable to start update: " + ex.Message); }
        }

        private void historyHL_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://pastebin.com/sTeM1akM");
        }

        private void browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            IHTMLDocument2 doc = Browser.Document as IHTMLDocument2;
            if (doc == null) return;
            doc.execCommand("SelectAll", false, null);
            doc.execCommand("FontSize", false, 2);
            doc.execCommand("Unselect", false, null);
        }
    }
}
