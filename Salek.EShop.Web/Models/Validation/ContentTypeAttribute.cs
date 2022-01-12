using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Salek.EShop.Web.Models.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field)]
    public class ContentTypeAttribute : ValidationAttribute, IClientModelValidator
    {
        public string ContentType { get; set; }

        public ContentTypeAttribute(string contentType)
        {
            ContentType = contentType;
        }

        public string GetErrorMessage() =>
            $"Invalid content type for file.";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            else if (value is IFormFile formFile)
            {
                if (formFile.ContentType.ToLowerInvariant().Contains(ContentType.ToLowerInvariant()))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult(GetErrorMessage(), new List<string>() { validationContext.MemberName });
            }

            throw new NotImplementedException($"Validation not supported for type {value.GetType()}");
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-filecontent", GetErrorMessage());
            context.Attributes.Add("data-val-filecontent-type", ContentType);
        }
    }
}
