using System;
using System.ComponentModel;

namespace EFunTech.Sms.Portal
{
    [AttributeUsage(AttributeTargets.All)]
    public class ErrorMessageAttribute : Attribute
    {
        public ErrorMessageAttribute(string errorMessage){
            this.errorMessage = errorMessage;
        }

        private string errorMessage = string.Empty;
        public string Value
        { 
            get
            {
                return errorMessage;
            }
        }
    }

    public static class RegularExpressionPatterns
    {
        [Description("行動電話")]
        [ErrorMessage("不符合行動電話格式。範例：0912345678 或 886912345678。")]
        public const string PhoneNumber = @"^(09\d{8})$|^(886\d{9})$";

        [Description("市內電話")]
        [ErrorMessage("不符合市內電話格式。範例：1234567。")]
        public const string LocalCall = @"^\d+$";

        [Description("市內電話分機")]
        [ErrorMessage("不符合市內電話分機格式。範例：123。")]
        public const string LocalCallExt = @"^\d+$";

        [Description("Email")]
        [ErrorMessage("不符合電子郵件格式。範例：abc@gmail.com。")]
        public const string Email = @"^[\w-.]+@([\w-]+\.)+[\w-]+$";

        [Description("日期格式(e.g. 12/31)")]
        [ErrorMessage("不符合日期格式。範例：12/31。")]
        public const string Date_MMdd = @"^(([1-9])|(0[1-9])|(1[0-2]))\/(([0-9])|([0-2][0-9])|(3[0-1]))$";

        [Description("發送時間格式(e.g. 201512312359)")]
        [ErrorMessage("不符合發送時間格式。範例：201512312359。")]
        public const string SendTime = @"^(19|20)\d{2}(0[1-9]|1[012])(0[1-9]|[12]\d|3[01])([01][0-9]|2[0-3])([0-5][0-9])$";
    }
}