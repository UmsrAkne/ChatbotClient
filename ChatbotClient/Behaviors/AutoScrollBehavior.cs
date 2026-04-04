using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace ChatbotClient.Behaviors
{
    public class AutoScrollBehavior : Behavior<ItemsControl>
    {
        // スクロールを許可するかどうかの依存関係プロパティ
        public readonly static DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(AutoScrollBehavior), new PropertyMetadata(true));

        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

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
            if (e.Action == NotifyCollectionChangedAction.Add && IsEnabled)
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