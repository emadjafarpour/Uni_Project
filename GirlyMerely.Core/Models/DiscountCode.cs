using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirlyMerely.Core.Models
{
    public class DiscountCode : IBaseEntity
    {
        public int Id { get; set; }
        [Display(Name = "کد تخفیف")]
        public string DiscountCodeStr { get; set; }
        [Display(Name = "شروع فعالسازی")]
        public DateTime ActivationStartDate { get; set; }
        [Display(Name = "پایان فعالسازی")]
        public DateTime ActivationEndDate { get; set; }
        [Display(Name = "وضعیت")]
        public bool IsActive { get; set; } // wheather been used or not
        [Display(Name = "مقدار تخفیف")]
        public long Value { get; set; }    // the value of discount code 
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
