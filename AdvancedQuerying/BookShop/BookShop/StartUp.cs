using BookShop.Data;
using BookShop.Initializer;
using BookShop.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookShop
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);
            //string input = Console.ReadLine();
            //int input = int.Parse(Console.ReadLine());

            Console.WriteLine(RemoveBooks(db));
        }

        /// <summary>
        /// Return in a single string all book titles,
        /// each on a new line, that have age restriction,
        /// equal to the given command. Order the titles alphabetically.
        ///Read input from the console in your main method,
        ///and call your method with the necessary arguments.
        ///Print the returned string to the console. Ignore casing of the input.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            AgeRestriction ageRestriction;

            bool isParsed = Enum.TryParse<AgeRestriction>(command, true, out ageRestriction);
            if (!isParsed) return string.Empty;

            var books = context
                .Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .Select(b => b.Title)
                .OrderBy(title => title)
                .ToArray();

            return String.Join(Environment.NewLine, books);
        }

        /// <summary>
        /// Return in a single string titles of the golden edition books that have less than 5000 copies,
        /// each on a new line. Order them by book id ascending.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenBooks = context
                .Books
                .Where(b => b.EditionType == EditionType.Gold &&
                        b.Copies < 5000)
                .Select(b => b.Title)
                .ToArray();

            return String.Join(Environment.NewLine, goldenBooks);
        }

        /// <summary>
        /// Return in a single string all titles and prices of books with price higher than 40,
        /// each on a new row in the format given below. Order them by price descending.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder output = new StringBuilder(); 

            var books = context
                .Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToArray();

            foreach (var book  in books)
            {
                output.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return output.ToString().TrimEnd();
        }

        /// <summary>
        /// Return in a single string all titles of books that are NOT released on a given year.
        /// Order them by book id ascending.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .Select(b => b.Title)
                .ToArray();

            return String.Join(Environment.NewLine, books);
        }

        /// <summary>
        /// Return in a single string the titles of books by a given list of categories.
        /// The list of categories will be given in a single line separated with one or more spaces.
        /// Ignore casing. Order by title alphabetically.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.ToLower().Split(' ');

            var book = context
                .Books
                .Where(b => b.BookCategories
                    .Any(bc => categories
                    .Contains(bc.Category.Name.ToLower())))
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToArray();
              
            return String.Join(Environment.NewLine, book);
        }

        /// <summary>
        /// Return the title, edition type and price of all books that are released before a given date.
        /// The date will be a string in format dd-MM-yyyy.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder output = new StringBuilder();

            int[] input = date
                .Split('-')
                .Select(int.Parse)
                .ToArray();

            DateTime dateTime = new DateTime(input[2], input[1], input[0]);

            var books = context
                .Books
                .Where(b => b.ReleaseDate < dateTime )
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price,
                    b.ReleaseDate
                })
                .OrderByDescending(b => b.ReleaseDate)
                .ToArray();

            foreach (var book in books)
            {
                output.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return output.ToString().TrimEnd();
        }

        /// <summary>
        /// Return the full names of authors, whose first name ends with a given string.
        ///Return all names in a single string, each on a new row, ordered alphabetically.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context
                .Authors
                .Where(a => a.FirstName.EndsWith(input))
                .ToArray()
                .Select(a => $"{a.FirstName} {a.LastName}")
                .OrderBy(a => a)
                .ToArray();

            return String.Join(Environment.NewLine, authors);
        }

        /// <summary>
        /// Return the titles of book, which contain a given string. Ignore casing.
        /// Return all titles in a single string, each on a new row, ordered alphabetically.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context
                .Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToArray();

            return String.Join(Environment.NewLine, books);
        }

        /// <summary>
        /// Return all titles of books and their authors’ names for books,
        /// which are written by authors whose last names start with the given string.
        /// Return a single string with each title on a new row.Ignore casing.Order by book id ascending.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder output = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.Author.LastName
                    .ToLower()
                    .StartsWith(input.ToLower()))
                .Select(b => new
                {
                    b.Title,
                    FullName = $"{b.Author.FirstName} {b.Author.LastName}"
                })
                .ToArray();

            foreach (var book in books)
            {
                output.AppendLine($"{book.Title} ({book.FullName})");
            }

            return output.ToString().TrimEnd();
        }

        /// <summary>
        /// Return the number of books, which have a title longer than the number given as an input.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="lengthCheck"></param>
        /// <returns></returns>
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int booksCount = context
                .Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();

            return booksCount;
        }

        /// <summary>
        /// Return the total number of book copies for each author.
        /// Order the results descending by total book copies.
        /// Return all results in a single string, each on a new line.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var authors = context
                .Authors
                .Select(a => new
                {
                    FullName = $"{a.FirstName} {a.LastName}",
                    CopiesCount = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.CopiesCount)
                .ToArray();

            foreach (var author in authors)
            {
                output.AppendLine($"{author.FullName} - {author.CopiesCount}");
            }

            return output.ToString().TrimEnd();
        }

        /// <summary>
        /// Return the total profit of all books by category.
        /// Profit for a book can be calculated by multiplying
        /// its number of copies by the price per single book.
        /// Order the results by descending by total profit for category and ascending by category name.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var categories = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    CategoryProfit = c.CategoryBooks.Sum(b => b.Book.Copies * b.Book.Price)
                })
                .OrderByDescending(c => c.CategoryProfit)
                .ThenBy(c => c.Name)
                .ToArray();

            foreach (var category in categories)
            {
                output.AppendLine($"{category.Name} ${category.CategoryProfit:f2}");
            }

            return output.ToString().TrimEnd();
        }

        /// <summary>
        /// Get the most recent books by categories.
        /// The categories should be ordered by name alphabetically.
        /// Only take the top 3 most recent books from each category
        /// - ordered by release date (descending).
        /// Select and print the category name, and for each book
        /// – its title and release year.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var booksByCategories = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    Books = c.CategoryBooks.Select(b => new
                    {
                        b.Book.Title,
                        Date = b.Book.ReleaseDate.Value
                    })
                    .OrderByDescending(b => b.Date)
                    .Take(3)
                })
                .OrderBy(c => c.Name)
                .ToArray();

            foreach (var category in booksByCategories)
            {
                output.AppendLine($"--{category.Name}");
                foreach (var book in category.Books)
                {
                    output.AppendLine($"{book.Title} ({book.Date.Year})");
                }
            }

            return output.ToString().TrimEnd();
        }

        /// <summary>
        /// Increase the prices of all books released before 2010 by 5.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .ToArray();

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Remove all books, which have less than 4200 copies.
        /// Return an int - the number of books that were deleted from the database.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context
                .Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            int booksCount = books.Count();

            context.RemoveRange(books);

            context.SaveChanges();
            
            return booksCount;
        }
    }
}
