using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        // XML
        public static void Main(string[] args)
        {
            ProductShopContext dbContext = new ProductShopContext();

            //string xml = File.ReadAllText("../../../Datasets/categories-products.xml");

            string result = GetUsersWithProducts(dbContext);

            File.WriteAllText("../../../Result/users-and-products.xml.xml", result);

            Console.WriteLine(result);

            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();
            //Console.WriteLine("Database reset successfully!");
        }

        /// <summary>
        /// Problem 1
        /// Import the users from the provided file users.xml.
        /// Your method should return a string with the message
        /// $"Successfully imported {Users.Count}";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            ImportUsersDTO[] usersDTos = Deserialize<ImportUsersDTO[]>(inputXml, "Users");

            List<User> users = usersDTos
                 .Select(u => new User
                 {
                     FirstName = u.FirstName,
                     LastName = u.LastName,
                     Age = u.Age
                 })
                 .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        /// <summary>
        /// Problem 2
        /// Import the products from the provided file products.xml.
        /// Your method should return a string with the message
        /// $"Successfully imported {Products.Count}";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            ImportProductsDTO[] productsDTOs = Deserialize<ImportProductsDTO[]>(inputXml, "Products");

            List<Product> products = productsDTOs
                .Select(p => new Product
                {
                    Name = p.Name,
                    Price = p.Price,
                    SellerId = p.SellerId,
                    BuyerId = p.BuyerId
                })
                .ToList();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        /// <summary>
        /// Problem 3
        /// Import the categories from the provided file categories.xml. 
        /// Some of the names will be null, so you don't have to add them to the database.
        /// Just skip the record and continue.
        /// Your method should return a string with the message
        /// $"Successfully imported {Categories.Count}";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            ImportCategoriesDTO[] categoriesDTOs = Deserialize<ImportCategoriesDTO[]>(inputXml, "Categories");

            List<Category> categories = categoriesDTOs
                .Select(c => new Category
                {
                    Name = c.Name
                })
                .Where(c => c.Name != null)
                .ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        /// <summary>
        /// Problem 4
        /// Import the categories and products ids from the provided
        /// file categories-products.xml.
        /// If provided category or product id doesn't exist,
        /// skip the whole entry!
        /// Your method should return a string with the message
        /// $"Successfully imported {CategoryProducts.Count}";
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            ImportCategoryProductsDTO[] cpDto = Deserialize<ImportCategoryProductsDTO[]>(inputXml, "CategoryProducts");

            var categoriesId = context.Categories.Select(c => c.Id).ToList();
            var productsId = context.Products.Select(c => c.Id).ToList();

            List<CategoryProduct> cps = cpDto
                .Select(cp => new CategoryProduct
                {
                    CategoryId = cp.CategoryId,
                    ProductId = cp.ProductId
                })
                .Where(x => categoriesId
                    .Contains(x.CategoryId) && productsId
                        .Contains(x.ProductId))
                .ToList();

            context.CategoryProducts.AddRange(cps);
            context.SaveChanges();

            return $"Successfully imported {cps.Count}";
        }

        /// <summary>
        /// Problem 5
        /// Get all products in a specified price range between 500 and 1000 (inclusive).
        /// Order them by price (from lowest to highest). Select only the product name,
        /// price and the full name of the buyer. Take top 10 records.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetProductsInRange(ProductShopContext context)
        {
            List<ExportProductInRange> products = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(c => new ExportProductInRange
                {
                    Name = c.Name,
                    Price = c.Price,
                    Buyer = c.Buyer.FirstName + " " + c.Buyer.LastName
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToList();

            return Serialize(products, "Products");
        }

        /// <summary>
        /// Problem 6
        /// Get all users who have at least 1 sold item.
        /// Order them by the last name, then by the first name.
        /// Select the person's first and last name.
        /// For each of the sold products, select the product's name and price.
        /// Take top 5 records. 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetSoldProducts(ProductShopContext context)
        {
            List<ExportSoldProductsDTO> soldProducts = context
                .Users
                .Where(u => u.ProductsSold.Count() >= 1)
                .Select(sp => new ExportSoldProductsDTO
                {
                    FirstName = sp.FirstName,
                    LastName = sp.LastName,
                    SoldProducts = sp.ProductsSold.Select(p => new ExportUserPartDTO
                    {
                        Name = p.Name,
                        Price = p.Price
                    }).ToArray()
                })
                .OrderBy(x => x.LastName)
                .ThenBy(y => y.FirstName)
                .Take(5)
                .ToList();


            return Serialize(soldProducts, "Users");
        }

        /// <summary>
        /// Problem 7
        /// Get all categories. For each category select its name,
        /// the number of products, the average price of those products
        /// and the total revenue (total price sum) of those products 
        /// (regardless if they have a buyer or not).
        /// Order them by the number of products (descending) then by total revenue.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            List<ExportCategoriesByProductDTO> cp = context
                .Categories
                .Select(x => new ExportCategoriesByProductDTO
                {
                    Name = x.Name,
                    ProductCount = x.CategoryProducts.Count(),
                    AveragePrice = x.CategoryProducts.Average(p => p.Product.Price),
                    TotalRevenue = x.CategoryProducts.Sum(p => p.Product.Price)
                })
                .OrderByDescending(o => o.ProductCount)
                .ThenBy(t => t.TotalRevenue)
                .ToList();

            return Serialize(cp, "Categories");
        }

        /// <summary>
        /// Problem 8
        /// Select users who have at least 1 sold product. 
        /// Order them by the number of sold products (from highest to lowest).
        /// Select only their first and last name, age, 
        /// count of sold products and for each product 
        /// - name and price sorted by price (descending). Take top 10 records.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUsersWithProducts(ProductShopContext context)
        {

                List<ExportUserAndProducts> users = context.Users
                  .Include(x => x.ProductsSold) // not needed, added for Judge & ToList()                
                  .Where(x => x.ProductsSold.Count > 0)
                  .OrderByDescending(x => x.ProductsSold.Count)
                  .ToList() // not needed, added for Judge
                  .Select(u => new ExportUserAndProducts
                  {
                      FirstName = u.FirstName,
                      LastName = u.LastName,
                      Age = u.Age,
                      SoldProducts = new ExportSoldProductCount
                      {
                          ProductsCount = u.ProductsSold.Count,
                          Products = u.ProductsSold.Select(p => new ExportProduct
                          {
                              Name = p.Name,
                              Price = p.Price
                          })
                          .OrderByDescending(x => x.Price)
                         .ToList()
                      }
                  })
                  .Take(10)
                  .ToList();

            ExportAllUsersAndCountDTO allInfo = new ExportAllUsersAndCountDTO
            {
                CountOfUsers = context.Users.Count(x => x.ProductsSold.Any()),
                Users = users
            };


            return Serialize(allInfo, "Users");
        }

        //Helper methods
        private static T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            using (StringReader reader = new StringReader(inputXml))
            {
                T dtos = (T)xmlSerializer
                        .Deserialize(reader);
                return dtos;
            }
        }

        private static string Serialize<T>(T dto, string rootName)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            using (StringWriter writer = new StringWriter(sb))
            {
                xmlSerializer.Serialize(writer, dto, namespaces);

                return sb.ToString().TrimEnd();
            }
        }
    }
}