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
    public class HolidayDateController : Controller
    {
        private readonly HolidyDateRepository _holidayDateRepository;
        public HolidayDateController(HolidyDateRepository holidayDateRepository)
        {
            _holidayDateRepository = holidayDateRepository;
        }
        // GET: Admin/HolidayDate
        public ActionResult Index()
        {
            return View(_holidayDateRepository.GetAll().Where(a => a.IsDeleted == false).OrderByDescending(a => a.Id).ToList()); ;
        }

        // GET: Admin/HolidayDate/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HolidyDate HolidyDate,string datepicker)
        {
            if (ModelState.IsValid)
            {
                HolidyDate.HolidyDates = datepicker;
                _holidayDateRepository.Add(HolidyDate);
                return RedirectToAction("Index");
            }

            return View(HolidyDate);
        }



        // GET: Admin/HolidayDate/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HolidyDate HolidyDate = _holidayDateRepository.Get(id.Value);
            if (HolidyDate == null)
            {
                return HttpNotFound();
            }
            return PartialView(HolidyDate);
        }

        // POST: Admin/HolidayDate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _holidayDateRepository.Delete(id);
            return RedirectToAction("Index");
        }


    }
}