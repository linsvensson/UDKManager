using System.Windows.Media;
using MahApps.Metro.Controls;
using ZerO.UserControls;

namespace ZerO.Windows
{
    /// <summary>
    /// Interaction logic for CustomWindow.xaml
    /// </summary>
    public partial class CustomWindow
    {
        public InputUc InputUc = new InputUc();
        public UpdateUc UpdateUc = new UpdateUc();
        public TestMapUc TestMapUc = new TestMapUc();
        public NewClassUc NewClassUc = new NewClassUc();
        public ChooserUc ChooseUc = new ChooserUc();
        public AboutUc AboutUc = new AboutUc();

        public CustomWindow()
        {
            InitializeComponent();

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.LowQuality);
            RenderOptions.SetCachingHint(this, CachingHint.Cache);
        }

        public void ShowAbout(MetroWindow owner)
        {
            Content = AboutUc;
            AboutUc.Initialize(this);
            Title = "About";

            if (owner != null || Owner == null)
                Owner = owner;

            IsEnabled = true;
            ShowDialog();
        }

        public void ShowUpdate(MetroWindow owner)
        {
            Content = UpdateUc;
            UpdateUc.Initialize(this);
            Title = "Update";

            if (owner != null || Owner == null)
                Owner = owner;

            IsEnabled = true;
            ShowDialog();
        }

        public void ShowTestMap(MetroWindow owner)
        {
            Content = TestMapUc;
                TestMapUc.Initialize(this);
                Title = "New Test Map";

            if (owner != null || Owner == null)
                Owner = owner;

            IsEnabled = true;
            ShowDialog();
        }

        public void ShowNewClass(MetroWindow owner, CustomItem item)
        {
            Content = NewClassUc;
            NewClassUc.Initialize(this, item);
            Title = "New Class";

            if (owner != null || Owner == null)
                Owner = owner;

            IsEnabled = true;
            ShowDialog();
        }

        public void ShowInput(MetroWindow owner, CustomItem item, CustomType type, Action action)
        {
            Content = InputUc;
            InputUc.Initialize(this, item, type, action);

            if (owner != null || Owner == null)
                Owner = owner;

            IsEnabled = true;
            ShowDialog();
        }

        public void ShowChoose(MetroWindow owner, CustomType type)
        {
            Content = ChooseUc;
            ChooseUc.Initialize(this, type);
            Title = "Choose Project";

            if (owner != null || Owner == null)
                Owner = owner;

            IsEnabled = true;
            ShowDialog();
        }

        public void HideWindow()
        {
            IsEnabled = false;
            Hide();
        }
    }
}
