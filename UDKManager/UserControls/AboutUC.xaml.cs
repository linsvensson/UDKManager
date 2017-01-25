using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using ZerO.Windows;

namespace ZerO.UserControls
{
    /// <summary>
    /// Interaction logic for AboutUC.xaml
    /// </summary>
    public partial class AboutUc
    {
        public CustomWindow ParentWindow;

        public AboutUc()
        {
            InitializeComponent();

            // Create a FlowDocument
            var mcFlowDoc = new FlowDocument();

            // Create a paragraph with text
            var para = new Paragraph();
            para.Inlines.Add("This application is made to make UDK development less complicated. It makes otherwise tedious tasks like creating new projects easier. Just specify the UDK directories and exe and you're pretty much ready to go.");

            var para1 = new Paragraph();
            para1.Inlines.Add("* Test Maps are .bat files which run your UDK maps directly, instead of having to go to the editor every time.\n");
            para1.Inlines.Add("* The Search Tab can be used to search all of the class files in your Development folder.");

            var para2 = new Paragraph();
            para2.Inlines.Add("If you don't like the color scheme for the Text Editor, or if you want to edit the autocomplete list, locate the 'Unrealscript.xml' file in the application folder and edit it.\nReload it from the 'Reload -> Unrealscript.xml' for the changes to take effect.");

            // Add the paragraph to blocks of paragraph
            mcFlowDoc.Blocks.Add(para);
            mcFlowDoc.Blocks.Add(para1);
            mcFlowDoc.Blocks.Add(para2);

            // Set contents
            RichTextBox.Document = mcFlowDoc;
        }

        public void Initialize(CustomWindow parent)
        {
            IsEnabled = true;
            ParentWindow = parent;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            ParentWindow.HideWindow();
        }

        private void historyHL_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://pastebin.com/sTeM1akM");
        }
    }
}
