using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RoseSniffingPodcasts.Common
{
    public static class SelectedItems
    {
        private static readonly DependencyProperty SelectedItemsBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItemsBehavior",
                typeof(SelectedItemsBehavior),
                typeof(ListBox),
                null);

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.RegisterAttached(
            "Items",
            typeof(IList),
            typeof(SelectedItems),
            new PropertyMetadata(null, ItemsPropertyChanged));

        public static void SetItems(ListViewBase listBox, IList list)
        {
            listBox.SetValue(ItemsProperty, list);
        }

        public static IList GetItems(ListViewBase listBox)
        {
            return listBox.GetValue(ItemsProperty) as IList;
        }

        private static void ItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as ListViewBase;
            if (target != null)
            {
                GetOrCreateBehavior(target, e.NewValue as IList);
            }
        }

        private static SelectedItemsBehavior GetOrCreateBehavior(ListViewBase target, IList list)
        {
            var behavior = target.GetValue(SelectedItemsBehaviorProperty) as SelectedItemsBehavior;
            if (behavior == null)
            {
                behavior = new SelectedItemsBehavior(target, list);
                target.SetValue(SelectedItemsBehaviorProperty, behavior);
            }
            else
            {
                behavior.SetData(list);
            }

            return behavior;
        }
    }

    public class SelectedItemsBehavior
    {
        private readonly ListViewBase _listBox;
        private IList _boundList;

        public SelectedItemsBehavior(ListViewBase listBox, IList boundList)
        {
            _listBox = listBox;
            _listBox.SelectionChanged += OnSelectionChanged;
            SetData(boundList);
        }

        public void SetData(IList data)
        {
            var collectionEvents = _boundList as INotifyCollectionChanged;
            if (collectionEvents != null)
                collectionEvents.CollectionChanged -= OnBoundCollectionChanged;
            _boundList = data;
            collectionEvents = _boundList as INotifyCollectionChanged;
            if (collectionEvents != null)
                collectionEvents.CollectionChanged += OnBoundCollectionChanged;
            Populate();
        }

        private void Populate()
        {
            _listBox.SelectionChanged -= OnSelectionChanged;
            _listBox.SelectedItems.Clear();
            foreach (var item in _boundList)
            {
                _listBox.SelectedItems.Add(item);
            }
            _listBox.SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var collectionEvents = _boundList as INotifyCollectionChanged;
            if (collectionEvents != null)
                collectionEvents.CollectionChanged -= OnBoundCollectionChanged;
            _boundList.Clear();

            foreach (var item in _listBox.SelectedItems)
            {
                _boundList.Add(item);
            }
            if (collectionEvents != null)
                collectionEvents.CollectionChanged += OnBoundCollectionChanged;
        }

        private void OnBoundCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Populate();
        }
    }
}
