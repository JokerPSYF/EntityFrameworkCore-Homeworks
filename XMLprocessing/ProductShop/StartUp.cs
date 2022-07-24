using ProductShop.Data;
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
            string xml = File.ReadAllText("../../../Datasets/categories.xml");

            string result = ImportCategories(dbContext, xml);
            //File.WriteAllText("../../../Result/customers-total-sales", result);
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