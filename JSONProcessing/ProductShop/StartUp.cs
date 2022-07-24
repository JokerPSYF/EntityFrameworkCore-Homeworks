namespace ProductShop
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System;

    using Data;
    using DTOs.Category;
    using DTOs.CategoryProduct;
    using DTOs.Product;
    using DTOs.User;
    using Models;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Newtonsoft.Json;

    public class StartUp
    {
        private static string filePath;
        // JSON
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile(typeof(ProductShopProfile)));

            ProductShopContext dbContext = new ProductShopContext();
            InitializeOutputFilePath("users-and-products.json");

            //InitializeDatasetFilePath("categories-by-products.json");
            //string inputJson = File.ReadAllText(filePath);

           // dbContext.Database.EnsureDeleted();
           // dbContext.Database.EnsureCreated();

            Console.WriteLine($"Database copy was created!");

            string json = GetUsersWithProducts(dbContext);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Import the users from the provided file users.json.
        /// Your method should return a string with the message
        /// $"Successfully imported {Users.Count}";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputJson"></param>
        /// <returns></returns>
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            ImportUserDto[] userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

            ICollection<User> validUsers = new List<User>();
            foreach (ImportUserDto uDto in userDtos)
            {
                if (!IsValid(uDto))
                {
                    continue;
                }

                User user = Mapper.Map<User>(uDto);
                validUsers.Add(user);
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            return $"Successfully imported {validUsers.Count}";
        }

        /// <summary>
        /// Import the users from the provided file products.json.
        /// Your method should return a string with the message 
        /// $"Successfully imported {Products.Count}";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputJson"></param>
        /// <returns></returns>
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            ImportProductDto[] productDtos = JsonConvert
                .DeserializeObject<ImportProductDto[]>(inputJson);

            ICollection<Product> validProducts = new List<Product>();
            foreach (ImportProductDto pDto in productDtos)
            {
                if (!IsValid(pDto))
                {
                    continue;
                }

                Product product = Mapper.Map<Product>(pDto);
                validProducts.Add(product);
            }

            context.Products.AddRange(validProducts);
            context.SaveChanges();

            return $"Successfully imported {validProducts.Count}";
        }

        /// <summary>
        /// Import the users from the provided file categories.json.
        /// Some of the names will be null, so you don't have to add them to the database.
        /// Just skip the record and continue.
        /// Your method should return a string with the message
        /// $"Successfully imported {Categories.Count}";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputJson"></param>
        /// <returns></returns>
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            ImportCategoryDto[] categoryDtos = JsonConvert
                .DeserializeObject<ImportCategoryDto[]>(inputJson);

            ICollection<Category> validCategories = new List<Category>();
            foreach (ImportCategoryDto cDto in categoryDtos)
            {
                if (!IsValid(cDto))
                {
                    continue;
                }

                Category category = Mapper.Map<Category>(cDto);
                validCategories.Add(category);
            }

            context.Categories.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        /// <summary>
        /// Import the users from the provided file categories-products.json. 
        /// Your method should return a string with the message $"Successfully 
        /// imported {CategoryProducts.Count}";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputJson"></param>
        /// <returns></returns>
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            ImportCategoryProductDto[] categoryProductDtos = JsonConvert
                .DeserializeObject<ImportCategoryProductDto[]>(inputJson);

            ICollection<CategoryProduct> validCp = new List<CategoryProduct>();
            foreach (ImportCategoryProductDto cpDto in categoryProductDtos)
            {
                CategoryProduct categoryProduct = Mapper.Map<CategoryProduct>(cpDto);
                validCp.Add(categoryProduct);
            }

            context.CategoryProducts.AddRange(validCp);
            context.SaveChanges();

            return $"Successfully imported {validCp.Count}";
        }

        /// <summary>
        /// Get all products in a specified price range:  500 to 1000 (inclusive).
        /// Order them by price (from lowest to highest).
        /// Select only the product name, price and the full
        /// name of the seller. Export the result to JSON.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetProductsInRange(ProductShopContext context)
        {
            ExportProductsInRangeDto[] products = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .ProjectTo<ExportProductsInRangeDto>()
                .ToArray();

            string json = JsonConvert.SerializeObject(products, Formatting.Indented);
            return json;
        }

        /// <summary>
        /// Get all users who have at least 1 sold item with a buyer.
        /// Order them by the last name, then by the first name.
        /// Select the person's first and last name.
        /// For each of the sold products (products with buyers),
        /// select the product's name, price and the buyer's first and last name.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetSoldProducts(ProductShopContext context)
        {
            ExportUsersWithSoldProductsDto[] users = context
                .Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ProjectTo<ExportUsersWithSoldProductsDto>()
                .ToArray();

            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            return json;
        }

        /// <summary>
        /// Get all categories. Order them in descending order by
        /// the category's products count. For each category select its name,
        /// the number of products, the average price of those products 
        /// (rounded to the second digit after the decimal separator) 
        /// and the total revenue (total price sum and rounded to the
        /// second digit after the decimal separator) of those products
        /// (regardless if they have a buyer or not).
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context
                .Categories
                .Select(p => new
                {
                    category = p.Name,
                    productsCount = p.CategoryProducts.Count(),
                    averagePrice = p.CategoryProducts.Count == 0 ? 0.ToString("f2") :
                    p.CategoryProducts.Average(x => x.Product.Price).ToString("f2"),
                    totalRevenue = p.CategoryProducts.Sum(x => x.Product.Price).ToString("f2")
                })
                .OrderByDescending(c => c.productsCount)
                .ToArray(); 

            string json = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return json;
        }

        /// <summary>
        /// Get all users who have at least 1 sold product with a buyer.
        /// Order them in descending order by the number of sold products with a buyer.
        /// Select only their first and last name, age and for each product - name and price.
        /// Ignore all null values.
        /// Export the results to JSON.Follow the format below to better understand how to structure your data.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            //ExportUsersWithFullProductInfoDto[] users = context
            //    .Users
            //    .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
            //    .OrderByDescending(u => u.ProductsSold.Count(p => p.BuyerId.HasValue))
            //    .ProjectTo<ExportUsersWithFullProductInfoDto>()
            //    .ToArray();

            ExportUsersInfoDto serDto = new ExportUsersInfoDto()
            {
                Users = context
                    .Users
                    .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
                    .OrderByDescending(u => u.ProductsSold.Count(p => p.BuyerId.HasValue))
                    .Select(u => new ExportUsersWithFullProductInfoDto()
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Age = u.Age,
                        SoldProductsInfo = new ExportSoldProductsFullInfoDto()
                        {
                            SoldProducts = u.ProductsSold
                                .Where(p => p.BuyerId.HasValue)
                                .Select(p => new ExportSoldProductShortInfoDto()
                                {
                                    Name = p.Name,
                                    Price = p.Price
                                })
                                .ToArray()
                        }
                    })
                    .ToArray()
            };

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            string json = JsonConvert.SerializeObject(serDto, Formatting.Indented, serializerSettings);
            return json;
        }

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

        /// <summary>
        /// Executes all validation attributes in a class and returns true or false depending on validation result.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}
