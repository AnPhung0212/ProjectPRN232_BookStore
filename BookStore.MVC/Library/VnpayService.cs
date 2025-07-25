using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using VNPAY.NET;
using VNPAY.NET.Models;
using VNPAY.NET.Enums;

namespace BookStore.MVC.Library
{
    public class VnpayService : IVnpay
    {
        private string _tmnCode;
        private string _hashSecret;
        private string _baseUrl;
        private string _returnUrl;
        private string _version;
        private string _orderType;
        private readonly IConfiguration _configuration;

        public VnpayService(IConfiguration configuration)
        {
            _configuration = configuration;
            Initialize(_configuration["Vnpay:TmnCode"], _configuration["Vnpay:HashSecret"], _configuration["Vnpay:BaseUrl"], _configuration["Vnpay:ReturnUrl"]);
        }

        public void Initialize(string tmnCode, string hashSecret, string baseUrl, string returnUrl, string version = "2.1.0", string orderType = "other")
        {
            _tmnCode = tmnCode;
            _hashSecret = hashSecret;
            _baseUrl = baseUrl;
            _returnUrl = returnUrl;
            _version = version;
            _orderType = orderType;
        }

        public string GetPaymentUrl(PaymentRequest request)
        {
            try
            {
                var vnpay = new VNPayLibrary();
                vnpay.AddRequestData("vnp_Version", _version);
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", _tmnCode);
                vnpay.AddRequestData("vnp_Amount", ((long)(request.Money * 100)).ToString());
                string expireDateTime = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss");
                vnpay.AddRequestData("vnp_ExpireDate", expireDateTime);
                string orderDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                vnpay.AddRequestData("vnp_CreateDate", orderDateTime);
                vnpay.AddRequestData("vnp_CurrCode", "VND");

                // Get client IP address
                string ipAddress = GetIpAddress();
                vnpay.AddRequestData("vnp_IpAddr", ipAddress);

                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {request.PaymentId}");
                vnpay.AddRequestData("vnp_OrderType", _orderType);
                vnpay.AddRequestData("vnp_ReturnUrl", _returnUrl);
                vnpay.AddRequestData("vnp_TxnRef", request.PaymentId.ToString());


                string paymentUrl = vnpay.CreateRequestUrl(_baseUrl, _hashSecret);

                Console.WriteLine("Link thanh toán VNPay: " + paymentUrl);
                return paymentUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating VNPay URL: {ex.Message}");
                throw;
            }
        }

        public PaymentResult GetPaymentResult(IQueryCollection parameters)
        {
            var queryParams = parameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var transactionStatus = queryParams["vnp_TransactionStatus"];
            var amount = int.Parse(queryParams["vnp_Amount"]) / 100;

            var result = new PaymentResult
            {
                IsSuccess = transactionStatus == "00",
                PaymentId = int.Parse(queryParams["vnp_TxnRef"]),
                Description = transactionStatus == "00" ? "Payment successful" : "Payment failed",
                TransactionStatus = new TransactionStatus()
                {
                    Code = transactionStatus == "00" ? TransactionStatusCode.Code_00 : TransactionStatusCode.Code_02,
                    Description = transactionStatus == "00" ? "Payment Successful" : "Payment Failed"
                }
            };

            return result;
        }
        private string GetIpAddress()
        {
            try
            {
                string ipAddress = "127.0.0.1";
                var hostName = Dns.GetHostName();
                var ipHostInfo = Dns.GetHostEntry(hostName);
                var ipAddressList = ipHostInfo.AddressList;

                if (ipAddressList.Length > 0)
                {
                    var localIp = ipAddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                    if (localIp != null)
                    {
                        ipAddress = localIp.ToString();
                    }
                }

                return ipAddress;
            }
            catch
            {
                return "127.0.0.1";
            }
        }
        private string ComputeSecureHash(string data, string secret)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                var sb = new StringBuilder();
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}