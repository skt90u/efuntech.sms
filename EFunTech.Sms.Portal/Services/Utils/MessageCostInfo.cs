using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal
{
    public class MessageCostInfo
    {
        public MessageCostInfo(string sendBody, string mobile, Dictionary<string, string> replacements = null)
        {
            init(sendBody, mobile, replacements);
        }

        private void init(string sendBody, string mobile, Dictionary<string, string> replacements = null)
        {
            string e164PhoneNumber = MobileUtil.GetE164PhoneNumber(mobile);

            if (e164PhoneNumber == null)
                throw new Exception(string.Format("{0} 不是正確格式的手機門號", mobile));

            string message = sendBody;

            string SendBody = "";
            decimal MessageLength = 0; // 簡訊字數
            decimal MessageNum = 0; // 簡訊總共幾則
            decimal MessageCost = 0; // 簡訊花費點數
            string MessageFormatError = "";

            if (replacements != null)
            {
                foreach (var key in replacements.Keys)
                {
                    var val = replacements[key] ?? string.Empty;
                    message = replaceAllStr(message, key, val);
                }
            }

            SendBody = message;
            MessageLength = message.Length;

            if (MessageLength != 0) 
            {
                if (hasChinese(message)) 
                {
                    // 中文簡訊計算(長簡訊發送計算)
                    if (MessageLength > 333) {
                        var tempCharge = Math.Floor(MessageLength / 333);
                        var remainder = MessageLength - (tempCharge * 333);
                        if (remainder <= 70) {
                            MessageCost = tempCharge * 5 + 1;
                        } else {
                            MessageCost = tempCharge * 5 + Math.Ceiling(remainder / 67);
                        }
                    } else if (MessageLength <= 70) {
                        MessageCost = 1;
                    } else {
                        MessageCost = Math.Ceiling(MessageLength / 67);
                    }
                    //計算長簡訊的發送則數
                    if (MessageLength > 333) {
                        if (MessageLength <= 656) {
                            MessageNum = 2;
                        } else {
                            var length = MessageLength - 656;
                            MessageNum = 2 + Math.Ceiling(length / 328);
                        }
                    } else {
                        MessageNum = 1;
                    }
                }
                else 
                {
                    // 英文簡訊計算
                    if (!isPureEnglishCheck(message)) {
                        MessageFormatError = "純英文發送內容時不可有`^";
                    }

                    if (MessageLength > 765) {
                        var tempCharge = Math.Floor(MessageLength / 765);
                        var remainder = MessageLength - (tempCharge * 765);
                        if (remainder <= 160) {
                            MessageCost = tempCharge * 5 + 1;
                        } else {
                            MessageCost = tempCharge * 5 + Math.Ceiling(remainder / 153);
                        }
                    } else if (MessageLength <= 160) {
                        MessageCost = 1;
                    } else {
                        MessageCost = Math.Ceiling(MessageLength / 153);
                    }
                    //計算長簡訊的發送則數
                    if (MessageLength > 765) {
                        if (MessageLength <= 1520) {
                            MessageNum = 2;
                        } else {
                            var length = MessageLength - 1520;
                            MessageNum = 2 + (int)Math.Ceiling(length / 760);
                        }
                    } else {
                        MessageNum = 1;
                    }
                }
            }

            this.SendBody = SendBody;
            this.MessageLength = (int)MessageLength; // 簡訊字數
            this.MessageNum = (int)MessageNum; // 簡訊總共幾則

            this.MessageCost = (!MobileUtil.IsInternationPhoneNumber(e164PhoneNumber) ? 1.0m : 3.0m) * MessageCost; // 簡訊花費點數(國際簡訊以三倍計價)
            this.MessageFormatError = MessageFormatError;
        }

        private string replaceAllStr(string str, string oldValue, string newValue)
        {
            str = str.Replace(oldValue, newValue);
            return str;
        }

        private bool hasChinese(string message)
        {
            var found = false;

            for (var i = 0; i < message.Length; i++)
            {
                if ((int)message[i] > 127) {
                    found = true;
                    break;
                }
            }

            return found;
        }

        private bool isPureEnglishCheck(string message)
        {
            var found = false;

            var illegals = new string[]{"`", "^"};

            for (var i = 0; i < message.Length; i++)
            {
                if (illegals.Contains(message[i].ToString())) {
                    found = true;
                }
            }

            return !found;
        }

        public string SendBody { get; private set; }
        public int MessageLength { get; private set; } // 簡訊字數
        public int MessageNum { get; private set; } // 簡訊總共幾則
        public decimal MessageCost { get; private set; } // 簡訊花費點數
        public string MessageFormatError { get; private set; }
    }
}