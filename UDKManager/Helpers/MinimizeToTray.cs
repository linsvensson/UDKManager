using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using MahApps.Metro.Controls;

namespace ZerO.Helpers
{
    /// <summary>
    /// Class implementing support for "minimize to tray" functionality.
    /// </summary>
    public static class MinimizeToTray
    {
        private static readonly MinimizeToTrayInstance TrayInstance = new MinimizeToTrayInstance();

        /// <summary>
        /// Enables "minimize to tray" behavior for the specified Window.
        /// </summary>
        /// <param name="window">Window to enable the behavior for.</param>
        public static void Enable(MetroWindow window)
        {
            TrayInstance.Enable(window);
        }

        /// <summary>
        /// Diables "minimize to tray" behavior for the specified Window.
        /// </summary>
        /// <param name="window">Window to enable the behavior for.</param>
        public static void Disable(MetroWindow window)
        {
            TrayInstance.Disable();
        }

        /// <summary>
        /// Class implementing "minimize to tray" functionality for a Window instance.
        /// </summary>
        private class MinimizeToTrayInstance
        {
            private MetroWindow _window;
            private NotifyIcon _notifyIcon;
            private bool _balloonShown;

            /// <summary>
            /// Enables minimize to tray
            /// </summary>
            /// <param name="metroWindow">Window instance to attach to.</param>
// ReSharper disable once MemberHidesStaticFromOuterClass
            public void Enable(MetroWindow metroWindow)
            {
                Debug.Assert(metroWindow != null, "metroWindow parameter is null.");
                _window = metroWindow;
                _window.StateChanged += HandleStateChanged;
            }

            /// <summary>
            /// Disables minimize to tray
            /// </summary>
            public void Disable()
            {
                // Remove all event handlers
                _notifyIcon.MouseClick -= HandleNotifyIconOrBalloonClicked;
                _notifyIcon.BalloonTipClicked -= HandleNotifyIconOrBalloonClicked;
                _window.StateChanged -= HandleStateChanged;

                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();

                _window = null;
            }

            /// <summary>
            /// Handles the Window's StateChanged event.
            /// </summary>
            /// <param name="sender">Event source.</param>
            /// <param name="e">Event arguments.</param>
            private void HandleStateChanged(object sender, EventArgs e)
            {
                if (_notifyIcon == null)
                {
                    // Initialize NotifyIcon instance "on demand"
                    _notifyIcon = new NotifyIcon
                    {
                        Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location)
                    };
                    _notifyIcon.MouseClick += HandleNotifyIconOrBalloonClicked;
                    _notifyIcon.BalloonTipClicked += HandleNotifyIconOrBalloonClicked;
                }
                // Update copy of Window Title in case it has changed
                _notifyIcon.Text = _window.Title;

                // Show/hide Window and NotifyIcon
                var minimized = (_window.WindowState == WindowState.Minimized);
                _window.ShowInTaskbar = !minimized;
                _notifyIcon.Visible = minimized;
                if (!minimized || _balloonShown) return;
                // If this is the first time minimizing to the tray, show the user what happened
                _notifyIcon.ShowBalloonTip(1000, null, _window.Title, ToolTipIcon.None);
                _balloonShown = true;
            }

            /// <summary>
            /// Handles a click on the notify icon or its balloon.
            /// </summary>
            /// <param name="sender">Event source.</param>
            /// <param name="e">Event arguments.</param>
            private void HandleNotifyIconOrBalloonClicked(object sender, EventArgs e)
            {
                // Restore the Window
                _window.WindowState = WindowState.Normal;
            }
        }
    }
}
