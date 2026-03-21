using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace ChatbotClient.Behaviors
{
    public class AutoScrollBehavior : Behavior<ItemsControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            // AssociatedObject（アタッチ先のItemsControl）のItemsSourceが変わるのを監視
            if (AssociatedObject.ItemsSource is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged += OnCollectionChanged;
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject.ItemsSource is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged -= OnCollectionChanged;
            }

            base.OnDetaching();
        }

        private static T FindParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
            {
                return null;
            }

            if (parentObject is T parent)
            {
                return parent;
            }

            return FindParent<T>(parentObject);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                ScrollToBottom();
            }
        }

        private void ScrollToBottom()
        {
            // アイテムが描画されてからスクロールさせるため、優先度を下げて実行
            AssociatedObject.Dispatcher.BeginInvoke(
                new System.Action(() =>
                {
                    var scrollViewer = FindParent<ScrollViewer>(AssociatedObject);
                    scrollViewer?.ScrollToBottom();
                }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}