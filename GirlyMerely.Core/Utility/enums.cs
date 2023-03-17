using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirlyMerely.Core.Utility
{
    public enum DiscountType
    {
        Percentage = 1,
        Amount = 2
    }
    public enum GeoDivisionType
    {
        Country = 0,
        State = 1,
        City = 2,
    }

    public enum StaticContentTypes
    {
        Slider = 1,
        ContactUsMap = 2,
        Address = 6,
        Email = 7,
        Phone = 8,
        Guide = 9,
        AboutUs = 10,
        Terms = 11,
        ProductDeliveryMethod = 15,
        ProductReturnTime = 16
    }

    public enum PaymentStatus
    {
        IsPayed = 1,
        Registered = 2,
        Expired = 3
    }

    public enum PaymentAccounts
    {
        Mellat = 3
    }

    public enum SmsTemplateId : int
    {
        /// <summary>
        ///هیچ کدام
        /// </summary>
        None = 0,


        /// <summary>
        /// فرمت ارسال ثبت سفارش
        /// </summary>
        OrderConfirmation = 50620,


        /// <summary>
        /// فرمت ورود به اکانت
        /// </summary>
        LoginAccount = 50618,


        /// <summary>
        /// فرمت ساخت اکانت
        /// </summary>
        RegisterCode = 50619,


        /// <summary>
        /// فرمت بازیابی رمز عبور
        /// </summary>
        ReVerificationCode = 51140,


        /// <summary>
        /// فرمت تایید شماره سفارش
        /// </summary>
        OrderCofirmationCode = 52940,


        /// <summary>
        /// فرمت تایید عضویت
        /// </summary>
        MembershipConfirmationCode = 53239
    }
}
