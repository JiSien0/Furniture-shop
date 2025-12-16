using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FurnitureShop.Database;
using FurnitureShop.Models;
using FurnitureShop.Services;

namespace FurnitureShop.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseHelper _database;
        private bool _isDelivery;
        private string _searchText = string.Empty;
        private Category? _selectedCategory;
        private List<Furniture> _allFurniture = new List<Furniture>();

        public ObservableCollection<Furniture> FurnitureList { get; set; }
        public ObservableCollection<CartItem> Cart { get; set; }
        public ObservableCollection<Category> Categories { get; set; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    FilterFurniture();
                }
            }
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedCategory));
                    FilterFurniture();
                }
            }
        }

        public bool IsDelivery
        {
            get => _isDelivery;
            set
            {
                if (_isDelivery != value)
                {
                    _isDelivery = value;
                    OnPropertyChanged(nameof(IsDelivery));
                    OnPropertyChanged(nameof(IsPickup));
                }
            }
        }

        public bool IsPickup
        {
            get => !_isDelivery;
            set
            {
                if (value)
                {
                    IsDelivery = false;
                }
            }
        }

        public decimal TotalAmount => Cart.Sum(item => item.TotalPrice);

        public ICommand AddToCartCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand ClearCategoryCommand { get; }

        public MainViewModel()
        {
            _database = new DatabaseHelper();
            FurnitureList = new ObservableCollection<Furniture>();
            Cart = new ObservableCollection<CartItem>();
            Categories = new ObservableCollection<Category>();

            AddToCartCommand = new RelayCommand(AddToCart);
            RemoveFromCartCommand = new RelayCommand(RemoveFromCart);
            IncreaseQuantityCommand = new RelayCommand(IncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand(DecreaseQuantity);
            CheckoutCommand = new RelayCommand(Checkout, CanCheckout);
            ClearSearchCommand = new RelayCommand(_ => SearchText = string.Empty);
            ClearCategoryCommand = new RelayCommand(_ => SelectedCategory = null);

            // Инициализируем БД тестовыми данными
            var seeder = new DatabaseSeeder(_database);
            seeder.SeedData();

            LoadCategories();
            LoadFurniture();
        }

        private void LoadCategories()
        {
            Categories.Clear();
            var categories = _database.GetAllCategories();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        private void LoadFurniture()
        {
            _allFurniture = _database.GetAllFurniture();
            FilterFurniture();
        }

        private void FilterFurniture()
        {
            FurnitureList.Clear();

            var filtered = _allFurniture.AsEnumerable();

            // Фільтр по категорії
            if (SelectedCategory != null)
            {
                filtered = filtered.Where(f => f.CategoryId == SelectedCategory.Id);
            }

            // Пошук по назві або серійному номеру
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLower();
                filtered = filtered.Where(f =>
                    f.Name.ToLower().Contains(searchLower) ||
                    (f.SerialNumber != null && f.SerialNumber.ToLower().Contains(searchLower))
                );
            }

            foreach (var item in filtered)
            {
                FurnitureList.Add(item);
            }
        }

        public void RefreshFurniture()
        {
            LoadCategories();
            LoadFurniture();
        }

        private void AddToCart(object? parameter)
        {
            if (parameter is Furniture furniture)
            {
                var existingItem = Cart.FirstOrDefault(item => item.Furniture.Id == furniture.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        Furniture = furniture,
                        Quantity = 1
                    };
                    cartItem.PropertyChanged += CartItem_PropertyChanged;
                    Cart.Add(cartItem);
                }
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        private void RemoveFromCart(object? parameter)
        {
            if (parameter is CartItem cartItem)
            {
                cartItem.PropertyChanged -= CartItem_PropertyChanged;
                Cart.Remove(cartItem);
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        private void IncreaseQuantity(object? parameter)
        {
            if (parameter is CartItem cartItem)
            {
                cartItem.Quantity++;
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        private void DecreaseQuantity(object? parameter)
        {
            if (parameter is CartItem cartItem && cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        private bool CanCheckout(object? parameter)
        {
            return Cart.Count > 0;
        }

        private void Checkout(object? parameter)
        {
            try
            {
                var receipt = new Receipt
                {
                    ReceiptNumber = _database.GenerateReceiptNumber(),
                    Date = DateTime.Now,
                    TotalPrice = TotalAmount,
                    Delivery = IsDelivery
                };

                var receiptId = _database.CreateReceipt(receipt);
                receipt.Id = receiptId;

                foreach (var cartItem in Cart)
                {
                    var receiptItem = new ReceiptItem
                    {
                        ReceiptId = receiptId,
                        FurnitureId = cartItem.Furniture.Id,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Furniture.Price
                    };
                    _database.AddReceiptItem(receiptItem);
                }

                // Генеруємо PDF чек
                var pdfGenerator = new PdfReceiptGenerator();
                var pdfPath = pdfGenerator.GenerateReceipt(receipt, Cart.ToList(), _database);

                // Показуємо координати для самовивозу
                if (!IsDelivery)
                {
                    var storageInfo = string.Join("\n", Cart.Select(item =>
                        $"{item.Furniture.Name}: Ряд {item.Furniture.StorageRow}, Полиця {item.Furniture.StorageShelf}"));

                    MessageBox.Show(
                        $"Чек сформовано!\nНомер: {receipt.ReceiptNumber}\n\nКоординати на складі:\n{storageInfo}\n\nPDF чек збережено: {pdfPath}",
                        "Самовивіз",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        $"Чек сформовано!\nНомер: {receipt.ReceiptNumber}\nСума: {TotalAmount:N2} ₴\n\nPDF чек збережено: {pdfPath}\n\nОплатіть чек на касі або на сайті.",
                        "Доставка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                // Очищаємо кошик
                foreach (var item in Cart)
                {
                    item.PropertyChanged -= CartItem_PropertyChanged;
                }
                Cart.Clear();
                OnPropertyChanged(nameof(TotalAmount));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при створенні чека: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CartItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartItem.TotalPrice))
            {
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
