using System;
using System.Diagnostics;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Threading;
using MahApps.Metro.Controls;

namespace ZerO.Helpers
{
    /// <summary>
    /// Helper to show or close given splash window
    /// </summary>
    public static class Splasher
    {
        /// <summary>
        /// Get or set the splash screen window
        /// </summary>
        public static MetroWindow Splash { get; set; }

        /// <summary>
        /// Show splash screen
        /// </summary>
        public static void ShowSplash()
        {
            Splash?.Show();
        }
        /// <summary>
        /// Close splash screen
        /// </summary>
        public static void CloseSplash()
        {
            Splash?.Close();
        }
    }

    /// <summary>
    /// Message listener, singleton pattern.
    /// Inherit from DependencyObject to implement DataBinding.
    /// </summary>
    public class MessageListener : DependencyObject
    {
        /// <summary>
        /// 
        /// </summary>
        private static MessageListener _mInstance;

        /// <summary>
        /// 
        /// </summary>
        private MessageListener()
        {
        }

        /// <summary>
        /// Get MessageListener instance
        /// </summary>
        public static MessageListener Instance => _mInstance ?? (_mInstance = new MessageListener());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void ReceiveMessage(string message)
        {
            Message = message;
            Debug.WriteLine(Message);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Get or set received message
        /// </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(MessageListener), new UIPropertyMetadata(null));

    }

    /// <summary>
    /// 
    /// </summary>
    public static class DispatcherHelper
    {
        /// <summary>
        /// Simulate Application.DoEvents function of <see cref=" System.Windows.Forms.Application"/> class.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrames), frame);

            try
            {
                Dispatcher.PushFrame(frame);
            }
            catch (InvalidOperationException)
            {
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        private static object ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;

            return null;
        }
    }
}
