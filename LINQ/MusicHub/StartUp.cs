namespace MusicHub
{
    using System;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            //DbInitializer.ResetDatabase(context);

            //Test your solutions here
            //Console.WriteLine(ExportAlbumsInfo(context, 9));

            //Console.WriteLine(ExportSongsAboveDuration(context, 4));

        }

        /// <summary>
        /// Export all albums which are produced by the provided Producer Id.
        /// For each Album, get the Name, Release date in format "MM/dd/yyyy",
        /// Producer Name, the Album Songs with each Song Name,
        /// Price (formatted to the second digit) and the Song Writer Name.
        /// Sort the Songs by Song Name (descending) and by Writer (ascending).
        /// At the end export the Total Album Price with exactly two digits after the decimal place.
        /// Sort the Albums by their Total Price (descending).
        /// </summary>
        /// <param name="context"></param>
        /// <param name="producerId"></param>
        /// <returns></returns>
        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            var albums = context
                .Albums
                .Include(a => a.Producer)
                .Include(a => a.Songs)
                .ThenInclude(s => s.Writer)
                .ToArray()
                .Where(a => a.ProducerId == producerId)
                .Select(a => new
                {
                    a.Name,
                    a.Price,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                    ProducerName = a.Producer.Name,     
                    AlbumSongs = a.Songs
                    .Select(s => new
                    {
                        s.Name,
                        s.Price,
                        SongWriterName = s.Writer.Name
                    })
                    .OrderByDescending(s => s.Name)
                    .ThenBy(s => s.SongWriterName)
                })
                .OrderByDescending(a => a.Price)
                .ToArray();

            foreach (var a in albums)
            {
                sb.AppendLine($"-AlbumName: {a.Name}");
                sb.AppendLine($"-ReleaseDate: {a.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {a.ProducerName}");
                sb.AppendLine($"-Songs:");

                int songCount = 0;
                foreach (var s in a.AlbumSongs)
                {
                    sb.AppendLine($"---#{++songCount}");
                    sb.AppendLine($"---SongName: {s.Name}");
                    sb.AppendLine($"---Price: {s.Price:f2}");
                    sb.AppendLine($"---Writer: {s.SongWriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {a.Price:f2}");
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        ///  Export the songs which are above the given duration.
        ///  For each Song, export its Name, Performer Full Name, 
        ///  Writer Name, Album Producer and Duration (in format("c")).
        ///  Sort the Songs by their Name (ascending), 
        ///  by Writer (ascending) and by Performer (ascending).
        /// </summary>
        /// <param name="context"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            var songs = context
                .Songs
                .Include(s => s.SongPerformers)
                .ThenInclude(sp => sp.Performer)
                .Include(s => s.Writer)
                .Include(s => s.Album)
                .ThenInclude(a => a.Producer)
                .ToArray()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    s.Name,
                    PerformerName = s.SongPerformers.Select(sp => $"{sp.Performer.FirstName} {sp.Performer.LastName}")
                    .FirstOrDefault(),
                    WriterName = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                })
                .OrderBy(s => s.Name)
                .ThenBy(s => s.WriterName)
                .ThenBy(s => s.PerformerName)
                .ToArray();

            int songCount = 0;
            foreach (var s in songs)
            {
                sb.AppendLine($"-Song #{++songCount}");
                sb.AppendLine($"---SongName: {s.Name}");
                sb.AppendLine($"---Writer: {s.WriterName}");
                sb.AppendLine($"---Performer: {s.PerformerName}");
                sb.AppendLine($"---AlbumProducer: {s.AlbumProducer}");
                sb.AppendLine($"---Duration: {s.Duration}");
            }

            return sb.ToString().Trim();
        }
    }
}
