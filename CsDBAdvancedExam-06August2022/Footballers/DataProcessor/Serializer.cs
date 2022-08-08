namespace Footballers.DataProcessor
{
    using System;

    using Data;

    using System.Linq;

    using Formatting = Newtonsoft.Json.Formatting;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Xml.Serialization;
    using System.Text;
    using System.IO;
    using Footballers.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportCoachesWithTheirFootballers(FootballersContext context)
        {
            ExportCoachDTO[] coaches = context.Coaches
                .Include(c => c.Footballers)
                .ToArray()
                .Where(c => c.Footballers.Any())
                .Select(c => new ExportCoachDTO
                {
                    FootballersCount = c.Footballers.Count,
                    CoachName = c.Name,
                    Footballers = c.Footballers.OrderBy(f => f.Name)
                    .Select(f => new ExportFootballersOfCoachesDTO
                    {
                        Name = f.Name,
                        Position = f.PositionType.ToString()
                    })
                    .ToArray()
                })
                .OrderByDescending(c => c.FootballersCount)
                .ThenBy(c => c.CoachName)
                .ToArray();

            return Serialize(coaches, "Coaches");
        }

        public static string ExportTeamsWithMostFootballers(FootballersContext context, DateTime date)
        {
            var teamsFootballers = context.Teams.Distinct()
                .Include(x => x.TeamsFootballers)
                .ThenInclude(y => y.Footballer)
                .ToArray()
             .Where(t => t.TeamsFootballers.Any(x => x.Footballer.ContractStartDate >= date))
             .Select(x => new
             {
                 Name = x.Name,
                 Footballers = x.TeamsFootballers
                 .OrderByDescending(x => x.Footballer.ContractEndDate)
                 .ThenBy(x => x.Footballer.Name)
                 .Where(x => x.Footballer.ContractStartDate >= date)
                 .Select(f => new
                 {
                     FootballerName = f.Footballer.Name,
                     ContractStartDate = f.Footballer.ContractStartDate.ToString("MM/dd/yyyy"),
                     ContractEndDate = f.Footballer.ContractEndDate.ToString("MM/dd/yyyy"),
                     BestSkillType = f.Footballer.BestSkillType.ToString(),
                     PositionType = f.Footballer.PositionType.ToString(),
                 })
                 .ToArray()
             })
             .OrderByDescending(t => t.Footballers.Count())
             .ThenBy(t => t.Name)
             .Take(5)
             .ToArray();

            string json = JsonConvert.SerializeObject(teamsFootballers, Newtonsoft.Json.Formatting.Indented);
            return json;
        }

        private static string Serialize<T>(T dto, string rootName)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringWriter writer = new StringWriter(sb);
            xmlSerializer.Serialize(writer, dto, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
