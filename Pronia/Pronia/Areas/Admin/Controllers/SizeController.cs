using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Data;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SizeController : Controller
    {
        private readonly AppDbContext _context;

        public SizeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Size> sizes = await _context.Sizes.ToListAsync();
            return View(sizes);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Size size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = await _context.Categories.AnyAsync(c => c.Name == size.Name);
            if (result)
            {
                ModelState.AddModelError(nameof(Size.Name), $"this name: {size.Name} is already exists");
                return View();
            }

            size.CreatedAT = DateTime.Now;
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();

            return View(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);

            if (size is null) return NotFound();

            return View(size);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, Size? size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = await _context.Sizes.AnyAsync(c => c.Name == size.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError(nameof(Size.Name), $"this name: {size.Name} is already exists");
                return View();
            }

            Size? existed = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);

            if (existed.Name == size.Name) return RedirectToAction(nameof(Index));

            existed.Name = size.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}