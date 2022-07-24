namespace CarDealer
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    using Data;
    using Dtos.Export;
    using Dtos.Import;
    using Models;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext dbContext = new CarDealerContext();
            //string xml = File.ReadAllText("../../../Datasets/sales.xml");

            string result = GetTotalSalesByCustomer(dbContext);
            File.WriteAllText("../../../Result/customers-total-sales", result);
            Console.WriteLine(result);
            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();

            //Console.WriteLine("Database reset successfully!");
        }



        /// <summary>
        /// Problem 9
        /// Import the suppliers from the provided file suppliers.xml. 
        ///Your method should return a string with the message
        ///$"Successfully imported {suppliers.Count}";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Suppliers");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSupplierDto[]), xmlRoot);

            using StringReader reader = new StringReader(inputXml);
            ImportSupplierDto[] supplierDtos = (ImportSupplierDto[])xmlSerializer
                .Deserialize(reader);

            Supplier[] suppliers = supplierDtos
                .Select(dto => new Supplier()
                {
                    Name = dto.Name,
                    IsImporter = dto.IsImporter
                })
                .ToArray();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";
        }

        /// <summary>
        ///  Problem 10
        /// Import the parts from the provided file parts.xml.
        /// If the supplierId doesn't exist, skip the record.
        /// Your method should return a string with the message
        /// $"Successfully imported {parts.Count}";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Parts");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartDto[]), xmlRoot);

            using StringReader reader = new StringReader(inputXml);
            ImportPartDto[] partDtos = (ImportPartDto[])xmlSerializer
                .Deserialize(reader);

            ICollection<Part> parts = new List<Part>();
            foreach (ImportPartDto pDto in partDtos)
            {
                if (!context.Suppliers.Any(s => s.Id == pDto.SupplierId))
                {
                    continue;
                }

                Part part = new Part()
                {
                    Name = pDto.Name,
                    Price = pDto.Price,
                    Quantity = pDto.Quantity,
                    SupplierId = pDto.SupplierId
                };
                parts.Add(part);
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}"; ;
        }

        /// <summary>
        ///  Problem 11
        /// Import the cars from the provided file cars.xml.
        /// Select unique car part ids. If the part id doesn't exist, skip the part record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Cars");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCarDto[]), xmlRoot);

            using StringReader reader = new StringReader(inputXml);
            ImportCarDto[] carDtos = (ImportCarDto[])xmlSerializer
                .Deserialize(reader);

            ICollection<Car> cars = new List<Car>();
            foreach (ImportCarDto cDto in carDtos)
            {
                Car car = new Car()
                {
                    Make = cDto.Make,
                    Model = cDto.Model,
                    TravelledDistance = cDto.TraveledDistance
                };

                ICollection<PartCar> currCarParts = new List<PartCar>();
                foreach (int partId in cDto.Parts.Select(p => p.Id).Distinct())
                {
                    if (!context.Parts.Any(p => p.Id == partId))
                    {
                        continue;
                    }

                    currCarParts.Add(new PartCar()
                    {
                        Car = car,
                        PartId = partId
                    });
                }

                car.PartCars = currCarParts;
                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        /// <summary>
        /// Problem 12
        /// Import the customers from the provided file customers.xml.
        /// Your method should return a string with the message
        /// $"Successfully imported {customers.Count}";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            string rootName = "Customers";
            ImportCustomerDto[] customerDtos = Deserialize<ImportCustomerDto[]>(inputXml, rootName);

            Customer[] customers = customerDtos
                .Select(dto => new Customer()
                {
                    Name = dto.Name,
                    BirthDate = DateTime.Parse(dto.Birthdate, CultureInfo.InvariantCulture),
                    IsYoungDriver = dto.IsYoungDriver
                })
                .ToArray();
            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}";
        }

        /// <summary>
        /// Problem 13
        /// Import the sales from the provided file sales.xml.
        /// If car doesn't exist, skip whole entity.
        /// Your method should return a string with the message 
        /// $"Successfully imported {sales.Count}";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            string rootName = "Sales";
            ImportSaleDto[] saleDtos = Deserialize<ImportSaleDto[]>(inputXml, rootName);

            ICollection<Sale> sales = new List<Sale>();
            foreach (ImportSaleDto sDto in saleDtos)
            {
                if (!context.Cars.Any(c => c.Id == sDto.CarId))
                {
                    continue;
                }

                Sale sale = new Sale()
                {
                    Discount = sDto.Discount,
                    CarId = sDto.CarId,
                    CustomerId = sDto.CustomerId
                };
                sales.Add(sale);
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        /// <summary>
        /// Problem 14
        /// Get all cars with a distance of more than 2,000,000.
        /// Order them by make, then by model alphabetically. Take top 10 records.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            ExportCarsWithDistanceDto[] carDtos = context
                .Cars
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .Select(c => new ExportCarsWithDistanceDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TravelledDistance
                })
                .ToArray();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("cars");
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarsWithDistanceDto[]), xmlRoot);

            using StringWriter writer = new StringWriter(sb);
            xmlSerializer.Serialize(writer, carDtos, namespaces);

            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Problem 15
        /// Get all cars from make BMW and order them by model
        /// alphabetically and by traveled distance descending.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            ExportBMWCarsDto[] bmwCars = context
                .Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new ExportBMWCarsDto()
                {
                    Id = c.Id,
                    Model = c.Model,
                    TraveledDistance = c.TravelledDistance
                })
                .ToArray();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("cars");
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportBMWCarsDto[]), xmlRoot);
            using StringWriter writer = new StringWriter(sb);

            xmlSerializer.Serialize(writer, bmwCars, namespaces);

            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Problem 16
        /// Get all cars from make BMW and order them by model
        /// alphabetically and by traveled distance descending.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            ExportLocalSuppliersDto[] localSuppliers = context
                .Suppliers
                .Where(s => !s.IsImporter)
                .Select(s => new ExportLocalSuppliersDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            return Serialize(localSuppliers, "suppliers");
        }

        /// <summary>
        /// Problem 18
        /// Get all cars along with their list of parts.
        /// For the car get only make, model and traveled
        /// distance and for the parts get only name and price 
        /// and sort all parts by price (descending).
        /// Sort all cars by traveled distance (descending)
        /// then by the model (ascending). Select top 5 records.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            ExportCarsWithPartsDto[] cars = context
                .Cars
                .Select(c => new ExportCarsWithPartsDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TravelledDistance,
                    Parts = c.PartCars
                        .Select(cp => new ExportCarPartsDto()
                        {
                            Name = cp.Part.Name,
                            Price = cp.Part.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                })
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToArray();

            return Serialize(cars, "cars");
        }

        /// <summary>
        /// Problem 19
        /// Get all customers that have bought at least 1 car and get their names,
        /// bought cars count. and total spent money on cars.
        /// Order the result list by total spent money descending.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            ExportCustomerSaleDTO[] customers = context
               .Customers
               .Where(c => c.Sales.Count >= 1)
               .ToArray()
               .Select(c => new ExportCustomerSaleDTO()
               {
                   FullName = c.Name,
                   BoughtCars = c.Sales.Count,
                   SpentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(p => p.Part.Price))
               })
               .OrderByDescending(c => c.SpentMoney)
               .ThenByDescending(c => c.BoughtCars)
               .ToArray();

            return Serialize(customers, "customers");
        }

        /// <summary>
        /// Problem 19
        /// Get all sales with information about the car,
        /// customer and price of the sale with and without discount.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            ExportSaleDto[] sales = context
                .Sales
                .Select(s => new ExportSaleDto()
                {
                    Car = new ExportSaleCarDto()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TravelledDistance
                    },
                    CustomerName = s.Customer.Name,
                    Discount = s.Discount.ToString("f0"),
                    Price = s.Car.PartCars.Sum(cp => cp.Part.Price),
                    PriceWithDiscount = (s.Car.PartCars.Sum(cp => cp.Part.Price) -
                                        (s.Car.PartCars.Sum(cp => cp.Part.Price) * (s.Discount / 100)))
                })
                .ToArray();

            return Serialize(sales, "sales");
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