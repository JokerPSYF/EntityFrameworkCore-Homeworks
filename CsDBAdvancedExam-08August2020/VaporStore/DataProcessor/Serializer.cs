namespace VaporStore.DataProcessor
{
	using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
			var gbg = context.Genres
				.ToArray()
				.Where(x =>  genreNames.Contains(x.Name))
				.Select(y => new
                {
					Id = y.Id,
					Genre = y.Name,
					Games = y.Games
						.Where(x => x.Purchases.Any())
						.Select(z => new
                        {
							Id = z.Id,
							Title = z.Name,
							Developer = z.Developer.Name,
							Tags = String.Join(", ", z.GameTags.Select(a => a.Tag.Name)),
							Players = z.Purchases.Count
                        })
						.OrderByDescending(x => x.Players)
						.ThenBy(y => y.Id)
						.ToArray(),
					TotalPlayers = y.Games.Where(x => x.Purchases.Any()).Sum(z => z.Purchases.Count)
                })
				.OrderByDescending(x => x.TotalPlayers)
				.ThenBy(y => y.Id)
				.ToArray();

			return JsonConvert.SerializeObject(gbg, Formatting.Indented);
		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
			ExportUserDTO[] users = context.Users
				.Include(x => x.Cards)
				.ToArray()
				.Where(x => x.Cards.Any(y => y.Purchases.Any(z => z.Type.ToString() == storeType)))
				.Select(u => new ExportUserDTO
                {
					Username = u.Username,
					Purchases = u.Cards
						.SelectMany(x => x.Purchases).Where(p => p.Type.ToString() == storeType)
						.Select(c => new ExortPurchaseDTO
						{
							Card = c.Card.Number,
							Cvc = c.Card.Cvc,
							Date = c.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
							Game = new ExportGameDTO
                            {
								Title = c.Game.Name,
								Genre = c.Game.Genre.Name,
								Price = c.Game.Price
                            }
						})
						.OrderBy(p => p.Date)
						.ToArray(),
					TotalSpent = u.Cards
					.Sum(c => c.Purchases
						.Where(p => p.Type.ToString() == storeType)
							.Sum(p => p.Game.Price))
				})
				.OrderByDescending(u => u.TotalSpent)
				.ThenBy(u => u.Username)
				.ToArray();


			return Serialize(users, "Users");
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