using System;
using System.Data.SqlClient;
using System.Linq;

namespace P08.IncreaseMinionAge
{
    class P08StartUp
    {

        private static string connectionString =
            "Server=DESKTOP-7JEJ5UL\\SQLEXPRESS01;" +
            "Database=MinionsDB;" +
            "Integrated Security=true;";

        private static SqlConnection connection = new SqlConnection(connectionString);
        static void Main(string[] args)
        {
            int[] input = Console.ReadLine()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            connection.Open();

            using (connection)
            {
                foreach (var id in input)
                {
                    string queryText = @" UPDATE Minions
                                          SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                                          WHERE Id = @Id";

                    SqlCommand command = new SqlCommand(queryText, connection);
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }

                string text = @"SELECT Name, Age FROM Minions";
                SqlCommand com = new SqlCommand(text, connection);

                SqlDataReader reader = com.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Name"]} - {reader["Age"]}");
                    }
                }
            }
        }
    }
}
