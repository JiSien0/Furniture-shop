using System.Linq;
using FurnitureShop.Models;

namespace FurnitureShop.Database
{
    public class DatabaseSeeder
    {
        private readonly DatabaseHelper _database;

        public DatabaseSeeder(DatabaseHelper database)
        {
            _database = database;
        }

        public void SeedData()
        {
            // Проверяем, есть ли уже данные в БД
            var existingFurniture = _database.GetAllFurniture();
            if (existingFurniture.Any())
            {
                return; // Данные уже есть
            }

            // Додаємо категорії
            var categories = new[]
            {
                new Category { Name = "М'які меблі" },
                new Category { Name = "Шафи та гардероби" },
                new Category { Name = "Ліжка" },
                new Category { Name = "Столи" },
                new Category { Name = "Стільці" },
                new Category { Name = "Тумби" },
                new Category { Name = "Комоди" },
                new Category { Name = "Офісні крісла" }
            };

            foreach (var category in categories)
            {
                _database.AddCategory(category);
            }

            // Отримуємо додані категорії з ID
            var addedCategories = _database.GetAllCategories();

            // Додаємо тестові дані з категоріями та серійними номерами
            var furnitureItems = new[]
            {
                new Furniture
                {
                    Name = "Диван \"Комфорт\"",
                    Description = "Зручний тримісний диван з м'якою оббивкою",
                    Price = 35000,
                    StorageRow = "A",
                    StorageShelf = "12",
                    SerialNumber = "SF-001-2024",
                    CategoryId = addedCategories.FirstOrDefault(c => c.Name == "М'які меблі")?.Id
                },
                new Furniture
                {
                    Name = "Крісло \"Релакс\"",
                    Description = "Ергономічне крісло для відпочинку",
                    Price = 15000,
                    StorageRow = "A",
                    StorageShelf = "15",
                    SerialNumber = "SF-002-2024",
                    CategoryId = addedCategories.FirstOrDefault(c => c.Name == "М'які меблі")?.Id
                },
                new Furniture
                {
                    Name = "Шафа-купе \"Місткий\"",
                    Description = "Велика шафа з дзеркальними дверцятами",
                    Price = 45000,
                    StorageRow = "B",
                    StorageShelf = "3",
                    SerialNumber = "WR-001-2024",
                    CategoryId = addedCategories.FirstOrDefault(c => c.Name == "Шафи та гардероби")?.Id
                },
                new Furniture
                {
                    Name = "Ліжко \"Класика\"",
                    Description = "Двоспальне ліжко з ортопедичною основою",
                    Price = 28000,
                    StorageRow = "B",
                    StorageShelf = "7",
                    SerialNumber = "BD-001-2024",
                    CategoryId = addedCategories.FirstOrDefault(c => c.Name == "Ліжка")?.Id
                },
                new Furniture
                {
                    Name = "Стіл обідній \"Сімейний\"",
                    Description = "Розсувний стіл на 6-8 персон",
                    Price = 22000,
                    StorageRow = "C",
                    StorageShelf = "5",
                    SerialNumber = "TB-001-2024",
                    CategoryId = addedCategories.FirstOrDefault(c => c.Name == "Столи")?.Id
                },
                new Furniture
                {
                    Name = "Стілець \"Економ\"",
                    Description = "Зручний стілець для кухні або їдальні",
                    Price = 3500,
                    StorageRow = "C",
                    StorageShelf = "8",
                    SerialNumber = "CH-001-2024",
                    CategoryId = addedCategories.FirstOrDefault(c => c.Name == "Стільці")?.Id
                },
                new Furniture
                {
                    Name = "Тумба під ТВ \"Медіа\"",
                    Description = "Сучасна тумба з полицями для апаратури",
                    Price = 12000,
                    StorageRow = "A",
                    StorageShelf = "20",
                    SerialNumber = "CB-001-2024",
                    CategoryId = addedCategories.FirstOrDefault(c => c.Name == "Тумби")?.Id
                },
                new Furniture
                {
                    Name = "Комод \"Прованс\"",
                    Description = "Комод з 5 висувними ящиками",
                    Price = 18000,
                    StorageRow = "B",
                    StorageShelf = "12",
                    SerialNumber = "DR-001-2024",
                    CategoryId = addedCategories.FirstOrDefault(c => c.Name == "Комоди")?.Id
                },
                new Furniture
                {
                    Name = "Журнальний столик \"Модерн\"",
                    Description = "Стильний столик зі скляною стільницею",
                    Price = 8500,
                    StorageRow = "C",
                    StorageShelf = "2",
                    SerialNumber = "TB-002-2024",
                    CategoryId = addedCategories.FirstOrDefault(c => c.Name == "Столи")?.Id
                },
                new Furniture
                {
                    Name = "Крісло офісне \"Бос\"",
                    Description = "Крісло для керівника з регулюванням висоти",
                    Price = 16500,
                    StorageRow = "A",
                    StorageShelf = "18",
                    SerialNumber = "OF-001-2024",
                    CategoryId = addedCategories.FirstOrDefault(c => c.Name == "Офісні меблі")?.Id
                }
            };

            foreach (var furniture in furnitureItems)
            {
                _database.AddFurniture(furniture);
            }
        }
    }
}
