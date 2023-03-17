using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using GirlyMerely.Core.Models;
using GirlyMerely.Core.Utility;
using GirlyMerely.Infrastructure;
using GirlyMerely.Infrastructure.Helpers;
using GirlyMerely.Infrastructure.Repositories;
using GirlyMerely.Infratructure.Repositories;

namespace GirlyMerely.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly CustomersRepository _customerRepo;
        private readonly ShoppingRepository _shoppingRepository;
        private readonly LogsRepository _logsRepository;

        public PaymentController(CustomersRepository customerRepo,
            ShoppingRepository shoppingRepository,
            LogsRepository logsRepository)
        {
            _customerRepo = customerRepo;
            _shoppingRepository = shoppingRepository;
            _logsRepository = logsRepository;
        }

        // GET: Payment
        public ActionResult CallBack()
        {
            ViewBag.Bank = "ملت";
            MellatReturn();
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


        private void MellatReturn()
        {
            BypassCertificateError();
            if (string.IsNullOrEmpty(Request.Params["SaleReferenceId"]))
            {
                //ResCode=StatusPayment
                if (!string.IsNullOrEmpty(Request.Params["ResCode"]))
                {
                    _logsRepository.LogEvent("resCode is not null", 1, "");
                    ViewBag.Message = PaymentResult.MellatResult(Request.Params["ResCode"]);
                    ViewBag.SaleReferenceId = "**************";
                }
                else
                {
                    _logsRepository.LogEvent("resCode is null", 1, "");
                    ViewBag.Message = "رسید قابل قبول نیست";
                    ViewBag.SaleReferenceId = "**************";
                }
            }
            else
            {
                try
                {
                    long TerminalId = MellatGatewayHelper.TerminalId;
                    string UserName = MellatGatewayHelper.Username;
                    string UserPassword = MellatGatewayHelper.Password;
                    long SaleOrderId = 0;  //PaymentID 
                    long SaleReferenceId = 0;
                    string RefId = null;
                    try
                    {
                        SaleOrderId = long.Parse(Request.Params["SaleOrderId"].TrimEnd());
                        SaleReferenceId = long.Parse(Request.Params["SaleReferenceId"].TrimEnd());
                        RefId = Request.Params["RefId"].TrimEnd();
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = ex + "<br>" + " وضعیت:مشکلی در پرداخت بوجود آمده ، در صورتی که وجه پرداختی از حساب بانکی شما کسر شده است آن مبلغ به صورت خودکار برگشت داده خواهد شد ";
                        ViewBag.SaleReferenceId = "**************";
                        return;
                    }
                    var bpService = new MellatGateway.PaymentGatewayClient();
                    string Result;
                    Result = bpService.bpVerifyRequest(TerminalId, UserName, UserPassword, SaleOrderId, SaleOrderId, SaleReferenceId);

                    _logsRepository.LogEvent(Result, 1, "");
                    if (!string.IsNullOrEmpty(Result))
                    {
                        if (Result == "0")
                        {
                            _logsRepository.LogEvent("Enter result 0 if", 1, "");

                            long paymentID = Convert.ToInt64(SaleOrderId);
                            var customerId = 0;
                            var phoneNumber = string.Empty;
                            var CustomerName = string.Empty;

                            using (var _context = new MyDbContext())
                            {
                                var p = _context.EPayments.Find(paymentID);
                                _logsRepository.LogEvent(paymentID.ToString(), 1, "");
                                var i = _context.Invoices.Find(p.InvoiceId);
                                _logsRepository.LogEvent(p.InvoiceId.ToString(), 1, "");
                                customerId = i.CustomerId ?? 0;
                                var customer = _customerRepo.Get(customerId);
                                _logsRepository.LogEvent(customer == null ? "customer is null" : customer.Id.ToString(), 1, "");
                                var user = _context.Users.Find(customer?.UserId);
                                phoneNumber = user?.PhoneNumber;
                                CustomerName = user.FirstName;
                            }


                            //var message = $"{customer.User.FirstName} عزیز سفارش شما به شماره{customer.Invoices.Id} با موفقیت ثبت شد";
                            //var message = $"به فروشگاه گرلی مرلی خوش آمدید{Environment.NewLine} کد تایید شما:{code}";

                            SmsHelper.SendOrderStatus(CustomerName, Convert.ToInt64(phoneNumber));

                            _logsRepository.LogEvent("message sent", 1, "");
                            string IQresult;
                            IQresult = bpService.bpInquiryRequest(TerminalId, UserName, UserPassword, SaleOrderId, SaleOrderId, SaleReferenceId);

                            _logsRepository.LogEvent(IQresult, 1, "");
                            if (IQresult == "0")
                            {
                                UpdatePayment(paymentID, Result, SaleReferenceId, RefId, true);
                                ViewBag.Message = "پرداخت با موفقیت انجام شد.";
                                ViewBag.SaleReferenceId = SaleReferenceId;
                                // پرداخت نهایی
                                string Sresult;
                                // تایید پرداخت
                                Sresult = bpService.bpSettleRequest(TerminalId, UserName, UserPassword, SaleOrderId, SaleOrderId, SaleReferenceId);

                                _logsRepository.LogEvent(Sresult, 1, "");
                                if (Sresult != null)
                                {
                                    if (Sresult == "0" || Sresult == "45")
                                    {
                                        //تراکنش تایید و ستل شده است 
                                    }
                                    else
                                    {
                                        //تراکنش تایید شده ولی ستل نشده است
                                    }
                                }
                            }
                            else
                            {
                                string Rvresult;
                                //عملیات برگشت دادن مبلغ
                                Rvresult = bpService.bpReversalRequest(TerminalId, UserName, UserPassword, SaleOrderId, SaleOrderId, SaleReferenceId);

                                _logsRepository.LogEvent(Rvresult, 1, "");
                                ViewBag.Message = "تراکنش بازگشت داده شد";
                                ViewBag.SaleReferenceId = "**************";
                                long paymentId = Convert.ToInt64(SaleOrderId);
                                UpdatePayment(paymentId, IQresult, SaleReferenceId, RefId, false);
                            }
                        }
                        else
                        {
                            ViewBag.Message = PaymentResult.MellatResult(Result);
                            ViewBag.SaleReferenceId = "**************";
                            long paymentId = Convert.ToInt64(SaleOrderId);
                            UpdatePayment(paymentId, Result, SaleReferenceId, RefId, false);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "شماره رسید قابل قبول نیست";
                        ViewBag.SaleReferenceId = "**************";
                    }
                }
                catch (Exception ex)
                {
                    string errors = ex.Message;
                    ViewBag.Message = "مشکلی در پرداخت به وجود آمده است ، در صورتیکه وجه پرداختی از حساب بانکی شما کسر شده است آن مبلغ به صورت خودکار برگشت داده خواهد شد";
                    ViewBag.SaleReferenceId = "**************";
                }
            }
        }


        private void UpdatePayment(long PaymentID, string VResult, long SaleRefrenceID, string RefID, bool PeymentFinished = true)
        {
            using (var _context = new MyDbContext())
            {
                var p = _context.EPayments.Find(PaymentID);
                p.PaymentStatus = VResult;
                p.SaleRefrenceID = (int)SaleRefrenceID;
                p.PaymentFinished = PeymentFinished;
                if (RefID != null)
                {
                    p.ReferenceNumber = RefID;
                }
                _context.Entry(p).State = EntityState.Modified;
                _context.SaveChanges();
                if (PeymentFinished == true)
                {
                    var invoice = _context.Invoices.Include(i => i.InvoiceItems)
                        .FirstOrDefault(i => i.Id == p.InvoiceId);

                    if (invoice.InvoiceItems != null && invoice.InvoiceItems.Any())
                    {
                        //var countOfInvoiceItme = invoice.InvoiceItems.Count().ToString();
                        //_logsRepository.LogEvent("invoiceItme", 1, countOfInvoiceItme);

                        foreach (var item in invoice.InvoiceItems)
                        {
                            //_logsRepository.LogEvent("invoiceItme", 2, item.MainFeatureId.ToString());

                            var productMainFeature = _context.ProductMainFeatures.Find(item.MainFeatureId);
                            if (productMainFeature != null)
                            {
                                //_logsRepository.LogEvent("mainFeature", 3, productMainFeature.Quantity.ToString());

                                productMainFeature.Quantity -= item.Quantity;
                                _context.Entry(productMainFeature).State = EntityState.Modified;
                                _context.SaveChanges();

                                //_logsRepository.LogEvent("mainFeature", 4, productMainFeature.Quantity.ToString());
                            }
                        }
                    }

                    invoice.IsPayed = true;
                    _context.Entry(invoice).State = EntityState.Modified;
                    _context.SaveChanges();

                    //_logsRepository.LogEvent("isPayed", 5, invoice.IsPayed.ToString());


                    if (PeymentFinished == true && invoice.DiscountCodeId != null)
                    {
                        // deActive discountCode for currentCustomer
                        var discountCodeID = invoice.DiscountCodeId;
                        DiscountCode discount = _context.DiscountCodes.Find(discountCodeID);
                        discount.IsActive = false;
                        _context.Entry(discount).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
            }
        }

        //public ActionResult Test()
        //{
        //    var customer = _customerRepo.GetCurrentCustomer();
        //    SmsHelper.SendOrderStatus(customer.User.FirstName, Convert.ToInt64(customer.User.PhoneNumber));
        //    return View();
        //}
    }
}