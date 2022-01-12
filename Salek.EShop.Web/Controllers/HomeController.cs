using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Salek.EShop.Web.Models.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Salek.EShop.Web.Models;
using Salek.EShop.Web.Models.Database;
using Salek.EShop.Web.Models.ViewModels;

namespace Salek.EShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EShopDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, EShopDbContext eShopDbContext)
        {
            _logger = logger;
            _dbContext = eShopDbContext;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Index viewed");
            IndexViewModel indexViewModel = new IndexViewModel();
            indexViewModel.CarouselItems = _dbContext.CarouselItems.ToList();
            indexViewModel.ProductItems = _dbContext.ProductItems.ToList();

            return View(indexViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
