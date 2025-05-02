using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Data;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class HomeController : Controller
    {
        public readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            HomeVM homeVM = new HomeVM
            {
                Slides = await _context.Slides
                .OrderBy(s => s.Order)
                .Take(2)
                .ToListAsync(),

                Products = await _context.Products
                .Take(8)
                .Include(p => p.ProductImgs.Where(pi => pi.IsPrimary != null))
                .ToListAsync()
            };

            return View(homeVM);
        }
    }
}
