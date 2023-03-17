using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GirlyMerely.Web.Areas.Customer.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage ="شماره موبایل الزامی است")]
        [StringLength(11,ErrorMessage ="شماره موبایل نامعتبر است")]
        [Display(Name = "شماره موبایل")]
        public string phonenumber { get; set; }
    }
}