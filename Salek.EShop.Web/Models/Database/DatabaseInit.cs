using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Salek.EShop.Web.Models.Entity;
using Salek.EShop.Web.Models.Identity;

namespace Salek.EShop.Web.Models.Database
{
    public class DatabaseInit
    {
        public void Initialize(EShopDbContext eShopDbContext)
        {
            eShopDbContext.Database.EnsureCreated();

            // Generate dummy carousel items and product items if the created tables are empty
            if (eShopDbContext.CarouselItems.Count() == 0)
            {
                IList<CarouselItem> carouselItems = GenerateCarouselItems();
                foreach(var ci in carouselItems)
                {
                    eShopDbContext.CarouselItems.Add(ci);
                }
            }

            if (eShopDbContext.ProductItems.Count() == 0)
            {
                IList<ProductItem> productItems = GenerateProductItems();
                foreach(var pi in productItems)
                {
                    eShopDbContext.ProductItems.Add(pi);
                }
            }

            eShopDbContext.SaveChanges();
        }

        public List<CarouselItem> GenerateCarouselItems()
        {
            List<CarouselItem> carouselItems = new List<CarouselItem>()
            {
                new CarouselItem("/img/carousel/image1.jpg", "love-death-and-robots-x-wallpaper"),
                new CarouselItem("/img/carousel/image2.jpg", "LDR1"),
                new CarouselItem("/img/carousel/image3.jpg", "LDR2"),
                new CarouselItem("/img/carousel/image4.jpg", "LDR3"),
                new CarouselItem("/img/carousel/image5.jpg", "LDR4"),
            };

            return carouselItems;
        }

        public List<ProductItem> GenerateProductItems()
        {
            List<ProductItem> productItems = new List<ProductItem>()
            {
                new ProductItem(
                    "iRobot robotický vysavač Roomba Combo 113",
                    "Robotický vysavač a mop. Úklid všech typů tvrdých podlah a koberců. Vysává i vytírá zároveň. Funkce odloženého úklidu. Senzory proti pádu ze schodů. Automatické nabíjení v nabíjecí stanici.",
                    7999,
                    "/img/product/image1.jpeg",
                    "iRobot Roomba Combo 113"
                    ),
            };

            return productItems;
        }

        public async Task EnsureRoleCreated(RoleManager<Role> roleManager)
        {
            string[] roles = Enum.GetNames(typeof(RolesEnum));

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new Role(role));
            }
        }

        public async Task EnsureAdminCreated(UserManager<User> userManager)
        {
            User user = new User
            {
                UserName = "admin",
                Email = "admin@admin.cz",
                EmailConfirmed = true,
                FirstName = "jmeno",
                LastName = "prijmeni"
            };
            string password = "abc";

            User adminInDatabase = await userManager.FindByNameAsync(user.UserName);

            if (adminInDatabase == null)
            {

                IdentityResult result = await userManager.CreateAsync(user, password);

                if (result == IdentityResult.Success)
                {
                    string[] roles = Enum.GetNames(typeof(RolesEnum));
                    foreach (var role in roles)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
                else if (result != null && result.Errors != null && result.Errors.Count() > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        Debug.WriteLine($"Error during Role creation for Admin: {error.Code}, {error.Description}");
                    }
                }
            }

        }

        public async Task EnsureManagerCreated(UserManager<User> userManager)
        {
            User user = new User
            {
                UserName = "manager",
                Email = "manager@manager.cz",
                EmailConfirmed = true,
                FirstName = "jmeno",
                LastName = "prijmeni"
            };
            string password = "abc";

            User managerInDatabase = await userManager.FindByNameAsync(user.UserName);

            if (managerInDatabase == null)
            {

                IdentityResult result = await userManager.CreateAsync(user, password);

                if (result == IdentityResult.Success)
                {
                    string[] roles = Enum.GetNames(typeof(RolesEnum));
                    foreach (var role in roles)
                    {
                        if (role != RolesEnum.Admin.ToString())
                            await userManager.AddToRoleAsync(user, role);
                    }
                }
                else if (result != null && result.Errors != null && result.Errors.Count() > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        Debug.WriteLine($"Error during Role creation for Manager: {error.Code}, {error.Description}");
                    }
                }
            }

        }
    }
}
