using System.Windows;
using System.Windows.Input;

namespace GardenNookWpf.Views.MainPanel.Preparations
{
    public partial class AddTodoTaskWindow : Window
    {
        public string TaskText { get; private set; } = string.Empty;
        public string? CommentText { get; private set; }

        public AddTodoTaskWindow()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();

            var taskText = (TaskTextBox.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(taskText))
            {
                ShowValidation("Введите текст задачи.");
                return;
            }

            TaskText = taskText;
            CommentText = string.IsNullOrWhiteSpace(CommentTextBox.Text)
                ? null
                : CommentTextBox.Text.Trim();

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void HeaderClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ShowValidation(string message)
        {
            ValidationText.Text = message;
            ValidationText.Visibility = Visibility.Visible;
        }

        private void HideValidation()
        {
            ValidationText.Text = string.Empty;
            ValidationText.Visibility = Visibility.Collapsed;
        }
    }
}
