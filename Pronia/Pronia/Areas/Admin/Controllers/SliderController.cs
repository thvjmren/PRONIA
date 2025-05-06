using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Data;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Slider> slides = await _context.Slides.ToListAsync();
            return View(slides);
        }

        //public string Test()
        //{
        //    return Guid.NewGuid().ToString();
        //}

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (!slider.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError(nameof(Slider.Photo), "file type is incorrect");
                return View();
            }

            if (slider.Photo.Length > 1 * 1024 * 1024)
            {
                ModelState.AddModelError(nameof(Slider.Photo), "file size should be less than 1MB");
                return View();
            }

            bool exists = await _context.Slides.AnyAsync(s => s.Order == slider.Order);
            if (exists)
            {
                ModelState.AddModelError("Order", "this number is already taken, please choose another one");
                return View(slider);
            }

            string fileName = string.Concat(Guid.NewGuid().ToString(), slider.Photo.FileName).Substring(slider.Photo.FileName.LastIndexOf('.'));

            string path = Path.Combine(_env.WebRootPath, "assets", "images", "website-images", fileName);
            FileStream fl = new FileStream(path, FileMode.Create);
            await slider.Photo.CopyToAsync(fl);

            slider.Image = fileName;

            slider.CreatedAT = DateTime.Now;
            await _context.Slides.AddAsync(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        //if (!ModelState.IsValid) return View(slider);
        //return Content(slider.Photo.FileName + " " + slider.Photo.ContentType + " " + slider.Photo.Length);

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Slider? slider = await _context.Slides
                .Where(c => c.IsDeleted == false)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (slider is null) return NotFound();

            _context.Slides.Remove(slider);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }

}

