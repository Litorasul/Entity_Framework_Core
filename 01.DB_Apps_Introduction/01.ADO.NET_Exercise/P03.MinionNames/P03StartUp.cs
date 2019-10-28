using System;
using System.Data.SqlClient;


namespace P03.MinionNames
{
    class P03StartUp
    {
        private static string connectionString =
            "Server=DESKTOP-7JEJ5UL\\SQLEXPRESS01;" +
            "Database=MinionsDB;" +
            "Integrated Security=true;";

        private static SqlConnection connection = new SqlConnection(connectionString);
        static void Main(string[] args)
        {
            int villainID = int.Parse(Console.ReadLine());
            bool isThereSuchVillain = false;


            connection.Open();

            using (connection)
            {
                string queryText = $@"SELECT Name FROM Villains WHERE Id = {villainID}";

                SqlCommand command = new SqlCommand(queryText, connection);

                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {

                    while (reader.Read())
                    {
                        Console.WriteLine($"Villain: {reader["Name"]}");

                        isThereSuchVillain = true;
                    }

                }
            }
            connection = new SqlConnection(connectionString);
            connection.Open();

            using (connection)
            {
                string queryTwo = $@"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = {villainID}
                                ORDER BY m.Name";

                SqlCommand commandTwo = new SqlCommand(queryTwo, connection);

                SqlDataReader readerTwo = commandTwo.ExecuteReader();

                if (isThereSuchVillain)
                {
                    using (readerTwo)
                    {
                        int number = 1;
                        while (readerTwo.Read())
                        {
                            Console.WriteLine($"{number}. {readerTwo["Name"]} {readerTwo["Age"]}");
                            number++;
                        }
                    }
                }
                else
                {
                    Console.WriteLine
                        ($"No villain with ID {villainID} exists in the database.");
                }

            }

        }
        
    }
}
