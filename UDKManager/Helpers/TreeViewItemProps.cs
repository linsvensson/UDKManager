using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace ZerO.Helpers
{
    /// <summary>
    /// Extended Custom TreeView for better management of selected TreeViewItems
    /// </summary>
    public class TreeViewEx : TreeView
    {
        public TreeViewEx()
        {
            SelectedItemChanged += TreeViewEx_SelectedItemChanged;
            LayoutUpdated += TreeViewEx_LayoutUpdated;
        }

        void TreeViewEx_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue;
        }

        void TreeViewEx_LayoutUpdated(object sender, EventArgs e)
        {
            LayoutUpdated -= TreeViewEx_LayoutUpdated;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var sel = SelectedItem;
                SelectedItem = null; // Reset Binding to make initial binding work
                SelectedItem = sel;
            }));
        }

        #region SingleMode
        /// <summary>
        /// Identifies the SingleMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SingleModeProperty = DependencyProperty.Register("SingleMode", typeof(bool), typeof(TreeViewEx), null);

        /// <summary>
        /// Gets or sets the SingleMode possible Value of the bool object.
        /// </summary>
        public bool SingleMode
        {
            get { return (bool)GetValue(SingleModeProperty); }
            set { SetValue(SingleModeProperty, value); }
        }
        #endregion SingleMode

        #region SelectedValueGraph

        /// <summary>
        /// Gets or Sets the SelectedValueGraph possible Value of the TreeViewItem object.
        /// </summary>
        public IEnumerable SelectedValueGraph
        {
            get { return GetValue(SelectedValueGraphProperty) as IEnumerable; }
            set { SetValue(SelectedValueGraphProperty, value); }
        }

        public static readonly DependencyProperty SelectedValueGraphProperty =
            DependencyProperty.Register("SelectedValueGraph", typeof(IEnumerable), typeof(TreeViewEx), new PropertyMetadata(SelectedValueGraphProperty_Changed));

        private static void SelectedValueGraphProperty_Changed(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion SelectedValueGraph

        #region SelectedItem

        /// <summary>
        /// Gets or Sets the SelectedItem possible Value of the TreeViewItem object.
        /// </summary>
        public new object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TreeViewEx), new PropertyMetadata(SelectedItemProperty_Changed));

        static void SelectedItemProperty_Changed(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var targetObject = dependencyObject as TreeViewEx;
            if (targetObject == null) return;
            var tvi = targetObject.FindItemNode(targetObject.SelectedItem);
            if (tvi != null)
            {
                tvi.IsSelected = true;
            }
        }
        #endregion SelectedItem

        public TreeViewItem FindItemNode(object item)
        {
            TreeViewItem node = null;
            foreach (var data in Items)
            {
                node = ItemContainerGenerator.ContainerFromItem(data) as TreeViewItem;
                if (node == null) continue;
                if (data == item)
                    break;
                node = FindItemNodeInChildren(node, item);
                if (node != null)
                    break;
            }
            return node;
        }

        protected TreeViewItem FindItemNodeInChildren(TreeViewItem parent, object item)
        {
            TreeViewItem node = null;
            var isExpanded = parent != null && parent.IsExpanded;
            if (!isExpanded) //Can't find child container unless the parent node is Expanded once
            {
                if (parent != null)
                {
                    parent.IsExpanded = true;
                    parent.UpdateLayout();
                }
            }
            if (parent == null) return null;
            foreach (var data in parent.Items)
            {
                node = parent.ItemContainerGenerator.ContainerFromItem(data) as TreeViewItem;
                if (data == item && node != null)
                    break;
                node = FindItemNodeInChildren(node, item);
                if (node != null)
                    break;
            }
            if (node == null && parent.IsExpanded != isExpanded)
                parent.IsExpanded = isExpanded;
            if (node != null)
                parent.IsExpanded = true;
            return node;
        }


    }
        [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof (CustomItem))]
        public class CTreeView : TreeView
        {
            public CTreeView()
            {
                SelectedItemChanged += CTreeView_SelectedItemChanged;   
            }

            private void CTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
            {
                SelectedItem = e.NewValue;
            }

            #region SelectedItem
            /// <summary>
        /// Gets or Sets the SelectedItem possible Value of the TreeViewItem object.
        /// </summary>
        public new object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(CTreeView), new PropertyMetadata(SelectedItemProperty_Changed));

        static void SelectedItemProperty_Changed(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var targetObject = dependencyObject as CTreeView;
            if (targetObject == null) return;
            var tvi = targetObject.FindItemNode(targetObject.SelectedItem);
            if (tvi != null)
            {
                tvi.IsSelected = true;
                tvi.Focus();
                tvi.BringIntoView();
            }
        }

        public CustomItem FindItemNode(object item)
        {
            CustomItem node = null;
            foreach (var data in Items)
            {
                node = ItemContainerGenerator.ContainerFromItem(data) as CustomItem;
                if (node == null) continue;
                if (data == item)
                    break;
                node = FindItemNodeInChildren(node, item);
                if (node != null)
                    break;
            }
            return node;
        }

        protected CustomItem FindItemNodeInChildren(CustomItem parent, object item)
        {
            CustomItem node = null;
            var isExpanded = parent != null && parent.IsExpanded;
            if (!isExpanded) //Can't find child container unless the parent node is Expanded once
            {
                if (parent != null)
                {
                    parent.IsExpanded = true;
                    parent.UpdateLayout();
                }
            }
            if (parent == null) return null;
            foreach (var data in parent.Items)
            {
                node = parent.ItemContainerGenerator.ContainerFromItem(data) as CustomItem;
                if (data == item && node != null)
                    break;
                node = FindItemNodeInChildren(node, item);
                if (node != null)
                    break;
            }
            if (node == null && parent.IsExpanded != isExpanded)
                parent.IsExpanded = isExpanded;
            if (node != null)
                parent.IsExpanded = true;
            return node;
        }
        #endregion SelectedItem
        }
}
