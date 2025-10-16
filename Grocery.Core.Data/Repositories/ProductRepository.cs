using Grocery.Core.Data.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class ProductRepository : DatabaseConnection, IProductRepository
    {
        private readonly List<Product>_products;
        public ProductRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Product (
                [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                [Name] NVARCHAR(80) NOT NULL,
                [Stock] INTEGER NOT NULL,
                [ShelfLife] DATE,
                [Price] DECIMAL(5,2) NOT NULL
            )");
        }

        public List<Product> GetAll()
        {
            var items = new List<Product>();
            string selectQuery = "SELECT Id, Name, Stock, ShelfLife, Price FROM Product";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int stock = reader.GetInt32(2);
                    DateOnly shelfLife = reader.IsDBNull(3) ? default : DateOnly.FromDateTime(reader.GetDateTime(3));
                    decimal price = reader.GetDecimal(4);
                    items.Add(new Product(id, name, stock, shelfLife, price));
                }
            }
            CloseConnection();
            return items;
        }

        public Product Add(Product product)
        {
            string insertQuery = @"INSERT INTO Product(Name, Stock, ShelfLife, Price) 
                                   VALUES(@Name, @Stock, @ShelfLife, @Price); SELECT last_insert_rowid();";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("Name", product.Name);
                command.Parameters.AddWithValue("Stock", product.stock);
                command.Parameters.AddWithValue("ShelfLife", product.ShelfLife == default ? DBNull.Value : product.ShelfLife.ToDateTime(TimeOnly.MinValue));
                command.Parameters.AddWithValue("Price", product.Price);
                product.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return product;
        }

        public Product? Get(int id)
        {
            Product? product = null;
            string selectQuery = "SELECT Id, Name, Stock, ShelfLife, Price FROM Product WHERE Id = @Id";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("Id", id);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int productId = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int stock = reader.GetInt32(2);
                        DateOnly shelfLife = reader.IsDBNull(3) ? default : DateOnly.FromDateTime(reader.GetDateTime(3));
                        decimal price = reader.GetDecimal(4);
                        product = new Product(productId, name, stock, shelfLife, price);
                    }
                }
            }
            CloseConnection();
            return product;
        }

        public Product? Delete(Product item)
        {
            Product? deletedProduct = null;
            string deleteQuery = "DELETE FROM Product WHERE Id = @Id";
            OpenConnection();
            using (var command = new SqliteCommand(deleteQuery, Connection))
            {
                command.Parameters.AddWithValue("Id", item.Id);
                int affectedRows = command.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    deletedProduct = item;
                }
            }
            CloseConnection();
            return deletedProduct;
        }

        public Product? Update(Product item)
        {
            Product? updatedProduct = null;
            string updateQuery = @"UPDATE Product 
                          SET Name = @Name, Stock = @Stock, ShelfLife = @ShelfLife, Price = @Price 
                          WHERE Id = @Id";
            OpenConnection();
            using (var command = new SqliteCommand(updateQuery, Connection))
            {
                command.Parameters.AddWithValue("Name", item.Name);
                command.Parameters.AddWithValue("Stock", item.stock);
                command.Parameters.AddWithValue("ShelfLife", item.ShelfLife == default ? DBNull.Value : item.ShelfLife.ToDateTime(TimeOnly.MinValue));
                command.Parameters.AddWithValue("Price", item.Price);
                command.Parameters.AddWithValue("Id", item.Id);
                int affectedRows = command.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    updatedProduct = item;
                }
            }
            CloseConnection();
            return updatedProduct;
        }
    }
}
