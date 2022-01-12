using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salek.EShop.Web.Models.Identity
{
    public class User : IdentityUser<int>
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
    }
}
