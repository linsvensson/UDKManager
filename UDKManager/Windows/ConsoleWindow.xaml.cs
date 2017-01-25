using System.Windows;

namespace ZerO.Windows
{
    /// <summary>
    /// Interaction logic for Console.xaml
    /// </summary>
    public partial class ConsoleWindow
    {
        public ConsoleContent Cc = new ConsoleContent();

        public ConsoleWindow()
        {
            InitializeComponent();

            DataContext = Cc;
        }

        public void Write(string text)
        {
            Cc.ConsoleOutput.Add(text);    
            Scroller.ScrollToBottom();
        }
    }
}
