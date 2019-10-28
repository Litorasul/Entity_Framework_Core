using System;
using System.Data.SqlClient;

namespace P05.TownNamesCasing
{
    class P05StartUp
    {
        private static string connectionString =
            "Server=DESKTOP-7JEJ5UL\\SQLEXPRESS01;" +
            "Database=MinionsDB;" +
            "Integrated Security=true;";

        private static SqlConnection connection = new SqlConnection(connectionString);
        static void Main(string[] args)
        {
            string countryName = Console.ReadLine();

            connection.Open();

            using (connection)
            {
                string queryText = @"UPDATE Towns
                                     SET Name = UPPER(Name)
                                     WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";

                SqlCommand command = new SqlCommand(queryText, connection);
                command.Parameters.AddWithValue("@countryName", countryName);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    Console.WriteLine("No town names were affected.");
                    return;
                }

                Console.WriteLine($"{rowsAffected} town names were affected.");

                queryText = @"SELECT t.Name 
                              FROM Towns as t
                              JOIN Countries AS c ON c.Id = t.CountryCode
                              WHERE c.Name = @countryName";

                command = new SqlCommand(queryText, connection);
                command.Parameters.AddWithValue("@countryName", countryName);


                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.Write($"{reader["Name"]} ");
                    }

                }
            }
        }
    }
}
