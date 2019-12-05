using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Cinema.DataProcessor.ExportDto;
using Newtonsoft.Json;

namespace Cinema.DataProcessor
{
    using System;
    using System.Text;
    using System.Xml;
    using Data;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context
                .Movies
                .Where(m => m.Rating >= rating && m.Projections.Any(p => p.Tickets.Count > 0))
                .OrderByDescending(m => m.Rating)
                .ThenByDescending(m => m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)))
                .Select(m => new MoviesExportDto
                {
                    MovieName = m.Title,
                    Rating = m.Rating.ToString("F2"),
                    TotalIncomes = m.Projections.Sum(p => p.Tickets.Sum(t => t.Price))
                        .ToString("F2"),
                    Customers = m.Projections
                        .SelectMany(p => p.Tickets)
                        .Select(t => new CustomersMoviesExportDto
                        {
                            FirstName = t.Customer.FirstName,
                            LastName = t.Customer.LastName,
                            Balance = t.Customer.Balance.ToString("F2")
                        }).OrderByDescending(t => t.Balance)
                        .ThenBy(t => t.FirstName)
                        .ThenBy(t => t.LastName)
                        .ToList()
                })
                .Take(10)
                .ToList();

            return JsonConvert.SerializeObject(movies, (Newtonsoft.Json.Formatting) Formatting.Indented);
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var customers = context
                .Customers
                .Where(c => c.Age >= age)
                .OrderByDescending(c => c.Tickets.Sum(t => t.Price))
                .Select(c => new ExportTopCustomersDto
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    SpentMoney = c.Tickets.Sum(t => t.Price).ToString("F2"),
                    SpentTime = TimeSpan.FromMilliseconds(c.Tickets
                            .Sum(t => t.Projection.Movie.Duration.TotalMilliseconds))
                        .ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture)
                })
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportTopCustomersDto[]),
                new XmlRootAttribute("Customers"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}