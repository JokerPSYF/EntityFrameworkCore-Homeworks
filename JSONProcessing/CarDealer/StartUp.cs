using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

using CarDealer.Data;
using CarDealer.Models;
using CarDealer.DTO.Suppliers;
using CarDealer.DTO.Parts;
using System.Linq;
using CarDealer.DTO.Cars;
using CarDealer.DTO.Customers;
using CarDealer.DTO.Sales;

namespace CarDealer
{
    public class StartUp
    {
        private static string filePath;
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile(typeof(CarDealerProfile)));
            CarDealerContext dbContext = new CarDealerContext();

            // InitializeDatasetFilePath("sales.json");
            //string inputJson = File.ReadAllText(filePath);

            InitializeOutputFilePath("ordered-customers.json");

            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();

            string json = GetOrderedCustomers(dbContext);
            Console.WriteLine(json);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Problem 9
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

            return $"Successfully imported {Suppliers.Count}.";
        }

        /// <summary>
        /// Problem 10
        /// Import the suppliers from the provided file suppliers.json. 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputJson"></param>
        /// <returns></returns>
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            ImportPartDTO[] sup = JsonConvert.DeserializeObject<ImportPartDTO[]>(inputJson);

            int[] suplierIds = context.Suppliers.Select(s => s.Id).ToArray();

            ICollection<Part> parts = new List<Part>();
            foreach (ImportPartDTO pDto in sup)
            {
                if (!(IsValid(pDto)))
                {
                    continue;
                }
                if (!suplierIds.Contains(pDto.SupplierId))
                {
                    continue;
                }

                Part part = Mapper.Map<Part>(pDto);
                parts.Add(part);
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }

        /// <summary>
        /// Problem 11
        /// Import the customers from the provided file customers.json.
        ///Your method should return a string with the message
        ///$"Successfully imported {Customers.Count}.";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputJson"></param>
        /// <returns></returns>
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            ImportCarDTO[] carsJson = JsonConvert.DeserializeObject<ImportCarDTO[]>(inputJson);


            ICollection<Car> cars = new List<Car>();
            foreach (var cDto in carsJson)
            {
                Car car = new Car
                {
                    Make = cDto.Make,
                    Model = cDto.Model,
                    TravelledDistance = cDto.TravelledDistance
                };

                foreach (var partId in cDto.PartId.Distinct())
                {
                    car.PartCars.Add(new PartCar
                    {
                        PartId = partId
                    });
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        /// <summary>
        /// Problem 12
        /// Import the customers from the provided file customers.json.
        ///Your method should return a string with the message
        ///$"Successfully imported {Customers.Count}.";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputJson"></param>
        /// <returns></returns>
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            ImportCustomersDTO[] cust = JsonConvert.DeserializeObject<ImportCustomersDTO[]>(inputJson);

            ICollection<Customer> customers = new List<Customer>();
            foreach (ImportCustomersDTO cDto in cust)
            {
                if (!(IsValid(cDto)))
                {
                    continue;
                }

                Customer customer = Mapper.Map<Customer>(cDto);
                customers.Add(customer);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        /// <summary>
        /// Problem 13
        /// Import the sales from the provided file sales.json.
        /// Your method should return a string with the message 
        /// $"Successfully imported {Sales.Count}.";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputJson"></param>
        /// <returns></returns>
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            ImportSaleDTO[] saleDTOs = JsonConvert.DeserializeObject<ImportSaleDTO[]>(inputJson);

            ICollection<Sale> sales = new List<Sale>();
            foreach (ImportSaleDTO sDto in saleDTOs)
            {
                if (!(IsValid(sDto)))
                {
                    continue;
                }

                Sale sale = Mapper.Map<Sale>(sDto);
                sales.Add(sale);
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context
                .Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToArray();

            string json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        //Usable methods
        private static void InitializeDatasetFilePath(string fileName)
        {
            filePath =
                Path.Combine(Directory.GetCurrentDirectory(), "../../../Datasets/", fileName);
        }

        private static void InitializeOutputFilePath(string fileName)
        {
            filePath =
                Path.Combine(Directory.GetCurrentDirectory(), "../../../Results/", fileName);
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