using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GirlyMerely.Web.Areas.Customer.Models
{
    public class ActivationCodeViewModel
    {
        public string PhoneNumber { get; set; }

        public string UserId { get; set; }

        [Display(Name ="کد:")]
        public string Code { get; set; }
    }
}