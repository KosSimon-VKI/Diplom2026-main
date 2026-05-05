using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TransferModels.Staff;

namespace GardenNookWpf.Views.MainPanel.Staff
{
    public partial class StaffEditWindow : Window
    {
        private readonly bool _isEdit;

        public StaffEditWindow(StaffEditOptionsResponse options, StaffManagementDto? staff)
        {
            InitializeComponent();

            _isEdit = staff != null;
            TitleText.Text = _isEdit ? "Редактировать сотрудника" : "Добавить сотрудника";
            SaveButton.Content = _isEdit ? "Сохранить" : "Добавить";
            PasswordLabel.Text = _isEdit ? "Новый пароль" : "Пароль";
            PasswordHintText.Visibility = _isEdit ? Visibility.Visible : Visibility.Collapsed;

            RoleComboBox.ItemsSource = BuildRoleOptions(options?.Roles);
            FillFields(staff);

            Loaded += (_, _) =>
            {
                FullNameTextBox.Focus();
                FullNameTextBox.SelectAll();
            };
        }

        public StaffUpsertRequest Request { get; private set; } = new StaffUpsertRequest();

        private void FillFields(StaffManagementDto? staff)
        {
            FullNameTextBox.Text = staff?.FullName ?? string.Empty;
            LoginTextBox.Text = staff?.Login ?? string.Empty;
            RoleComboBox.SelectedValue = staff?.RoleId;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();

            var fullName = NormalizeText(FullNameTextBox.Text);
            if (string.IsNullOrWhiteSpace(fullName))
            {
                ShowValidation("Введите ФИО сотрудника.");
                return;
            }

            var login = NormalizeText(LoginTextBox.Text);
            if (string.IsNullOrWhiteSpace(login))
            {
                ShowValidation("Введите логин сотрудника.");
                return;
            }

            var roleId = RoleComboBox.SelectedValue is int selectedRoleId
                ? selectedRoleId
                : (int?)null;
            if (!roleId.HasValue || roleId.Value <= 0)
            {
                ShowValidation("Выберите роль сотрудника.");
                return;
            }

            var password = PasswordBox.Password?.Trim() ?? string.Empty;
            if (!_isEdit && string.IsNullOrWhiteSpace(password))
            {
                ShowValidation("Введите пароль сотрудника.");
                return;
            }

            Request = new StaffUpsertRequest
            {
                FullName = fullName,
                Login = login,
                Password = password,
                RoleId = roleId
            };

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

        private static List<RoleOption> BuildRoleOptions(IReadOnlyCollection<StaffRoleOptionDto>? source)
        {
            var result = new List<RoleOption>
            {
                new RoleOption(null, "Выберите роль")
            };

            result.AddRange((source ?? Array.Empty<StaffRoleOptionDto>())
                .OrderBy(x => x.Name)
                .Select(x => new RoleOption(x.Id, x.Name)));

            return result;
        }

        private static string NormalizeText(string? value)
        {
            return string.Join(" ", (value ?? string.Empty)
                .Trim()
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }

        private sealed class RoleOption
        {
            public RoleOption(int? id, string name)
            {
                Id = id;
                Name = name;
            }

            public int? Id { get; }
            public string Name { get; }
        }
    }
}
