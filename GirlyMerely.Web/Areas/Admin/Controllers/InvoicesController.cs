using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GirlyMerely.Core.Models;
using GirlyMerely.Core.Utility;
using GirlyMerely.Infrastructure;
using GirlyMerely.Infrastructure.Dtos.Order;
using GirlyMerely.Infrastructure.Repositories;
using GirlyMerely.Web.ViewModels;
using Stimulsoft.Base;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;

namespace GirlyMerely.Web.Areas.Admin.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly InvoicesRepository _repo;
        private readonly GeoDivisionsRepository _GeoRepo;
        private readonly MyDbContext _context;

        public InvoicesController(InvoicesRepository repo, GeoDivisionsRepository geoRepo, MyDbContext context)
        {
            StiLicense.LoadFromString("6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkcgIvwL0jnpsDqRpWg5FI5kt2G7A0tYIcUygBh1sPs7koivWV0htru4Pn2682yhdY3+9jxMCVTKcKAjiEjgJzqXgLFCpe62hxJ7/VJZ9Hq5l39md0pyydqd5Dc1fSWhCtYqC042BVmGNkukYJQN0ufCozjA/qsNxzNMyEql26oHE6wWE77pHutroj+tKfOO1skJ52cbZklqPm8OiH/9mfU4rrkLffOhDQFnIxxhzhr2BL5pDFFCZ7axXX12y/4qzn5QLPBn1AVLo3NVrSmJB2KiwGwR4RL4RsYVxGScsYoCZbwqK2YrdbPHP0t5vOiLjBQ+Oy6F4rNtDYHn7SNMpthfkYiRoOibqDkPaX+RyCany0Z+uz8bzAg0oprJEn6qpkQ56WMEppdMJ9/CBnEbTFwn1s/9s8kYsmXCvtI4iQcz+RkUWspLcBzlmj0lJXWjTKMRZz+e9PmY11Au16wOnBU3NHvRc9T/Zk0YFh439GKd/fRwQrk8nJevYU65ENdAOqiP5po7Vnhif5FCiHRpxgF");
            _repo = repo;
            _GeoRepo = geoRepo;
            _context = context;
        }

        public  ActionResult Print(int invoiceId)
        {
            TempData["id"] = invoiceId;
            return View();
        }
      

        public  ActionResult report(/*int invoiceId*/)
        {
            var invoiceId = (int)TempData["id"];
            StiReport report = new StiReport();
            var Order = _repo.GetPrintDTO(invoiceId);
            report.Load(Server.MapPath("~/Reports/Report.mrt"));
            report.Compile();
            report.RegBusinessObject("Order", Order);
            return StiMvcViewer.GetReportSnapshotResult(report);
        }


        public virtual ActionResult ExportReport()
        {
            return StiMvcViewer.ExportReportResult();
        }


        public virtual ActionResult PrintReport()
        {
            return StiMvcViewer.PrintReportResult(this.HttpContext);
        }


        [AllowAnonymous]
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
                ShipingStatus = Invoice.ShipingStatus
            };

            List<OrderItems> Items = new List<OrderItems>();
            var orderitem = _context.InvoiceItems.Where(x => x.Id == id).ToList();
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


        [AllowAnonymous]
        public ActionResult viewerEvent()
        {
            return StiMvcViewer.ViewerEventResult(HttpContext);
        }


        public ActionResult Index()
        {
            var invoices = _repo.GetInvoices();
            var vm = new List<InvoiceTableViewModel>();
            foreach (var invoice in invoices)
            {
                vm.Add(new InvoiceTableViewModel(invoice));
            }
            return View(vm.OrderByDescending(v=>v.PersianDate).ToList());
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = _repo.Get(id.Value);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            ViewBag.GeoDivisionId = new SelectList(_GeoRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", invoice.GeoDivisionId);
            return View(invoice);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Invoice invoice)
        {
            if (ModelState.IsValid)
            {

                _repo.Update(invoice);
                return RedirectToAction("Index");
            }
            ViewBag.GeoDivisionId = new SelectList(_GeoRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", invoice.GeoDivisionId);
            return View(invoice);
        }


        //public ActionResult ViewInvoice(int invoiceId)
        //{
        //    var vm = new ViewInvoiceViewModel();
        //    var invoice = _repo.GetInvoice(invoiceId);
        //    vm.Invoice = invoice;
        //    vm.ShipingDate = invoice.ShipingDate;
        //    vm.PersianDate = new PersianDateTime(invoice.AddedDate).ToString();
        //    vm.InvoiceItems = new List<InvoiceItemWithMainFeatureViewModel>();
        //    // Getting Invoice Item SubFeatures
        //    foreach (var invoiceItem in invoice.InvoiceItems)
        //    {
        //        var invoiceItemWithMainFeature = new InvoiceItemWithMainFeatureViewModel
        //        {
        //            InvoiceItem = _repo.GetInvoiceItemsById(invoiceItem.Id),
        //            MainFeature = _repo.GetInvoiceItemsMainFeature(invoiceItem.Id)
        //        };
        //        vm.InvoiceItems.Add(invoiceItemWithMainFeature);

        //    }
        //    return View(vm);
        //}
        public ActionResult ViewInvoice(int invoiceId)
        {
            var vm = new ViewInvoiceViewModel();
            var invoice = _repo.GetInvoice(invoiceId);
            vm.Invoice = invoice;
            vm.ShipingDate = invoice.ShipingDate;
            vm.PersianDate = new PersianDateTime(invoice.AddedDate).ToString();
            vm.InvoiceItems = new List<InvoiceItemWithMainFeatureViewModel>();
            // Getting Invoice Item SubFeatures
            foreach (var invoiceItem in invoice.InvoiceItems)
            {
                var invoiceItemWithMainFeature = new InvoiceItemWithMainFeatureViewModel
                {
                    InvoiceItem = _repo.GetInvoiceItemsById(invoiceItem.Id),
                    MainFeature = _repo.GetInvoiceItemsMainFeature(invoiceItem.Id)
                };
                vm.InvoiceItems.Add(invoiceItemWithMainFeature);

            }
            return View(vm);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = _repo.Get(id.Value);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersianDate = new PersianDateTime(invoice.AddedDate).ToString();
            return PartialView(invoice);
        }


        public ActionResult ShipingStatus(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = _repo.Get(id.Value);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersianDate = new PersianDateTime(invoice.AddedDate).ToString();
            return PartialView(invoice);
        }


        [HttpPost, ActionName("ShipingStatus")]
        [ValidateAntiForgeryToken]
        public ActionResult ShipingStatus(int id)
        {
            var invoice = _repo.Get(id);
            invoice.ShipingStatus = true;
            _repo.Update(invoice);
            return RedirectToAction("Index");
        }


        // POST: Admin/Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var invoice = _repo.Get(id);
            _repo.Delete(id);
            return RedirectToAction("Index");
        }

    }
}