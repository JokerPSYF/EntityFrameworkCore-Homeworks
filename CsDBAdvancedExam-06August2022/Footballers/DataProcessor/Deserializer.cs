namespace Footballers.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using System.Linq;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            List<Coach> listValidCoaches = new List<Coach>();

            ImportCoach[] coachesDTO = Deserialize<ImportCoach[]>(xmlString, "Coaches");

            foreach (var coachdto in coachesDTO)
            {
                if (!IsValid(coachdto) || String.IsNullOrEmpty(coachdto.Nationality))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Coach validCoach = new Coach()
                {
                    Name = coachdto.Name,
                    Nationality = coachdto.Nationality
                };

                foreach (var footballersdto in coachdto.Footballers)
                {
                    bool isStartDateValid =
                                  DateTime.TryParseExact(footballersdto.ContractStartDate, "dd/MM/yyyy",
                                  CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate);

                    bool isEndDateValid =
                        DateTime.TryParseExact(footballersdto.ContractEndDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate);

                    bool isDatesValid = isEndDateValid && isStartDateValid;

                    if (!IsValid(footballersdto) || (!isDatesValid))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (footballersdto.BestSkillType > 4)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (footballersdto.PositionType > 3)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;

                    }

                    if (startDate > endDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Footballer footballer = new Footballer()
                    {
                        Name = footballersdto.Name,
                        ContractStartDate = startDate,
                        ContractEndDate = endDate,
                        BestSkillType = (BestSkillType)footballersdto.BestSkillType,
                        PositionType = (PositionType)footballersdto.PositionType

                    };

                    validCoach.Footballers.Add(footballer);
                }
                listValidCoaches.Add(validCoach);
                sb.AppendLine($"Successfully imported coach - {validCoach.Name} with" +
                    $" {validCoach.Footballers.Count} footballers.");
            }

            context.Coaches.AddRange(listValidCoaches);
            context.SaveChanges();

            return sb.ToString().Trim();
        }
        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            List<Team> validTeams = new List<Team>();

            ImportTeamDTO[] tmDTO = JsonConvert.DeserializeObject<ImportTeamDTO[]>(jsonString);

            foreach (var tm in tmDTO)
            {
                if (!IsValid(tm))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (int.Parse(tm.Trophies) <= 0 || String.IsNullOrEmpty(tm.Nationality))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Team team = new Team()
                {
                    Name = tm.Name,
                    Nationality = tm.Nationality,
                    Trophies = int.Parse(tm.Trophies)
                };

                foreach (var foot in tm.Footballers.Distinct())
                {
                    if (!context.Footballers.Any(x => x.Id == foot))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    TeamFootballer tf = new TeamFootballer()
                    {
                        Team = team,
                        FootballerId = foot
                    };

                    team.TeamsFootballers.Add(tf);
                }
                validTeams.Add(team);
                sb.AppendLine($"Successfully imported team - {team.Name} with {team.TeamsFootballers.Count} footballers.");
            }

            context.Teams.AddRange(validTeams);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
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
