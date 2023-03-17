using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirlyMerely.Infrastructure.Dtos.Order
{
    public class Order
    {
        public string CustomerName { get; set; }

        public long ShippingPriceAmount { get; set; }

        public string ShipingDate { get; set; }
        public string PersianDate { get; set; }
        public string InvoiceNumber { get; set; }
        public bool ShipingStatus { get; set; }

        public long DiscountAmount { get; set; }

        public long TotalPriceBeforeDiscount { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }
        public string PostalCode { get; set; }

        public long TotalPrice { get; set; }

        private List<OrderItems> _OrderItems;

        public List<OrderItems> OrderItems
        {
            get { return _OrderItems ?? (_OrderItems = new List<OrderItems>()); }
            set { _OrderItems = value; }
        }


    }
}
