using System;
using System.Data.SqlClient;

namespace P04.AddMinion
{
    class P04StartUp
    {
        private static string connectionString =
            "Server=DESKTOP-7JEJ5UL\\SQLEXPRESS01;" +
            "Database=MinionsDB;" +
            "Integrated Security=true;";

        private static SqlConnection connection = new SqlConnection(connectionString);

        static void Main(string[] args)
        {
            Console.Write("Minion:");
            string[] minionInfo = Console.ReadLine().Split();

            Console.Write("Villain:");
            string villain = Console.ReadLine();

            string minionName = minionInfo[0];
            int minionAge = int.Parse(minionInfo[1]);
            string minionTown = minionInfo[2];
            int villianID;
            int minionID;
            int townID;

            using (connection)
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                SqlCommand command = connection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    string queryText;
                    var result = SelectVillain(command, villain);

                    if (result == null)
                    {
                        queryText = @"INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";
                        command.CommandText = queryText;
                        command.Parameters.AddWithValue("@villainName", villain);
                        command.ExecuteNonQuery();
                        result = SelectVillain(command, villain);
                        villianID = (int)result;
                        Console.WriteLine($"Villain {villain} was added to the database.");

                    }
                    else
                    {
                        villianID = (int) result;
                    }

                    result = SelectTown(command, minionTown);
                    if (result == null)
                    {
                        queryText = @"INSERT INTO Towns (Name) VALUES (@townName)";
                        command.CommandText = queryText;
                        command.Parameters.AddWithValue("@townName", minionTown);
                        command.ExecuteNonQuery();
                        result = SelectTown(command, minionTown);
                        townID = (int)result;
                        Console.WriteLine($"Town {minionTown} was added to the database.");
                    }
                    else
                    {
                        townID = (int)result;
                    }

                    result = SelectMinion(command, minionName);
                    if (result == null)
                    {
                        queryText = @"INSERT INTO Minions (Name, Age, TownId) VALUES (@nam, @age, @townId)";
                        command.CommandText = queryText;
                        command.Parameters.AddWithValue("@nam", minionName);
                        command.Parameters.AddWithValue("@age", minionAge);
                        command.Parameters.AddWithValue("@townId", townID);
                        command.ExecuteNonQuery();
                        result = SelectMinion(command, minionName);
                        minionID = (int)result;
                    }
                    else
                    {
                        minionID = (int)result;
                    }

                    queryText = @"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@villainId, @minionId)";
                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@villainId", villianID);
                    command.Parameters.AddWithValue("@minionId", minionID);
                    command.ExecuteNonQuery();

                    transaction.Commit();
                  
                    Console.WriteLine($"Successfully added {minionName} to be minion of {villain}.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                        
                    }
                }
            }
        }

        private static object SelectTown(SqlCommand command, string minionTown)
        {
            string queryText = @"SELECT Id FROM Towns WHERE Name = @townName"; ;
            command.CommandText = queryText;
            command.Parameters.AddWithValue("@townName", minionTown);
            var result = command.ExecuteScalar();
            return result;
        }

        private static object SelectMinion(SqlCommand command, string minionName)
        {
            string queryText = @"SELECT Id FROM Minions WHERE Name = @minionName"; ;
            command.CommandText = queryText;
            command.Parameters.AddWithValue("@minionName", minionName);
            var result = command.ExecuteScalar();
            return result;
        }

        private static object SelectVillain(SqlCommand command, string villain)
        {
            string queryText = @"SELECT Id FROM Villains WHERE Name = @Name";
            command.CommandText = queryText;
            command.Parameters.AddWithValue("@Name", villain);

            object result = command.ExecuteScalar();
            return result;
        }
    }
}
