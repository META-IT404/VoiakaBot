using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace VoiakaBotBackEnd
{
    internal static class WorkingToDB
    {
        [Obsolete]
        public static List<Product> GetAllProduct()
        {
            List<Product> products = new List<Product>();
            using (SqlConnection connection = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\\DataBaseVoiakaBot.mdf;User Id=AlterEgo;Password=123; TrustServerCertificate=True"))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Production";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product()
                            {
                                ID = (int)reader["Id"],
                                Name = reader["NameProduct"].ToString(),
                                Price = (decimal)reader["PriceProduct"],
                                QuantityProduct = (int)reader["QuantityProduct"],
                                WarrantyProduct = reader["WarrantyProduct"].ToString()
                            });
                        }
                    }
                }
            }
            return products;
        }
        public static bool BuyProduct(int idProduct, string userName)
        {
            using (SqlConnection connection = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\\DataBaseVoiakaBot.mdf;Id=Mary5;Password=123"))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    List<string> list = new List<string>();
                    cmd.CommandText = $"SELECT NameProduct, PriceProduct, WarrantyProduct FROM Production WHERE Id = {idProduct}";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            list.Add(reader["NameProduct"].ToString());
                            list.Add(reader["PriceProduct"].ToString());
                            list.Add(reader["WarrantyProduct"].ToString());
                            reader.Close();
                            cmd.CommandText = $"INSERT Сustomer (NameCustomer, NameProduct, PriceProduct, WarrantyProduct) VALUES ({userName}, {list[0]}, {list[1]}, {list[2]})";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = $"DELETE FROM Production WHERE {idProduct}";
                            cmd.ExecuteNonQuery();
                            return true;
                        }
                        else { return false; }
                    }

                }
            }
        }
        public static void SetProduct(string nameProduct, decimal price, int quantity, string warranty)
        {
            using (SqlConnection connection = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\\DataBaseVoiakaBot.mdf;Id=Mary5;Password=123"))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"INSERT Production (NameProduct, PriceProduct, QuantityProduct, WarrantyProduct) VALUES ({nameProduct}, {price}, {quantity}, {warranty})";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static string GetOrders()
        {
            string str = "Имя заказчика//Имя продукта//Цена//Колчество//Гарантия";
            using (SqlConnection connection = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\\DataBaseVoiakaBot.mdf;Id=Mary5;Password=123"))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Production";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            str += $"{reader["NameCustomer"]}//{reader["NameProduct"]}//{reader["PriceProduct"]}//{reader["QuantityProduct"]}//{reader["WarrantyProduct"]}\n";
                        }
                    }
                }
            }
            return str;
        }
        public static bool DelOrders(string idOrder)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\\DataBaseVoiakaBot.mdf;Id=Mary5;Password=123"))
                {
                    connection.Open();
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = $"DELETE FROM Сustomer WHERE {idOrder}";
                        cmd.ExecuteNonQuery();
                        return true ;
                    }
                }
            }
            catch (Exception ex) { return false; }
        }
    }
}
