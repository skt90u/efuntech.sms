using EFunTech.Sms.Schema;
using OneApi.Client.Impl;
using OneApi.Config;
using OneApi.Model;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.CodeGenerator
{
    public class TestOneApi
    {
        public void Case1()
        {
            string userName = "ENVOTIONS";
            string password = "Envo6183";
            string requestId = "1428275602020481488";

            var configuration= new Configuration(userName, password);
            var SMSClient= new SMSClient(configuration);

            DeliveryReportList deliveryReportList = SMSClient.SmsMessagingClient.GetDeliveryReportsByRequestId(requestId);
        }

        public void Case1_()
        {
            string userName = "ENVOTIONS";
            string password = "Envo6183";
            string requestId = "1428275602020481488";

            var configuration = new Configuration(userName, password);
            var SMSClient = new SMSClient(configuration);

            var deliveryReportList = SMSClient.SmsMessagingClient.GetDeliveryReportsByRequestId(requestId);
        }


        public void Case3()
        {
            string senderAddress = SendMessageRule.DefaultSenderAddress;
            string message = "message4";
            var recipientAddress = new string[] { "+886921859698" };
            string requestId = "";

            var configuration = new Configuration("ENVOTIONS", "Envo6183");

            // Initialize SMSClient using the Configuration object
            var SMSClient = new SMSClient(configuration);

            // Send SMS 
            var result = SMSClient.SmsMessagingClient.SendSMS(new SMSRequest(senderAddress, message, recipientAddress));
            requestId = result.ClientCorrelator;
            //requestId = "1430206446300481957";
            // Wait for 30 seconds to give enought time for the message to be delivered
            System.Threading.Thread.Sleep(30000);

            // Get 'Delivery Reports'
            DeliveryReportList deliveryReportList = SMSClient.SmsMessagingClient.GetDeliveryReportsByRequestId(requestId);
            Console.WriteLine(deliveryReportList);
        }

        public static class MobileUtil
        {
            private const string defaultIso3166 = "TW";
            private static List<RegionInfo> regionInfos = null;
            public static List<RegionInfo> GetCountriesByIso3166()
            {
                if (regionInfos == null)
                {
                    RegionInfo defaultCountry = null;

                    var countries = new List<RegionInfo>();
                    foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                    {
                        var country = new RegionInfo(culture.LCID);
                        if (countries.Where(p => p.Name == country.Name).Count() == 0)
                        {
                            if (country.Name == defaultIso3166)
                            {
                                defaultCountry = country;
                            }
                            else
                            {
                                countries.Add(country);
                            }
                        }
                    }

                    if (defaultCountry != null) countries.Insert(0, defaultCountry);

                    regionInfos = countries;
                }
                return regionInfos;
            }

            public static PhoneNumber Parse(string mobile)
            {
                var iso3166s = GetCountriesByIso3166().Select(p => p.Name).ToList();

                iso3166s.Insert(0, defaultIso3166);

                PhoneNumber number = null;
                foreach (var iso3166 in iso3166s)
                {
                    PhoneNumber _number = PhoneNumberUtil.GetInstance().Parse(mobile, defaultIso3166);

                    if (PhoneNumberUtil.GetInstance().IsPossibleNumber(_number))
                    {
                        number = _number;
                        break;
                    }
                }

                return number;
            }

            public static bool IsPossibleNumber(string mobile)
            {
                return Parse(mobile) != null;
            }

            public static string Format(string mobile, PhoneNumberFormat fmt)
            {
                PhoneNumber number = Parse(mobile);
                return number != null ? PhoneNumberUtil.GetInstance().Format(number, fmt) : null /* 目前認為傳回null，正好可以表示這個輸入的號碼不合法 */;
            }
        }

        public void Case2()
        {
            //return;

            string mobile = "0928873075";

            try
            {
                foreach (var fmt in new PhoneNumberFormat[] { 
                    //PhoneNumberFormat.INTERNATIONAL, 
                    //PhoneNumberFormat.NATIONAL, 
                    PhoneNumberFormat.E164 })
                {
                    string formattedNumber = MobileUtil.Format(mobile, fmt);

                    formattedNumber = formattedNumber.Replace("+", string.Empty);

                    string userName = "ENVOTIONS";
                    string password = "Envo6183";

                    var configuration = new Configuration(userName, password);
                    var SMSClient = new SMSClient(configuration);

                    var SMSRequest = new SMSRequest(userName, formattedNumber, new string[] { formattedNumber });

                    //SendMessageResult sendMessageResult = SMSClient.SmsMessagingClient.SendSMS(SMSRequest);

                    //string requestId = sendMessageResult.ClientCorrelator; // you can use this to get deliveryReportList later.

                    
                    //Console.WriteLine(formattedNumber);
                    //Console.WriteLine(requestId);
                }
            }
            catch 
            {
                throw;
            }
        }
    }
}
