using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MusicHub.Data.Models;
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
            throw new NotImplementedException();
        }

        public static string ImportSongs(MusicHubDbContext context, string xmlString)
        {
            throw new NotImplementedException();
        }

        public static string ImportSongPerformers(MusicHubDbContext context, string xmlString)
        {
            throw new NotImplementedException();
        }

        private static bool IsValid(Object obj)
        {
            var validator = new ValidationContext(obj); 

            var validationRes = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validator, validationRes, true);
        }
    }
}