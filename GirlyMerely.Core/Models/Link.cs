﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirlyMerely.Core.Models
{
    public class Link :IBaseEntity
    {
        public int Id { get; set; }

        public string ShortLink { get; set; }
        public string OrginalLink { get; set; }
        public string InsertUser { get; set; }
        public DateTime? InsertDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
