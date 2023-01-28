using FlyBuy.Data;
using FlyBuy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlyBuy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public OrderAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Manager,Worker")]
        [HttpGet]
        public IActionResult Index()
        {
            List<Order> orders = _context.Orders.ToList();
            List<OrderItem> order_items = _context.OrderItems.ToList();

            var items = orders.GroupBy(u => u.CreatedDate, (key, items) => new
            {
                Month = key,
                Total = orders.Where(x => x.CreatedDate == key).Count()
            }
            ).OrderBy(o => o.Month).ToList();

            return Ok(items);
        }

        [Authorize(Roles = "Admin,Manager,Worker")]
        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var test = await _context.OrderItems.Include(p => p.Product).GroupBy(u => u.ProductId, (key, items) => new
            {
                ProductId = key,
                ProductName = items.Where(x=>x.ProductId == key).Select(x=>x.Product.Name).Distinct(),
                QuantityOrdered =  items.Where(x => x.ProductId == key).Select(q=>q.Quantity).Sum(),
            }).ToListAsync();
           
            return Ok(test);
        }

        [Authorize(Roles = "Admin,Manager,Worker")]
        [HttpGet("GetEarnings")]
        public async Task<IActionResult> GetEarnings()
        {
            var earninngs = await _context.OrderItems.Include(order => order.Order)
                                                .GroupBy(d => d.Order.CreatedDate, (key, value) => new
            {
                Month = key,
                Earnings = value.Where(m => m.Order.CreatedDate == key).Select(m=>m.Price).Sum(),
                Quantity = value.Where(m => m.Order.CreatedDate == key).Select(m => m.Quantity).Sum()
            }).OrderBy(o => o.Month).ToListAsync();

            return Ok(earninngs);
        }
    }
}