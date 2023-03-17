using GirlyMerely.Core.Models;
using GirlyMerely.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GirlyMerely.Web.ViewModels
{
    public class DiscountCodeResponseViewModel
    {
        public long FinalPrice { get; set; }
        public long DiscountAmount { get; set; }
        public long ShippingAmount {get; set;}
        public string Response { get; set; }
    }

    public class CustomerInfoView
    {
        public CustomerInfoView(Invoice invoice, GeoDivisionsRepository geoDevisionRepository)
        {
            this.Name = invoice.CustomerName;
            this.InvoiceNumber = invoice.InvoiceNumber;
            this.Address = geoDevisionRepository.GetGeoDevisionTitle(invoice.GeoDivisionId.Value) + " " + invoice.Address;
            this.PostalCode = invoice.PostalCode;
            this.Phone = invoice.Phone;
            this.Email = invoice.Email;
        }

        public string Name { get; set; }
        public string InvoiceNumber { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

    }

}