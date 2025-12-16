using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FurnitureShop.Models;
using FurnitureShop.ViewModels;

namespace FurnitureShop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            // Додаємо обробник гарячої клавіші
            this.KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+A відкриває адмін-панель
            if (e.Key == Key.A && Keyboard.Modifiers == ModifierKeys.Control)
            {
                var adminWindow = new AdminWindow();
                adminWindow.ShowDialog();

                // Оновлюємо список товарів після закриття адмін-панелі
                if (DataContext is MainViewModel viewModel)
                {
                    viewModel.RefreshFurniture();
                }
            }
        }

        private void FurnitureItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is Furniture furniture)
            {
                var detailWindow = new ProductDetailWindow(furniture);
                var result = detailWindow.ShowDialog();

                // Якщо користувач натиснув "Додати в кошик"
                if (result == true && DataContext is MainViewModel viewModel)
                {
                    viewModel.AddToCartCommand.Execute(furniture);
                }
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            // Зупиняємо подію, щоб не відкривалося детальне вікно
            e.Handled = true;
        }
    }
}
