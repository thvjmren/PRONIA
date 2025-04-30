using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Data;
using Pronia.Models;
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
        public IActionResult Index()
        {
            Product product = new Product();
            Category category = new Category();

            List<Slider> slides = new List<Slider>
            {
                new Slider
                {
                    Title="Title-1",
                    Subtitle="Subtitle-1",
                    Description="Description-1",
                    Image="1-1-524x617.png",
                    Order=3,
                    CreatedAT=DateTime.Now
                },
                new Slider
                {
                    Title="Title-2",
                    Subtitle="Subtitle-2",
                    Description="Description-2",
                    Image="1-2-524x617.png",
                    Order=2,
                    CreatedAT=DateTime.Now
                },
                    new Slider
                {
                    Title="Title-3",
                    Subtitle="Subtitle-3",
                    Description="Description-3",
                    Image="PL0109.425.jpg",
                    Order=1,
                    CreatedAT=DateTime.Now
                },
            };

            _context.Slides.AddRange(slides);
            _context.SaveChanges();

            HomeVM homeVM = new HomeVM
            {
                Slides = _context.Slides
                .OrderBy(s => s.Order)
                .Take(2)
                .ToList(),

                Products = _context.Products
                .Include(p => p.ProductImgs.Where(pi => pi.IsPrimary != null))
                .Take(8)
                .ToList()
            };

            return View(homeVM);
        }
    }
}
