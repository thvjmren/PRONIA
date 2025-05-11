﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Data;
using Pronia.Models;
using Pronia.Utilities.Enums;
using Pronia.Utilities.Extensions;
using Pronia.ViewModels;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<GetProductVM> productVMs = await _context.Products.Select(p => new GetProductVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                SKU = p.SKU,
                CategoryName = p.Category.Name,
                MainImage = p.ProductImgs.FirstOrDefault(pi => pi.IsPrimary == true).Image
            }).ToListAsync();

            return View(productVMs);
        }

        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new CreateProductVM
            {
                Categories = ViewBag.Categories = await _context.Categories.ToListAsync()
            };

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(productVM);
            }

            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(CreateProductVM.CategoryId), "category does not exist");
                return View(productVM);
            }

            if (!productVM.MainPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateProductVM.MainPhoto), "file type is incorrect");
                return View(productVM);
            }

            if (!productVM.MainPhoto.ValidateSize(FileSize.KB, 500))
            {
                ModelState.AddModelError(nameof(CreateProductVM.MainPhoto), "file size must be less than 500 KB");
                return View(productVM);
            }

            bool nameResult = await _context.Products.AnyAsync(p => p.Name == productVM.Name);
            if (nameResult)
            {
                ModelState.AddModelError(nameof(productVM.Name), $"same name:{productVM.Name} is already used");
                return View(productVM);
            }

            ProductImg mainImage = new ProductImg
            {
                Image = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                IsPrimary = true,
                CreatedAT = DateTime.Now
            };

            Product product = new Product
            {
                Name = productVM.Name,
                Price = productVM.Price.Value,
                SKU = productVM.SKU,
                Description = productVM.Description,
                CategoryId = productVM.CategoryId.Value,
                ProductImgs = new List<ProductImg> { mainImage }
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Product? product = await _context.Products.Include(p => p.ProductImgs).FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                Description = product.Description,
                CategoryId = product.CategoryId,
                PrimaryImage = product.ProductImgs.FirstOrDefault(p => p.IsPrimary == true).Image,
                Categories = await _context.Categories.ToListAsync()
            };

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();

            if (!ModelState.IsValid) return View(productVM);

            if (productVM.MainPhoto is not null)
            {
                if (!productVM.MainPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.MainPhoto), "file type is incorrect");
                    return View(productVM);
                }

                if (!productVM.MainPhoto.ValidateSize(FileSize.KB, 1000))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.MainPhoto), "file size must be less than 500 KB");
                    return View(productVM);
                }
            }

            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(UpdateProductVM.CategoryId), "CATEGORY DOES NOT EXIST");
                return View(productVM);
            }

            bool nameResult = await _context.Products.AnyAsync(p => p.Name == productVM.Name && p.Id != id);
            if (nameResult)
            {
                ModelState.AddModelError(nameof(UpdateProductVM.CategoryId), $"product: {productVM.Name} is already exist...");
                return View(productVM);
            }

            Product? existed = await _context.Products.Include(p => p.ProductImgs).FirstOrDefaultAsync(p => p.Id == id);

            if (productVM.MainPhoto is not null)
            {
                ProductImg productImg = new ProductImg()
                {
                    Image = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                    IsPrimary = true,
                    CreatedAT = DateTime.Now
                };

                ProductImg existedMain = existed.ProductImgs.FirstOrDefault(pi => pi.IsPrimary == true);
                existedMain.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImgs.Remove(existedMain);
                existed.ProductImgs.Add(productImg);
            }

            existed.Name = productVM.Name;
            existed.Price = productVM.Price.Value;
            existed.SKU = productVM.SKU;
            existed.CategoryId = productVM.CategoryId.Value;
            existed.Description = productVM.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Product? product = await _context.Products.Include(p => p.ProductImgs).FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            foreach (ProductImg productImage in product.ProductImgs)
            {

                _context.ProductImgs.Remove(productImage);
                productImage.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website_images");
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}