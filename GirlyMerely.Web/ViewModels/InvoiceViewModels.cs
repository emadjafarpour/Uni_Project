using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GirlyMerely.Core.Models;

namespace GirlyMerely.Web.ViewModels
{
    public class InvoiceTableViewModel
    {
        public InvoiceTableViewModel()
        {
        }
        public InvoiceTableViewModel(Invoice invoice)
        {
            this.Invoice = invoice;
            this.PersianDate = new PersianDateTime(invoice.AddedDate).ToString();
            this.Customer = invoice.CustomerName;
            this.ShipingDate = invoice.ShipingDate;
            this.Customer = "-";
            if (invoice.CustomerName != null)
            {
                this.Customer = invoice.CustomerName;
            }
            else
            {
                if (invoice.CustomerId != null)
                {
                    var user = invoice.Customer.User;
                    this.Customer = $"{user.FirstName} {user.LastName}";
                }
                else
                {
                    this.Customer = "-";
                }
            }
        }
        public Invoice Invoice { get; set; }
        [Display(Name = "تاریخ ثبت")]
        public string PersianDate { get; set; }

        public string Customer { get; set; }
        public string ShipingDate { get; set; }
    }

    public class ViewInvoiceViewModel
    {
        public Invoice Invoice { get; set; }
        public string PersianDate { get; set; }
        public string ShipingDate { get; set; }
        public List<ProductMainFeature> ProductMainFeatures { get; set; }
        public List<InvoiceItemWithMainFeatureViewModel> InvoiceItems { get; set; }
    }
    public class InvoiceItemWithMainFeatureViewModel
    {
        public InvoiceItem InvoiceItem { get; set; }
        public string MainFeature { get; set; }
    }
}