using Grocery.Core.Data.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class GroceryListItemsRepository : DatabaseConnection, IGroceryListItemsRepository
    {
        public GroceryListItemsRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS GroceryListItem (
                [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                [GroceryListId] INTEGER NOT NULL,
                [ProductId] INTEGER NOT NULL,
                [Amount] INTEGER NOT NULL,
                UNIQUE(GroceryListId, ProductId)
            )");
        }

        public List<GroceryListItem> GetAll()
        {
            var items = new List<GroceryListItem>();
            string selectQuery = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItem";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);
                    items.Add(new GroceryListItem(id, groceryListId, productId, amount));
                }
            }
            CloseConnection();
            return items;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int id)
        {
            var items = new List<GroceryListItem>();
            string selectQuery = $"SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItem WHERE GroceryListId = {id}";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int itemId = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);
                    items.Add(new GroceryListItem(itemId, groceryListId, productId, amount));
                }
            }
            CloseConnection();
            return items;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            string insertQuery = $"INSERT OR IGNORE INTO GroceryListItem(GroceryListId, ProductId, Amount) VALUES(@GroceryListId, @ProductId, @Amount);";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("GroceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("ProductId", item.ProductId);
                command.Parameters.AddWithValue("Amount", item.amount);
                command.ExecuteNonQuery();
            }
            var result = GetByListAndProduct(item.GroceryListId, item.ProductId);
            CloseConnection();
            return result ?? item;
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            string deleteQuery = $"DELETE FROM GroceryListItem WHERE Id = {item.Id};";
            OpenConnection();
            Connection.ExecuteNonQuery(deleteQuery);
            CloseConnection();
            return item;
        }

        public GroceryListItem? Get(int id)
        {
            GroceryListItem? result = null;
            string selectQuery = $"SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItem WHERE Id = {id}";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int itemId = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);
                    result = new GroceryListItem(itemId, groceryListId, productId, amount);
                }
            }
            CloseConnection();
            return result;
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            string updateQuery = $"UPDATE GroceryListItem SET GroceryListId = @GroceryListId, ProductId = @ProductId, Amount = @Amount WHERE Id = {item.Id};";
            OpenConnection();
            using (SqliteCommand command = new(updateQuery, Connection))
            {
                command.Parameters.AddWithValue("GroceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("ProductId", item.ProductId);
                command.Parameters.AddWithValue("Amount", item.amount);
                command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }

        public GroceryListItem? GetByListAndProduct(int groceryListId, int productId)
        {
            GroceryListItem? result = null;
            string selectQuery = $"SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItem WHERE GroceryListId = @GroceryListId AND ProductId = @ProductId";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("GroceryListId", groceryListId);
                command.Parameters.AddWithValue("ProductId", productId);
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int glId = reader.GetInt32(1);
                    int pId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);
                    result = new GroceryListItem(id, glId, pId, amount);
                }
            }
            CloseConnection();
            return result;
        }
    }
}
