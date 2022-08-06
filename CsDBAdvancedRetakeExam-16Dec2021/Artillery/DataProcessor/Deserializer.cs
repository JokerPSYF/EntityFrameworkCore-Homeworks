namespace Artillery.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Artillery.Data;
    using Artillery.Data.Models;
    using Artillery.DataProcessor.ImportDto;
    using System.Linq;
    using Newtonsoft.Json;
    using Artillery.Data.Models.Enums;

    public class Deserializer
    {
        private const string ErrorMessage =
                "Invalid data.";
        private const string SuccessfulImportCountry =
            "Successfully import {0} with {1} army personnel.";
        private const string SuccessfulImportManufacturer =
            "Successfully import manufacturer {0} founded in {1}.";
        private const string SuccessfulImportShell =
            "Successfully import shell caliber #{0} weight {1} kg.";
        private const string SuccessfulImportGun =
            "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

        public static string ImportCountries(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            ImportCountriesDTO[] countriesDTOs = Deserialize<ImportCountriesDTO[]>(xmlString, "Countries");

            List<Country> validCaounries = new List<Country>();

            foreach (var countryDto in countriesDTOs)
            {
                if (!IsValid(countryDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Country validCountry = new Country()
                {
                    CountryName = countryDto.CountryName,
                    ArmySize = countryDto.ArmySize
                };

                validCaounries.Add(validCountry);
                sb.AppendLine($"Successfully import {validCountry.CountryName} with {validCountry.ArmySize} army personnel.");
            }

            context.AddRange(validCaounries);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportManufacturers(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            ImportManufacturerDTO[] manufacturersDTOs = Deserialize<ImportManufacturerDTO[]>(xmlString, "Manufacturers"); 
            List<Manufacturer> validManufacturers = new List<Manufacturer>();

            foreach (var manufDto in manufacturersDTOs )
            {
                if (!IsValid(manufDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (validManufacturers.Any(x => x.ManufacturerName == manufDto.ManufacturerName))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Manufacturer validManufacturer = new Manufacturer()
                {
                    ManufacturerName = manufDto.ManufacturerName,
                    Founded = manufDto.Founded
                };

                validManufacturers.Add(validManufacturer);

                string[] place = validManufacturer.Founded.Split(", ");

                sb.AppendLine
                    ($"Successfully import manufacturer {validManufacturer.ManufacturerName}" +
                    $" founded in {place[place.Length - 2 ]}, {place[place.Length - 1]}.");
            }

            context.AddRange(validManufacturers);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            ImportShellsDTO[] shellsDto = Deserialize<ImportShellsDTO[]>(xmlString, "Shells");
            List<Shell> listWithValidShells = new List<Shell>();

            foreach (var shell in shellsDto)
            {
                if (!IsValid(shell))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Shell validShell = new Shell()
                {
                    ShellWeight = shell.ShellWeight,
                    Caliber = shell.Caliber
                };

                listWithValidShells.Add(validShell);
                sb.AppendLine($"Successfully import shell caliber #{validShell.Caliber} weight {validShell.ShellWeight} kg.");
            }

            context.AddRange(listWithValidShells);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportGunsDTO[] gunsDTOs = JsonConvert.DeserializeObject<ImportGunsDTO[]>(jsonString);

            List<Gun> listOfValidGuns = new List<Gun>();

            foreach (var gundto in gunsDTOs)
            {
                GunType gunType;
                bool IsGunTypeValid = Enum.TryParse<GunType>(gundto.GunType, true, out gunType);

                if (!IsValid(gundto) || !IsGunTypeValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Gun gun = new Gun()
                {
                    ManufacturerId = gundto.ManufacturerId,
                    GunWeight = gundto.GunWeight,
                    BarrelLength = gundto.BarrelLength,
                    NumberBuild = gundto.NumberBuild,
                    Range = gundto.Range,
                    GunType = gunType,
                    ShellId = gundto.ShellId,
                };

                foreach (var countryId in gundto.Countries)
                {

                    CountryGun countryGun = new CountryGun()
                    {
                        CountryId = countryId.Id,
                        Gun = gun
                    };

                    gun.CountriesGuns.Add(countryGun);
                }

                listOfValidGuns.Add(gun);
                sb.AppendLine($"Successfully import gun {gunType} with a total weight of {gun.GunWeight} kg. and barrel length of {gun.BarrelLength} m.");
            }

            context.Guns.AddRange(listOfValidGuns);
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
