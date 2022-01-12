using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salek.EShop.Web.Models.Database;
using Salek.EShop.Web.Models.Entity;
using Salek.EShop.Web.Models.Identity;
using Salek.EShop.Web.Models.Implementation;

namespace Salek.EShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = nameof(RolesEnum.Admin) + ", " + nameof(RolesEnum.Manager))]
    public class ProductController : Controller
    {
        private readonly EShopDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public ProductController(EShopDbContext eShopDbContext, IWebHostEnvironment env)
        {
            _dbContext = eShopDbContext;
            _env = env;
        }

        public IActionResult Select()
        {
            IList<ProductItem> productItems = _dbContext.ProductItems.ToList();

            return View(productItems);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductItem productItem)
        {
            if (productItem != null && productItem.Image != null)
            {
                FileUpload fileUpload = new FileUpload(_env.WebRootPath, "img/product", "image");
                productItem.ImageSource = await fileUpload.FileUploadAsync(productItem.Image);

                if (!string.IsNullOrWhiteSpace(productItem.ImageSource))
                {
                    _dbContext.ProductItems.Add(productItem);

                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(ProductController.Select));
                }
            }

            return View(productItem);
        }

        public IActionResult Edit(int id)
        {
            var foundItem = _dbContext.ProductItems.FirstOrDefault(c => c.Id == id);
            if (foundItem != null)
            {
                return View(foundItem);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductItem productItem)
        {
            var foundItem = _dbContext.ProductItems.FirstOrDefault(c => c.Id == productItem.Id);
            if (foundItem != null)
            {
                if (productItem != null && productItem.Image != null)
                {
                    FileUpload fileUpload = new FileUpload(_env.WebRootPath, "img/product", "image");
                    productItem.ImageSource = await fileUpload.FileUploadAsync(productItem.Image);

                    if (!string.IsNullOrWhiteSpace(productItem.ImageSource))
                    {
                        foundItem.ImageSource = productItem.ImageSource;
                    }
                }

                foundItem.Name = productItem.Name;
                foundItem.Description = productItem.Description;
                foundItem.Price = productItem.Price;
                foundItem.ImageAlt = productItem.ImageAlt;

                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(ProductController.Select));
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            DbSet<ProductItem> carouselItems = _dbContext.ProductItems;
            var productItem = carouselItems.FirstOrDefault(i => i.Id == id);
            if (productItem != null)
            {
                carouselItems.Remove(productItem);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ProductController.Select));
        }
    }
}
