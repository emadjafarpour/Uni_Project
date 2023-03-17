using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using GirlyMerely.Core.Models;

namespace GirlyMerely.Web.ViewModels
{
    public class DiscountFormViewModel
    {
        [DisplayName("عنوان تخفیف")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(500, ErrorMessage = "{0} باید کمتر از 500 کارکتر باشد")]
        public string Title { get; set; }
        public int DiscountType { get; set; }
        [DisplayName("میزان تخفیف")]
        [Required(ErrorMessage = "لطفا میزان تخفیف را وارد کنید")]
        public long Amount { get; set; }
        public List<int> BrandIds { get; set; }
        public List<int> ProductIds { get; set; }
        public List<int> ProductGroupIds { get; set; }
        public bool IsOffer { get; set; }
        public int? OfferId { get; set; }

        // Edit Props
        public string GroupIdentifier { get; set; }
        public List<Discount> PreviousDiscounts { get; set; }
    }

    public class TestimonialViewModel
    {
        public TestimonialViewModel()
        {
        }

        public TestimonialViewModel(Testimonial testimonial)
        {
            this.Name = testimonial.Name;
            this.Image = testimonial.Image;
            this.Description = testimonial.Description;
            this.PersianDate = new PersianDateTime(testimonial.InsertDate.Value).ToString("dddd d MMMM yyyy");
        }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string PersianDate { get; set; }
    }
    public class ContactUsViewModel
    {
        public string Map { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
    public class SocialViewModel
    {
        public string whatsap { get; set; }
        public string instagram { get; set; }
        public string twiter { get; set; }
        public string google { get; set; }
        public string youtube { get; set; }
        public string facebook { get; set; }
    }

    public class CartModel
    {
        public CartModel()
        {
        }

        public CartModel(string json)
        {
            JObject jObject = JObject.Parse(json);
            var jItems = jObject["CartItems"].ToList();
            var cartItems = new List<CartItemModel>();
            var productDetails = new ProductFeatureValue();
            var productFeatureValue = new ProductFeatureValue();

            foreach (var item in jItems)
            {
                cartItems.Add(new CartItemModel()
                {
                    Id = Convert.ToInt32(item["Id"]),
                    ProductName = (string)item["ProductName"],
                    Image = (string)item["Image"],
                    Price = Convert.ToInt64(item["Price"]),
                    MainFeatureId = Convert.ToInt32(item["MainFeatureId"]),
                    Quantity = Convert.ToInt32(item["Quantity"]),
                    ProductMainFeatureName = (string)item["ProductMainFeatureName"],
                });
            }

            this.CartItems = cartItems;
            this.TotalPrice = Convert.ToInt64(jObject["TotalPrice"]);
        }

        //public List<CartItemModel> CartItems { get; set; }
        private List<CartItemModel> _cartItems;

        public List<CartItemModel> CartItems
        {
            get { return _cartItems ?? (_cartItems = new List<CartItemModel>()); }
            set { _cartItems = value; }
        }

        public long TotalPrice { get; set; }
    }
    public class CartItemModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ProductMainFeatureName { get; set; }
        public string Image { get; set; }
        public long Price { get; set; }
        public int Quantity { get; set; }
        public int MainFeatureId { get; set; }


        private List<CartItemDetails> _cartItemDetails;

        public List<CartItemDetails> cartItemDetails
        {
            get { return _cartItemDetails ?? (_cartItemDetails = new List<CartItemDetails>()); }
            set { _cartItemDetails = value; }
        }
    }

    public class CartItemDetails
    {
        public string FeatureName { get; set; }
        public string Value { get; set; }

        private List<CartItemSubDetails> _cartItemSubDetails;

        public List<CartItemSubDetails> cartItemSubDetails
        {
            get { return _cartItemSubDetails ?? (_cartItemSubDetails = new List<CartItemSubDetails>()); }
            set { _cartItemSubDetails = value; }
        }


    }

    public class CartItemSubDetails
    {
        public string FeatureName { get; set; }
        public string Value { get; set; }

    }

    public class WishListModel
    {
        public WishListModel()
        {
        }

        public WishListModel(string json)
        {
            JObject jObject = JObject.Parse(json);
            var jItems = jObject["WishListItems"].ToList();
            var wishListItems = new List<WishListItemModel>();
            foreach (var item in jItems)
            {
                wishListItems.Add(new WishListItemModel()
                {
                    Id = Convert.ToInt32(item["Id"]),
                    ProductName = (string)item["ProductName"],
                    Image = (string)item["Image"],
                });
            }

            this.WishListItems = wishListItems;
        }
        public List<WishListItemModel> WishListItems { get; set; }
    }
    public class WishListItemModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
    }

    public class CommentFormViewModel
    {
        public int? ParentId { get; set; }
        public int? ArticleId { get; set; }
        public int? ProductId { get; set; }
        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "{0} باید کمتر از 300 کارکتر باشد")]
        public string Name { get; set; }
        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "ایمیل نا معتبر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(400, ErrorMessage = "{0} باید کمتر از 400 کارکتر باشد")]
        public string Email { get; set; }
        [Display(Name = "پیام")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(800, ErrorMessage = "{0} باید کمتر از 800 کارکتر باشد")]
        public string Message { get; set; }
    }
    public class CheckoutForm
    {
        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Name { get; set; }
        [DisplayName("تاریخ ارسال")]
        public DateTime ShipingDate { get; set; }
        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "ایمیل نا معتبر")]
        [MaxLength(400, ErrorMessage = "{0} باید کمتر از 400 کارکتر باشد")]
        public string Email { get; set; }
        [Display(Name = "تلفن")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(400, ErrorMessage = "{0} باید کمتر از 400 کارکتر باشد")]
        public string Phone { get; set; }
        [Display(Name = "آدرس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [Display(Name = "کد پستی")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string PostalCode { get; set; }
        public int GeoDivisionId { get; set; }
        [Display(Name = "پیام")]
        [DataType(DataType.MultilineText)]
        [MaxLength(800, ErrorMessage = "{0} باید کمتر از 800 کارکتر باشد")]
        public string Message { get; set; }
        public string InvoiceNumber { get; set; }
        public string DiscountCode { get; set; }
        public DateTime RegisterDate { get; set; }
    }

    public class CheckoutViewModel
    {
        public CartModel Cart { get; set; }
        public CheckoutForm Form { get; set; }
    }

    public class InvoiceDetails
    {
        //public Invoice Invoice { get; set; }
        private Invoice _invoice;

        public Invoice Invoice
        {
            get { return _invoice ?? (_invoice = new Invoice()); }
            set { _invoice = value; }
        }


        //public List<InvoiceItem> InvoiceItems { get; set; }
        private List<InvoiceItem> _invoiceItems;

        public List<InvoiceItem> InvoiceItems
        {
            get { return _invoiceItems ?? (_invoiceItems = new List<InvoiceItem>()); }
            set { _invoiceItems = value; }
        }

        public string PersianRegisterDate { get; set; }

    }
}