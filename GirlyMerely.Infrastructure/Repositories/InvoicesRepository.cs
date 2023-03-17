using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using GirlyMerely.Core.Models;
using GirlyMerely.Infrastructure.Dtos.Order;

namespace GirlyMerely.Infrastructure.Repositories
{
    public class InvoicesRepository : BaseRepository<Invoice, MyDbContext>
    {
        private readonly MyDbContext _context;
        private readonly LogsRepository _logger;
        public InvoicesRepository(MyDbContext context, LogsRepository logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }


        public List<Invoice> GetInvoices()
        {
            return _context.Invoices.Include(i => i.Customer.User).Where(i => i.IsDeleted == false).ToList();
        }
        //public Invoice GetInvoice(int invoiceId)
        //{
        //    return _context.Invoices.Include(i => i.Customer.User).Where(i => i.IsDeleted == false).Include(i=>i.InvoiceItems).FirstOrDefault(i=>i.Id == invoiceId);
        //}
        public Invoice GetInvoice(int invoiceId)
        {
            return _context.Invoices.Include(i => i.Customer.User).Where(i => i.IsDeleted == false).Include(i => i.InvoiceItems).FirstOrDefault(i => i.Id == invoiceId);
        }

        public Invoice GetInvoice(string invoiceNumber)
        {
            return _context.Invoices.Include(i => i.Customer.User).Include(i => i.InvoiceItems).Include(i => i.DiscountCode).FirstOrDefault(i => i.InvoiceNumber == invoiceNumber);
        }

        //public string GetInvoiceItemsMainFeature(int invoiceItemId)
        //{
        //    var invoiceItem = _context.InvoiceItems.Find(invoiceItemId);
        //    var mainFeature = _context.ProductMainFeatures.Include(m=>m.SubFeature).FirstOrDefault(m=>m.Id == invoiceItem.MainFeatureId);
        //    if (mainFeature.SubFeature == null)
        //        return mainFeature.Value;
        //    return mainFeature.SubFeature.Value;
        //}
        public string GetInvoiceItemsMainFeature(int invoiceItemId)
        {
            var invoiceItem = _context.InvoiceItems.Find(invoiceItemId);
            var mainFeature = _context.ProductMainFeatures.Include(m => m.SubFeature).FirstOrDefault(m => m.Id == invoiceItem.MainFeatureId);
            if (mainFeature.SubFeature == null)
                return mainFeature.Value;
            return mainFeature.SubFeature.Value;
        }

        //public InvoiceItem GetInvoiceItemsById(int id)
        //{
        //    return _context.InvoiceItems.Include(i=>i.Product).FirstOrDefault(i => i.Id == id);
        //}
        public InvoiceItem GetInvoiceItemsById(int id)
        {
            return _context.InvoiceItems.Include(i => i.Product).FirstOrDefault(i => i.Id == id);
        }

        public List<Product> GertTopSoldProducts(int take)
        {
            List<Product> products = new List<Product>();
            var productIds = _context.InvoiceItems.GroupBy(i => i.ProductId)
                .OrderByDescending(pi => pi.Count())
                .Select(g => g.Key).ToList();
            foreach (var id in productIds)
            {
                if (products.Count < take)
                {
                    var product = _context.Products.FirstOrDefault(p => p.Id == id);
                    if (product != null && product.IsDeleted == false)
                    {
                        products.Add(product);
                    }
                }
            }

            return products;
        }

        public List<Invoice> GetCustomerInvoices(int customerId)
        {
            return _context.Invoices.Where(i => i.IsDeleted == false && i.CustomerId == customerId).OrderByDescending(p => p.Id).ToList();
        }

        public InvoiceItem AddInvoiceItem(InvoiceItem invoiceItem)
        {
            var user = GetCurrentUser();
            invoiceItem.InsertDate = DateTime.Now;
            if (user != null)
                invoiceItem.InsertUser = user.UserName;

            _context.InvoiceItems.Add(invoiceItem);
            _context.SaveChanges();

            return invoiceItem;
        }
        public List<InvoiceItem> GetInvoiceItemsByInvoiceId(int id)
        {
            return _context.InvoiceItems.Include(i => i.Product).Where(i => i.IsDeleted == false && i.InvoiceId == id)
                .ToList();
        }
        public Invoice GetInvoiceWithGeo(int id)
        {
            return _context.Invoices.Include(i => i.GeoDivision).FirstOrDefault(i => i.Id == id);
        }


        public Order GetPrintDTO(int id)
        {
            var Invoice = _context.Invoices.Find(id);
            Order _FactorDetails = new Order()
            {
                CustomerName = Invoice.CustomerName,
                Address = Invoice.Address,
                Phone = Invoice.Phone,
                PostalCode = Invoice.PostalCode,
                ShipingDate = Invoice.ShipingDate,
                TotalPrice = Invoice.TotalPrice,
                DiscountAmount = Invoice.DiscountAmount,
                ShippingPriceAmount = Invoice.ShippingPriceAmount,
                TotalPriceBeforeDiscount = Invoice.TotalPrice,
                ShipingStatus = Invoice.ShipingStatus,
                PersianDate = DateTime.Now.ToLocalTime().ToShortDateString(),

                InvoiceNumber = Invoice.InvoiceNumber


            };

            List<OrderItems> Items = new List<OrderItems>();
            var orderitem = _context.InvoiceItems.Where(x => x.InvoiceId == id).ToList();
            foreach (var item in orderitem)
            {
                var Producte = _context.Products.Find(item.ProductId);
                Items.Add(new OrderItems()
                {
                    ProductId = Producte.Id.ToString(),
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Title = Producte.Title
                });
            }

            _FactorDetails.OrderItems.AddRange(Items);

            return _FactorDetails;
        }
    }
}
