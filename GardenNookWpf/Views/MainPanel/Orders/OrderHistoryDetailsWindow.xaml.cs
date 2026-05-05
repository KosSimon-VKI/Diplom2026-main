using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TransferModels.Orders;

namespace GardenNookWpf.Views.MainPanel.Orders
{
    public partial class OrderHistoryDetailsWindow : Window
    {
        private readonly ObservableCollection<CompositionCardViewModel> _compositionCards = new ObservableCollection<CompositionCardViewModel>();

        public OrderHistoryDetailsWindow(OrderHistoryDetailsDto details)
        {
            InitializeComponent();
            CompositionCards.ItemsSource = _compositionCards;
            Bind(details);
        }

        private void Bind(OrderHistoryDetailsDto details)
        {
            OrderTitleText.Text = $"Заказ №{details.OrderId}";
            OrderMetaText.Text =
                $"Клиент: {BuildClientDisplay(details.ClientName, details.ClientPhone)}\n" +
                $"Дата: {FormatDate(details.CreatedAt)}\n" +
                $"Тип: {details.OrderType} | Статус: {details.Status}\n" +
                $"Сумма: {details.TotalPrice:0.##} ₽ | Ккал: {details.TotalCalories:0.##}";

            if (!string.IsNullOrWhiteSpace(details.Comment))
            {
                CommentText.Text = "Комментарий: " + details.Comment;
                CommentText.Visibility = Visibility.Visible;
            }

            foreach (var dish in details.Dishes ?? new List<OrderHistoryDishItemDto>())
            {
                _compositionCards.Add(new CompositionCardViewModel
                {
                    Title = "Блюдо: " + dish.Name,
                    Meta = $"Количество: {FormatQuantity(dish.Quantity)} · Стоимость: {dish.FinalPrice:0.##} ₽",
                    Extra = BuildToppingsLine(dish.Toppings)
                });
            }

            foreach (var drink in details.Drinks ?? new List<OrderHistoryDrinkItemDto>())
            {
                var extra = new List<string>();
                if (!string.IsNullOrWhiteSpace(drink.MilkIngredientName))
                {
                    extra.Add("Молоко: " + drink.MilkIngredientName);
                }

                if (!string.IsNullOrWhiteSpace(drink.CoffeeIngredientName))
                {
                    extra.Add("Кофе: " + drink.CoffeeIngredientName);
                }

                var toppings = BuildToppingsLine(drink.Toppings);
                if (!string.IsNullOrWhiteSpace(toppings))
                {
                    extra.Add(toppings);
                }

                _compositionCards.Add(new CompositionCardViewModel
                {
                    Title = "Напиток: " + drink.Name,
                    Meta = $"Количество: {FormatQuantity(drink.Quantity)} · Стоимость: {drink.FinalPrice:0.##} ₽",
                    Extra = string.Join(Environment.NewLine, extra)
                });
            }

            foreach (var topping in details.Toppings ?? new List<OrderHistoryToppingItemDto>())
            {
                _compositionCards.Add(new CompositionCardViewModel
                {
                    Title = "Добавка: " + topping.Name,
                    Meta = $"Количество: {FormatQuantity(topping.Quantity)} · Стоимость: {topping.TotalPrice:0.##} ₽"
                });
            }

            if (_compositionCards.Count == 0)
            {
                _compositionCards.Add(new CompositionCardViewModel
                {
                    Title = "Состав заказа не указан",
                    Meta = string.Empty
                });
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
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

        private static string BuildToppingsLine(IEnumerable<OrderHistoryLinkedToppingDto>? toppings)
        {
            var lines = (toppings ?? new List<OrderHistoryLinkedToppingDto>())
                .Where(x => x.Quantity > 0m)
                .Select(x => $"+ {x.Name} x{FormatQuantity(x.Quantity)}")
                .ToList();
            return lines.Count == 0 ? string.Empty : string.Join(Environment.NewLine, lines);
        }

        private static string FormatDate(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("dd.MM.yyyy HH:mm", CultureInfo.CurrentCulture) : "-";
        }

        private static string FormatQuantity(decimal value)
        {
            return value == decimal.Truncate(value)
                ? value.ToString("0", CultureInfo.CurrentCulture)
                : value.ToString("0.##", CultureInfo.CurrentCulture);
        }

        private static string BuildClientDisplay(string name, string phone)
        {
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(phone))
            {
                return $"{name} ({phone})";
            }

            return string.IsNullOrWhiteSpace(name) ? phone : name;
        }

        private sealed class CompositionCardViewModel
        {
            public string Title { get; set; } = string.Empty;
            public string Meta { get; set; } = string.Empty;
            public string Extra { get; set; } = string.Empty;
            public Visibility ExtraVisibility => string.IsNullOrWhiteSpace(Extra) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
