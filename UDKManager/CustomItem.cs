using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ZerO
{
    /// <summary>
    /// Extended Custom TreeViewItem class for holding more information
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(CustomItem))]
    public class CustomItem : TreeViewItem
    {
        public string Path;
        public int Level;
        public int Id;
        public string Content;
        public CustomItem ParentFolder;
        public BitmapImage Image { get; set; }

        /// <summary>
        /// Set the right image for the item
        /// </summary>
        public void SetImage()
        {
            if (((string) Tag).Equals("class"))
            {
                var uri = new Uri("pack://application:,,,/Resources/class.png");
                Image = new BitmapImage(uri);
            }

            else if (((string) Tag).Equals("project"))
            {
                var uri = new Uri("pack://application:,,,/Resources/project.png");
                Image = new BitmapImage(uri);
            }

            else if (((string) Tag).Equals("ini"))
            {
                var uri = new Uri("pack://application:,,,/Resources/ini.png");
                Image = new BitmapImage(uri);
            }

            else if (((string)Tag).Equals("search"))
            {
                var uri = new Uri("pack://application:,,,/Resources/searchResult.png");
                Image = new BitmapImage(uri);
            }

            else
            {
                var uri = new Uri("pack://application:,,,/Resources/testmap.png");
                Image = new BitmapImage(uri);
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            ((TreeViewItem)element).IsExpanded = true;
        }
    }
}
