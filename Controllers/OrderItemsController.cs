using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlyBuy.Data;
using Microsoft.AspNetCore.Authorization;

namespace FlyBuy.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Manager,Worker")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.OrderItems.Include(o => o.Order).Include(o => o.Product);
            
            return View(await applicationDbContext.ToListAsync());
        }

        
        [Authorize(Roles = "Admin,Manager,Worker")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var orderDetails = await _context.OrderItems.Include(p=>p.Product).Where(o=>o.OrderId == id).ToListAsync();
            var x = await _context.OrderItems.Where(o => o.OrderId == id).Select(n => n.Order.CustomerName).ToArrayAsync();
            var y = x[1];
            ViewData["Customer"] = y;
            return View(orderDetails);
        }

        private bool OrderItemExists(int id)
        {
          return (_context.OrderItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
