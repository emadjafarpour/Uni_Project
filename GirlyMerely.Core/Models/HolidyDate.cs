using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirlyMerely.Core.Models
{
   public class HolidyDate:IBaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="تاریخ تعطیلات")]
        public string HolidyDates { get; set; }




        public string InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
