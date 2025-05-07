using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Data;
using Pronia.Models;
using Pronia.Utilities.Enums;
using Pronia.Utilities.Extensions;
using Pronia.ViewModels;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        //private readonly string[] Roots = new string[] { "assets", "images", "website-images" };
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<GetSliderVM> sliderVMs = await _context.Slides.Select(s =>

                new GetSliderVM
                {
                    Id = s.Id,
                    Title = s.Title,
                    Image = s.Image,
                    CreatedAT = s.CreatedAT,
                    Order = s.Order
                }
           ).ToListAsync();
            return View(sliderVMs);
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
        public async Task<IActionResult> Create(CreateSliderVM sliderVM)
        {
            if (!ModelState.IsValid) return View(sliderVM);

            if (sliderVM.Photo is null)
            {
                ModelState.AddModelError(nameof(CreateSliderVM.Photo), "select a file");
                return View();
            }

            if (!sliderVM.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateSliderVM.Photo), "file type is incorrect");
                return View();
            }

            if (!sliderVM.Photo.ValidateSize(FileSize.MB, 1))
            {
                ModelState.AddModelError(nameof(CreateSliderVM.Photo), "file size should be less than 1MB");
                return View();
            }

            string fileName = await sliderVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");

            bool exists = await _context.Slides.AnyAsync(s => s.Order == sliderVM.Order);
            if (exists)
            {
                ModelState.AddModelError("Order", "this number is already taken, please choose another one");
                return View(sliderVM);
            }

            Slider slider = new Slider
            {
                Title = sliderVM.Title,
                Subtitle = sliderVM.Subtitle,
                Description = sliderVM.Description,
                Order = sliderVM.Order,
                Image = fileName,
                CreatedAT = DateTime.Now
            };
            await _context.Slides.AddAsync(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        //return Content(slider.Photo.FileName + " " + slider.Photo.ContentType + " " + slider.Photo.Length);

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Slider? slider = await _context.Slides
                .Where(c => c.IsDeleted == false)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (slider is null) return NotFound();

            slider.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");

            _context.Remove(slider);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }

}

