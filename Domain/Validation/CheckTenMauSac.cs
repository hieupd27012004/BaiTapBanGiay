using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Validation
{
    public class CheckTenMauSac : ValidationAttribute
    {
        private readonly AppDbcontext _context;

        public CheckTenMauSac()
        {
            _context = new AppDbcontext();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var name = value as string;
            if (string.IsNullOrEmpty(name))
                return ValidationResult.Success;

            var objectType = validationContext.ObjectType;
            var idProperty = objectType.GetProperty("IdMauSac");
            if (idProperty == null)
            {
                // Handle the case where Id property is not found
                return ValidationResult.Success; // or return an error message if needed
            }

            var mauSacId = (Guid)idProperty.GetValue(validationContext.ObjectInstance);
            var existing = _context.mauSacs
                .AsNoTracking() // Add this to avoid tracking issues
                .Any(ms => ms.TenMauSac == name && ms.IdMauSac != mauSacId);

            if (existing)
            {
                return new ValidationResult("Tên màu sắc đã tồn tại", new[] { "TenMauSac" });
            }
            return ValidationResult.Success;
        }
    }
}