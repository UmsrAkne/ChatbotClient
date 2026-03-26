using System.Windows;
using System.Windows.Input;

namespace ChatbotClient.Controls
{
    public partial class CustomButton
    {
        // Content用プロパティ
        public readonly static DependencyProperty ButtonContentProperty =
            DependencyProperty.Register(nameof(ButtonContent), typeof(object), typeof(CustomButton), new PropertyMetadata(null));

        // Command用プロパティ
        public readonly static DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register(nameof(ButtonCommand), typeof(ICommand), typeof(CustomButton), new PropertyMetadata(null));

        public readonly static DependencyProperty ButtonCommandParameterProperty =
            DependencyProperty.Register(nameof(ButtonCommandParameter), typeof(object), typeof(CustomButton), new PropertyMetadata(null));

        public CustomButton()
        {
            InitializeComponent();
        }

        public object ButtonContent
        {
            get => GetValue(ButtonContentProperty);
            set => SetValue(ButtonContentProperty, value);
        }

        public ICommand ButtonCommand
        {
            get => (ICommand)GetValue(ButtonCommandProperty);
            set => SetValue(ButtonCommandProperty, value);
        }

        public object ButtonCommandParameter
        {
            get => GetValue(ButtonCommandParameterProperty);
            set => SetValue(ButtonCommandParameterProperty, value);
        }
    }
}