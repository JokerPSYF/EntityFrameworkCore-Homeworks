namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            ImportPlaysDTO[] playsDTOs = Deserialize<ImportPlaysDTO[]>(xmlString, "Plays");

            ICollection<Play> validPlays = new List<Play>();

            foreach (var play in playsDTOs)
            {
                if (!IsValid(play))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                TimeSpan duration = TimeSpan.Parse(play.Duration);

                if (duration < new TimeSpan(1, 0, 0))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Genre plGenre;
                bool IsGenreValid = Enum.TryParse<Genre>(play.Genre, true, out plGenre);

                if (!IsGenreValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Play validPlay = new Play()
                {
                    Title = play.Title,
                    Duration = duration,
                    Rating = play.Rating,
                    Genre = plGenre,
                    Description = play.Genre,
                    Screenwriter = play.Screenwriter
                };

                validPlays.Add(validPlay);
                sb.AppendLine($"Successfully imported {play.Title} with genre {plGenre} and a rating of {play.Rating}!");
            }

            context.Plays.AddRange(validPlays);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            ImportCastDTO[] castDTOs = Deserialize<ImportCastDTO[]>(xmlString, "Casts");

            List<Cast> validCasts = new List<Cast>();

            foreach (var cast in castDTOs)
            {
                if (!IsValid(cast))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Cast validCast = new Cast()
                {
                    FullName = cast.FullName,
                    IsMainCharacter = cast.IsMainCharacter,
                    PhoneNumber = cast.PhoneNumber,
                    PlayId = cast.PlayId
                };
                validCasts.Add(validCast);
                string mainOrLesser = cast.IsMainCharacter ? "main" : "lesser";
                sb.AppendLine($"Successfully imported actor {cast.FullName} as a {mainOrLesser} character!");
            }

            context.Casts.AddRange(validCasts);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportTheatreDTO[] importTheatreDTOs = JsonConvert.DeserializeObject<ImportTheatreDTO[]>(jsonString);
            ICollection<Theatre> validTheatres = new List<Theatre>();

            foreach (var theatreDTO in importTheatreDTOs)
            {
                if (!IsValid(theatreDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Theatre validTheatre = new Theatre()
                {
                    Name = theatreDTO.Name,
                    NumberOfHalls = theatreDTO.NumberOfHalls,
                    Director = theatreDTO.Director
                };

                foreach (var ticket in theatreDTO.Tickets)
                {
                    if (!IsValid(ticket))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Ticket validTicket = new Ticket()
                    {
                        Price = ticket.Price,
                        RowNumber = ticket.RowNumber,
                        PlayId = ticket.PlayId
                    };
                    validTheatre.Tickets.Add(validTicket);
                }

                sb.AppendLine($"Successfully imported theatre {validTheatre.Name} with #{validTheatre.Tickets.Count} tickets!");
                validTheatres.Add(validTheatre);
            }

            context.Theatres.AddRange(validTheatres);
            context.SaveChanges();


            return sb.ToString().Trim();
        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }

        private static T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringReader reader = new StringReader(inputXml);
            T dtos = (T)xmlSerializer
                .Deserialize(reader);

            return dtos;
        }
    }
}
