using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Deployment.Internal;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using GirlyMerely.Core.Models;
using GirlyMerely.Core.Utility;
using GirlyMerely.Infrastructure;
using GirlyMerely.Infrastructure.Helpers;
using GirlyMerely.Infrastructure.Repositories;
using GirlyMerely.Infratructure.Dtos.Product;
using GirlyMerely.Infratructure.Services;
using GirlyMerely.Web.MellatGateway;
using GirlyMerely.Web.Providers;
using GirlyMerely.Web.ViewModels;
using GirlyMerely.Infratructure.Repositories;
using Newtonsoft.Json;
using GirlyMerely.Core.Utility;

namespace GirlyMerely.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly ProductService _productService;
        private readonly ProductsRepository _productsRepo;
        private readonly ProductGroupsRepository _productGroupRepo;
        private readonly FeaturesRepository _featuresRepo;
        private readonly BrandsRepository _brandsRepo;
        private readonly ProductGalleriesRepository _productGalleryRepo;
        private readonly ProductCommentsRepository _productCommentsRepository;
        private readonly ProductMainFeaturesRepository _productMainFeaturesRepo;
        private readonly ProductFeatureValuesRepository _productFeatureValueRepo;
        private readonly GeoDivisionsRepository _geoDivisionRepo;
        private readonly CustomersRepository _customerRepo;
        private readonly UsersRepository _userRepo;
        private readonly InvoicesRepository _invoiceRepo;
        private readonly ShoppingRepository _shoppingRepo;
        private readonly ShippingRepository _shippingRepo;
        private readonly OffersRepository _offerRepo;
        private readonly HolidyDateRepository _holidyDateRepository;
        private readonly LogsRepository _logsRepository;
        private readonly StaticContentDetailsRepository _staticContentDetailsRepository;
        private readonly StaticContentTypesRepository _staticContentTypesRepository;


        public ShopController(ProductService productService,
            ProductGroupsRepository productGroupRepo,
            FeaturesRepository featuresRepo,
            BrandsRepository brandsRepo, ProductsRepository productsRepo,
            ProductGalleriesRepository productGalleryRepo,
            ProductCommentsRepository productCommentsRepository,
            ProductMainFeaturesRepository productMainFeaturesRepo,
            ProductFeatureValuesRepository productFeatureValueRepo,
            GeoDivisionsRepository geoDivisionRepo,
            CustomersRepository customerRepo,
            UsersRepository userRepo,
            ShoppingRepository shoppingRepo,
            ShippingRepository shippingRepository,
            InvoicesRepository invoiceRepo,
            HolidyDateRepository holidyDateRepository,
            OffersRepository offersRepository,
            LogsRepository logsRepository,
            StaticContentDetailsRepository staticContentDetailsRepository,
            StaticContentTypesRepository staticContentTypesRepository)
        {
            _productService = productService;
            _productGroupRepo = productGroupRepo;
            _featuresRepo = featuresRepo;
            _brandsRepo = brandsRepo;
            _productsRepo = productsRepo;
            _productGalleryRepo = productGalleryRepo;
            _productCommentsRepository = productCommentsRepository;
            _productMainFeaturesRepo = productMainFeaturesRepo;
            _productFeatureValueRepo = productFeatureValueRepo;
            _geoDivisionRepo = geoDivisionRepo;
            _customerRepo = customerRepo;
            _userRepo = userRepo;
            _invoiceRepo = invoiceRepo;
            _shoppingRepo = shoppingRepo;
            _shippingRepo = shippingRepository;
            _offerRepo = offersRepository;
            _holidyDateRepository = holidyDateRepository;
            _logsRepository = logsRepository;
            _staticContentDetailsRepository = staticContentDetailsRepository;
            _staticContentTypesRepository = staticContentTypesRepository;
        }

        // GET: Products
        [Route("Shop/ProductList/{id}")]
        [Route("Shop/ProductList")]
        [Route("Shop/ProductList/Search/{searchString}")]
        [Route("Shop/ProductList/{id}/Search/{searchString}")]
        public ActionResult Index(int? id, string searchString = null)
        {
            int? offerId = null;
            try
            {
                offerId = int.Parse(Request.QueryString["offer"]);
            }
            catch
            {

            }

            ViewBag.OfferId = offerId;

            var vm = new ProductListViewModel();
            vm.SelectedGroupId = id ?? 0;
            var productGroups = new List<ProductGroup>();
            if ((id == null || id == 0) && offerId == null)
            {
                vm.Features = _featuresRepo.GetAllFeatures();
                vm.Brands = _brandsRepo.GetAll();

                var childrenGroups = _productGroupRepo.GetChildrenProductGroups();
                vm.ProductGroups = childrenGroups;
                ViewBag.Title = "محصولات";
            }
            else if (offerId != null)
            {
                vm.Features = _featuresRepo.GetAllFeatures();
                vm.Brands = _offerRepo.GetOfferBrands(offerId.Value);

                var childrenGroups = _offerRepo.GetOfferProductGroups(offerId.Value);
                vm.ProductGroups = childrenGroups;

                var offer = _offerRepo.Get(offerId.Value);
                if (offer != null)
                    ViewBag.Title = offer.Title;

            }
            else
            {
                vm.Features = _featuresRepo.GetAllGroupFeatures(id.Value);
                vm.Brands = _brandsRepo.GetAllGroupBrands(id.Value);
                var selectedProductGroup = _productGroupRepo.Get(id.Value);
                var childrenGroups = _productGroupRepo.GetChildrenProductGroups(id.Value);
                vm.ProductGroups = childrenGroups;
                ViewBag.ProductGroupName = selectedProductGroup.Title;
                ViewBag.ProductGroupId = selectedProductGroup.Id;
                ViewBag.Title = $"محصولات {selectedProductGroup.Title}";
            }

            ViewBag.SearchString = searchString;
            return View(vm);
        }


        [Route("ProductsGrid")]
        public ActionResult ProductsGrid(GridViewModel grid)
        {
            var products = new List<Product>();
            var brandsIntArr = new List<int>();

            if (string.IsNullOrEmpty(grid.brands) == false)
            {
                var brandsArr = grid.brands.Split('-').ToList();
                brandsArr.ForEach(b => brandsIntArr.Add(Convert.ToInt32(b)));
            }

            var subFeaturesIntArr = new List<int>();
            if (string.IsNullOrEmpty(grid.subFeatures) == false)
            {
                var subFeaturesArr = grid.subFeatures.Split('-').ToList();
                subFeaturesArr.ForEach(b => subFeaturesIntArr.Add(Convert.ToInt32(b)));
            }

            if (grid.OfferId == null || grid.OfferId < 1)
                products = _productService.GetProductsGrid(grid.categoryId, brandsIntArr, subFeaturesIntArr, grid.priceFrom, grid.priceTo, grid.searchString);
            else
                products = _productService.GetOfferProducts(grid.categoryId, brandsIntArr, grid.priceFrom, grid.priceTo, grid.searchString, grid.OfferId);

            #region Sorting

            if (grid.sort != "date")
            {
                switch (grid.sort)
                {
                    case "name":
                        products = products.OrderBy(p => p.Title).ToList();
                        break;
                    case "sale":
                        products = products.OrderByDescending(p => _productService.GetProductSoldCount(p)).ToList();
                        break;
                    case "price-high-to-low":
                        products = products.OrderByDescending(p => _productService.GetProductPriceAfterDiscount(p)).ToList();
                        break;
                    case "price-low-to-high":
                        products = products.OrderBy(p => _productService.GetProductPriceAfterDiscount(p)).ToList();
                        break;
                }
            }
            #endregion

            var count = products.Count;
            var skip = grid.pageNumber * grid.take - grid.take;
            int pageCount = (int)Math.Ceiling((double)count / grid.take);
            ViewBag.PageCount = pageCount;
            ViewBag.CurrentPage = grid.pageNumber;

            products = products.Skip(skip).Take(grid.take).ToList();

            var vm = new List<ProductWithPriceDto>();
            foreach (var product in products)
            {
                var item = _productService.CreateProductWithPriceDto(product);
                if (item!= null)
                    vm.Add(item);
            }

            return PartialView(vm.OrderByDescending(p=>p.Price).ThenByDescending(a => a.Id).ToList());
        }


        public ActionResult LatestProductsSidebar(int take)
        {
            var products = _productService.GetTopSoldProductsWithPrice(take);
            var vm = new List<ProductWithPriceViewModel>();
            foreach (var product in products)
                vm.Add(new ProductWithPriceViewModel(product));

            return PartialView(vm);
        }


        [Route("Shop/Product/{id}/{title}")]
        [Route("Shop/Product/{id}")]
        [Route("Shop/ProductDetails/{id}")]
        public ActionResult ProductDetails(int id)
        {
            var product = _productsRepo.GetProduct(id);
            var productGallery = _productGalleryRepo.GetProductGalleries(id);
            var productMainFeatures = _productMainFeaturesRepo.GetProductMainFeatures(id);
            var productFeatureValues = _productFeatureValueRepo.GetProductFeatures(id);
            var price = _productService.GetProductPrice(product);
            var priceAfterDiscount = _productService.GetProductPriceAfterDiscount(product);
            var productComments = _productCommentsRepository.GetProductComments(id);
            var productCommentsVm = new List<ProductCommentViewModel>();


            foreach (var item in productComments)
                productCommentsVm.Add(new ProductCommentViewModel(item));

            var discountPercentage = 0;
            if (price > 0)
            {
                //discountPercentage = (int)(priceAfterDiscount * 100 / price);
                discountPercentage = (int)(100 - (priceAfterDiscount * 100 / price));
            }

            var vm = new ProductDetailsViewModel()
            {
                Product = product,
                ProductGalleries = productGallery,
                ProductMainFeatures = productMainFeatures,
                ProductFeatureValues = productFeatureValues,
                Price = price,
                PriceAfterDiscount = priceAfterDiscount,
                ProductComments = productCommentsVm,
                DiscountPercentage = discountPercentage,
                ProductDeliveryMethod = _staticContentDetailsRepository.GetSingleContentByTypeId((int)StaticContentTypes.ProductDeliveryMethod),
                ProductReturnTime = _staticContentDetailsRepository.GetSingleContentByTypeId((int)StaticContentTypes.ProductReturnTime)
            };
                       
            return View(vm);
        }


        public ActionResult RelatedProductsSection(int productId, int take)
        {
            var products = _productService.GetRelatedProducts(productId, take);
            return PartialView(products);
        }


        [HttpPost]
        public ActionResult PostComment(CommentFormViewModel form)
        {
            if (ModelState.IsValid)
            {
                var comment = new ProductComment()
                {
                    ProductId = form.ProductId.Value,
                    ParentId = form.ParentId,
                    Name = form.Name,
                    Email = form.Email,
                    Message = form.Message,
                    AddedDate = DateTime.Now,
                };
                _productCommentsRepository.Add(comment);
                return RedirectToAction("ContactUsSummary", "Home");
            }
            return RedirectToAction("ProductDetails", new { id = form.ProductId });
        }


        public string GetProductPrice(int productId, int mainFeatureId)
        {
            var product = _productsRepo.Get(productId);
            var price = _productService.GetProductPrice(product, mainFeatureId);
            var productStockCount = _productService.GetProductStockCount(productId, mainFeatureId);
            var priceAfterDiscount = _productService.GetProductPriceAfterDiscount(product, mainFeatureId);
            var result = new
            {
                price = price.ToString("##,###"),
                priceAfterDiscount = priceAfterDiscount.ToString("##,###"),
                count = productStockCount
            };
            var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return jsonStr;
        }


        // Add to cart for ios device
        public string AddItemToCart(int productId, int? mainFeatureId, int count = 1)
        {

            count = count <= 0 ? 1 : count;
            var cartModel = new CartModel();
            var cartItemsModel = new List<CartItemModel>();
            if (Session["cart"] == null)
            {
                Session.Add("cart", "");
            }

            var response = "ran out";
            try
            {

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (Session["cart"] != null && Session["cart"].ToString() != "")
                {
                    string cartJsonStr = Session["cart"].ToString();
                    cartModel = new CartModel(cartJsonStr);
                    cartItemsModel = cartModel.CartItems;
                }
                else if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                    cartItemsModel = cartModel.CartItems;
                }
                #endregion

                ProductWithPriceDto product;
                int productStockCount;
                if (mainFeatureId == null)
                {
                    mainFeatureId = _productMainFeaturesRepo.GetByProductId(productId).Id;
                }
                product = _productService.CreateProductWithPriceDto(productId, mainFeatureId.Value);
                productStockCount = _productService.GetProductStockCount(productId, mainFeatureId.Value);
                var ProductMainFeatureName = _productService.GetProductMainFeatureName(productId, mainFeatureId.Value);

                if (productStockCount > 0)
                {
                    if (cartItemsModel.Any(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value))
                    {
                        if (((cartItemsModel.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value).Quantity) + count) <= productStockCount)
                        {
                            cartItemsModel.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value).Quantity += count;
                            cartModel.TotalPrice += (product.PriceAfterDiscount * count);
                            response = "added";
                        }
                        else
                        {
                            response = "ran out";
                        }
                    }
                    else
                    {
                        if (productStockCount >= count)
                        {
                            cartItemsModel.Add(new CartItemModel()
                            {
                                Id = product.Id,
                                ProductName = product.ShortTitle,
                                Price = product.PriceAfterDiscount,
                                Quantity = count,
                                MainFeatureId = mainFeatureId.Value,
                                Image = product.Image,
                                ProductMainFeatureName = ProductMainFeatureName

                            });
                            cartModel.TotalPrice += (product.PriceAfterDiscount * count);
                            response = "added";
                        }
                        else
                        {

                            response = "ran out";
                        }
                    }
                    cartModel.CartItems = cartItemsModel;
                    var jsonStr = HttpUtility.HtmlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(cartModel)).Replace(";", "");
                    //var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cartModel);

                    cartCookie.Values.Set("cart", jsonStr);

                    cartCookie.Expires = DateTime.Now.AddHours(12);
                    cartCookie.SameSite = SameSiteMode.Lax;

                    Response.Cookies.Add(cartCookie);

                    Session.Remove("cart");
                    Session.Add("cart", JsonConvert.SerializeObject(cartModel));
                }
                else
                {
                    response = "ran out";
                }
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
                response = "error";
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(response);
        }


        // Add to cart for other device
        [HttpPost]
        public string AddToCart(int productId, int? mainFeatureId, int count = 1)
        {
            var response = "ran out";
            try
            {
                count = count <= 0 ? 1 : count;
                var cartModel = new CartModel();
                var cartItemsModel = new List<CartItemModel>();

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                    cartItemsModel = cartModel.CartItems;
                }
                #endregion

                ProductWithPriceDto product;
                int productStockCount;
                if (mainFeatureId == null)
                {
                    mainFeatureId = _productMainFeaturesRepo.GetByProductId(productId).Id;
                }
                product = _productService.CreateProductWithPriceDto(productId, mainFeatureId.Value);
                productStockCount = _productService.GetProductStockCount(productId, mainFeatureId.Value);
                var ProductMainFeatureName = _productService.GetProductMainFeatureName(productId, mainFeatureId.Value);

                if (productStockCount > 0)
                {
                    if (cartItemsModel.Any(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value))
                    {
                        if (((cartItemsModel.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value).Quantity) + count) <= productStockCount)
                        {
                            cartItemsModel.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId.Value).Quantity += count;
                            cartModel.TotalPrice += (product.PriceAfterDiscount * count);
                            response = "added";
                        }
                        else
                        {
                            response = "ran out";
                        }
                    }
                    else
                    {
                        if (productStockCount >= count)
                        {
                            cartItemsModel.Add(new CartItemModel()
                            {
                                Id = product.Id,
                                ProductName = product.ShortTitle,
                                Price = product.PriceAfterDiscount,
                                Quantity = count,
                                MainFeatureId = mainFeatureId.Value,
                                Image = product.Image,
                                ProductMainFeatureName = ProductMainFeatureName
                            });
                            cartModel.TotalPrice += (product.PriceAfterDiscount * count);
                            response = "added";
                        }
                        else
                        {
                            response = "ran out";
                        }
                    }

                    cartModel.CartItems = cartItemsModel;
                    var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cartModel);

                    cartCookie.Values.Set("cart", jsonStr);

                    cartCookie.Expires = DateTime.Now.AddHours(12);
                    cartCookie.SameSite = SameSiteMode.Lax;

                    Response.Cookies.Add(cartCookie);
                }
                else
                {
                    response = "ran out";
                }
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
                response = "error";
            }


            return Newtonsoft.Json.JsonConvert.SerializeObject(response);
        }


        public void RemoveItemFromCart(int productId, int? mainFeatureId, string complete = null)
        {

            try
            {
                var cartModel = new CartModel();

                if (Session["cart"] == null)
                {
                    Session.Add("cart", "");
                }

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (Session["cart"] != null && Session["cart"].ToString() != "")
                {
                    string cartJsonStr = Session["cart"].ToString();
                    cartModel = new CartModel(cartJsonStr);
                }
                else if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }
                #endregion

                if (cartModel.CartItems.Any(i => i.Id == productId && i.MainFeatureId == mainFeatureId))
                {
                    var itemToRemove = cartModel.CartItems.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId);
                    if (complete == "true" || itemToRemove.Quantity < 2)
                    {
                        cartModel.TotalPrice -= itemToRemove.Price * itemToRemove.Quantity;
                        cartModel.CartItems.Remove(itemToRemove);
                    }
                    else if (complete == "false")
                    {
                        cartModel.TotalPrice -= itemToRemove.Price;
                        cartModel.CartItems.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId).Quantity -= 1;
                    }
                }
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cartModel);
                cartCookie.Values.Set("cart", jsonStr);
                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);


                Session.Remove("cart");
                Session.Add("cart", JsonConvert.SerializeObject(cartModel));
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }

        }


        [HttpPost]
        public void RemoveFromCart(int productId, int? mainFeatureId, string complete = null)
        {
            try
            {
                var cartModel = new CartModel();

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }
                #endregion

                if (cartModel.CartItems.Any(i => i.Id == productId && i.MainFeatureId == mainFeatureId))
                {
                    var itemToRemove = cartModel.CartItems.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId);
                    if (complete == "true" || itemToRemove.Quantity < 2)
                    {
                        cartModel.TotalPrice -= itemToRemove.Price * itemToRemove.Quantity;
                        cartModel.CartItems.Remove(itemToRemove);
                    }
                    else if (complete == "false")
                    {
                        cartModel.TotalPrice -= itemToRemove.Price;
                        cartModel.CartItems.FirstOrDefault(i => i.Id == productId && i.MainFeatureId == mainFeatureId).Quantity -= 1;
                    }
                }
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(cartModel);
                cartCookie.Values.Set("cart", jsonStr);
                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
        }


        [HttpPost]
        public void AddToWishList(int productId)
        {
            try
            {
                var withListModel = new WishListModel();
                var withListItemsModel = new List<WishListItemModel>();

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["wishList"] ?? new HttpCookie("wishList");

                if (!string.IsNullOrEmpty(cartCookie.Values["wishList"]))
                {
                    string cartJsonStr = cartCookie.Values["wishList"];
                    withListModel = new WishListModel(cartJsonStr);
                    withListItemsModel = withListModel.WishListItems;
                }
                #endregion

                var product = _productsRepo.Get(productId);
                if (withListItemsModel.Any(i => i.Id == productId) == false)
                {
                    withListItemsModel.Add(new WishListItemModel()
                    {
                        Id = product.Id,
                        ProductName = product.ShortTitle,
                        Image = product.Image
                    });
                }
                withListModel.WishListItems = withListItemsModel;
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(withListModel);
                cartCookie.Values.Set("wishList", jsonStr);

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("wishList", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
        }


        [HttpPost]
        public void RemoveFromWishList(int productId)
        {
            try
            {
                var withListModel = new WishListModel();

                #region Checking for cookie
                HttpCookie cartCookie = Request.Cookies["wishList"] ?? new HttpCookie("wishList");

                if (!string.IsNullOrEmpty(cartCookie.Values["wishList"]))
                {
                    string cartJsonStr = cartCookie.Values["wishList"];
                    withListModel = new WishListModel(cartJsonStr);
                }
                #endregion

                if (withListModel.WishListItems.Any(i => i.Id == productId))
                {
                    var itemToRemove = withListModel.WishListItems.FirstOrDefault(i => i.Id == productId);
                    withListModel.WishListItems.Remove(itemToRemove);
                }
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(withListModel);
                cartCookie.Values.Set("wishList", jsonStr);
                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("wishList", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }
        }


        public ActionResult WishList()
        {
            return View();
        }


        public ActionResult WishListTable()
        {
            var wishListModel = new WishListModel();

            try
            {
                HttpCookie cartCookie = Request.Cookies["wishList"] ?? new HttpCookie("wishList");

                if (!string.IsNullOrEmpty(cartCookie.Values["wishList"]))
                {
                    string cartJsonStr = cartCookie.Values["wishList"];
                    wishListModel = new WishListModel(cartJsonStr);
                }

            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("wishList", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }

            return PartialView(wishListModel);
        }


        public ActionResult Cart()
        {
            return View();
        }


        public ActionResult CartTable()
        {
            var cartModel = new CartModel();

            try
            {

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
                        cartModel = new CartModel(cartJsonStr);
                    }
                }

            }
            catch (Exception e)
            {
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                cartCookie.Values.Set("cart", "");

                cartCookie.Expires = DateTime.Now.AddHours(12);
                cartCookie.SameSite = SameSiteMode.Lax;
                Response.Cookies.Add(cartCookie);
            }

            return PartialView(cartModel);
        }


        [HttpGet]
        [CustomerAuthorize]
        public ActionResult Checkout()
        {
            #region Getting Cart

            var cartModel = new CartModel();
            HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

            if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
            {
                try
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }
                catch
                {

                }
            }

            if (cartModel.TotalPrice == 0)
                if (Session["cart"] != null && Session["cart"].ToString() != "")
                {
                    string cartJsonStr = Session["cart"].ToString();
                    cartModel = new CartModel(cartJsonStr);
                }

            #endregion

            ViewBag.InvoiceNumber = GenerateInvoiceNumber();
            ViewBag.DiscountCode = "";

            #region Creating CheckoutForm Fields
            var currentCustomer = _customerRepo.GetCurrentCustomer();
            if (currentCustomer == null)
                return Redirect("/Customer/Auth/Login/?returnUrl=/Shop/Checkout");

            var form = new CheckoutForm();
            form.Address = currentCustomer.Address;
            form.Name = $"{currentCustomer.User.FirstName} {currentCustomer.User.LastName}";
            form.RegisterDate = DateTime.Now;
            form.InvoiceNumber = GenerateInvoiceNumber();
            form.Email = currentCustomer.User.Email;
            form.Phone = currentCustomer.User.PhoneNumber;
            form.PostalCode = currentCustomer.PostalCode;

            ViewBag.PersianRegisterDate = new PersianDateTime(form.RegisterDate).ToString("dddd d MMMM yyyy");
            ViewBag.GeoDivisionId = new SelectList(_geoDivisionRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", currentCustomer.GeoDivisionId);
            ViewBag.CitiezOfTehranId = new SelectList(_geoDivisionRepo.GetCitiezByParentId(24), "Id", "Title", currentCustomer.GeoDivisionId);
            #endregion

            var vm = new CheckoutViewModel()
            {
                Cart = cartModel,
                Form = form
            };

            // getting correponding shipping price
            ViewBag.ShippingPrice = _shippingRepo.GetCorrespondingShippingPrice(cartModel.TotalPrice);
            var HolidyDate = _holidyDateRepository.GetAll().Where(a => a.IsDeleted == false).ToList();
            List<string> ListHolidyDate = new List<string>();
            for (int i = 0; i < HolidyDate.Count(); i++)
            {
                ListHolidyDate.Add(HolidyDate[i].HolidyDates.ToString());
            }
            ViewBag.ListHolidyDate = ListHolidyDate;

            return View(vm);
        }


        public List<SelectListItem> GetCitiez(int id)
        {
            var GetCitiez = new SelectList(_geoDivisionRepo.GetCitiezByParentId(id), "Id", "Title").ToList();
            return GetCitiez;
        }


        //[HttpPost]
        //[CustomerAuthorize]
        //public int Checkout(CheckoutForm form, int GeoDivisionId, string datepicker)
        //{
        //    var errors = new List<string>();
        //    if (ModelState.IsValid)
        //    {
        //        #region Getting Cart
        //        var cartModel = new CartModel();

        //        HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

        //        if (Session["cart"] != null && Session["cart"].ToString() != "")
        //        {
        //            string cartJsonStr = Session["cart"].ToString();
        //            cartModel = new CartModel(cartJsonStr);
        //        }
        //        else if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
        //        {
        //            string cartJsonStr = cartCookie.Values["cart"];
        //            cartModel = new CartModel(cartJsonStr);
        //        }

        //        #endregion
        //        var currentCustomer = _customerRepo.GetCurrentCustomer();
        //        if (currentCustomer != null)
        //        {
        //            try
        //            {
        //                #region Setting Invoice Fields
        //                var invoice = new Invoice();
        //                invoice.AddedDate = DateTime.Now;
        //                invoice.InvoiceNumber = form.InvoiceNumber;
        //                invoice.Address = form.Address;
        //                invoice.CustomerId = currentCustomer.Id;
        //                invoice.CustomerName = form.Name;
        //                invoice.Email = form.Email;
        //                invoice.Phone = form.Phone;
        //                invoice.Address = form.Address;
        //                invoice.PostalCode = form.PostalCode;
        //                invoice.GeoDivisionId = GeoDivisionId;
        //                invoice.IsDeleted = false;
        //                invoice.IsPayed = false;
        //                invoice.ShipingStatus = false;
        //                var HolidyDate = _holidyDateRepository.GetAll().Where(a => a.IsDeleted == false).ToList();
        //                invoice.ShipingDate = datepicker;
        //                for (int i = 0; i < HolidyDate.Count; i++)
        //                {
        //                    if (HolidyDate[i].HolidyDates == datepicker)

        //                    {
        //                        invoice.ShipingDate = "";
        //                        break;
        //                    }
        //                }
        //                #endregion
        //                #region Getting cart items from db

        //                long invoiceTotalPrice = 0;
        //                var invoiceItems = new List<InvoiceItem>();
        //                foreach (var item in cartModel.CartItems)
        //                {
        //                    var product = _productService.CreateProductWithPriceDto(item.Id, item.MainFeatureId);
        //                    var productStockCount = _productService.GetProductStockCount(item.Id, item.MainFeatureId);

        //                    if (productStockCount >= item.Quantity)
        //                    {
        //                        var mainFeature = _productMainFeaturesRepo.Get(item.MainFeatureId);
        //                        if (mainFeature != null && mainFeature.IsDeleted == false)
        //                        {
        //                            invoiceTotalPrice += product.PriceAfterDiscount * item.Quantity;
        //                            var invoiceItem = new InvoiceItem();
        //                            invoiceItem.InvoiceId = 0; //Setting it to 0 for now going to update it after saving invoice
        //                            invoiceItem.ProductId = product.Id;
        //                            invoiceItem.Price = product.PriceAfterDiscount;
        //                            invoiceItem.Quantity = item.Quantity;
        //                            invoiceItem.TotalPrice = product.PriceAfterDiscount * item.Quantity;
        //                            invoiceItem.MainFeatureId = item.MainFeatureId;
        //                            invoiceItem.IsDeleted = false;
        //                            invoiceItems.Add(invoiceItem);
        //                        }
        //                        else
        //                        {
        //                            errors.Add($"محصول ( {product.ShortTitle} ) با مشخصات درخواستی شما موجود نیست لطفا سبد خرید خود را بروزرسانی کنید");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        errors.Add($"محصول ( {product.ShortTitle} ) به تعداد درخواستی شما موجود نیست لطفا سبد خرید خود را بروزرسانی کنید");
        //                    }
        //                }
        //                if (errors.Count == 0)
        //                {
        //                    // checkin' for discount code validity
        //                    var discountCode = _shoppingRepo.GetActiveDiscountCode(form.DiscountCode, currentCustomer.Id);
        //                    long discountCodeAmount = 0;
        //                    int? discountCodeId = null;
        //                    if (discountCode != null)
        //                    {
        //                        discountCodeAmount = discountCode.Value;
        //                        discountCodeId = discountCode.Id;
        //                    }

        //                    // adding shipping price to the invoice
        //                    var shippingPrice = _shippingRepo.GetCorrespondingShippingPrice(invoiceTotalPrice);
        //                    var shippingAmount = shippingPrice == null ? 0 : shippingPrice.ShippingPriceValue;

        //                    //Adding Invoice
        //                    invoice.ShippingPriceAmount = shippingAmount;
        //                    invoice.DiscountAmount = discountCodeAmount;
        //                    invoice.TotalPriceBeforeDiscount = invoiceTotalPrice;
        //                    invoice.TotalPrice = invoiceTotalPrice - discountCodeAmount + shippingAmount;
        //                    invoice.DiscountCodeId = discountCodeId;
        //                    invoice.ShippingPrice = shippingPrice;
        //                    _invoiceRepo.Add(invoice);
        //                    if (discountCode != null) _shoppingRepo.DeactiveDiscountCode(discountCode.Id); // deactive discount code

        //                    // Adding invoice Items
        //                    foreach (var inv in invoiceItems)
        //                    {
        //                        inv.InvoiceId = invoice.Id;
        //                        _invoiceRepo.AddInvoiceItem(inv);
        //                    }
        //                    cartCookie.Values.Set("cart", "");

        //                    cartCookie.Expires = DateTime.Now.AddHours(12);
        //                    cartCookie.SameSite = SameSiteMode.Lax;
        //                    Response.Cookies.Add(cartCookie);

        //                    Session.Remove("cart");
        //                    Session.Add("cart", JsonConvert.SerializeObject(new CartModel()));
        //                    var id = invoice.Id;
        //                    //return RedirectToAction("CheckoutSummary", new { id = invoice.Id });
        //                    return id;
        //                }
        //                #endregion
        //                ViewBag.Errors = errors;
        //                ViewBag.PersianRegisterDate = new PersianDateTime(form.RegisterDate).ToString("dddd d MMMM yyyy");
        //                ViewBag.GeoDivisionId = new SelectList(_geoDivisionRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", form.GeoDivisionId);
        //                var vm = new CheckoutViewModel()
        //                {
        //                    Cart = cartModel,
        //                    Form = form
        //                };
        //                //return View(vm);
        //                return 2;
        //            }
        //            catch (Exception e)
        //            {
        //                errors.Add("سبد خرید معتبر نیست.");
        //                ViewBag.Errors = errors;
        //                ViewBag.PersianRegisterDate = new PersianDateTime(form.RegisterDate).ToString("dddd d MMMM yyyy");
        //                ViewBag.GeoDivisionId = new SelectList(_geoDivisionRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", form.GeoDivisionId);
        //                var vm = new CheckoutViewModel()
        //                {
        //                    Cart = cartModel,
        //                    Form = form
        //                };
        //                ViewBag.DiscountCode = "";
        //                //return View(vm);
        //                return 1;
        //            }
        //        }
        //    }
        //    //return Redirect("/Customer/Auth/Login");
        //    return 0;
        //}
        [HttpPost]
        //[CustomerAuthorize]
        public int Checkout(CheckoutForm form, int GeoDivisionId, string datepicker)
        {
            var errors = new List<string>();
            if (ModelState.IsValid)
            {
                #region Getting Cart
                var cartModel = new CartModel();
                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    try
                    {
                        string cartJsonStr = cartCookie.Values["cart"];
                        cartModel = new CartModel(cartJsonStr);
                    }
                    catch
                    {

                    }
                }

                if (cartModel.TotalPrice == 0)
                    if (Session["cart"] != null && Session["cart"].ToString() != "")
                    {
                        string cartJsonStr = Session["cart"].ToString();
                        cartModel = new CartModel(cartJsonStr);
                    }

                #endregion

                var currentCustomer = _customerRepo.GetCurrentCustomer();
                if (currentCustomer != null)
                {
                    try
                    {
                        #region Setting Invoice Fields
                        var invoice = new Invoice();
                        invoice.AddedDate = DateTime.Now;
                        invoice.InvoiceNumber = form.InvoiceNumber;
                        invoice.Address = form.Address;
                        invoice.CustomerId = currentCustomer.Id;
                        invoice.CustomerName = form.Name;
                        invoice.Email = form.Email;
                        invoice.Phone = form.Phone;
                        invoice.Address = form.Address;
                        invoice.PostalCode = form.PostalCode;
                        invoice.GeoDivisionId = GeoDivisionId;
                        invoice.IsDeleted = false;
                        invoice.IsPayed = false;
                        invoice.ShipingStatus = false;
                        var HolidyDate = _holidyDateRepository.GetAll().Where(a => a.IsDeleted == false).ToList();
                        invoice.ShipingDate = datepicker;
                        for (int i = 0; i < HolidyDate.Count; i++)
                        {
                            if (HolidyDate[i].HolidyDates == datepicker)

                            {
                                invoice.ShipingDate = "";
                                break;
                            }
                        }
                        #endregion
                        #region Getting cart items from db

                        long invoiceTotalPrice = 0;
                        var invoiceItems = new List<InvoiceItem>();

                        foreach (var item in cartModel.CartItems)
                        {
                            var product = _productService.CreateProductWithPriceDto(item.Id, item.MainFeatureId);
                            var productStockCount = _productService.GetProductStockCount(item.Id, item.MainFeatureId);

                            if (productStockCount >= item.Quantity)
                            {
                                var mainFeature = _productMainFeaturesRepo.Get(item.MainFeatureId);
                                if (mainFeature != null && mainFeature.IsDeleted == false)
                                {
                                    invoiceTotalPrice += product.PriceAfterDiscount * item.Quantity;
                                    var invoiceItem = new InvoiceItem();
                                    invoiceItem.InvoiceId = 0; //Setting it to 0 for now going to update it after saving invoice
                                    invoiceItem.ProductId = product.Id;
                                    invoiceItem.Price = product.PriceAfterDiscount;
                                    invoiceItem.Quantity = item.Quantity;
                                    invoiceItem.TotalPrice = product.PriceAfterDiscount * item.Quantity;
                                    invoiceItem.MainFeatureId = item.MainFeatureId;
                                    invoiceItem.IsDeleted = false;
                                    invoiceItems.Add(invoiceItem);
                                }
                                else
                                {
                                    errors.Add($"محصول ( {product.ShortTitle} ) با مشخصات درخواستی شما موجود نیست لطفا سبد خرید خود را بروزرسانی کنید");
                                }
                            }
                            else
                            {
                                errors.Add($"محصول ( {product.ShortTitle} ) به تعداد درخواستی شما موجود نیست لطفا سبد خرید خود را بروزرسانی کنید");
                            }
                        }

                        if (errors.Count == 0)
                        {
                            // checkin' for discount code validity
                            var discountCode = _shoppingRepo.GetActiveDiscountCode(form.DiscountCode, currentCustomer.Id);
                            long discountCodeAmount = 0;
                            int? discountCodeId = null;
                            if (discountCode != null)
                            {
                                discountCodeAmount = discountCode.Value;
                                discountCodeId = discountCode.Id;
                            }

                            // adding shipping price to the invoice
                            var shippingPrice = _shippingRepo.GetCorrespondingShippingPrice(invoiceTotalPrice);
                            var shippingAmount = shippingPrice == null ? 0 : shippingPrice.ShippingPriceValue;

                            //Adding Invoice
                            invoice.ShippingPriceAmount = shippingAmount;
                            invoice.DiscountAmount = discountCodeAmount;
                            invoice.TotalPriceBeforeDiscount = invoiceTotalPrice;
                            invoice.TotalPrice = invoiceTotalPrice - discountCodeAmount + shippingAmount;
                            invoice.DiscountCodeId = discountCodeId;
                            invoice.ShippingPrice = shippingPrice;
                            _invoiceRepo.Add(invoice);

                            //if (discountCode != null) _shoppingRepo.DeactiveDiscountCode(discountCode.Id); // deactive discount code

                            // Adding invoice Items
                            foreach (var inv in invoiceItems)
                            {
                                inv.InvoiceId = invoice.Id;
                                _invoiceRepo.AddInvoiceItem(inv);
                            }
                            cartCookie.Values.Set("cart", "");

                            cartCookie.Expires = DateTime.Now.AddHours(12);
                            cartCookie.SameSite = SameSiteMode.Lax;
                            Response.Cookies.Add(cartCookie);

                            Session.Remove("cart");
                            Session.Add("cart", JsonConvert.SerializeObject(new CartModel()));
                            var id = invoice.Id;
                            //return RedirectToAction("CheckoutSummary", new { id = invoice.Id });
                            return id;
                        }
                        #endregion
                        ViewBag.Errors = errors;
                        ViewBag.PersianRegisterDate = new PersianDateTime(form.RegisterDate).ToString("dddd d MMMM yyyy");
                        ViewBag.GeoDivisionId = new SelectList(_geoDivisionRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", form.GeoDivisionId);
                        var vm = new CheckoutViewModel()
                        {
                            Cart = cartModel,
                            Form = form
                        };
                        //return View(vm);
                        return 2;
                    }
                    catch (Exception e)
                    {
                        errors.Add("سبد خرید معتبر نیست.");
                        ViewBag.Errors = errors;
                        ViewBag.PersianRegisterDate = new PersianDateTime(form.RegisterDate).ToString("dddd d MMMM yyyy");
                        ViewBag.GeoDivisionId = new SelectList(_geoDivisionRepo.GetGeoDivisionsByType((int)GeoDivisionType.State), "Id", "Title", form.GeoDivisionId);
                        var vm = new CheckoutViewModel()
                        {
                            Cart = cartModel,
                            Form = form
                        };
                        ViewBag.DiscountCode = "";
                        //return View(vm);
                        return 1;
                    }
                }
            }
            //return Redirect("/Customer/Auth/Login");
            return 0;
        }


        [CustomerAuthorize]
        public ActionResult CheckoutSummary(int id)
        {
            var invoice = _invoiceRepo.GetInvoiceWithGeo(id);
            var invoiceItems = _invoiceRepo.GetInvoiceItemsByInvoiceId(id);
            var vm = new InvoiceDetails()
            {
                Invoice = invoice,
                InvoiceItems = invoiceItems,
                PersianRegisterDate = new PersianDateTime(invoice.AddedDate).ToString("dddd d MMMM yyyy")
            };

            return View(vm);
        }


        //[HttpPost]
        //public ActionResult Payment2(int invoiceId)
        //{
        //    var invoice = _invoiceRepo.Get(invoiceId);
        //    var currentCustomer = _customerRepo.GetCurrentCustomer();
        //    var obj = new MellatGateway.bpPayRequest();
        //    obj.Body.terminalId =
        //    obj.Body.payerId = currentCustomer.Id.ToString();
        //    obj.Body.amount = invoice.TotalPrice;
        //    obj.Body.orderId = invoice.Id;
        //    obj.Body.callBackUrl = "/"
        //    return null;
        //}
        [HttpPost]
        public ActionResult Payment(int invoiceId)
        {
            var invoice = _invoiceRepo.GetInvoice(invoiceId);
            var orderId = GenerateInvoiceNumber();
            var currentCustomer = _customerRepo.GetCurrentCustomer();
            try
            {
                using (var _context = new MyDbContext())
                {
                    var p = new EPayment()
                    {
                        Amount = invoice.TotalPrice,
                        //Amount = 10000,
                        InvoiceId = invoice.Id,
                        PaymentStatus = "-100",
                        PaymentAccountId = (int)PaymentAccounts.Mellat,
                        InsertDate = DateTime.Now,
                        InsertUser = currentCustomer.User.UserName,
                        PaymentFinished = false,
                    };
                    _context.EPayments.Add(p);
                    _context.SaveChanges();
                    //var pLog = new EPaymentLog()
                    //{
                    //    EPaymentId = p.Id,
                    //    Message = "Redirecting customer to payment page",
                    //    LogType = "Post",
                    //    PaymentKey = 0,
                    //    Amount = invoice.TotalPrice,
                    //    IsDeleted = false,
                    //    InsertDate = DateTime.Now,
                    //    InsertUser = currentCustomer.User.UserName,
                    //};
                    //_context.EPaymentLogs.Add(pLog);
                    //_context.SaveChanges();

                    int paymentId = p.Id;
                    if (paymentId > 0)
                    {
                        BypassCertificateError();
                        var result = PreparePaymentGateway(invoice, paymentId);
                        if (result != null)
                        {
                            // 45zm24554654,0 
                            string[] ResultArray = result.Split(',');
                            if (ResultArray[0] == "0")
                            {
                                p.PaymentStatus = "-100";
                                p.SaleRefrenceID = 0;
                                p.ReferenceNumber = ResultArray[1];
                                p.PaymentFinished = false;
                                _context.Entry(p).State = EntityState.Modified;
                                _context.SaveChanges();
                                NameValueCollection collection = new NameValueCollection();
                                collection.Add("RefId", ResultArray[1]);
                                Response.Write(PayHelper.PreparePOSTForm("https://bpm.shaparak.ir/pgwchannel/startpay.mellat", collection));
                            }
                            else
                            {
                                p.PaymentStatus = ResultArray[0];
                                p.SaleRefrenceID = 0;
                                p.ReferenceNumber = null;
                                p.PaymentFinished = false;
                                _context.Entry(p).State = EntityState.Modified;
                                _context.SaveChanges();
                                ViewBag.Message = PaymentResult.MellatResult(ResultArray[0]);
                            }
                        }
                        else
                        {
                            ViewBag.Message = "امکان اتصال به درگاه بانک وجود ندارد";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return View();
        }


        public void BypassCertificateError()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                delegate (
                    Object sender1,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };
        }


        public string PreparePaymentGateway(Invoice invoice, int paymentId)
        {
            string RedirectPage = "https://www.girlymerely.com" + Url.Action("CallBack", "Payment");
            var payment = new MellatGateway.PaymentGatewayClient();
            try
            {
                var localDate = GetDate();
                var localTime = GetTime();

                var gateWay = payment.bpPayRequest(terminalId: MellatGatewayHelper.TerminalId,
                    userName: MellatGatewayHelper.Username,
                    userPassword: MellatGatewayHelper.Password,
                    orderId: paymentId,
                    amount: invoice.TotalPrice * 10,
                    //amount: 1000 * 10,
                    localDate: localDate,
                    localTime: localTime,
                    additionalData: invoice.Id.ToString(),
                    callBackUrl: RedirectPage,
                    payerId: invoice.Customer.Id.ToString());
                return gateWay;

            }
            catch (Exception error)
            {
                throw new Exception(error.Message);
            }
        }


        public string GetDate()
        {
            return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') +
                   DateTime.Now.Day.ToString().PadLeft(2, '0');
        }


        public string GetTime()
        {
            return DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') +
                   DateTime.Now.Second.ToString().PadLeft(2, '0');
        }


        public string GenerateInvoiceNumber()
        {
            var bytes = Guid.NewGuid().ToByteArray();
            var code = "";
            for (int i = 0; code.Length <= 16 && i < bytes.Length; i++)
            {
                code += (bytes[i] % 10).ToString();
            }

            return code;
        }


        [HttpPost]
        public string ApplyDiscountCode(string discountCodeStr, string invoiceNumber = "")
        {
            long finalPrice = 0;
            long discountAmount = 0;
            var invoice = _invoiceRepo.GetInvoice(invoiceNumber);

            if (invoice == null)
            {
                var cartModel = new CartModel();

                HttpCookie cartCookie = Request.Cookies["cart"] ?? new HttpCookie("cart");

                if (!string.IsNullOrEmpty(cartCookie.Values["cart"]))
                {
                    string cartJsonStr = cartCookie.Values["cart"];
                    cartModel = new CartModel(cartJsonStr);
                }

                finalPrice = cartModel.TotalPrice;
            }
            else
            {
                finalPrice = invoice.TotalPrice;
            }

            DiscountCodeResponseViewModel discountCodeResponse = new DiscountCodeResponseViewModel();

            // getting corrseponding shipping price
            var shippingPrice = _shippingRepo.GetCorrespondingShippingPrice(finalPrice);
            discountCodeResponse.ShippingAmount = shippingPrice != null ? shippingPrice.ShippingPriceValue : 0;


            // first we need to check if "discountCode" is valid
            var customer = _customerRepo.GetCurrentCustomer();

            if (customer == null)
            {
                discountCodeResponse.Response = "login";
                discountCodeResponse.FinalPrice = finalPrice;
                return Newtonsoft.Json.JsonConvert.SerializeObject(discountCodeResponse);
            }

            // then we recalculate the cart items' price with regard to discountCode price
            var discountCode = _shoppingRepo.GetActiveDiscountCode(discountCodeStr, customer.Id);
            
            // check log table


            if (discountCode == null)
            {
                discountCodeResponse.Response = "invalid";
            }
            else
            {
                finalPrice -= discountCode.Value;
                discountAmount = discountCode.Value;
                discountCodeResponse.Response = "valid";
            }


            discountCodeResponse.FinalPrice = finalPrice + discountCodeResponse.ShippingAmount;
            discountCodeResponse.DiscountAmount = discountAmount;

            return Newtonsoft.Json.JsonConvert.SerializeObject(discountCodeResponse);

        }


        [HttpPost]
        public void HeartBeat()
        {
            if (Session["cart"] == null)
            {
                Session.Add("cart", JsonConvert.SerializeObject(new CartModel()));
            }
        }
    }
}