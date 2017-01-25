using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using ZerO.Helpers;

namespace ZerO.Windows
{
    public enum FolderBrowserResult
    {
        None,
        Ok,
        Cancel
    }

    public enum BrowseType
    {
        None,
        Folder,
        File
    }

    /// <summary>
    /// Interaction logic for FolderBrowser.xaml
    /// </summary>
    public partial class FolderBrowser
    {
        public string SelectedPath, SelectedName;
        public string InitialDirectory;
        public bool IsShowing;
        public string Filter;

        internal FolderBrowserResult Result { get; set; } = FolderBrowserResult.None;
        internal BrowseType Type { get; set; } = BrowseType.None;

        public FolderBrowser()
        {
            InitializeComponent();
        }

        public FolderBrowserResult Show(MetroWindow owner, BrowseType browseType, string message)
        {
            Type = browseType;

            Title = Type == BrowseType.File ? "File Browser" : "Folder Browser";

            FolderTreeView.Items.Clear();

            LoadDirectories();

            MessageLabel.Content = message;

            try
            {
                if (owner != null || Owner == null)
                    Owner = owner;
            }

            catch (Exception ex) { Globals.Logger.Error(ex.Message); }

            IsShowing = true;
            IsEnabled = true;

            ShowDialog();
            var browserResult = Result;

            return browserResult;
        }

        public void LoadDirectories()
        {
            if (!string.IsNullOrEmpty(InitialDirectory))
            {
                var drives = Directory.GetDirectories(InitialDirectory);
                foreach (var info in drives.Select(drive => new DirectoryInfo(drive)))
                {
                    FolderTreeView.Items.Add(GetItem(info));
                }
            }

            else
            {
                var drives = DriveInfo.GetDrives();
                foreach (var drive in drives)
                {
                    FolderTreeView.Items.Add(GetItem(drive));
                }
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            Result = FolderBrowserResult.Ok;
            HideWindow();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = FolderBrowserResult.Cancel;
            HideWindow();
        }

        private void HideWindow()
        {
            InitialDirectory = "";
            Filter = "";
            IsEnabled = true;
            IsShowing = false;

            Hide();
        }

        private void AddDummy(TreeViewItem item)
        {
            item.Items.Add(new DummyTreeViewItem());
        }

        private bool HasDummy(TreeViewItem item)
        {
            return item.HasItems && (item.Items.OfType<TreeViewItem>().ToList().FindAll(tvi => tvi is DummyTreeViewItem).Count > 0);
        }

        private void RemoveDummy(TreeViewItem item)
        {
            var dummies = item.Items.OfType<TreeViewItem>().ToList().FindAll(tvi => tvi is DummyTreeViewItem);
            foreach (var dummy in dummies)
            {
                item.Items.Remove(dummy);
            }
        }

        void item_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;
            if (!HasDummy(item)) return;
            Cursor = Cursors.Wait;
            RemoveDummy(item);
            ExploreDirectories(item);

            if (Type == BrowseType.File)
                ExploreFiles(item);

            Cursor = Cursors.Arrow;
        }

        private TreeViewItem GetItem(DriveInfo drive)
        {
            var item = new TreeViewItem
            {
                Header = drive.Name,
                DataContext = drive,
                Tag = drive
            };
            AddDummy(item);
            item.Expanded += item_Expanded;
            return item;
        }

        private TreeViewItem GetItem(DirectoryInfo directory)
        {
            if (directory == null) throw new ArgumentNullException("directory");
            var item = new TreeViewItem
            {
                Header = directory.Name,
                DataContext = directory,
                Tag = directory.FullName
            };
            AddDummy(item);
            item.Expanded += item_Expanded;
            return item;
        }

        private static TreeViewItem GetItem(FileSystemInfo file)
        {
                var item = new TreeViewItem
                {
                    Header = file.Name,
                    DataContext = file,
                    Tag = file.FullName
                };
                return item;
        }

        private void ExploreDirectories(ItemsControl item)
        {
            if (item == null) throw new ArgumentNullException("item");
            var skip = false;
            var directoryInfo = (DirectoryInfo)null;
            var info = item.Tag as DriveInfo;
            if (info != null)
            {
                directoryInfo = info.RootDirectory;

                if (!directoryInfo.Exists)
                    skip = true;
            }
            else
            {
                var tag = item.Tag as DirectoryInfo;
                var fileInfo = item.Tag as FileInfo;
                if (tag != null)
                {
                    directoryInfo = tag;
                }
                else
                {
                    if (fileInfo != null)
                    {
                        directoryInfo = fileInfo.Directory;
                    }
                    else if (item.Tag is string)
                    {
                        directoryInfo = new DirectoryInfo(item.Tag.ToString());
                    }
                }
            }
            if (ReferenceEquals(directoryInfo, null)) return;

            if (skip) return;

            foreach (var directory in from directory in directoryInfo.GetDirectories() let isHidden = (directory.Attributes & FileAttributes.Hidden) == 
                                          FileAttributes.Hidden let isSystem = (directory.Attributes & FileAttributes.System) == FileAttributes.System where !isHidden && 
                                          !isSystem select directory)
            {
                item.Items.Add(GetItem(directory));
            }
        }

        private void ExploreFiles(ItemsControl item)
        {
            var directoryInfo = (DirectoryInfo)null;
            var tag = item.Tag as DriveInfo;
            var info = item.Tag as DirectoryInfo;
            if (tag != null)
            {
                directoryInfo = tag.RootDirectory;
            }
            else
            {
                var fileInfo = item.Tag as FileInfo;
                if (info != null)
                {
                    directoryInfo = info;
                }
                else
                {
                    if (fileInfo != null)
                    {
                        directoryInfo = fileInfo.Directory;
                    }
                    else if (item.Tag is string)
                    {
                        directoryInfo = new DirectoryInfo(item.Tag.ToString());
                    }
                }
            }
            if (ReferenceEquals(directoryInfo, null)) return;
            foreach (var file in directoryInfo.GetFiles())
            {
                var isHidden = (file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                var isSystem = (file.Attributes & FileAttributes.System) == FileAttributes.System;

                if (Filter != "" || Filter != null)
                {
                    if (file.Extension != Filter) continue;
                    if (!isHidden && !isSystem)
                        item.Items.Add(GetItem(file));
                }

                else
                {
                    if (!isHidden && !isSystem)
                    {
                        item.Items.Add(GetItem(file));
                    }
                }
            }
        }

        private void OnItemSelected(object sender, RoutedEventArgs e)
        {
            var info = FolderTreeView.SelectedItem as TreeViewItem;

            if (info == null) return;
            SelectedName = info.Header as string;
            SelectedPath = info.Tag as string;
        }

        private void folderBrowser_Closing(object sender, CancelEventArgs e)
        {
            IsEnabled = false;
            IsShowing = false;
        }
    }

    public class DummyTreeViewItem : TreeViewItem
    {
        public DummyTreeViewItem()
        {
            Header = "Dummy";
            Tag = "Dummy";
        }
    }
}
