//using GirlyMerely.Core.Models;
//using GirlyMerely.Infrastructure.Services;
//using GirlyMerely.Notification.SmsProvider;
//using GirlyMerely.Utility;
//using GirlyMerely.Utility.Enums;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace GirlyMerely.Infrastructure.Helpers
//{
//    public class SmsHelper
//    {
//        private readonly ISmsLogService _smsLogService;
//        public SmsHelper(ISmsLogService SmsLogService)
//        {
//            _smsLogService = SmsLogService;
//        }

//        public void SendMessage(string phone, string body, SmsTemplateId templateId, List<SmsTemplateParameter> parameters)
//        {
//            SMSLog smsLog = new SMSLog();
//            smsLog.ReceiverMobileNo = phone;
//            smsLog.MessageBody = body;
//            smsLog.SendDateTime = DateTime.Now;
//            smsLog.IsFlash = false;
//            smsLog.PatternCode = "";
//            smsLog.LineNumber = StaticVariables.LineNumber;

//            SMSDotIrProvider sMSDotIrProvider = new SMSDotIrProvider();
//            sMSDotIrProvider.SendSMS(ref smsLog, templateId, parameters);

//            if (_smsLogService != null)
//            {
//                _smsLogService.AddOrUpdate(smsLog);
//            }
//        }
//    }
//}
