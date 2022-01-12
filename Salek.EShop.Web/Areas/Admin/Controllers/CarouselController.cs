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
    public class CarouselController : Controller
    {
        private readonly EShopDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public CarouselController(EShopDbContext eShopDbContext, IWebHostEnvironment env)
        {
            _dbContext = eShopDbContext;
            _env = env;
        }

        public IActionResult Select()
        {
            IList<CarouselItem> carouselItems = _dbContext.CarouselItems.ToList();

            return View(carouselItems);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarouselItem carouselItem)
        {
            if (carouselItem != null && carouselItem.Image != null)
            {
                FileUpload fileUpload = new FileUpload(_env.WebRootPath, "img/carousel", "image");
                carouselItem.ImageSource = await fileUpload.FileUploadAsync(carouselItem.Image);

                ModelState.Clear();
                TryValidateModel(carouselItem);
                if (ModelState.IsValid)
                {
                    _dbContext.CarouselItems.Add(carouselItem);

                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(CarouselController.Select));
                }
            }

            return View(carouselItem);
        }

        public IActionResult Edit(int id)
        {
            var foundItem = _dbContext.CarouselItems.FirstOrDefault(c => c.Id == id);
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
        public async Task<IActionResult> Edit(CarouselItem carouselItem)
        {
            var foundItem = _dbContext.CarouselItems.FirstOrDefault(c => c.Id == carouselItem.Id);
            if (foundItem != null)
            {
                if (carouselItem != null && carouselItem.Image != null)
                {
                    FileUpload fileUpload = new FileUpload(_env.WebRootPath, "img/carousel", "image");
                    carouselItem.ImageSource = await fileUpload.FileUploadAsync(carouselItem.Image);

                    if (!string.IsNullOrWhiteSpace(carouselItem.ImageSource))
                    {
                        foundItem.ImageSource = carouselItem.ImageSource;
                    }
                }
                else
                {
                    carouselItem.ImageSource = " ";
                }

                ModelState.Clear();
                TryValidateModel(carouselItem);
                if (ModelState.IsValid)
                {
                    foundItem.ImageAlt = carouselItem.ImageAlt;
                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(CarouselController.Select));
                }

                //foundItem.ImageAlt = carouselItem.ImageAlt;
                //await _dbContext.SaveChangesAsync();

                //return RedirectToAction(nameof(CarouselController.Select));
                return View(carouselItem);
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            DbSet<CarouselItem> carouselItems = _dbContext.CarouselItems;
            var carouselItem = carouselItems.FirstOrDefault(i => i.Id == id);
            if (carouselItem != null)
            {
                carouselItems.Remove(carouselItem);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(CarouselController.Select));
        }
    }
}
