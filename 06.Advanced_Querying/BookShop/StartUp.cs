using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BookShop.Models;
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

               Console.WriteLine(GetTotalProfitByCategory(db));
            }
        }


        //Problem 01 - 100%
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

        //Problem 02 - 100%
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

        //Problem 03 - 100%
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - ${b.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 04 - 100%
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
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

        //Problem 05 - 100%
        public static string GetBooksByCategory(BookShopContext context, string input)
        {

            string[] categories = input
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToArray();

            var books = context
                .Books
                .Where(b => b.BookCategories.Any(bc =>
                    categories.Contains(bc.Category.Name.ToLower())))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            return String.Join(Environment.NewLine, books);
        }

        //Problem 06 - 100%
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();

            DateTime dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", null);

            var books = context
                .Books
                .Where(b => b.ReleaseDate < dateTime)
                .Select(b => new
                {
                    b.ReleaseDate,
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .OrderByDescending(b => b.ReleaseDate)
                .ToList();
            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price:F2}");
            }

            return sb.ToString().TrimEnd();

        }

        //Problem 07 - 75%
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context
                .Authors
                .Where(a => a.FirstName.ToLower().EndsWith(input.ToLower()))
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName
                })
                .ToList();

            foreach (var a in authors)
            {
                sb.AppendLine($"{a.FirstName} {a.LastName}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 08 - 100%
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => new {b.Title})
                .OrderBy(b  => b.Title)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 09 - 100%
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(b => new
                {
                    b.BookId,
                    b.Title,
                    FullName = b.Author.FirstName +" "+ b.Author.LastName
                })
                .OrderBy(b => b.BookId)
                .ToList();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} ({b.FullName})");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 10 - 100%
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int booksCount = context
                .Books
                .Count(b => b.Title.Length > lengthCheck);

            return booksCount;
        }

        //Problem 11 - 100%
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var copies = context
                .Authors
                .Select(a => new
                {
                    Name = $"{a.FirstName} {a.LastName}",
                    BookCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.BookCopies)
                .ToList();

            foreach (var a in copies)
            {
                sb.AppendLine($"{a.Name} - {a.BookCopies}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 12 - 100%
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categories = context
                .Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    TotalProfit = c.CategoryBooks.Sum(b => b.Book.Copies * b.Book.Price)
                })
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c => c.CategoryName)
                .ToList();

            foreach (var c in categories)
            {
                sb.AppendLine($"{c.CategoryName} ${c.TotalProfit:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 13 - 100%
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categoriestWithThreeMostRecentBooks = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    BooksCount = c.CategoryBooks.Sum(bc => bc.Book.BookId),
                    Books = c.CategoryBooks.Select(bc => bc.Book).OrderByDescending(b => b.ReleaseDate).Take(3)
                })
                .OrderBy(c => c.CategoryName)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var cat in categoriestWithThreeMostRecentBooks)
            {
                sb.AppendLine($"--{cat.CategoryName}");

                foreach (Book book in cat.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().Trim();
        }

        //Problem 14 - 100%
        public static void IncreasePrices(BookShopContext context)
        {
            const int ReleaseDate = 2010;
            const decimal IncreaseValue = 5;

            List<Book> booksWhichPriceWillBeIncreased = context.Books
                .Where(b => b.ReleaseDate.Value.Year < ReleaseDate)
                .ToList();

            booksWhichPriceWillBeIncreased.ForEach(b => b.Price += IncreaseValue);

            context.SaveChanges();
        }

        //Problem 15 - 100%
        public static int RemoveBooks(BookShopContext context)
        {
            const int CopiesHighValue = 4200;

            Book[] booksToBeRemoved = context.Books
                .Where(b => b.Copies < CopiesHighValue)
                .ToArray();

            int booksToBeRemovedCount = booksToBeRemoved.Length;

            context.Books.RemoveRange(booksToBeRemoved);
            context.SaveChanges();

            return booksToBeRemovedCount;
        }
    }
}
