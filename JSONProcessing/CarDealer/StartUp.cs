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

            InitializeOutputFilePath("sales-discounts.json");

            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();

            string json = GetSalesWithAppliedDiscount(dbContext);
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

        /// <summary>
        /// Problem 14
        /// Get all customers ordered by their birth date ascending. 
        /// If two customers are born on the same date first print 
        /// those who are not young drivers (e.g., print experienced drivers first).
        /// Export the list of customers to JSON in the format provided below.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            GetOrderedCustomersDTO[] customers = context
                .Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new GetOrderedCustomersDTO
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate,
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToArray();

            string json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        /// <summary>
        /// Problem 15
        /// Get all cars from making Toyota and order them by
        /// model alphabetically and by traveled distance descending.
        /// Export the list of cars to JSON in the format provided below.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            GetToyotaCarsDTO[] toyotaCars = context
                .Cars
                .Where(c => c.Make == "Toyota")
                .Select(c => new GetToyotaCarsDTO
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToArray();

            string json = JsonConvert.SerializeObject(toyotaCars, Formatting.Indented);

            return json;
        }

        /// <summary>
        /// Problem 16
        /// Get all suppliers that do not import parts from abroad.
        /// Get their id, name and the number of parts they can offer to supply.
        /// Export the list of suppliers to JSON in the format provided below.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            ExportLocalSuppliersDTO[] suppliers = context
                .Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new ExportLocalSuppliersDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            string json = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return json;
        }

        /// <summary>
        /// Problem 17
        /// Get all cars along with their list of parts. 
        /// For the car get only make, model and traveled
        /// distance and for the parts get only name and price
        /// (formatted to 2nd digit after the decimal point). 
        /// Export the list of cars and their parts to JSON in the format provided below.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            //var cars = context
            //    .Cars
            //    .Select(c => new
            //    {
            //        car = new
            //        {
            //            c.Make,
            //            c.Model,
            //            c.TravelledDistance
            //        },
            //        parts = c.PartCars.Select(pc => new
            //        {
            //            pc.Part.Name,
            //            Price = pc.Part.Price.ToString("F2")
            //        })
            //    })
            //    .ToArray();

            //ExportCarDTO[] cars = context
            //    .Cars
            //    .Select(c => new ExportCarDTO
            //    {
            //        Make = c.Make,
            //        Model = c.Model,
            //        TravelledDistance = c.TravelledDistance,
            //        Parts = c.PartCars
            //        .Select(pc => new GetPartsDTO()
            //        {
            //            Name = pc.Part.Name,
            //            Price = pc.Part.Price.ToString("f2")
            //        })
            //        .ToArray()
            //    }).ToArray();

            var cars = context.Cars
              .Select(c => new
              {
                  car = new
                  {
                      c.Make,
                      c.Model,
                      c.TravelledDistance
                  },
                  parts = c.PartCars.Select(pc => new
                  {
                      pc.Part.Name,
                      Price = pc.Part.Price.ToString("F2")
                  })
              })
              .ToArray();


            string json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }

        /// <summary>
        /// Prbolem 18
        /// Get all customers that have bought at least 1 car and get their names,
        /// bought cars count and total spent money on cars.
        /// Order the result list by total spent money descending
        /// then by total bought cars again in descending order.
        /// Export the list of customers to JSON in the format provided below.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context
                .Customers
                .Where(c => c.Sales.Count >= 1)
                .ToArray()
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(p => p.Part.Price))
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToArray();

            string json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        /// <summary>
        /// Problem 19
        /// Get first 10 sales with information about the car, 
        /// customer and price of the sale with and without discount. 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales.Select(s => new
            {
                car = new
                {
                    s.Car.Make,
                    s.Car.Model,
                    s.Car.TravelledDistance
                },
                customerName = s.Customer.Name,
                Discount = s.Discount.ToString("F2"),
                price = s.Car.PartCars.Sum(p => p.Part.Price).ToString("f2"),
                priceWithDiscount = (s.Car.PartCars.Sum(p => p.Part.Price)
                - s.Car.PartCars.Sum(p => p.Part.Price) * s.Discount / 100).ToString("F2")
            })
                .Take(10)
                .ToArray();

            return JsonConvert.SerializeObject(sales, Formatting.Indented);
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