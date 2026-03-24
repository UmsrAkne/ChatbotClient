using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChatbotClient.Models;
using ChatbotClient.ViewModels;
using Microsoft.Xaml.Behaviors;

namespace ChatbotClient.Behaviors
{
    public class DropFilesBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject == null)
            {
                return;
            }

            AssociatedObject.AllowDrop = true;
            AssociatedObject.DragOver += OnDragOver;
            AssociatedObject.Drop += OnDrop;
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.DragOver -= OnDragOver;
                AssociatedObject.Drop -= OnDrop;
                AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
            }

            base.OnDetaching();
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            try
            {
                var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files == null || files.Length == 0)
                {
                    return;
                }

                var viewModel = AssociatedObject?.DataContext as MainWindowViewModel;
                if (viewModel == null)
                {
                    return;
                }

                var attachedFiles =
                    files.Where(f => !string.IsNullOrWhiteSpace(f))
                        .Select(f => new AttachedFile() { FullPath = f, FileName = Path.GetFileName(f), });

                viewModel.AttachedFiles.AddRange(attachedFiles);
                e.Handled = true;
            }
            catch
            {
                // ignore unexpected errors to avoid crashing on a drop
                e.Handled = true;
            }
        }

        // Todo : 後ほど別のビヘイビアに分割する予定
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