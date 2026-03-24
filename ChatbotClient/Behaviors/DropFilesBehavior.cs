using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.DragOver -= OnDragOver;
                AssociatedObject.Drop -= OnDrop;
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
    }
}