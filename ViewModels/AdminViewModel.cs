using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FurnitureShop.Database;
using FurnitureShop.Models;
using Microsoft.Win32;

namespace FurnitureShop.ViewModels
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseHelper _database;
        private Furniture? _selectedFurniture;
        private string _name = string.Empty;
        private string? _description;
        private decimal _price;
        private string? _storageRow;
        private string? _storageShelf;
        private string? _imagePath;
        private string? _imagePath2;
        private string? _imagePath3;
        private string? _serialNumber;
        private Category? _selectedCategory;

        public ObservableCollection<Furniture> FurnitureList { get; set; }
        public ObservableCollection<Category> Categories { get; set; }

        public Furniture? SelectedFurniture
        {
            get => _selectedFurniture;
            set
            {
                if (_selectedFurniture != value)
                {
                    _selectedFurniture = value;
                    OnPropertyChanged(nameof(SelectedFurniture));
                    LoadSelectedFurniture();
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string? Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        public string? StorageRow
        {
            get => _storageRow;
            set
            {
                _storageRow = value;
                OnPropertyChanged(nameof(StorageRow));
            }
        }

        public string? StorageShelf
        {
            get => _storageShelf;
            set
            {
                _storageShelf = value;
                OnPropertyChanged(nameof(StorageShelf));
            }
        }

        public string? ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        public string? ImagePath2
        {
            get => _imagePath2;
            set
            {
                _imagePath2 = value;
                OnPropertyChanged(nameof(ImagePath2));
            }
        }

        public string? ImagePath3
        {
            get => _imagePath3;
            set
            {
                _imagePath3 = value;
                OnPropertyChanged(nameof(ImagePath3));
            }
        }

        public string? SerialNumber
        {
            get => _serialNumber;
            set
            {
                _serialNumber = value;
                OnPropertyChanged(nameof(SerialNumber));
            }
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand SelectImage2Command { get; }
        public ICommand SelectImage3Command { get; }

        public AdminViewModel()
        {
            _database = new DatabaseHelper();
            FurnitureList = new ObservableCollection<Furniture>();
            Categories = new ObservableCollection<Category>();

            AddCommand = new RelayCommand(_ => AddFurniture());
            UpdateCommand = new RelayCommand(_ => UpdateFurniture(), _ => SelectedFurniture != null);
            DeleteCommand = new RelayCommand(_ => DeleteFurniture(), _ => SelectedFurniture != null);
            ClearCommand = new RelayCommand(_ => ClearForm());
            SelectImageCommand = new RelayCommand(_ => SelectImage(1));
            SelectImage2Command = new RelayCommand(_ => SelectImage(2));
            SelectImage3Command = new RelayCommand(_ => SelectImage(3));

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
            FurnitureList.Clear();
            var furniture = _database.GetAllFurniture();
            foreach (var item in furniture)
            {
                FurnitureList.Add(item);
            }
        }

        private void LoadSelectedFurniture()
        {
            if (SelectedFurniture != null)
            {
                Name = SelectedFurniture.Name;
                Description = SelectedFurniture.Description;
                Price = SelectedFurniture.Price;
                StorageRow = SelectedFurniture.StorageRow;
                StorageShelf = SelectedFurniture.StorageShelf;
                ImagePath = SelectedFurniture.ImagePath;
                ImagePath2 = SelectedFurniture.ImagePath2;
                ImagePath3 = SelectedFurniture.ImagePath3;
                SerialNumber = SelectedFurniture.SerialNumber;
                SelectedCategory = SelectedFurniture.CategoryId.HasValue
                    ? Categories.FirstOrDefault(c => c.Id == SelectedFurniture.CategoryId.Value)
                    : null;
            }
        }

        private void AddFurniture()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Будь ласка, введіть назву товару", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Price <= 0)
            {
                MessageBox.Show("Будь ласка, введіть коректну ціну", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var furniture = new Furniture
            {
                Name = Name,
                Description = Description,
                Price = Price,
                StorageRow = StorageRow,
                StorageShelf = StorageShelf,
                ImagePath = ImagePath,
                ImagePath2 = ImagePath2,
                ImagePath3 = ImagePath3,
                SerialNumber = SerialNumber,
                CategoryId = SelectedCategory?.Id
            };

            _database.AddFurniture(furniture);
            LoadFurniture();
            ClearForm();
            MessageBox.Show("Товар успішно додано!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateFurniture()
        {
            if (SelectedFurniture == null)
            {
                MessageBox.Show("Будь ласка, виберіть товар для редагування", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Будь ласка, введіть назву товару", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Price <= 0)
            {
                MessageBox.Show("Будь ласка, введіть коректну ціну", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedFurniture.Name = Name;
            SelectedFurniture.Description = Description;
            SelectedFurniture.Price = Price;
            SelectedFurniture.StorageRow = StorageRow;
            SelectedFurniture.StorageShelf = StorageShelf;
            SelectedFurniture.ImagePath = ImagePath;
            SelectedFurniture.ImagePath2 = ImagePath2;
            SelectedFurniture.ImagePath3 = ImagePath3;
            SelectedFurniture.SerialNumber = SerialNumber;
            SelectedFurniture.CategoryId = SelectedCategory?.Id;

            _database.UpdateFurniture(SelectedFurniture);
            LoadFurniture();
            ClearForm();
            MessageBox.Show("Товар успішно оновлено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteFurniture()
        {
            if (SelectedFurniture == null)
            {
                MessageBox.Show("Будь ласка, виберіть товар для видалення", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Зберігаємо шляхи до картинок перед видаленням
                var imagePath = SelectedFurniture.ImagePath;
                var imagePath2 = SelectedFurniture.ImagePath2;
                var imagePath3 = SelectedFurniture.ImagePath3;

                // Перевіряємо, чи використовується товар в чеках
                if (_database.IsFurnitureUsedInReceipts(SelectedFurniture.Id))
                {
                    var result = MessageBox.Show(
                        $"Товар '{SelectedFurniture.Name}' використовується в історії чеків!\n\n" +
                        "Натисніть 'Так' щоб видалити товар разом з історією чеків і картинкою (незворотно)\n" +
                        "Натисніть 'Ні' щоб скасувати видалення",
                        "Увага! Товар використовується",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        _database.DeleteFurnitureWithReceipts(SelectedFurniture.Id);

                        // Видаляємо файли картинок
                        DeleteImageFile(imagePath);
                        DeleteImageFile(imagePath2);
                        DeleteImageFile(imagePath3);

                        LoadFurniture();
                        ClearForm();
                        MessageBox.Show("Товар, пов'язані чеки та картинки успішно видалено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    var result = MessageBox.Show(
                        $"Ви впевнені, що хочете видалити '{SelectedFurniture.Name}'?\n\n" +
                        "Картинка товару також буде видалена з папки.",
                        "Підтвердження",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _database.DeleteFurniture(SelectedFurniture.Id);

                        // Видаляємо файли картинок
                        DeleteImageFile(imagePath);
                        DeleteImageFile(imagePath2);
                        DeleteImageFile(imagePath3);

                        LoadFurniture();
                        ClearForm();
                        MessageBox.Show("Товар та картинки успішно видалено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при видаленні товару: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteImageFile(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            try
            {
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
            catch (Exception ex)
            {
                // Логуємо помилку, але не показуємо користувачу - товар вже видалено
                System.Diagnostics.Debug.WriteLine($"Не вдалося видалити файл картинки: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            SelectedFurniture = null;
            Name = string.Empty;
            Description = null;
            Price = 0;
            StorageRow = null;
            StorageShelf = null;
            ImagePath = null;
            ImagePath2 = null;
            ImagePath3 = null;
            SerialNumber = null;
            SelectedCategory = null;
        }

        private void SelectImage(int imageNumber)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Зображення (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|Всі файли (*.*)|*.*",
                Title = $"Виберіть зображення {imageNumber} товару"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var sourceFile = openFileDialog.FileName;
                    var fileName = Path.GetFileName(sourceFile);

                    // Створюємо папку Images якщо її немає
                    var imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                    Directory.CreateDirectory(imagesFolder);

                    // Генеруємо унікальне ім'я файлу якщо файл з таким іменем вже існує
                    var destinationFile = Path.Combine(imagesFolder, fileName);

                    // Якщо файл вже існує, додаємо timestamp
                    if (File.Exists(destinationFile) && sourceFile != destinationFile)
                    {
                        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                        var extension = Path.GetExtension(fileName);
                        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        fileName = $"{fileNameWithoutExt}_{timestamp}{extension}";
                        destinationFile = Path.Combine(imagesFolder, fileName);
                    }

                    // Копіюємо файл (якщо це не той самий файл)
                    if (sourceFile != destinationFile)
                    {
                        File.Copy(sourceFile, destinationFile, overwrite: false);
                        MessageBox.Show($"Зображення {imageNumber} скопійовано в папку проєкту!\n\nФайл: {fileName}", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    // Зберігаємо шлях до скопійованого файлу
                    switch (imageNumber)
                    {
                        case 1:
                            ImagePath = destinationFile;
                            break;
                        case 2:
                            ImagePath2 = destinationFile;
                            break;
                        case 3:
                            ImagePath3 = destinationFile;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при копіюванні зображення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    // У випадку помилки зберігаємо оригінальний шлях
                    switch (imageNumber)
                    {
                        case 1:
                            ImagePath = openFileDialog.FileName;
                            break;
                        case 2:
                            ImagePath2 = openFileDialog.FileName;
                            break;
                        case 3:
                            ImagePath3 = openFileDialog.FileName;
                            break;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
