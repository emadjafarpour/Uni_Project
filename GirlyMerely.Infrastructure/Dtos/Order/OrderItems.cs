using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirlyMerely.Infrastructure.Dtos.Order
{
    public class OrderItems
    {
        public string Title { get; set; }

        public long Price { get; set; }

        public int Quantity { get; set; }

        public string ProductId { get; set; }
    }
}
