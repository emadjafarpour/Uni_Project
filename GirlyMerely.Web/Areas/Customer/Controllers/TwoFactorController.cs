using GirlyMerely.Core.Utility;
using GirlyMerely.Infrastructure.Repositories;
using GirlyMerely.Web.Areas.Customer.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GirlyMerely.Web.Areas.Customer.Controllers
{
    public class TwoFactorController : Controller
    {

        private readonly CustomersRepository _customersRepository;

        private readonly UsersRepository _usersRepository;

        public TwoFactorController(CustomersRepository customersRepository
        ,UsersRepository usersRepository)
        {
            _usersRepository = usersRepository;

            _customersRepository = customersRepository;
        }
        // GET: Customer/TwoFactor
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ActivationCode(string UserId, string Code)
        {
            var user = _usersRepository.GetUser(UserId);

            if (user.ActivationCode == Code)
            {
                user.IsActive = true;

                _usersRepository.UpdateUser(user);

                return RedirectToAction("Login", "Auth");
            }

            ActivationCodeViewModel model1 = new ActivationCodeViewModel()
            {
                PhoneNumber = user.PhoneNumber,
                UserId = user.Id
            };

            ViewBag.result = "fail";

            return View("ActivationCode", model1);

        }
    }
}