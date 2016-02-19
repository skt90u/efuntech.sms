using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFunTech.Sms.Schema;
using EFunTech.Sms.Portal.Models;
using Newtonsoft.Json;

namespace EFunTech.Sms.CodeGenerator
{
    public class SendMessageRuleExamples
    {
        private string GetFilePath(string filename) { 
            
            string dir = @"C:\Project\efuntech.sms\EFunTech.Sms.Portal\GenerateResult\SendMessageRuleExamples";

            Utils.MakeSureDirExist(dir);
            
            return System.IO.Path.Combine(dir, filename + ".json");
        }

        // 載入大量名單
        public void Example01() { 
            var model = new SendMessageRuleModel();

            model.SendTitle = string.Empty;
            model.SendBody = "I_AM_SendBody";
            
            model.RecipientFromType = RecipientFromType.FileUpload;
            model.RecipientFromFileUpload = new RecipientFromFileUploadModel 
            { 
                UploadedFileId = 123
            };
            model.RecipientFromCommonContact = new RecipientFromCommonContactModel 
            { 
                ContactIds = "1,2,3,4,5"
            };
            model.RecipientFromGroupContact = new RecipientFromGroupContactModel
            {
                ContactIds = "1,2,3,4,5"
            };
            model.RecipientFromManualInput = new RecipientFromManualInputModel
            {
                PhoneNumbers = "886921855554"
            };

            model.SendTimeType = SendTimeType.Immediately;
            model.SendDeliver = new SendDeliverModel {
                SendTime = DateTime.UtcNow,
            };
            model.SendCycleEveryDay = new SendCycleEveryDayModel {
                SendTime = DateTime.UtcNow,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
            };
            model.SendCycleEveryWeek = new SendCycleEveryWeekModel {
                SendTime = DateTime.UtcNow,
                DayOfWeeks = "1111111",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
            };
            model.SendCycleEveryMonth = new SendCycleEveryMonthModel {
                SendTime = DateTime.UtcNow,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
            };
            model.SendCycleEveryYear = new SendCycleEveryYearModel {
                SendTime = DateTime.UtcNow,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
            };

            model.SendCustType = SendCustType.OneWay;
            model.UseParam = false;
            model.SendMessageType = SendMessageType.SmsMessage;
            //model.CreatedUser = null;

            string output = JsonConvert.SerializeObject(model, Formatting.Indented);

            System.IO.File.WriteAllText(GetFilePath("載入大量名單"), output);
        }

        // 常用聯絡人
        // 由群組中選取
        // 手動輸入
    }
}
