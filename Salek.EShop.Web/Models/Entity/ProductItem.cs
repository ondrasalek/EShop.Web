using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Salek.EShop.Web.Models.Entity
{
    [Table(nameof(ProductItem))]
    public class ProductItem
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [StringLength(255)]
        [Required]
        public string Name { get; set; }

        [StringLength(255)]
        [Required]
        public string Description { get; set; }

        [Required]
        public double Price { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }

        [StringLength(255)]
        [Required]
        public string ImageSource { get; set; }

        [StringLength(50)]
        public string ImageAlt { get; set; }


        public ProductItem(
            string name,
            string description,
            double price,
            string imageSource,
            string imageAlt)
        {
            Name = name;
            Description = description;
            Price = price;
            ImageSource = imageSource;
            ImageAlt = imageAlt;
        }

        public ProductItem()
        {

        }
    }
}
