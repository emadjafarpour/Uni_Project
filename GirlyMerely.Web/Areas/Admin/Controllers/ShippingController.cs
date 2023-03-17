using GirlyMerely.Core.Models;
using GirlyMerely.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GirlyMerely.Web.Areas.Admin.Controllers
{
    public class ShippingController : Controller
    {
        public readonly ShippingRepository _repo;

        public ShippingController(ShippingRepository shippingRepository)
        {
            _repo = shippingRepository;
        }

        // GET: Admin/Shipping
        public ActionResult Index()
        {
            var shippingPrices = _repo.GetAll();
            return View(shippingPrices);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(ShippingPrice shippingPrice)
        {
            if(ModelState.IsValid)
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    ViewBag.Message = "کاربر وارد کننده پیدا نشد.";
                    return View(shippingPrice);
                }

                _repo.Add(shippingPrice);
                return Redirect("/Admin/Shipping");
            }

            return View();
        }

        public ActionResult Edit(int? id)
        {
            var shippingPrice = _repo.Get(id.Value);
            if (shippingPrice == null)
                return Redirect("/Admin/Shipping");

            return View(shippingPrice);
        }

        [HttpPost]
        public ActionResult Edit(ShippingPrice shippingPrice)
        {
            if (ModelState.IsValid)
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    ViewBag.Message = "کاربر وارد کننده پیدا نشد.";
                    return View(shippingPrice);
                }

                _repo.Update(shippingPrice);
                return Redirect("/Admin/Shipping");
            }
            return View();
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var shippingPrice = _repo.Get(id.Value);
            if (shippingPrice == null)
                return Redirect("/Admin/Shipping");

            return PartialView(shippingPrice);
        }

        // POST: Admin/Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var shippingPrice = _repo.Get(id);

            //#region Delete Article Image
            //if (article.Image != null)
            //{
            //    if (System.IO.File.Exists(Server.MapPath("/Files/ArticleImages/Image/" + article.Image)))
            //        System.IO.File.Delete(Server.MapPath("/Files/ArticleImages/Image/" + article.Image));

            //    if (System.IO.File.Exists(Server.MapPath("/Files/ArticleImages/Thumb/" + article.Image)))
            //        System.IO.File.Delete(Server.MapPath("/Files/ArticleImages/Thumb/" + article.Image));
            //}
            //#endregion

            _repo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}