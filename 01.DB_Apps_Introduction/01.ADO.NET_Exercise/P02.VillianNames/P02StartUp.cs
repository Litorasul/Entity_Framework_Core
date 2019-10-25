using System;
using System.Data.SqlClient;

namespace P02.VillianNames
{
    class P02StartUp
    {
        private static string connectionString = 
            "Server=DESKTOP-7JEJ5UL\\SQLEXPRESS01;"+
            "Database=MinionsDB;"+
            "Integrated Security=true;";

        private static SqlConnection conection = new SqlConnection(connectionString);
        static void Main(string[] args)
        {
            conection.Open();

            using (conection)
            {
                string queryText = @"  SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                                       FROM Villains AS v 
                                       JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                                       GROUP BY v.Id, v.Name 
                                       HAVING COUNT(mv.VillainId) > 3 
                                       ORDER BY COUNT(mv.VillainId)";

                SqlCommand command = new SqlCommand(queryText, conection);

                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Name"]} - {reader["MinionsCount"]}");
                    }
                }
            }
        }
    }
}
