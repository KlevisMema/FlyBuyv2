using FlyBuy.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FlyBuy.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FlyBuy.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewData["Exlusive"] = _context.Products.Where(p => p.Exclusive == true).Take(2).OrderBy(p => p.LastUpdated).ToList();
            ViewData["Rating"] = _context.Ratings.OrderByDescending(d => d.Time).Take(4).ToList();
            ViewData["LatestProducts"] = _context.Products.Where(p => p.Exclusive == false  &&  p.Collection != "Summer").OrderByDescending(P => P.LastUpdated).Take(8).ToList();
            ViewData["AgeCategory"] = _context.AgeCategories.Where(t => t.Name == "Women" || t.Name == "Men").ToList();
            ViewData["SummerCollection"] = _context.Products.Where(p => p.Collection == "Summer").Take(4).ToList();
            return View();
        }

        public IActionResult AgeCategory(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var displayByCategory = _context.Products.Where(c => c.Category.Id == id);          
            return View(displayByCategory);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ProductDetail(int? id)
        {
            Random random = new();

            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var suggestions = _context.Products.Where(m => m.ProductCategoryId == product.ProductCategoryId).ToList().Where(x => x.CategoryId == product.CategoryId).ToList();
            ViewBag.Suggestion = suggestions.OrderBy(x => random.Next()).Take(4);

            return View(product);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}