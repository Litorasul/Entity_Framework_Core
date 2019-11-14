using System;
using System.Linq;
using System.Text;
using BookShop.Models.Enums;

namespace BookShop
{
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
               //DbInitializer.ResetDatabase(db);

               Console.WriteLine(GetGoldenBooks(db));
            }
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(b => new
                {
                    b.Title
                })
                .OrderBy(b => b.Title)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.EditionType == EditionType.Gold)
                .Where(b => b.Copies < 5000)
                .Select(b => new
                {
                    b.BookId,
                    b.Title
                })
                .OrderBy(b => b.BookId)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }
    }
}
