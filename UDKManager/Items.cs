using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ProjectManager
{
    public class Project
    {
        public string Path;
        public string Name {get; set; }
        public BitmapImage Image { get; set; }

        public Project(string name, string path)
        {
             Uri uri = new Uri("pack://application:,,,/Resources/project.png");
             Image = new BitmapImage(uri);
        }
    }

    public class Class
    {
        public string Path;
        public string Name {get; set; }
        public BitmapImage Image { get; set; }

        public Class(string name, string path)
        {
             Uri uri = new Uri("pack://application:,,,/Resources/class.png");
             Image = new BitmapImage(uri);
        }
    }
}
