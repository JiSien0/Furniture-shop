using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FurnitureShop.Models;

namespace FurnitureShop
{
    public partial class ProductDetailWindow : Window
    {
        public Furniture Furniture { get; set; }
        private List<string> _imagePaths;
        private int _currentImageIndex = 0;

        public ProductDetailWindow(Furniture furniture)
        {
            InitializeComponent();
            Furniture = furniture;
            _imagePaths = new List<string>();
            LoadFurnitureDetails();
        }

        private void LoadFurnitureDetails()
        {
            ProductName.Text = Furniture.Name;
            ProductPrice.Text = $"{Furniture.Price:N2} ₴";
            ProductDescription.Text = Furniture.Description ?? "Опис відсутній";
            ProductRow.Text = Furniture.StorageRow ?? "—";
            ProductShelf.Text = Furniture.StorageShelf ?? "—";

            // Збираємо всі наявні шляхи до зображень
            if (!string.IsNullOrEmpty(Furniture.ImagePath))
                _imagePaths.Add(Furniture.ImagePath);
            if (!string.IsNullOrEmpty(Furniture.ImagePath2))
                _imagePaths.Add(Furniture.ImagePath2);
            if (!string.IsNullOrEmpty(Furniture.ImagePath3))
                _imagePaths.Add(Furniture.ImagePath3);

            // Показуємо перше зображення
            if (_imagePaths.Any())
            {
                ShowImage(0);
                CreateThumbnails();
                UpdateNavigationButtons();
            }
        }

        private void ShowImage(int index)
        {
            if (index >= 0 && index < _imagePaths.Count)
            {
                _currentImageIndex = index;
                try
                {
                    ProductImage.Source = new BitmapImage(new Uri(_imagePaths[index]));
                }
                catch
                {
                    // Якщо зображення не вдалося завантажити
                    ProductImage.Source = null;
                }
                UpdateThumbnailsSelection();
            }
        }

        private void CreateThumbnails()
        {
            ThumbnailsPanel.Children.Clear();

            for (int i = 0; i < _imagePaths.Count; i++)
            {
                int index = i; // Для замикання в lambda
                var border = new Border
                {
                    Width = 60,
                    Height = 60,
                    Margin = new Thickness(5),
                    BorderThickness = new Thickness(2),
                    BorderBrush = i == 0 ? new SolidColorBrush(Color.FromRgb(33, 150, 243)) : Brushes.Transparent,
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                var image = new Image
                {
                    Stretch = Stretch.UniformToFill,
                    Tag = index
                };

                try
                {
                    image.Source = new BitmapImage(new Uri(_imagePaths[i]));
                }
                catch { }

                border.Child = image;
                border.MouseDown += (s, e) => ShowImage(index);

                ThumbnailsPanel.Children.Add(border);
            }
        }

        private void UpdateThumbnailsSelection()
        {
            for (int i = 0; i < ThumbnailsPanel.Children.Count; i++)
            {
                var border = ThumbnailsPanel.Children[i] as Border;
                if (border != null)
                {
                    border.BorderBrush = i == _currentImageIndex
                        ? new SolidColorBrush(Color.FromRgb(33, 150, 243))
                        : Brushes.Transparent;
                }
            }
        }

        private void UpdateNavigationButtons()
        {
            // Ховаємо кнопки навігації якщо зображення тільки одне або немає зображень
            bool showNavigation = _imagePaths.Count > 1;
            PrevButton.Visibility = showNavigation ? Visibility.Visible : Visibility.Collapsed;
            NextButton.Visibility = showNavigation ? Visibility.Visible : Visibility.Collapsed;
        }

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            if (_imagePaths.Count > 0)
            {
                _currentImageIndex = (_currentImageIndex - 1 + _imagePaths.Count) % _imagePaths.Count;
                ShowImage(_currentImageIndex);
            }
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            if (_imagePaths.Count > 0)
            {
                _currentImageIndex = (_currentImageIndex + 1) % _imagePaths.Count;
                ShowImage(_currentImageIndex);
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
