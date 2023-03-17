using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirlyMerely.Core.Models
{
    public class SMSLog : IBaseEntity
    {
        public int Id { get; set; }

        public string SMSProvider { get; set; } = "sms.ir";
        public string ReceiverMobileNo { get; set; }
        public string MessageBody { get; set; }
        public DateTime SendDateTime { get; set; }
        public bool IsFlash { get; set; }
        public string PatternCode { get; set; }
        public int? LineNumberId { get; set; }
        public string LineNumber { get; set; }
        public string ResponseMessage { get; set; }
        public string BatchKey { get; set; }
        public string PanelMessageId { get; set; }


        public string InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
