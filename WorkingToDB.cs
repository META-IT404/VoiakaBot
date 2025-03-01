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
        public async static Task<List<Product>> GetAllProduct()
        {
            List<Product> products = new List<Product>();
            using (SqlConnection connection = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\\DataBaseVoiakaBot.mdf;Integrated Security=True"))
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
        public async static Task<string> BuyProduct(int idProduct, string userName)
        {
            using (SqlConnection connection = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\\DataBaseVoiakaBot.mdf;Integrated Security=True"))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    try
                    {
                        List<string> list = new List<string>();
                        cmd.CommandText = $"SELECT NameProduct, PriceProduct, WarrantyProduct FROM Production WHERE Id = {idProduct}";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.ReadAsync().Result)
                            {
                                list.Add(reader["NameProduct"].ToString());
                                list.Add(reader["PriceProduct"].ToString());
                                list.Add(reader["WarrantyProduct"].ToString());
                                reader.Close();


                                cmd.CommandText = $"DELETE FROM Production WHERE Id = @idProduct";
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@idProduct", SqlDbType.Int).Value = idProduct;
                                await cmd.ExecuteNonQueryAsync();

                            }
                            else { return null; }
                        }
                        cmd.CommandText = $"INSERT Сustomer (NameCustomer, NameProduct, PriceProduct, WarrantyProduct) VALUES (@NameCustomer, @NameProduct, @PriceProduct, @WarrantyProduct)";
                        cmd.Parameters.Add("@NameCustomer", SqlDbType.NVarChar).Value = userName;
                        cmd.Parameters.Add("@NameProduct", SqlDbType.NVarChar).Value = list[0];
                        cmd.Parameters.Add("@PriceProduct", SqlDbType.Decimal).Value = list[1];
                        cmd.Parameters.Add("@WarrantyProduct", SqlDbType.NVarChar).Value = list[2];
                        await cmd.ExecuteNonQueryAsync();
                        cmd.Parameters.Clear();
                        cmd.CommandText = "SELECT QuantityProduct FROM Production WHERE Id = @idProduct";
                        cmd.Parameters.Add("@idProduct", SqlDbType.Int).Value = idProduct;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.ReadAsync().Result)
                            {
                                int count = (int)reader["QuantityProduct"];
                                reader.Close();
                                if ( count > 0)
                                {
                                    cmd.CommandText = "UPDATE Production SET QuantityProduct = @QuantityProduct WHERE Id = @idProduct";
                                    cmd.Parameters.Add("@QuantityProduct", SqlDbType.Int).Value = count - 1;
                                    await cmd.ExecuteNonQueryAsync();
                                }
                                else
                                {
                                    cmd.CommandText = $"DELETE FROM Production WHERE Id = @idProduct";
                                    await cmd.ExecuteNonQueryAsync();
                                }
                            }
                        }
                        return list[0];
                    }
                    catch (Exception ex) { return null; }

                }
            }
        }
        public async static void SetProduct(string nameProduct, decimal price, int quantity, string warranty)
        {
            using (SqlConnection connection = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\\DataBaseVoiakaBot.mdf;Integrated Security=True"))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"INSERT Production (NameProduct, PriceProduct, QuantityProduct, WarrantyProduct) VALUES (@NameProduct, @PriceProduct, @QuantityProduct, @WarrantyProduct)";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async static Task<string> GetOrders()
        {
            string str = "Имя заказчика//Имя продукта//Цена//Колчество//Гарантия";
            using (SqlConnection connection = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\\DataBaseVoiakaBot.mdf;Integrated Security=True"))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Production";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.ReadAsync().Result)
                        {
                            str += $"{reader["NameCustomer"]}//{reader["NameProduct"]}//{reader["PriceProduct"]}//{reader["QuantityProduct"]}//{reader["WarrantyProduct"]}\n";
                        }
                    }
                }
            }
            return str;
        }
        public async static void DelOrders(string idOrder)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\\DataBaseVoiakaBot.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = $"DELETE FROM Сustomer WHERE Id = {idOrder}";
                        await cmd.ExecuteNonQueryAsync();

                    }
                }
            }
            catch (Exception ex) { }
        }
    }
}
