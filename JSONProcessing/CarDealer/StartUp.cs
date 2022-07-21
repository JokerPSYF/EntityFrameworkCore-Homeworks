using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.Models;
using CarDealer.DTO;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CarDealer
{
    public class StartUp
    {
        private static string filePath;
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile(typeof(CarDealerProfile)));
            CarDealerContext dbContext = new CarDealerContext();

            InitializeDatasetFilePath("suppliers.json");
            string inputJson = File.ReadAllText(filePath);


            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();

            string json = ImportSuppliers(dbContext,filePath);
            Console.WriteLine(json);
           // File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Import the suppliers from the provided file suppliers.json. 
        /// Your method should return a string with the message
        /// $"Successfully imported {Suppliers.Count}.";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputJson"></param>
        /// <returns></returns>
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            ImportSuppliersDto[] sup = JsonConvert.DeserializeObject<ImportSuppliersDto[]>(inputJson);

            ICollection<Supplier> Suppliers = new List<Supplier>();
            foreach (ImportSuppliersDto sDto in sup)
            {
                if (!IsValid(sDto))
                {
                    continue;
                }

                Supplier suply = Mapper.Map<Supplier>(sDto);
                Suppliers.Add(suply);
            }

            context.Suppliers.AddRange(Suppliers);
            context.SaveChanges();

            return $"Successfully imported {Suppliers.Count}";
        }

        private static void InitializeDatasetFilePath(string fileName)
        {
            filePath =
                Path.Combine(Directory.GetCurrentDirectory(), "../../../Datasets/", fileName);
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}