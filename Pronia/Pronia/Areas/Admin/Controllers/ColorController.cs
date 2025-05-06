using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Data;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ColorController : Controller
    {
        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Color> colors = await _context.Colors.ToListAsync();
            return View(colors);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Color color)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = await _context.Colors.AnyAsync(c => c.Name == color.Name);
            if (result)
            {
                ModelState.AddModelError(nameof(Color.Name), $"this name: {color.Name} is already exists");
                return View();
            }

            color.CreatedAT = DateTime.Now;
            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();

            return View(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (color is null) return NotFound();

            return View(color);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, Color? color)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = await _context.Colors.AnyAsync(c => c.Name == color.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError(nameof(Color.Name), $"this name: {color.Name} is already exists");
                return View();
            }

            Color? existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (existed.Name == color.Name) return RedirectToAction(nameof(Index));

            existed.Name = color.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Color? color = await _context.Colors
                .Where(c => c.IsDeleted == false)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (color is null) return NotFound();

            _context.Colors.Remove(color);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
