namespace Theatre.DataProcessor
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Theatre.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var theaters = context.Theatres
                .Where(x => x.NumberOfHalls >= numbersOfHalls && x.Tickets.Count >= 20)
                .Select(x => new
                {
                    Name = x.Name,
                    Halls = x.NumberOfHalls,
                    TotalIncome = decimal.Parse(x.Tickets.Where(y => y.RowNumber >= 1 && y.RowNumber <= 5).Sum(z => z.Price).ToString("F2")),
                    Tickets = x.Tickets.Select(y => new
                    {
                        Price = y.Price,
                        RowNumber = y.RowNumber
                    })
                    .Where(t => t.RowNumber >= 1 && t.RowNumber <= 5)
                    .OrderByDescending(x => x.Price)
                    .ToList()
                })
                .OrderByDescending(x => x.Halls)
                .ThenBy(x => x.Name)
                .ToList();

            string json = JsonConvert.SerializeObject(theaters, Newtonsoft.Json.Formatting.Indented);

            return json;
        }

        public static string ExportPlays(TheatreContext context, double rating)
        {

            ExportPlayDTO[] plays = context.Plays
                .Include(x => x.Casts)
                .ToArray()
                .Where(p => p.Rating <= rating)
                .Select(x => new ExportPlayDTO
                {
                    Title = x.Title,
                    Duration = x.Duration.ToString("c"), //WARNING
                    Rating = x.Rating == 0 ? "Premier" : x.Rating.ToString(),
                    Genre = x.Genre.ToString(),
                    Actors = x.Casts
                    .Where(ac => ac.IsMainCharacter == true)
                    .Select(a => new ExportActorDTO
                    {
                        FullName = a.FullName,
                        MainCharacter = $"Plays main character in '{x.Title}'."
                    })
                    .OrderByDescending(b => b.FullName)
                    .ToArray()
                })
                .OrderBy(a => a.Title)
                .ThenByDescending(b => b.Genre)
                .ToArray();



            return Serialize(plays, "Plays");
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
