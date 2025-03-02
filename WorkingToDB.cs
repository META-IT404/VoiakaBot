using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using VoiakaBot;

namespace VoiakaBotBackEnd
{
    internal static class WorkingToDB
    {
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
                                Price = reader["PriceProduct"].ToString(),
                                QuantityProduct = (int)reader["QuantityProduct"],
                                WarrantyProduct = reader["WarrantyProduct"].ToString(),
                                State = reader["State"].ToString()
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
                        cmd.CommandText = $"SELECT NameProduct, PriceProduct, WarrantyProduct, State FROM Production WHERE Id = {idProduct}";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.ReadAsync().Result)
                            {
                                list.Add(reader["NameProduct"].ToString());
                                list.Add(reader["PriceProduct"].ToString());
                                list.Add(reader["WarrantyProduct"].ToString());
                                list.Add(reader["State"].ToString());
                                reader.Close();
                            }
                            else { return null; }
                        }
                        //todo вернуть когда понадобиться база заказчиков
                        //cmd.CommandText = $"INSERT Сustomer (NameCustomer, NameProduct, PriceProduct, WarrantyProduct, State) VALUES (@NameCustomer, @NameProduct, @PriceProduct, @WarrantyProduct, @State)";
                        //cmd.Parameters.Add("@NameCustomer", SqlDbType.NVarChar).Value = userName;
                        //cmd.Parameters.Add("@NameProduct", SqlDbType.NVarChar).Value = list[0];
                        //cmd.Parameters.Add("@PriceProduct", SqlDbType.NVarChar).Value = list[1];
                        //cmd.Parameters.Add("@WarrantyProduct", SqlDbType.NVarChar).Value = list[2];
                        //cmd.Parameters.Add("@State", SqlDbType.NVarChar).Value = list[3];
                        //await cmd.ExecuteNonQueryAsync();
                        //cmd.Parameters.Clear();
                        //cmd.CommandText = "SELECT QuantityProduct FROM Production WHERE Id = @idProduct";
                        //cmd.Parameters.Add("@idProduct", SqlDbType.Int).Value = idProduct;
                        //using (SqlDataReader reader = cmd.ExecuteReader())
                        //{
                        //    if (reader.ReadAsync().Result)
                        //    {
                        //        int count = (int)reader["QuantityProduct"];
                        //        reader.Close();
                        //        if (count > 0)
                        //        {
                        //            cmd.CommandText = "UPDATE Production SET QuantityProduct = @QuantityProduct WHERE Id = @idProduct";
                        //            cmd.Parameters.Add("@QuantityProduct", SqlDbType.Int).Value = count - 1;
                        //            await cmd.ExecuteNonQueryAsync();
                        //        }
                        //        //else
                        //        //{
                        //        //    cmd.CommandText = $"DELETE FROM Production WHERE Id = @idProduct";
                        //        //    await cmd.ExecuteNonQueryAsync();
                        //        //}
                        //    }
                        //}
                        return list[0];
                    }
                    catch (Exception ex) { return null; }

                }
            }
        }
        public async static void SetProduct(string nameProduct, string price, int quantity, string warranty, string state)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\\DataBaseVoiakaBot.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = $"INSERT Production (NameProduct, PriceProduct, QuantityProduct, WarrantyProduct, State) VALUES (@NameProduct, @PriceProduct, @QuantityProduct, @WarrantyProduct, @State)";
                        cmd.Parameters.Add("@NameProduct", SqlDbType.NVarChar).Value = nameProduct;
                        cmd.Parameters.Add("@PriceProduct", SqlDbType.NVarChar).Value = price;
                        cmd.Parameters.Add("@QuantityProduct", SqlDbType.Int).Value = quantity;
                        cmd.Parameters.Add("@WarrantyProduct", SqlDbType.NVarChar).Value = warranty;
                        cmd.Parameters.Add("@State", SqlDbType.NVarChar).Value = state;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch { }
        }
        public async static Task<List<Orders>> GetOrders()
        {
            try
            {
                List<Orders> orders = new List<Orders>();
                using (SqlConnection connection = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\\DataBaseVoiakaBot.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Сustomer";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.ReadAsync().Result)
                            {
                                orders.Add(new Orders()
                                {
                                    ID = (int)reader["Id"],
                                    Name = reader["NameCustomer"].ToString(),
                                    NameProduct = reader["NameProduct"].ToString(),
                                    PriceProduct = reader["PriceProduct"].ToString(),
                                    WarrantyProduct = (string)reader["WarrantyProduct"],
                                    State = (string)reader["State"]
                                });
                            }
                        }
                    }
                }
                return orders;
            }
            catch
            {
                return null;
            }
        }
        public async static void SetProductUpDownt(Product product, int count)
        {
            using (SqlConnection connection = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\\DataBaseVoiakaBot.mdf;Integrated Security=True"))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"UPDATE Production SET QuantityProduct = @QuantityProduct WHERE Id = {product.ID}";
                    cmd.Parameters.Add("@QuantityProduct", SqlDbType.Int).Value = count;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async static Task<Product> GetProductUpDown(int idProduct)
        {
            try
            {
                Product product = null;
                using (SqlConnection connection = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\\DataBaseVoiakaBot.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT * FROM Production WHERE Id = {idProduct}";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.ReadAsync().Result)
                            {
                                product = new Product()
                                {
                                    ID = (int)reader[0],
                                    Name = (string)reader[1],
                                    Price = (string)reader[2],
                                    QuantityProduct = (int)reader[3],
                                    WarrantyProduct = (string)reader[4],
                                    State = (string)reader[5]
                                };
                            }
                        }
                    }
                }
                return product;
            }
            catch
            {
                return null;
            }
        }
        public async static void DelProduct(int idProduct)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Directory.GetCurrentDirectory()}\\DataBaseVoiakaBot.mdf;Integrated Security=True"))
                {
                    connection.Open();
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = $"DELETE FROM Production WHERE Id = {idProduct}";
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch
            {
            }
        }
        public async static void DelOrders(int idOrder)
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
