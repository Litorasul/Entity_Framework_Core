using System;
using Microsoft.EntityFrameworkCore;
using P03_FootballBetting.Data;

namespace P03_FootballBetting
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new FootballBettingContext())
            {
                db.Database.Migrate();
                Console.WriteLine("******");
            }
        }
    }
}
