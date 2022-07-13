using BookShop.Data;
using BookShop.Initializer;

namespace BookShop
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);
        }
    }
}
