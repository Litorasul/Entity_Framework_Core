using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Cinema.Data.Models;
using Cinema.DataProcessor.ImportDto;
using Newtonsoft.Json;

namespace Cinema.DataProcessor
{
    using System;

    using Data;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie 
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat 
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection 
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket 
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var movies = JsonConvert.DeserializeObject<MoviesImportDto[]>(jsonString);
            var moviesToAdd = new List<Movie>();
            var sb = new StringBuilder();

            foreach (var dto in movies)
            {
                if (IsValid(dto))
                {
                    var movie = new Movie
                    {
                        Title = dto.Title,
                        Genre = dto.Genre,
                        Duration = dto.Duration,
                        Rating = dto.Rating,
                        Director = dto.Director
                    };

                    moviesToAdd.Add(movie);
                    sb.AppendLine(string.Format(SuccessfulImportMovie, movie.Title, movie.Genre, movie.Rating.ToString("F2")));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }
            context.Movies.AddRange(moviesToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var halls = JsonConvert.DeserializeObject<HallImportDto[]>(jsonString);
            
            StringBuilder sb = new StringBuilder();

            foreach (var dto in halls)
            {
                if (IsValid(dto))
                {
                    var hall = new Hall
                    {
                        Name = dto.Name,
                        Is4Dx = dto.Is4Dx,
                        Is3D = dto.Is3D
                    };

                    context.Halls.Add(hall);

                    AddSeatsInDb(context, hall.Id, dto.Seats);

                    string projectionType = GetProjectionType(hall);

                    sb.AppendLine
                        (string.Format(SuccessfulImportHallSeat, dto.Name, projectionType, dto.Seats));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }


        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ProjectionImportDto[])
            , new XmlRootAttribute("Projections"));

            var projections = (ProjectionImportDto[])xmlSerializer
                .Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();

            foreach (var dto in projections)
            {
                if (IsValid(dto) 
                    && IsValidMovieId(context, dto.MovieId)
                    && IsValidHallId(context, dto.HallId))
                {
                    var projection = new Projection
                    {
                        MovieId = dto.MovieId,
                        HallId = dto.HallId,
                        DateTime = DateTime.ParseExact
                            (dto.DateTime,
                            "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                    };

                    context.Projections.Add(projection);
                    context.SaveChanges();
                    string dateTimeResult = projection.DateTime
                        .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                    sb.AppendLine(string.Format(SuccessfulImportProjection,
                        projection.Movie.Title,
                        dateTimeResult));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

     


        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(CustomerImportDto[])
                , new XmlRootAttribute("Customers"));

            var customers = (CustomerImportDto[])xmlSerializer
                .Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();

            foreach (var dto in customers)
            {
                
                if (IsValid(dto))
                {
                    var customer = new Customer
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        Age = dto.Age,
                        Balance = dto.Balance
                    };

                    context.Customers.Add(customer);
                    AddAllTickets(context, customer.Id, dto.Tickets);

                    sb.AppendLine(string.Format(SuccessfulImportCustomerTicket,
                        dto.FirstName, dto.LastName, dto.Tickets.Length));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

    

        private static bool IsValid(Object obj)
        {
            var validator = new ValidationContext(obj);

            var validationRes = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validator, validationRes, true);
        }

        private static void AddSeatsInDb(CinemaContext context, int hallId, int seatCount)
        {
            var seats = new List<Seat>();
            for (int i = 0; i < seatCount; i++)
            {
                var seat = new Seat
                {
                    HallId = hallId
                };

                seats.Add(seat);
            }

            context.AddRange(seats);
            context.SaveChanges();
        }

        private static string GetProjectionType(Hall hall)
        {
            string result = "Normal";

            if (hall.Is4Dx && hall.Is3D)
            {
                result = "4Dx/3D";
            }
            else if (hall.Is4Dx && !hall.Is3D)
            {
                result = "4Dx";
            }
            else if (!hall.Is4Dx && hall.Is3D)
            {
                result = "3D";
            }

            return result;
        }

        private static bool IsValidMovieId(CinemaContext context, int movieId)
        {
            return context.Movies.Any(m => m.Id == movieId);
        }

        private static bool IsValidHallId(CinemaContext context, int hallId)
        {
            return context.Halls.Any(h => h.Id == hallId);
        }

        private static void AddAllTickets
            (CinemaContext context, int customerId, TicketCustomerImportDto[] tickets)
        {
            var ticketsToAdd = new List<Ticket>();

            foreach (var dto in tickets)
            {
                //TODO validate ProjectionId in Tickets
                if (IsValid(dto))
                {
                    var ticket = new Ticket
                    {
                        ProjectionId = dto.ProjectionId,
                        CustomerId = customerId,
                        Price = dto.Price
                    };
                    ticketsToAdd.Add(ticket);
                }
            }

            context.Tickets.AddRange(ticketsToAdd);
            context.SaveChanges();
        }
    }
}