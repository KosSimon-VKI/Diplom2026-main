using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GardenNookWpf.Views.MainPanel.Preparations;
using GardenNookWpf.Views.MainPanel.TechnicalCards;
using GardenNookWpf.Views.Shell;
using GardenNookWpf.Views.Shell.Controllers;

namespace GardenNookWpf.Views.Shell.Sections.Preparations
{
    public partial class PreparationsView : UserControl, IMainSectionView
    {
        private const string AdminRole = "Администратор";

        private readonly PreparationsSectionController _controller;
        private readonly bool _canAddTasks;

        public PreparationsView(HttpClient httpClient, string userRole)
        {
            _controller = new PreparationsSectionController(httpClient, userRole);
            _canAddTasks = !string.Equals(userRole, AdminRole, StringComparison.CurrentCultureIgnoreCase);

            InitializeComponent();

            AddTodoTaskButton.Visibility = _canAddTasks ? Visibility.Visible : Visibility.Collapsed;
            AddTaskButton.Visibility = _canAddTasks ? Visibility.Visible : Visibility.Collapsed;
            _controller.BusyStateChanged += Controller_BusyStateChanged;
        }

        public bool IsBusy => _controller.IsBusy;

        public async Task ActivateAsync()
        {
            SetStatus(string.Empty);
            var result = await _controller.LoadBoardAsync();
            if (!result.Success)
            {
                ClearBoard();
                SetStatus(result.Message);
                return;
            }

            RenderBoard();
        }

        public void Deactivate()
        {
            _controller.Deactivate();
        }

        private async void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller.IsBusy || !_canAddTasks)
            {
                return;
            }

            if (_controller.SemiFinishedOptions.Count == 0)
            {
                MessageBox.Show("Список полуфабрикатов пуст.", "Garden Nook", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var addWindow = new AddPreparationTaskWindow(_controller.SemiFinishedOptions)
            {
                Owner = Window.GetWindow(this)
            };

            if (addWindow.ShowDialog() != true)
            {
                return;
            }

            var selectedId = addWindow.SelectedSemiFinishedId.GetValueOrDefault();
            var taskText = string.IsNullOrWhiteSpace(addWindow.SelectedSemiFinishedName)
                ? $"Полуфабрикат #{selectedId}"
                : addWindow.SelectedSemiFinishedName;

            SetStatus(string.Empty);
            var result = await _controller.CreatePreparationTaskAsync(taskText, addWindow.SelectedSemiFinishedId, addWindow.CommentText);
            if (!result.Success)
            {
                SetStatus(result.Message);
                return;
            }

            RenderBoard();
        }

        private async void AddTodoTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller.IsBusy || !_canAddTasks)
            {
                return;
            }

            var addWindow = new AddTodoTaskWindow
            {
                Owner = Window.GetWindow(this)
            };

            if (addWindow.ShowDialog() != true)
            {
                return;
            }

            SetStatus(string.Empty);
            var result = await _controller.CreatePreparationTaskAsync(addWindow.TaskText, null, addWindow.CommentText);
            if (!result.Success)
            {
                SetStatus(result.Message);
                return;
            }

            RenderBoard();
        }

        private async void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button ||
                button.Tag is not PreparationsSectionController.PreparationTaskDisplayModel task)
            {
                return;
            }

            if (_controller.IsBusy || !_canAddTasks)
            {
                return;
            }

            (bool Success, string Message) result;

            if (!task.IsLinkedToSemiFinished)
            {
                result = await _controller.CompleteTodoTaskAsync(task.TaskId);
            }
            else
            {
                var completeWindow = new CompletePreparationTaskWindow(task.SemiFinishedName)
                {
                    Owner = Window.GetWindow(this)
                };

                if (completeWindow.ShowDialog() != true)
                {
                    return;
                }

                result = await _controller.CompletePreparationTaskAsync(task.TaskId, completeWindow.StockGrams, completeWindow.ProductionDate);
            }

            if (!result.Success)
            {
                SetStatus(result.Message);
                return;
            }

            SetStatus(string.Empty);
            RenderBoard();
        }

        private async void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button ||
                button.Tag is not PreparationsSectionController.PreparationTaskDisplayModel task)
            {
                return;
            }

            if (_controller.IsBusy || !_canAddTasks || !task.IsLinkedToSemiFinished)
            {
                return;
            }

            SetStatus(string.Empty);
            var result = await _controller.DeletePreparationTaskAsync(task.TaskId);
            if (!result.Success)
            {
                SetStatus(result.Message);
                return;
            }

            RenderBoard();
        }

        private async void DeleteExistingPreparation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not int preparationId)
            {
                return;
            }

            if (_controller.IsBusy)
            {
                return;
            }

            var confirmationWindow = new ConfirmDeletePreparationWindow
            {
                Owner = Window.GetWindow(this)
            };

            if (confirmationWindow.ShowDialog() != true)
            {
                return;
            }

            SetStatus(string.Empty);
            var result = await _controller.DeleteExistingPreparationAsync(preparationId);
            if (!result.Success)
            {
                SetStatus(result.Message);
                return;
            }

            RenderBoard();
        }

        private async void TaskCard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsInsideTaskActionButton(e.OriginalSource as DependencyObject))
            {
                return;
            }

            if (sender is not FrameworkElement element ||
                element.Tag is not PreparationsSectionController.PreparationTaskDisplayModel task)
            {
                return;
            }

            await OpenTaskTechnicalCardFromTaskAsync(task);
        }

        private async Task OpenTaskTechnicalCardFromTaskAsync(PreparationsSectionController.PreparationTaskDisplayModel task)
        {
            if (_controller.IsBusy)
            {
                return;
            }

            if (!task.IsLinkedToSemiFinished)
            {
                return;
            }

            if (!task.TechnicalCardId.HasValue)
            {
                MessageBox.Show("Для выбранного полуфабриката не привязана тех. карта.", "Garden Nook", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SetStatus(string.Empty);
            var result = await _controller.LoadTechnicalCardAsync(task.TechnicalCardId.Value, task.SemiFinishedName);
            if (!result.Success || result.Card == null)
            {
                SetStatus(result.Message);
                return;
            }

            var technicalCardWindow = new TechnicalCardWindow(result.Card)
            {
                Owner = Window.GetWindow(this)
            };
            technicalCardWindow.ShowDialog();
        }

        private static bool IsInsideTaskActionButton(DependencyObject? source)
        {
            for (var current = source; current != null; current = VisualTreeHelper.GetParent(current))
            {
                if (current is Button button &&
                    (string.Equals(button.Name, "CompleteTaskButton", StringComparison.Ordinal) ||
                     string.Equals(button.Name, "DeleteTaskButton", StringComparison.Ordinal)))
                {
                    return true;
                }
            }

            return false;
        }

        private void RenderBoard()
        {
            TasksList.ItemsSource = _controller.TaskItems;
            ExistingList.ItemsSource = _controller.ExistingItems;

            ShowTaskState(_controller.TaskItems.Count == 0);
            ShowExistingState(_controller.ExistingItems.Count == 0);
        }

        private void ShowTaskState(bool isEmpty)
        {
            TasksScrollViewer.Visibility = isEmpty ? Visibility.Collapsed : Visibility.Visible;
            EmptyTasksText.Visibility = isEmpty ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ShowExistingState(bool isEmpty)
        {
            ExistingScrollViewer.Visibility = isEmpty ? Visibility.Collapsed : Visibility.Visible;
            EmptyExistingText.Visibility = isEmpty ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ClearBoard()
        {
            TasksList.ItemsSource = null;
            ExistingList.ItemsSource = null;
            ShowTaskState(true);
            ShowExistingState(true);
        }

        private void SetStatus(string message)
        {
            StatusText.Text = message;
            StatusText.Visibility = string.IsNullOrWhiteSpace(message)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void Controller_BusyStateChanged(bool isBusy)
        {
            RootGrid.IsEnabled = !isBusy;
        }
    }
}
