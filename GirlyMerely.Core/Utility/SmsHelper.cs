using GirlyMerely.Core.Models;
using SmsIrRestful;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirlyMerely.Core.Utility
{
    public static class SmsHelper
    {
        public static string GetToken()
        {
            SmsIrRestful.Token tk = new SmsIrRestful.Token();

            string result = tk.GetToken("70273a9df712c09c638b3104", "&dBDSEHK*&(yrdkj456");

            return result;
        }

        public static bool SendLoginSms(string PhoneNumber, string Code)
        {
            var token = SmsHelper.GetToken();

            var restVerificationCode = new RestVerificationCode()
            {
                Code = Code,
                MobileNumber = PhoneNumber
            };

            var restVerificationCodeRespone = new VerificationCode().Send(token, restVerificationCode);

            if (restVerificationCodeRespone.IsSuccessful)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool SendOrderStatus(string name, long Phonenumber)
        {
            var token = SmsHelper.GetToken();

            var ultraFastSend = new UltraFastSend()
            {
                Mobile = Phonenumber,
                TemplateId = (int)SmsTemplateId.OrderConfirmation,
                ParameterArray = new List<UltraFastParameters>()
                {
                    //new UltraFastParameters()
                    //{
                    //    Parameter = "Id" , ParameterValue = "#50618"
                    //},
                    new UltraFastParameters()
                    {
                        Parameter = "Name" , ParameterValue = name
                    }
                }.ToArray()
            };

            UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

            if (ultraFastSendRespone.IsSuccessful)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool SendActivationCode(string phoneNumber, string code)
        {
            //var token = SmsHelper.GetToken();

            //var ultraFastSend = new UltraFastSend()
            //{
            //    Mobile = long.Parse(phoneNumber),
            //    TemplateId = (int)SmsTemplateId.MembershipConfirmationCode,
            //    ParameterArray = new List<UltraFastParameters>()
            //    {
            //        new UltraFastParameters()
            //        {
            //            Parameter = "token" ,
            //        }
            //    }.ToArray()
            //};

            //UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

            //if (ultraFastSendRespone.IsSuccessful)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            var token = SmsHelper.GetToken();

            var restVerificationCode = new RestVerificationCode()
            {
                Code = code,
                MobileNumber = phoneNumber

            };

            var restVerificationCodeRespone = new VerificationCode().Send(token, restVerificationCode);

            if (restVerificationCodeRespone.IsSuccessful)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool SendResetPasswordLink(long phonenumber, string link)
        {
            var token = SmsHelper.GetToken();

            var ultraFastSend = new UltraFastSend()
            {
                Mobile = phonenumber,
                TemplateId = (int)SmsTemplateId.ReVerificationCode,
                ParameterArray = new List<UltraFastParameters>()
                {
                    new UltraFastParameters()
                    {
                        Parameter = "token" , ParameterValue = link
                    }
                }.ToArray()
            };

            UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

            if (ultraFastSendRespone.IsSuccessful)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
