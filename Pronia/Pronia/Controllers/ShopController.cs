using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Data;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Product? product = _context.Products
                .Include(p => p.ProductImgs.OrderByDescending(pi => pi.IsPrimary))
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product is null) return NotFound();

            DetailsVM detailsVM = new DetailsVM
            {
                Product = product,
                RelatedProducts = _context.Products
                .Where(p => p.Category.Id == product.CategoryId && p.Id != id)
                .Take(8)
                .Include(p => p.ProductImgs.Where(pi => pi.IsPrimary != null))
                .ToList()
            };

            return View(detailsVM);
        }
    }
}
