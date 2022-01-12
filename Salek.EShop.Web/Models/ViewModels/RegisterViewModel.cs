using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Salek.EShop.Web.Models.ViewModels
{
    public class RegisterViewModel
    {
        private const string ErrorMessagePassword = 
            "Passwords must be at least 4 characters.<BR>" +
            "Passwords must have at least one non alphanumeric character.<BR>" +
            "Passwords must have at least one digit('0'-'9').<BR>" +
            "Passwords must have at least one uppercase('A'-'Z').";

        [Required]
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        //[UniqueCharacters(6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{4,}$", ErrorMessage = RegisterViewModel.ErrorMessagePassword)]
        public string Password { get; set; }

        [Required]
        [DisplayName("Repeat Password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match.")]
        public string RepeatedPassword { get; set; }
    }
}