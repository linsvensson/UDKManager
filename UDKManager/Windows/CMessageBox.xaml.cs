using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using ZerO.Helpers;

namespace ZerO
{
    public enum MessageBoxIconType
    {
        None,
        Info,
        Warning,
        Error,
        Question
    }

    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class CMessageBox
    {
        private MessageBoxButton _buttons = MessageBoxButton.OK;
        private MessageBoxIconType _msgIcon = MessageBoxIconType.None;

        public bool IsShowing;

        public CMessageBox()
        {
            InitializeComponent();
        }

        #region internal Properties
        internal MessageBoxButton Buttons
        {
            get { return _buttons; }
            set
            {
                _buttons = value;
                // Set all Buttons Visibility Properties
                SetButtonsVisibility();
            }
        }

        internal MessageBoxIconType MsgIcon
        {
            get { return _msgIcon; }
            set
            {
                _msgIcon = value;
                // Set all Buttons Visibility Properties
                SetIconVisibility();
            }
        }

        internal MessageBoxResult Result { get; set; } = MessageBoxResult.None;

        #endregion

        #region SetIcon Method
        internal void SetIconVisibility()
        {
            switch (_msgIcon)
            {
                case MessageBoxIconType.None:
                    IconImage = null;
                    break;
                case MessageBoxIconType.Info:
                    IconImage.Source = Globals.GetBitmapImage("info");
                    break;
                case MessageBoxIconType.Error:
                    IconImage.Source = Globals.GetBitmapImage("error");
                    break;
                case MessageBoxIconType.Question:
                    IconImage.Source = Globals.GetBitmapImage("question");
                    break;
                case MessageBoxIconType.Warning:
                    IconImage.Source = Globals.GetBitmapImage("warning");
                    break;
            }
        }
        #endregion

        #region SetButtonsVisibility Method
        internal void SetButtonsVisibility()
        {
            switch (_buttons)
            {
                case MessageBoxButton.OK:
                    OkButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Collapsed;
                    YesButton.Visibility = Visibility.Collapsed;
                    NoButton.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.OKCancel:
                    OkButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;
                    YesButton.Visibility = Visibility.Collapsed;
                    NoButton.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    OkButton.Visibility = Visibility.Collapsed;
                    CancelButton.Visibility = Visibility.Collapsed;
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    OkButton.Visibility = Visibility.Collapsed;
                    CancelButton.Visibility = Visibility.Visible;
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    break;
            }
        }
        #endregion

        #region Button Click Events
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            HideWindow();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            HideWindow();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            HideWindow();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            HideWindow();
        }
        #endregion

        #region Deactivated Event
        private void Window_Deactivated(object sender, EventArgs e)
        {
            // If only an OK button is displayed, 
            // allow the user to just move away from this dialog box
            if (Buttons == MessageBoxButton.OK)
                Hide();
        }
        #endregion

        public MessageBoxResult Show(string message)
        {
            return Show(null, message, string.Empty, MessageBoxButton.OK , MessageBoxIconType.None);
        }

        public MessageBoxResult Show(string message, string caption)
        {
            return Show(null, message, caption, MessageBoxButton.OK, MessageBoxIconType.None);
        }

        public MessageBoxResult Show(MetroWindow owner, string message, string caption, MessageBoxButton buttons, MessageBoxIconType icon)
        {
            if (owner != null || Owner == null)
                Owner = owner;

            TitleTextBlock.Text = caption;
            MessageTextBlock.Text = message;
            Buttons = buttons;
            MsgIcon = icon;

            IsEnabled = true;
            IsShowing = true;

                ShowDialog();
                var result = Result;

            return result;
        }

        private void HideWindow()
        {
            IsEnabled = false;
            IsShowing = false;

            Hide();
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            if (Result == MessageBoxResult.None)
                e.Cancel = true;

            IsEnabled = false;
            IsShowing = false;
        }
    }
}
