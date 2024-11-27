using AppData.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Validation
{
    public class CheckVoucher : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var voucher = (Voucher)validationContext.ObjectInstance;

            // Kiểm tra nếu LoaiGiamGia là theo %
            if (voucher.LoaiGiamGia == 1) // 1 là theo %
            {
                if (value is int giaTriGiam && (giaTriGiam < 1 || giaTriGiam > 100))
                {
                    return new ValidationResult("Giá trị giảm phải nằm trong khoảng từ 1% đến 100%.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
