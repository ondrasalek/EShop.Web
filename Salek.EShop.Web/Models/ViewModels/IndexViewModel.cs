using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salek.EShop.Web.Models.Entity;

namespace Salek.EShop.Web.Models.ViewModels
{
    public class IndexViewModel
    {
        public IList<CarouselItem> CarouselItems { get; set; }
        public IList<ProductItem> ProductItems { get; set; }
    }
}
