using GirlyMerely.Core.Models;
using GirlyMerely.Core.Utility.Convertor;
using GirlyMerely.Infrastructure.Repositories;
using GirlyMerely.Infratructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GirlyMerely.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class DiscountCodesController : Controller
    {
        private readonly ShoppingRepository _shoppingRepository;
        private readonly CustomersRepository _customersRepository;

        public DiscountCodesController(ShoppingRepository shoppingRepository,
            CustomersRepository customersRepository)
        {
            _shoppingRepository = shoppingRepository;
            _customersRepository = customersRepository;
        }

        //public ActionResult Index()
        //{
        //    List<DiscountCode> discountCodes = _shoppingRepository.GetDiscountCodes();

        //    return View(discountCodes);
        //}
        //public ActionResult Create()
        //{
        //    ViewBag.CustomerId = new SelectList(_customersRepository.GetCustomers(), "Id","User.FirstName");

        //    return View();
        //}
        //[HttpPost]
        //public ActionResult Create(DiscountCode discountCode)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (!HttpContext.User.Identity.IsAuthenticated)
        //        {
        //            ViewBag.Message = "کاربر واردکننده پیدا نشد !";

        //            return View(discountCode);
        //        }

                
        //        //Convert to Miladi
        //        discountCode.ActivationStartDate = discountCode.ActivationStartDate.ToMiladi();
        //        discountCode.ActivationEndDate = discountCode.ActivationEndDate.ToMiladi();
        //        _shoppingRepository.Add(discountCode);

        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewBag.CustomerId = new SelectList(_customersRepository.GetCustomers(), "Id", "User.FirstName", discountCode.CustomerId);
        //    return View();
        //}
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    DiscountCode discountCode = _shoppingRepository.Get(id.Value);
        //    if (discountCode == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    // convert to shamsi
        //    discountCode.ActivationStartDate = discountCode.ActivationStartDate.ToShamsiDateTime();
        //    discountCode.ActivationEndDate = discountCode.ActivationEndDate.ToShamsiDateTime();

        //    ViewBag.CustomerId = new SelectList(_customersRepository.GetCustomers(), "Id", "User.FirstName", discountCode.CustomerId);
        //    return View(discountCode);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(DiscountCode discountCode)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //Convert to Miladi
        //        discountCode.ActivationStartDate = discountCode.ActivationStartDate.ToMiladi();
        //        discountCode.ActivationEndDate = discountCode.ActivationEndDate.ToMiladi();

        //        _shoppingRepository.Update(discountCode);

        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.CustomerId = new SelectList(_customersRepository.GetCustomers(), "Id", "User.FirstName", discountCode.CustomerId);
        //    return View(discountCode);
        //}
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    DiscountCode discountCode = _shoppingRepository.Get(id.Value);

        //    if (discountCode == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return PartialView(discountCode);
        //}
        //[HttpPost,ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    _shoppingRepository.Delete(id);

        //    return RedirectToAction(nameof(Index));
        //}
    }
}