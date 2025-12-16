using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using FurnitureShop.Models;

namespace FurnitureShop.Database
{
    public class DatabaseHelper
    {
        private const string ConnectionString = "Data Source=furniture_shop.db";

        public DatabaseHelper()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();

            // Таблиця категорій
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Category (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                )";
            command.ExecuteNonQuery();

            // Таблиця товарів
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Furniture (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    Price REAL NOT NULL,
                    StorageRow TEXT,
                    StorageShelf TEXT,
                    ImagePath TEXT,
                    ImagePath2 TEXT,
                    ImagePath3 TEXT,
                    SerialNumber TEXT,
                    CategoryId INTEGER,
                    FOREIGN KEY (CategoryId) REFERENCES Category(Id)
                )";
            command.ExecuteNonQuery();

            // Таблиця чеків
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Receipt (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReceiptNumber TEXT UNIQUE NOT NULL,
                    Date TEXT NOT NULL,
                    TotalPrice REAL NOT NULL,
                    Delivery INTEGER NOT NULL
                )";
            command.ExecuteNonQuery();

            // Таблиця позицій чека
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS ReceiptItem (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReceiptId INTEGER NOT NULL,
                    FurnitureId INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Price REAL NOT NULL,
                    FOREIGN KEY (ReceiptId) REFERENCES Receipt(Id),
                    FOREIGN KEY (FurnitureId) REFERENCES Furniture(Id)
                )";
            command.ExecuteNonQuery();
        }

        // Category CRUD
        public List<Category> GetAllCategories()
        {
            var categories = new List<Category>();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Category";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                categories.Add(new Category
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }

            return categories;
        }

        public void AddCategory(Category category)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Category (Name) VALUES (@Name)";
            command.Parameters.AddWithValue("@Name", category.Name);
            command.ExecuteNonQuery();
        }

        // Furniture CRUD
        public List<Furniture> GetAllFurniture()
        {
            var furnitureList = new List<Furniture>();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT f.*, c.Name as CategoryName
                FROM Furniture f
                LEFT JOIN Category c ON f.CategoryId = c.Id";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var furniture = new Furniture
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Price = (decimal)reader.GetDouble(3),
                    StorageRow = reader.IsDBNull(4) ? null : reader.GetString(4),
                    StorageShelf = reader.IsDBNull(5) ? null : reader.GetString(5),
                    ImagePath = reader.IsDBNull(6) ? null : reader.GetString(6),
                    ImagePath2 = reader.FieldCount > 7 && !reader.IsDBNull(7) ? reader.GetString(7) : null,
                    ImagePath3 = reader.FieldCount > 8 && !reader.IsDBNull(8) ? reader.GetString(8) : null,
                    SerialNumber = reader.FieldCount > 9 && !reader.IsDBNull(9) ? reader.GetString(9) : null,
                    CategoryId = reader.FieldCount > 10 && !reader.IsDBNull(10) ? reader.GetInt32(10) : null
                };

                // Додаємо категорію якщо вона є
                if (furniture.CategoryId.HasValue && !reader.IsDBNull(reader.FieldCount - 1))
                {
                    furniture.Category = new Category
                    {
                        Id = furniture.CategoryId.Value,
                        Name = reader.GetString(reader.FieldCount - 1)
                    };
                }

                furnitureList.Add(furniture);
            }

            return furnitureList;
        }

        public void AddFurniture(Furniture furniture)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Furniture (Name, Description, Price, StorageRow, StorageShelf, ImagePath, ImagePath2, ImagePath3, SerialNumber, CategoryId)
                VALUES (@Name, @Description, @Price, @StorageRow, @StorageShelf, @ImagePath, @ImagePath2, @ImagePath3, @SerialNumber, @CategoryId)";

            command.Parameters.AddWithValue("@Name", furniture.Name);
            command.Parameters.AddWithValue("@Description", furniture.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Price", (double)furniture.Price);
            command.Parameters.AddWithValue("@StorageRow", furniture.StorageRow ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@StorageShelf", furniture.StorageShelf ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ImagePath", furniture.ImagePath ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ImagePath2", furniture.ImagePath2 ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ImagePath3", furniture.ImagePath3 ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@SerialNumber", furniture.SerialNumber ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CategoryId", furniture.CategoryId.HasValue ? (object)furniture.CategoryId.Value : DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void UpdateFurniture(Furniture furniture)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Furniture
                SET Name = @Name,
                    Description = @Description,
                    Price = @Price,
                    StorageRow = @StorageRow,
                    StorageShelf = @StorageShelf,
                    ImagePath = @ImagePath,
                    ImagePath2 = @ImagePath2,
                    ImagePath3 = @ImagePath3,
                    SerialNumber = @SerialNumber,
                    CategoryId = @CategoryId
                WHERE Id = @Id";

            command.Parameters.AddWithValue("@Id", furniture.Id);
            command.Parameters.AddWithValue("@Name", furniture.Name);
            command.Parameters.AddWithValue("@Description", furniture.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Price", (double)furniture.Price);
            command.Parameters.AddWithValue("@StorageRow", furniture.StorageRow ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@StorageShelf", furniture.StorageShelf ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ImagePath", furniture.ImagePath ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ImagePath2", furniture.ImagePath2 ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ImagePath3", furniture.ImagePath3 ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@SerialNumber", furniture.SerialNumber ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CategoryId", furniture.CategoryId.HasValue ? (object)furniture.CategoryId.Value : DBNull.Value);

            command.ExecuteNonQuery();
        }

        public bool IsFurnitureUsedInReceipts(int furnitureId)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM ReceiptItem WHERE FurnitureId = @FurnitureId";
            command.Parameters.AddWithValue("@FurnitureId", furnitureId);

            var count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }

        public void DeleteFurniture(int id)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Furniture WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }

        public void DeleteFurnitureWithReceipts(int id)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Видаляємо всі позиції чеків з цим товаром
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM ReceiptItem WHERE FurnitureId = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }

                // Видаляємо товар
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Furniture WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Receipt operations
        public int CreateReceipt(Receipt receipt)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Receipt (ReceiptNumber, Date, TotalPrice, Delivery)
                VALUES (@ReceiptNumber, @Date, @TotalPrice, @Delivery);
                SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("@ReceiptNumber", receipt.ReceiptNumber);
            command.Parameters.AddWithValue("@Date", receipt.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@TotalPrice", (double)receipt.TotalPrice);
            command.Parameters.AddWithValue("@Delivery", receipt.Delivery ? 1 : 0);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void AddReceiptItem(ReceiptItem item)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO ReceiptItem (ReceiptId, FurnitureId, Quantity, Price)
                VALUES (@ReceiptId, @FurnitureId, @Quantity, @Price)";

            command.Parameters.AddWithValue("@ReceiptId", item.ReceiptId);
            command.Parameters.AddWithValue("@FurnitureId", item.FurnitureId);
            command.Parameters.AddWithValue("@Quantity", item.Quantity);
            command.Parameters.AddWithValue("@Price", (double)item.Price);

            command.ExecuteNonQuery();
        }

        public Receipt? GetReceiptById(int id)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Receipt WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Receipt
                {
                    Id = reader.GetInt32(0),
                    ReceiptNumber = reader.GetString(1),
                    Date = DateTime.Parse(reader.GetString(2)),
                    TotalPrice = (decimal)reader.GetDouble(3),
                    Delivery = reader.GetInt32(4) == 1
                };
            }

            return null;
        }

        public List<ReceiptItem> GetReceiptItems(int receiptId)
        {
            var items = new List<ReceiptItem>();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM ReceiptItem WHERE ReceiptId = @ReceiptId";
            command.Parameters.AddWithValue("@ReceiptId", receiptId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new ReceiptItem
                {
                    Id = reader.GetInt32(0),
                    ReceiptId = reader.GetInt32(1),
                    FurnitureId = reader.GetInt32(2),
                    Quantity = reader.GetInt32(3),
                    Price = (decimal)reader.GetDouble(4)
                });
            }

            return items;
        }

        public string GenerateReceiptNumber()
        {
            return $"RCP-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
    }
}
