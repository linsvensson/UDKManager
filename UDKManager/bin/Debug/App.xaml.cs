using System.Runtime;
using System.Windows;
using System.Windows.Media.Animation;

namespace ZerO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : Application
    {
        private void StartupHandler(object sender, StartupEventArgs e)
        {
            ProfileOptimization.SetProfileRoot(@"Profile");
            ProfileOptimization.StartProfile("Startup.Profile");

            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(Timeline),
                new FrameworkPropertyMetadata { DefaultValue = 10 });
        }

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            // Always call method in base class, so that the event gets raised.
            base.OnSessionEnding(e);

            TextEditor.SafeSaveAll();
            Shutdown();
            //var reason = e.ReasonSessionEnding;

            //var res = Globals.MsgBox.Show(Globals.Main, "Exiting Windows will terminate this app and you may lose any changes made.\nAre you sure?", "End Session", MessageBoxButton.YesNo, MessageBoxIconType.Warning);
            //if (res == MessageBoxResult.No)
            //{
            //    e.Cancel = true;
            //    TextEditor.SafeSaveAll();
            //}

            //else
            //{
                
            //    Globals.Main.Shutdown();

            //    if (reason == ReasonSessionEnding.Shutdown)
            //        Process.Start("shutdown", "/s /t 0");	// starts the shutdown application 
            //    // the argument /s is to shut down the computer
            //    // the argument /t 0 is to tell the process that the specified operation needs to be completed after 0 seconds
            //    else if (reason == ReasonSessionEnding.)

            //}
        }
    }
}
