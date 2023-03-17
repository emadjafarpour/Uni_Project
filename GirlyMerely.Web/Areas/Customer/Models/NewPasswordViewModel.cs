using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GirlyMerely.Web.Areas.Customer.Models
{
    public class NewPasswordViewModel
    {
        public string phoneNumber { get; set; }

        public string token { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage ="رمز عبور الزامی است")]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="عدم هماهنگی رمز عبور و تکرار رمز عبور")]
        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [Display(Name = "تکرار رمز عبور")]
        public string ConfirmPassword { get; set; }
    }
}