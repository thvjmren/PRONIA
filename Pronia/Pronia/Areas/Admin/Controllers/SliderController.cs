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
                return View(sliderVM);
            }

            if (!sliderVM.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateSliderVM.Photo), "file type is incorrect");
                return View(sliderVM);
            }

            if (!sliderVM.Photo.ValidateSize(FileSize.MB, 1))
            {
                ModelState.AddModelError(nameof(CreateSliderVM.Photo), "file size should be less than 1MB");
                return View(sliderVM);
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

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Slider? slider = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (slider is null) return NotFound();

            UpdateSliderVM updateSliderVM = new UpdateSliderVM
            {
                Title = slider.Title,
                Subtitle = slider.Subtitle,
                Description = slider.Description,
                Order = slider.Order,
                Image = slider.Image
            };

            return View(updateSliderVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateSliderVM sliderVM)
        {
            if (!ModelState.IsValid) return View(sliderVM);


            Slider existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();

            bool exists = await _context.Slides.AnyAsync(s => s.Order == sliderVM.Order && s.Id != existed.Id);
            if (exists)
            {
                ModelState.AddModelError("Order", "this number is already taken, please choose another one");
                return View(sliderVM);
            }

            if (sliderVM.Photo is not null)
            {
                if (!sliderVM.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateSliderVM.Photo), "file type is incorrect");
                    return View(sliderVM);
                }

                if (!sliderVM.Photo.ValidateSize(FileSize.MB, 1))
                {
                    ModelState.AddModelError(nameof(UpdateSliderVM.Photo), "file size should be less than 1MB");
                    return View(sliderVM);
                }
                string fileName = await sliderVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                existed.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.Image = fileName;
            }

            else if (string.IsNullOrEmpty(existed.Image))
            {
                ModelState.AddModelError(nameof(UpdateSliderVM.Photo), "select a file");
                return View(sliderVM);
            }

            existed.Title = sliderVM.Title;
            existed.Subtitle = sliderVM.Subtitle;
            existed.Order = sliderVM.Order;
            existed.Description = sliderVM.Description;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



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

