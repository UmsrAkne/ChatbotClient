using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using ChatbotClient.Models;
using ChatbotClient.ViewModels;
using Microsoft.Xaml.Behaviors;

namespace ChatbotClient.Behaviors
{
    public class DeleteListItemBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject == null)
            {
                return;
            }

            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
            }

            base.OnDetaching();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete)
            {
                return;
            }

            var listBox = AssociatedObject;
            if (listBox == null)
            {
                return;
            }

            var viewModel = listBox.DataContext as MainWindowViewModel;
            if (viewModel == null)
            {
                return;
            }

            // Collect selected items first to avoid modifying a collection during enumeration
            var toRemove = listBox.SelectedItems.Cast<AttachedFile>().ToList();
            if (toRemove.Count == 0)
            {
                return;
            }

            foreach (var item in toRemove)
            {
                viewModel.AttachedFiles.Remove(item);
            }

            e.Handled = true;
        }
    }
}