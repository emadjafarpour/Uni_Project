using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirlyMerely.Core.Models
{
    public class ShippingPrice : IBaseEntity
    {
        public int Id { get; set; }


        [Display(Name = "ماکسیمم ارزش سبد خرید")]
        [Range(0, int.MaxValue, ErrorMessage = "لطفا یک مقدار معتبر وارد نمایید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long MaxCartOverallPrice { get; set; }

        [Display(Name = "هزینه ارسال")]
        [Range(0, int.MaxValue, ErrorMessage = "لطفا یک مقدار معتبر وارد نمایید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long ShippingPriceValue { get; set; }


        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        public string InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
