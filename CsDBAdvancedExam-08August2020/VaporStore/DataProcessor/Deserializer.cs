namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";

        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportGamesDTO[] gamesDTOs = JsonConvert.DeserializeObject<ImportGamesDTO[]>(jsonString);

            foreach (var gameDto in gamesDTOs)
            {
                Developer dev;
                Genre genre;

                if (!IsValid(gameDto) || !gameDto.Tags.Any())
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!context.Developers.Any(d => d.Name == gameDto.Developer))
                {
                    dev = new Developer() { Name = gameDto.Developer };
                    context.Developers.Add(dev);
                }
                else
                {
                    dev = context.Developers.First(d => d.Name == gameDto.Developer);
                }

                if (!context.Genres.Any(g => g.Name == gameDto.Genre))
                {
                    genre = new Genre() { Name = gameDto.Genre };
                    context.Genres.Add(genre);
                }
                else
                {
                    genre = context.Genres.First(g => g.Name == gameDto.Genre);
                }

                Game game = new Game()
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = DateTime.Parse(gameDto.ReleaseDate, CultureInfo.InvariantCulture, DateTimeStyles.None),
                    Developer = dev,
                    Genre = genre
                };

                foreach (var tagName in gameDto.Tags)
                {
                    Tag tag;

                    if (!context.Tags.Any(t => t.Name == tagName))
                    {
                        tag = new Tag() { Name = tagName };
                        context.Tags.Add(tag);
                    }
                    else
                    {
                        tag = context.Tags.First(t => t.Name == tagName);
                    }

                    GameTag gameTag = new GameTag()
                    {
                        Tag = tag,
                        Game = game
                    };

                    game.GameTags.Add(gameTag);
                }

                context.Games.Add(game);
                context.SaveChanges();
                sb.AppendLine($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags");
            }

            return sb.ToString().Trim();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            List<User> validUsers = new List<User>();

            ImportUserDTO[] userDTOs = JsonConvert.DeserializeObject<ImportUserDTO[]>(jsonString);

            foreach (var userDTO in userDTOs)
            {
                if (!IsValid(userDTO) || !userDTO.Cards.Any() || !userDTO.Cards.All(IsValid))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                User user = new User()
                {
                    FullName = userDTO.FullName,
                    Username = userDTO.Username,
                    Email = userDTO.Email,
                    Age = userDTO.Age,
                };

                foreach (var cardDto in userDTO.Cards)
                {
                    Card card = new Card()
                    {
                        Number = cardDto.Number,
                        Cvc = cardDto.Cvc,
                        Type = Enum.Parse<CardType>(cardDto.Type)
                    };

                    user.Cards.Add(card);
                }

                validUsers.Add(user);
                sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            List<Purchase> validP = new List<Purchase>();

            ImportPurchaseDTO[] pDtos = Deserialize<ImportPurchaseDTO[]>(xmlString, "Purchases");

            foreach (var p in pDtos)
            {
                if (!IsValid(p))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Game game = context.Games.First(x => x.Name == p.Title);
                Card card = context.Cards.First(x => x.Number == p.Card);

                Purchase purchase = new Purchase()
                {
                    Type = Enum.Parse<PurchaseType>(p.Type),
                    ProductKey = p.Key,
                    Date = DateTime.ParseExact(p.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    Card = card,
                    Game = game
                };

                validP.Add(purchase);
                sb.AppendLine($"Imported {purchase.Game.Name} for {purchase.Card.User.Username}");
            }

            context.Purchases.AddRange(validP);
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