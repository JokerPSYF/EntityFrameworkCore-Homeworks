namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportDepartmentCellDto[] dcDTO = JsonConvert.DeserializeObject<ImportDepartmentCellDto[]>(jsonString);
            ICollection<Department> validDeps = new List<Department>();

            foreach (var dep in dcDTO)
            {
                if (!IsValid(dep) || !dep.Cells.Any() || dep.Cells.Any(c => !IsValid(c)))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                Department department = new Department()
                {
                    Name = dep.Name
                };

                foreach (var cell in dep.Cells)
                {
                    Cell currCell = new Cell()
                    {
                        CellNumber = cell.CeilNumber,
                        HasWindow = cell.HasWindow
                    };
                    department.Cells.Add(currCell);
                }

                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");

                validDeps.Add(department);
            }

            context.Departments.AddRange(validDeps);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportPrisonerDTO[] prDto = JsonConvert.DeserializeObject<ImportPrisonerDTO[]>(jsonString);
            ICollection<Prisoner> validPrisoners = new List<Prisoner>();

            foreach (ImportPrisonerDTO pr in prDto)
            {

                bool isIncarcerationDateValid =
                                   DateTime.TryParseExact(pr.IncarcerationDate, "dd/MM/yyyy",
                                   CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime incarcerationDate);

                if (!IsValid(pr) || !isIncarcerationDateValid)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                if (pr.Mails.Any(m => !IsValid(m)))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                DateTime? releaseDate = null;
                if (!String.IsNullOrEmpty(pr.ReleaseDate))
                {
                    bool isReleaseDateValid =
                                  DateTime.TryParseExact(pr.ReleaseDate, "dd/MM/yyyy",
                                  CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime releaseDateValue);

                    if (!isReleaseDateValid)
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }
                    releaseDate = releaseDateValue;
                }


                Prisoner prisoner = new Prisoner()
                {
                    FullName = pr.FullName,
                    Nickname = pr.Nickname,
                    Age = pr.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = releaseDate,
                    Bail = pr.Bail,
                    CellId = pr.CellId
                };

                foreach (var mail in pr.Mails)
                {
                    Mail currMail = new Mail()
                    {
                        Description = mail.Description,
                        Sender = mail.Sender,
                        Address = mail.Address
                    };
                    prisoner.Mails.Add(currMail);
                }

                validPrisoners.Add(prisoner);

                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }

            context.Prisoners.AddRange(validPrisoners);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            string xmlRoot = "Officers";

            ImportOfficersDTO[] offDto = Deserialize<ImportOfficersDTO[]>(xmlString, xmlRoot);

            ICollection<Officer> validOfficers = new List<Officer>();

            foreach (var officerDTO in offDto)
            {
                if (!IsValid(officerDTO))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Weapon offWeapon;
                var isValidWeapon = Enum.TryParse<Weapon>(officerDTO.Weapon, true, out offWeapon);

                Position offPosition;
                var isValidPosition = Enum.TryParse<Position>(officerDTO.Position, true, out offPosition);

                if (!isValidPosition || !isValidWeapon)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Officer officer = new Officer()
                {
                    FullName = officerDTO.Name,
                    Salary = officerDTO.Money,
                    Position = offPosition,
                    Weapon = offWeapon,
                    DepartmentId = officerDTO.DepartmentId,
                };

                foreach (var pri in officerDTO.Prisoners)
                {
                    OfficerPrisoner op = new OfficerPrisoner()
                    {
                        Officer = officer,
                        PrisonerId = pri.Id
                    };

                    officer.OfficerPrisoners.Add(op);
                }

                validOfficers.Add(officer);
                sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
            }

            context.Officers.AddRange(validOfficers);
            context.SaveChanges();


            return sb.ToString().Trim();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }

        //Helper methods
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