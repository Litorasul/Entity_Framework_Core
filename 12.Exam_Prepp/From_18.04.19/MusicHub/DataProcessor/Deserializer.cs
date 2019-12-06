using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MusicHub.Data.Models;
using MusicHub.Data.Models.Enums;
using MusicHub.DataProcessor.ImportDtos;
using Newtonsoft.Json;

namespace MusicHub.DataProcessor
{
    using System;

    using Data;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data";

        private const string SuccessfullyImportedWriter 
            = "Imported {0}";
        private const string SuccessfullyImportedProducerWithPhone 
            = "Imported {0} with phone: {1} produces {2} albums";
        private const string SuccessfullyImportedProducerWithNoPhone
            = "Imported {0} with no phone number produces {1} albums";
        private const string SuccessfullyImportedSong 
            = "Imported {0} ({1} genre) with duration {2}";
        private const string SuccessfullyImportedPerformer
            = "Imported {0} ({1} songs)";

        public static string ImportWriters(MusicHubDbContext context, string jsonString)
        {
            var writers = JsonConvert.DeserializeObject<ImportWritersDto[]>(jsonString);
            var writersToAdd = new List<Writer>();
            StringBuilder sb = new StringBuilder();
           
            foreach (var dto in writers)
            {
                if (IsValid(dto))
                {
                    var writer = new Writer
                    {
                        Name = dto.Name,
                        Pseudonym = dto.Pseudonym
                    };

                    writersToAdd.Add(writer);
                    sb.AppendLine(string.Format(SuccessfullyImportedWriter, dto.Name));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }
            
            context.Writers.AddRange(writersToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProducersAlbums(MusicHubDbContext context, string jsonString)
        {
            var producers = JsonConvert.DeserializeObject<ImportProducersDto[]>(jsonString);

            var producersToAdd = new List<Producer>();

            StringBuilder sb = new StringBuilder();


            foreach (var dto in producers)
            {
                bool hasInvalidAlbum = dto.Albums.Any(a => IsValid(a) == false);

                if (IsValid(dto) && !hasInvalidAlbum)
                {
                    var producer = new Producer
                    {
                        Name = dto.Name,
                        Pseudonym = dto.Pseudonym,
                        PhoneNumber = dto.PhoneNumber
                    };
                    
                    
                    foreach (var albumDto in dto.Albums)
                    {
                        if (IsValid(albumDto))
                        {
                            var album = new Album
                            {
                                Name = albumDto.Name,
                                ReleaseDate = DateTime
                                    .ParseExact(albumDto.ReleaseDate, 
                                        "dd/MM/yyyy",  
                                        CultureInfo.InvariantCulture),
                                ProducerId = producer.Id
                            };

                            producer.Albums.Add(album);
                        }
                    }

                    producersToAdd.Add(producer);

                    if (producer.PhoneNumber == null)
                    {
                        sb.AppendLine(string.Format(SuccessfullyImportedProducerWithNoPhone
                            , producer.Name, producer.Albums.Count));

                    }
                    else
                    {
                        sb.AppendLine(string.Format(SuccessfullyImportedProducerWithPhone
                            , producer.Name, producer.PhoneNumber, producer.Albums.Count));
                    }
                        
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.Producers.AddRange(producersToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSongs(MusicHubDbContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportSongsDto[]),
                new XmlRootAttribute("Songs"));

            var songs = (ImportSongsDto[])xmlSerializer
                .Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();

            foreach (var dto in songs)
            {
                if (IsValid(dto) 
                    && ValidWriterId(context, dto.WriterId) 
                    && ValidAlbumId(context, dto.AlbumId)
                    && Enum.IsDefined(typeof(Genre), dto.Genre))
                {
                    var song = new Song
                    {
                        Name = dto.Name,
                        Duration = TimeSpan.ParseExact(dto.Duration,
                            "c", CultureInfo.InvariantCulture),
                        CreatedOn = DateTime.ParseExact(dto.CreatedOn, "dd/MM/yyyy"
                        , CultureInfo.InvariantCulture),
                        Genre = Enum.Parse<Genre>(dto.Genre),
                        AlbumId = dto.AlbumId,
                        WriterId = dto.WriterId,
                        Price = dto.Price
                    };

                    context.Songs.Add(song);
                    context.SaveChanges();
                    string durationResult = dto.Duration;
                    sb.AppendLine(string.Format(SuccessfullyImportedSong,
                    dto.Name, dto.Genre, durationResult));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }



        public static string ImportSongPerformers(MusicHubDbContext context, string xmlString)
        {
           var xmlSerializer = new XmlSerializer(typeof(ImportPerformerDto[]),
               new XmlRootAttribute("Performers"));

           var performers = (ImportPerformerDto[]) xmlSerializer
               .Deserialize(new StringReader(xmlString));

           StringBuilder sb = new StringBuilder();

           foreach (var dto in performers)
           {
               bool hasInvalidSong = dto.SongIds.Any(s => ValidSongId(context, s.Id) == false);

               if (IsValid(dto) && !hasInvalidSong)
               {
                   var performer = new Performer
                   {
                       FirstName = dto.FirstName,
                       LastName = dto.LastName,
                       Age = dto.Age,
                       NetWorth = dto.NetWorth
                   };
                   context.Performers.Add(performer);

                   foreach (var id in dto.SongIds)
                   {
                       var song = context.Songs
                           .First(s => s.Id == id.Id);
                       var performerSong = new SongPerformer
                       {
                           PerformerId = performer.Id,
                           SongId = id.Id
                       };
                       context.SongsPerformers.Add(performerSong);
                   }

                   sb.AppendLine(string.Format(SuccessfullyImportedPerformer
                       , dto.FirstName, performer.PerformerSongs.Count));
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

        private static bool ValidWriterId(MusicHubDbContext context, int dtoWriterId)
        {
            return context.Writers.Any(w => w.Id == dtoWriterId);
        }

        private static bool ValidAlbumId(MusicHubDbContext context, int? dtoAlbumId)
        {
            return context.Albums.Any(a => a.Id == dtoAlbumId);
        }

        private static bool ValidSongId(MusicHubDbContext context, int songId)
        {
            return context.Songs.Any(s => s.Id == songId);
        }

    }
}