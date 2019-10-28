using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace P07.PrintMinionNames
{
    class P07StartUp
    {
        private static string connectionString =
            "Server=DESKTOP-7JEJ5UL\\SQLEXPRESS01;" +
            "Database=MinionsDB;" +
            "Integrated Security=true;";

        private static SqlConnection connection = new SqlConnection(connectionString);
        static void Main(string[] args)
        {
            connection.Open();

            using (connection)
            {
                string queryText = @"SELECT Name FROM Minions";

                SqlCommand command = new SqlCommand(queryText, connection);

                SqlDataReader reader = command.ExecuteReader();

                List<string> namesList = new List<string>();

                using (reader)
                {
                    while (reader.Read())
                    {
                        namesList.Add((string)reader["Name"]);
                    }
                }

                while (namesList.Count > 0)
                {
                    Console.WriteLine(namesList[0]);
                    namesList.RemoveAt(0);

                    if (namesList.Count > 0)
                    {
                        Console.WriteLine(namesList[namesList.Count - 1]);
                        namesList.RemoveAt(namesList.Count - 1);
                    }
                }
            }
        }
    }
}
