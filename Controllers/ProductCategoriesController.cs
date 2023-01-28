using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlyBuy.Data;
using FlyBuy.Models;
using Microsoft.AspNetCore.Authorization;

namespace FlyBuy.Controllers
{
    public class ProductCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Manager,Worker")]
        public async Task<IActionResult> Index()
        {
            return _context.ProductCategories != null ?
                        View(await _context.ProductCategories.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.ProductCategories'  is null.");
        }

        [Authorize(Roles = "Admin,Manager,Worker")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Manager,Worker")]
        [HttpPost]
        public JsonResult Create(ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productCategory);
                _context.SaveChanges();

            }
            return new JsonResult(Ok());
        }


        [Authorize(Roles = "Admin,Manager,Worker")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProductCategories == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategories.FindAsync(id);
            if (productCategory == null)
            {
                return NotFound();
            }
            return View(productCategory);
        }

        [Authorize(Roles = "Admin,Manager,Worker")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] ProductCategory productCategory)
        {
            if (id != productCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCategoryExists(productCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productCategory);
        }

        [Authorize(Roles = "Admin,Manager,Worker")]
        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var ProducCategory = _context.ProductCategories.Find(id);
            _context.ProductCategories.Remove(ProducCategory);
            _context.SaveChanges();
            return new JsonResult(Ok());
        }

        private bool ProductCategoryExists(int id)
        {
            return (_context.ProductCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
