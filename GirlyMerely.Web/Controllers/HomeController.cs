using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GirlyMerely.Core.Models;
using GirlyMerely.Core.Utility;
using GirlyMerely.Infrastructure.Repositories;
using GirlyMerely.Infratructure.Repositories;
using GirlyMerely.Infratructure.Services;
using GirlyMerely.Web.ViewModels;

namespace GirlyMerely.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly StaticContentDetailsRepository _staticContentRepo;
        private readonly OffersRepository _offersRepo;
        private readonly ProductService _productService;
        private readonly TestimonialsRepository _testimonialRepo;
        private readonly PartnersRepository _partnersRepo;
        private readonly ArticlesRepository _articlesRepo;
        private readonly ContactFormsRepository _contactFormRepo;
        private readonly ProductGroupsRepository _productGroupRepo;

        public HomeController(StaticContentDetailsRepository staticContentRepo,
            OffersRepository offersRepo,
            ProductService productService,
            TestimonialsRepository testimonialRepo,
            PartnersRepository partnersRepo,
            ArticlesRepository articlesRepo,
            ContactFormsRepository contactFormRepo,
         
            ProductGroupsRepository productGroupRepo)
        {
            _staticContentRepo = staticContentRepo;
            _offersRepo = offersRepo;
            _productService = productService;
            _testimonialRepo = testimonialRepo;
            _partnersRepo = partnersRepo;
            _articlesRepo = articlesRepo;
            _contactFormRepo = contactFormRepo;
            _productGroupRepo = productGroupRepo;
        
        }

        public ActionResult Index()
        {           
            return View();
        }


        // Main Menu
        public ActionResult HeaderSection()
        {
            var mainProductGroups = _productGroupRepo.GetChildrenProductGroups();
            ViewBag.MainProductGroups = mainProductGroups;
            ViewBag.Phone = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.Phone)?.Title;
            return PartialView();
        }


        public ActionResult SliderSection()
        {
            var content = _staticContentRepo.GetContentByTypeId((int)StaticContentTypes.Slider);
            return PartialView(content);
        }


        public ActionResult TopSoldProductsSection(int take)
        {
            var products = _productService.GetTopSoldProductsWithPrice(take).OrderByDescending(a => a.Price).ToList();

            return PartialView(products);
        }


        public ActionResult PopularProductGroupsSection(int take)
        {
            var products = _productService.GetPopularProductGroups(take);

            return PartialView(products);
        }


        public ActionResult LatestProductsSection(int take)
        {
            var products = _productService.GetLatestProductsWithPrice(take).OrderByDescending(a=>a.Price).ToList();
            return PartialView(products);
     
        }


        public ActionResult OffersSection()
        {
            var offers = _offersRepo.GetAll();
            offers = offers.OrderBy(o => o.Id).ToList();

            return PartialView(offers);
        }

        public ActionResult OffersSection2()
        {
            var offer = _offersRepo.Get(4);

            return PartialView(offer);
        }

        public ActionResult LatestProductsSection2(int take, int? skip)
        {
            var products = _productService.GetLatestProductsWithPrice(take, skip).OrderByDescending(a => a.Price).ToList();
            return PartialView(products);
        }


        public ActionResult PartnersSection()
        {
            var partners = _partnersRepo.GetAll();
            return PartialView(partners);
        }


        public ActionResult CartSection()
        {
            try
            {
                var cartModel = new CartModel();
                cartModel.CartItems = new List<CartItemModel>();

                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (Session["cart"] != null && Session["cart"].ToString() != "")
                {
                    string cartJsonStr = Session["cart"].ToString();
                    cartModel = new CartModel(cartJsonStr);
                }

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    if (cartModel.CartItems == null || cartModel.CartItems.Count == 0) // sessions has not been used
                    {
                        string cartJsonStr = cartCookie.Values["cart"];
                        string[] arrayValues = cartJsonStr.Split(',');
                        string[] Quantity = arrayValues[4].Split(':');
                        var ValueQuantity = Convert.ToInt32(Quantity[1]);
                        if (ValueQuantity > 0) { cartModel = new CartModel(cartJsonStr); }
                        
                    }
                }

                return PartialView(cartModel);

            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;

                var cartModel = new CartModel();
                cartModel.CartItems = new List<CartItemModel>();

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }
                return PartialView(cartModel);

            }
        }


        public ActionResult TestimonialsSection()
        {
            var testimonials = _testimonialRepo.GetAll();
            var vm = testimonials.Select(testimonial => new TestimonialViewModel(testimonial)).ToList();

            return PartialView(vm);
        }


        public ActionResult LatestArticlesSection()
        {
            var articles = _articlesRepo.GetLatestArticles(3);
            var vm = articles.Select(item => new LatestArticlesViewModel(item)).ToList();

            return PartialView(vm);
        }


        public ActionResult Shop()
        {
            return View();
        }


        public ActionResult ShopList()
        {
            return View();
        }


        public ActionResult Product()
        {
            return View();
        }


        public ActionResult Login()
        {
            return View();
        }


        public ActionResult Register()
        {
            return View();
        }


        public ActionResult WhishList()
        {
            return View();
        }


        public ActionResult Blog()
        {
            return View();
        }


        public ActionResult BlogDetails()
        {
            return View();
        }


        public ActionResult Guide()
        {
            var guide = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.Guide);
            if (guide != null)
                ViewBag.Guide = guide.ShortDescription;
            return View();
        }


        public ActionResult Checkout()
        {   
            return View();
        }


        //[HttpPost]
        //public ActionResult Pay()
        //{
        //    var a = new MellatGateway.bpPayRequest()
        //    {
        //    };
        //    a.Body.
        //    return View();
        //}
        public ActionResult Cart()
        {
            return View();
        }


        [Route("AboutUs")]
        public ActionResult About()
        {
            var content = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.AboutUs);
            ViewBag.AboutUs = content;
            return View();
        }


        [Route("ContactUs")]
        public ActionResult Contact()
        {
            var map = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.ContactUsMap);
            var phone = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.Phone);
            var email = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.Email);
            var address = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.Address);
            var vm = new ContactUsViewModel()
            {
                Map = map.Description,
                Phone = phone.Title,
                Email = email.Title,
                Address = address.Title
            };
            
            return View(vm);
        }


       public ActionResult Social()
        {
            var vm = new SocialViewModel()
            {
                instagram = _staticContentRepo.Get(12).Link,
                twiter = _staticContentRepo.Get(13).Link,
                facebook = _staticContentRepo.Get(16).Link,
                google = _staticContentRepo.Get(14).Link,
                youtube = _staticContentRepo.Get(15).Link,
                whatsap = _staticContentRepo.Get(11).Link
            };
            ViewBag.phone = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.Phone).Title;
            return PartialView(vm);
        }


        [Route("Terms")]
        public ActionResult Terms()
        {
            var content = _staticContentRepo.GetSingleContentByTypeId((int)StaticContentTypes.Terms);
            ViewBag.Terms = content;

            return View();
        }


        public ActionResult ContactUsForm()
        {
            return PartialView();
        }


        [HttpPost]
        public ActionResult ContactUsForm(ContactForm contactForm)
        {
            if (ModelState.IsValid)
            {
                _contactFormRepo.Add(contactForm);
                return RedirectToAction("ContactUsSummary");
            }
            return View(contactForm);
        }


        public ActionResult ContactUsSummary()
        {
            return View();
        }


        public ActionResult UploadImage(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string vImagePath = String.Empty;
            string vMessage = String.Empty;
            string vFilePath = String.Empty;
            string vOutput = String.Empty;
            try
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    var vFileName = DateTime.Now.ToString("yyyyMMdd-HHMMssff") +
                                    Path.GetExtension(upload.FileName).ToLower();
                    var vFolderPath = Server.MapPath("/Upload/");
                    if (!Directory.Exists(vFolderPath))
                    {
                        Directory.CreateDirectory(vFolderPath);
                    }
                    vFilePath = Path.Combine(vFolderPath, vFileName);
                    upload.SaveAs(vFilePath);
                    vImagePath = Url.Content("/Upload/" + vFileName);
                    vMessage = "Image was saved correctly";
                }
            }
            catch
            {
                vMessage = "There was an issue uploading";
            }
            vOutput = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + vImagePath + "\", \"" + vMessage + "\");</script></body></html>";
            return Content(vOutput);
        }
    }
}