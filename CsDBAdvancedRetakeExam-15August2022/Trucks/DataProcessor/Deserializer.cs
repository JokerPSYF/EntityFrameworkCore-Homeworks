namespace Trucks.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ImportDto;
    using System.Linq;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            List<Despatcher> validDesps = new List<Despatcher>();

            ImportDespatcherDTO[] despDTOs = Deserialize<ImportDespatcherDTO[]>(xmlString, "Despatchers");

            foreach (var dto in despDTOs)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Despatcher validD = new Despatcher()
                {
                    Name = dto.Name,
                    Position = dto.Position
                };

                foreach (var truckDTO in dto.Trucks)
                {
                    if (!IsValid(truckDTO))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Truck truck = new Truck()
                    {
                        RegistrationNumber = truckDTO.RegistrationNumber,
                        VinNumber = truckDTO.VinNumber,
                        TankCapacity = truckDTO.TankCapacity,
                        CargoCapacity = truckDTO.CargoCapacity,
                        CategoryType = (CategoryType)truckDTO.CategoryType,
                        MakeType = (MakeType)truckDTO.MakeType
                    };

                    validD.Trucks.Add(truck);
                }

                validDesps.Add(validD);
                sb.AppendLine($"Successfully imported despatcher - {validD.Name} with {validD.Trucks.Count} trucks.");
            }

            context.Despatchers.AddRange(validDesps);
            context.SaveChanges();

            return sb.ToString().Trim();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportClientDTO[] clientDTOs = JsonConvert.DeserializeObject<ImportClientDTO[]>(jsonString);

            List<Client> validClients = new List<Client>();

            foreach (var Cdto in clientDTOs)
            {
                if (!IsValid(Cdto) || Cdto.Type == "usual")
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Client validC = new Client()
                {
                    Name = Cdto.Name,
                    Nationality = Cdto.Nationality,
                    Type = Cdto.Type,
                };

                foreach (var truckId in Cdto.Trucks.Distinct())
                {
                    if (!context.Trucks.Any(x => x.Id == truckId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Truck truck = context.Trucks.First(x => x.Id == truckId);

                    ClientTruck clientTruck = new ClientTruck()
                    {
                        Client = validC,
                        Truck = truck 
                    };

                    validC.ClientsTrucks.Add(clientTruck);
                }
                context.Clients.Add(validC);
                context.SaveChanges();
                sb.AppendLine($"Successfully imported client - {validC.Name} with {validC.ClientsTrucks.Count} trucks.");
            }

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
