using IPE.SmsIrRestful.TPL.NetCore;
using GirlyMerely.Core.Models;
using GirlyMerely.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace GirlyMerely.Notification.SmsProvider
{

    public interface ISMSProvider
    {
        void SendSMS(ref SMSLog smsLogs, SmsTemplateId templateId, List<SmsTemplateParameter> parameters);

        void SendSMSByPattern(List<SMSLog> smsLogs, IDictionary<string, string> parameters);
    }

    public class SmsTemplateParameter
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class SMSDotIrProvider : ISMSProvider
    {
        #region Panel Info

        private string Username = "09128505174";
        private string Password = "Safarilo@123456";
        private string userApiKey = "e84e255cf17f55d6de50255c";
        private string secretKey = "it66)%#YegBC!@*&";

        #endregion

        public void SendSMS(ref SMSLog smsLog, SmsTemplateId templateId, List<SmsTemplateParameter> parameters)
        {
            var token = GetToken();
            try
            {
                var parameterList = new List<UltraFastParameters>();
                parameters.ForEach(item =>
                {
                    var newItem = new UltraFastParameters()
                    {
                        Parameter = item.Key,
                        ParameterValue = item.Value
                    };
                    parameterList.Add(newItem);
                });

                var mobileNo = Convert.ToInt64(smsLog.ReceiverMobileNo);
                var ultraFastSend = new UltraFastSend()
                {
                    Mobile = mobileNo,
                    TemplateId = (int)templateId,
                    ParameterArray = parameterList.ToArray()
                };

                UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

                //var messageSendObject = new MessageSendObject()
                //{
                //    Messages = new List<string> { smsLog.MessageBody }.ToArray(),
                //    MobileNumbers = new List<string> { smsLog.ReceiverMobileNo }.ToArray(),
                //    LineNumber = smsLog.LineNumber,
                //    SendDateTime = null,
                //    CanContinueInCaseOfError = true
                //};

                //MessageSendResponseObject messageSendResponseObject = new MessageSend().Send(token, messageSendObject);

                //if (messageSendResponseObject.IsSuccessful)
                //{
                //    smsLog.ResponseMessage = messageSendResponseObject.Message;
                //}
                //else
                //{

                //}
            }
            catch (Exception ex)
            {

            }
        }

        public void SendGroupSms(List<SMSLog> smsLogs)
        {
            var token = GetToken();
            try
            {
                List<string> messages = new List<string>();
                List<string> numbers = new List<string>();
                foreach (var log in smsLogs)
                {
                    messages.Add(log.MessageBody);
                    numbers.Add(log.ReceiverMobileNo);
                }

                var messageSendObject = new MessageSendObject()
                {
                    Messages = messages.ToArray(),
                    MobileNumbers = numbers.ToArray(),
                    LineNumber = smsLogs[0].LineNumber,
                    SendDateTime = null,
                    CanContinueInCaseOfError = true
                };

                MessageSendResponseObject messageSendResponseObject = new MessageSend().Send(token, messageSendObject);

                if (messageSendResponseObject.IsSuccessful)
                {

                }
                else
                {

                }
            }
            catch
            {

            }
        }

        public void SendSMSByPattern(List<SMSLog> smsLogs, IDictionary<string, string> parameters)
        {

        }

        private string GetToken()
        {

            Token tk = new Token();
            string result = tk.GetToken(userApiKey, secretKey);

            return result;
        }
    }
}