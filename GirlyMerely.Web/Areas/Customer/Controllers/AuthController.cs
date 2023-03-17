using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using GirlyMerely.Core.Models;
using GirlyMerely.Core.Utility;
using GirlyMerely.Infrastructure.Repositories;
using GirlyMerely.Web.ViewModels;
using GirlyMerely.Web.Areas.Customer.Models;
using ResetPasswordViewModel = GirlyMerely.Web.Areas.Customer.Models.ResetPasswordViewModel;

namespace GirlyMerely.Web.Areas.Customer.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private UsersRepository _userRepo;

        public AuthController()
        {
        }

        //public AuthController(CustomersRepository customersRepository)
        //{
        //    _CustomersRepository = customersRepository;
        //}
        public UsersRepository UserRepo
        {
            get
            {
                return _userRepo ?? new UsersRepository();
            }
            private set
            {
                _userRepo = value;
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //public CustomersRepository CustomersRepository

        //{
        //    get
        //    {
        //        return _CustomersRepository ?? new CustomersRepository();
        //    }
        //    private set
        //    {
        //        _CustomersRepository = value;
        //    }
        //}



        //
        // GET: /Auth/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [AllowAnonymous]
        public ActionResult LoginPartial(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView();
        }
        [AllowAnonymous]
        public ActionResult RegisterPartial(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView();
        }
        [AllowAnonymous]
        public ActionResult AccessDenied(string returnUrl = null)
        {
            ViewBag.ReturnUrl = "/Admin/Dashboard/Index";
            if (!string.IsNullOrEmpty(returnUrl))
                ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        //
        // POST: /Auth/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(ViewModels.LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards Auth lockout
            // To enable password failures to trigger Auth lockout, change to shouldLockout: true
            var user = UserManager.FindByName(model.UserName);
            if (user == null)
            {
                ViewBag.LoginError = "نام کاربری وارد شده صحیح نیست.";
                //ModelState.AddModelError(string.Empty, "نام کاربری وارد شده صحیح نیست.");
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        if (!string.IsNullOrEmpty(returnUrl))
                            return RedirectToLocal(returnUrl); // Or use returnUrl

                        return RedirectToLocal("/Customer/Dashboard"); // Or use returnUrl
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ViewBag.LoginError = "نام کاربری یا رمز عبور وارد شده صحیح نیست.";
                    //ModelState.AddModelError("", "نام کاربری وارد شده صحیح نیست.");
                    return View(model);
            }
        }

        //
        // GET: /Auth/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Auth/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterCustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                #region Check for duplicate username or email
                if (UserRepo.UserNameExists(model.UserName))
                {
                    ViewBag.RegisterError = "نام کاربری قبلا ثبت شده.";
                    ModelState.AddModelError("", "نام کاربری قبلا ثبت شده");
                    return View(model);
                }
                if (model.Email != null)
                {
                    if (UserRepo.EmailExists(model.Email))
                    {
                        ViewBag.RegisterError = "ایمیل قبلا ثبت شده.";
                        ModelState.AddModelError("", "ایمیل قبلا ثبت شده");
                        return View(model);
                    }
                }

                //if (model.PhoneNumbre != null)
                //{
                //    if (UserRepo.PhoneNumberExists(model.PhoneNumbre))
                //    {
                //        ViewBag.RegisterError = "شماره موبایل قبلا ثبت شده است.";
                //        //ModelState.AddModelError("", "ایمیل قبلا ثبت شده");
                //        return View(model);
                //    }
                //}
                #endregion

                var user = new User { UserName = model.UserName, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName,
                    PhoneNumberConfirmed = false, PhoneNumber = model.PhoneNumbre, IsActive = false, ActivationCode = "" };
                UserRepo.CreateUser(user, model.Password);


                //var user = new GirlyMerely.Web.Models.ApplicationUser { UserName = model.UserName, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, PhoneNumberConfirmed = false, PhoneNumber = model.PhoneNumbre, IsActive = false, ActivationCode = "" };
                //var result = await UserManager.CreateAsync(user, model.Password);

                if (user.Id != null)
                {
                    // Add Customer Role
                    UserRepo.AddUserRole(user.Id, StaticVariables.CustomerRoleId);


                    Random _random = new Random();

                    string code = _random.Next(1000, 9999).ToString();
                    // Add Customer
                    var customer = new Core.Models.Customer()
                    {
                        UserId = user.Id,
                        IsDeleted = false,
                        //GeoDivisionId=24
                        //IsActive=false,
                        //ActivationCode=code
                    };
                    UserRepo.AddCustomer(customer);
                    user.ActivationCode = code;
                    UserRepo.UpdateUser(user);
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable Auth confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your Auth", "Please confirm your Auth by clicking <a href=\"" + callbackUrl + "\">here</a>");



                    //var message = $"به فروشگاه گرلی مرلی خوش آمدید{Environment.NewLine} کد تایید شما:{code}";
                    //SmsHelper.SendActivationCode(model.PhoneNumbre, message);

                    //فرستادن کد تایید عضویت
                    SmsHelper.SendActivationCode(model.PhoneNumbre, code);
                    //فرستادن کد تایید عضویت پایان 

                    ActivationCodeViewModel model1 = new ActivationCodeViewModel()
                    {
                        PhoneNumber = model.PhoneNumbre,
                        UserId = user.Id
                    };


                    return View("ActivationCode", model1);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ForgetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserRepo.GetUsers().Where(x => x.PhoneNumber == model.phonenumber).SingleOrDefault();

                var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                var passwordResetLink = Url.Action("ResetPassword", "Auth", new { phonenumber = model.phonenumber, token = token });

                string shortlink = UserRepo.ShortLink(passwordResetLink);

                Uri uri = new Uri("http://www.girlymerely.com" + "/s/" + shortlink + "/");

                SmsHelper.SendResetPasswordLink(Convert.ToInt64(user.PhoneNumber), shortlink);

                ViewBag.State = true;

                return View();
            }

            ViewBag.State = false;

            return View(model);
        }

        [Route("s/{key}")]
        [AllowAnonymous]
        public ActionResult ShortKeyRedirect(string key)
        {
            var link = UserRepo.GetAllLink().Where(x => x.ShortLink == key).FirstOrDefault().OrginalLink;

            Uri uri = new Uri("http://www.girlymerely.com" + link);

            return Redirect(uri.AbsoluteUri);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string token, string phonenumber)
        {
            NewPasswordViewModel newPasswordViewModel = new NewPasswordViewModel()
            {
                token = token,
                phoneNumber = phonenumber,

            };

            return View(newPasswordViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(NewPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.Users.Where(x => x.PhoneNumber == model.phoneNumber).SingleOrDefault();

                if (user != null)
                {
                    var result = await UserManager.ResetPasswordAsync(user.Id, model.token, model.Password);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login");
                    }
                    return View(model);
                }
                return View(model);
            }
            return View(model);
        }

        // GET: /Auth/ForgotPasswordConfirmation
        [HttpPost]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        // GET: /Account/ExternalLoginFailure
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Dashboard");
        }
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }


        #endregion
    }
}