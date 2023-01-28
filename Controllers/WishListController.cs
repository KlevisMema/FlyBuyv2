using FlyBuy.Data;
using FlyBuy.Models;
using FlyBuy.SessionConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyBuy.Controllers
{
    public class WishListController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishListController(ApplicationDbContext context)
        {
            _context = context;
        }

        private const string wishListKey = "WishList";

        [AllowAnonymous]
        public IActionResult Index()
        {
            List<Product> Product = HttpContext.Session.GetJson<List<Product>>(wishListKey) ?? new List<Product>();

            return View(Product);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Add(int id)
        {
            Product product = await _context.Products.FindAsync(id);

            List<Product> Product = HttpContext.Session.GetJson<List<Product>>(wishListKey) ?? new List<Product>();

            Product likedItem = Product.Where(x => x.Id == id).FirstOrDefault();

            if(likedItem == null)
            {
                Product.Add(product);
            }
            else
            {
                
            }


            HttpContext.Session.SetJson(wishListKey, Product);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [AllowAnonymous]
        public IActionResult Remove(int id)
        {
            List<Product> Product = HttpContext.Session.GetJson<List<Product>>(wishListKey) ?? new List<Product>();

            Product.RemoveAll(x => x.Id == id);

            if (Product.Count == 0)
            {
                HttpContext.Session.Remove(wishListKey);
            }
            else
            {
                HttpContext.Session.SetJson(wishListKey, Product);
            }

            return RedirectToAction("Index");
        }

    }
}
