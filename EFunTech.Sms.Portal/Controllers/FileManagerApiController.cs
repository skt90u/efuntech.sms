
using JUtilSharp.Database;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EFunTech.Sms.Schema;
using System.Net;
using EFunTech.Sms.Portal.Controllers.Common;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Hosting;
using EFunTech.Sms.Core;
using Ionic.Zip;
using System.Text;


namespace EFunTech.Sms.Portal.Controllers
{
    /// <summary>
    /// HttpPost 傳遞參數是 FormCollection，因此以下傳遞方式
    /// 這是錯誤的
    /// public ActionResult MessageReceiverList(HttpPostedFileBase attachment, string arg2, string arg3, Class arg4)
    /// 這是正確的
    /// public ActionResult MessageReceiverList(HttpPostedFileBase attachment, string arg2, string arg3)
    /// 結論，不可傳遞類別
    /// </summary>
    public class FileManagerApiController : MvcControllerBase
    {
        public FileManagerApiController(IUnitOfWork unitOfWork, ILogService logService)
            :base(unitOfWork, logService)
        {
            
        }


        #region UploadedMessageReceiverList


        //public static readonly TimeSpan DefaultTimezoneOffset = TimeZoneInfo.Local.BaseUtcOffset;
        //public static readonly TimeSpan DefaultTimezoneOffset = TimeSpan.Zero;

        
        /// <summary>
        /// 上傳簡訊接收者
        /// </summary>
        [HttpPost]
        [Route("FileManagerApi/UploadMessageReceiverList")]
        public ActionResult UploadMessageReceiverList(HttpPostedFileBase attachment, bool useParam)
        {
            try
            {
                UploadedFile uploadedFile = SaveUploadedFile(attachment, useParam ? UploadedFileType.SendParamMessage : UploadedFileType.SendMessage);

                List<UploadedMessageReceiverList> list = LoadFile<UploadedMessageReceiverList>(attachment);

                return HandleUploadedFile(list, uploadedFile, useParam);
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                return HttpExceptionResult("上傳簡訊接收者失敗", ex);
            }
        }

        private class UploadedMessageReceiverList
        {
            // 姓名	手機門號	電子郵件	傳送日期
            // Irene	0911111111      	sample@hipage.hinet.net	200802141700

            /// <summary>
            /// 「姓名」非必填欄位。但主要為了報表使用，若有報表需求，建議填寫。
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 「手機門號」為必填欄位。
            /// </summary>
            [Required]
            public string Mobile { get; set; }

            /// <summary>
            /// 「電子郵件」非必填欄位。如果你要同時發簡訊到Email，則可填寫。									
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// 「傳送時間」非必填欄位。如果您要傳給每個收訊者的發送時間不相同，則務必逐筆設定發送日期。
            ///     如果要設定傳送日期，則此欄位之接受訊息者的接收時間必須全部填寫不能空白。Ex：要傳給100個用戶，但其中有 5 個用戶發送時間不同，則需一一設定100個發簡訊的時間。		
            ///     其格式為西元年月日時分（除年份外，其餘皆2位數  ）。   ex:200802051330，表示2008年2月5日下午1點30分傳送。									
            /// 傳送日期以此 Excel 檔案所設定的時間為準。
            /// </summary>
            public string SendTime { get; set; }

            /// <summary>
            /// 參數一
            ///     只有在 SendParamMessage 執行上傳(UseParam = true)，這個參數才會用到
            /// </summary>
            public string Param1 { get; set; }

            /// <summary>
            /// 參數二
            ///     只有在 SendParamMessage 執行上傳(UseParam = true)，這個參數才會用到
            /// </summary>
            public string Param2 { get; set; }

            /// <summary>
            /// 參數三
            ///     只有在 SendParamMessage 執行上傳(UseParam = true)，這個參數才會用到
            /// </summary>
            public string Param3 { get; set; }

            /// <summary>
            /// 參數四
            ///     只有在 SendParamMessage 執行上傳(UseParam = true)，這個參數才會用到
            /// </summary>
            public string Param4 { get; set; }

            /// <summary>
            /// 參數五
            ///     只有在 SendParamMessage 執行上傳(UseParam = true)，這個參數才會用到
            /// </summary>
            public string Param5 { get; set; }
        }

        private ActionResult HandleUploadedFile(List<UploadedMessageReceiverList> list, UploadedFile uploadedFile, bool useParam)
        {
            using (var scope = this.unitOfWork.CreateTransactionScope())
            {
                var repository = this.unitOfWork.Repository<UploadedMessageReceiver>();

                var successCnt = 0;

                // 清空目前使用者上傳的所有收訊人名單 // 20150913 Norman, 暫時不可以清空
                // this.unitOfWork.Repository<UploadedMessageReceiver>().Delete(p => p.CreatedUser.Id == CurrentUser.Id);

                //foreach (var model in list)
                for (int i = 0; i < list.Count; i++)
                {
                    var model = list[i];

                    if (string.IsNullOrEmpty(model.Mobile)) continue;

                    var entity = new UploadedMessageReceiver();
                    entity.RowNo = i+1;
                    entity.Name = model.Name;
                    entity.Mobile = model.Mobile;
                    entity.E164Mobile = MobileUtil.GetE164PhoneNumber(model.Mobile);
                    entity.Region = MobileUtil.GetRegionName(model.Mobile);
                    entity.Email = model.Email;

                    entity.SendTime = Converter.ToUniversalTime(model.SendTime, Converter.yyyyMMddHHmm, ClientTimezoneOffset);
                    entity.ClientTimezoneOffset = ClientTimezoneOffset;
                    entity.SendTimeString = model.SendTime;
                    entity.UseParam = useParam;
                    entity.Param1 = model.Param1;
                    entity.Param2 = model.Param2;
                    entity.Param3 = model.Param3;
                    entity.Param4 = model.Param4;
                    entity.Param5 = model.Param5;
                    entity.CreatedUserId = CurrentUserId;
                    entity.CreatedTime = uploadedFile.CreatedTime;
                    entity.UploadedFile = uploadedFile;
                    entity.UploadedSessionId = uploadedFile.Id;
                    

                    var error = string.Empty;
                    var isValid = this.validationService.Validate(entity, out error);

                    if (isValid)
                        successCnt++;

                    // 目前就算驗證不過也沒關係，仍然可以存檔
                    entity = repository.Insert(entity);
                }

                scope.Complete();

                string message = successCnt == list.Count
                    ? string.Format("上傳簡訊接收者成功，總共上傳{0}筆資料", list.Count)
                    : string.Format("上傳簡訊接收者成功，總共上傳{0}筆資料({1}筆有效，{2}筆無效)", list.Count, successCnt, list.Count - successCnt);

                var result = new UploadedMessageReceiverListResult
                {
                    FileName = uploadedFile.FileName,
                    Message = message,
                    ValidCount = successCnt,
                    InvalidCount = list.Count - successCnt,
                    UploadedSessionId = uploadedFile.Id,
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region UploadBlacklist

        /// <summary>
        /// 上傳黑名單
        /// </summary>
        [HttpPost]
        [Route("FileManagerApi/UploadBlacklist")]
        public ActionResult UploadBlacklist(HttpPostedFileBase attachment)
        {
            try
            {
                UploadedFile uploadedFile = SaveUploadedFile(attachment, UploadedFileType.Blacklist);

                List<UploadedBlacklist> list = LoadFile<UploadedBlacklist>(attachment);

                return HandleUploadedFile(list, uploadedFile);
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                return HttpExceptionResult("上傳黑名單失敗", ex);
            }
        }

        private class UploadedBlacklist
        {
            public string Name { get; set; }
            public string Mobile { get; set; }
            public string Remark { get; set; }
        }

        private ActionResult HandleUploadedFile(List<UploadedBlacklist> list, UploadedFile uploadedFile)
        {
            using(var scope = this.unitOfWork.CreateTransactionScope())
            {
                var repository = this.unitOfWork.Repository<Blacklist>();
                
                var successCnt = 0;

                foreach (var model in list)
                {
                    if (string.IsNullOrEmpty(model.Mobile)) continue;

                    var entity = new Blacklist();
                    entity.Name = model.Name;
                    entity.Mobile = model.Mobile;
                    entity.E164Mobile = MobileUtil.GetE164PhoneNumber(model.Mobile);
                    entity.Region = MobileUtil.GetRegionName(model.Mobile);
                    entity.Enabled = true;
                    entity.Remark = model.Remark;
                    entity.UpdatedTime = uploadedFile.CreatedTime;
                    entity.CreatedUserId = CurrentUserId;
                    entity.UpdatedUserName = CurrentUserName;
                    entity.UploadedFile = uploadedFile;

                    var error = string.Empty;
                    var isValid = this.validationService.Validate(entity, out error);

                    if (isValid)
                    {
                        entity = repository.Insert(entity);
                        successCnt++;
                    }
                }

                scope.Complete();

                string message = successCnt == list.Count
                    ? string.Format("上傳黑名單成功，總共上傳{0}筆資料", list.Count)
                    : string.Format("上傳黑名單成功，總共上傳{0}筆資料({1}筆成功，{2}筆失敗)", list.Count, successCnt, list.Count - successCnt);

                var result = new FileUploadResult
                {
                    FileName = uploadedFile.FileName,
                    Message = message,
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        private class UploadedContact
        {
            // 姓名	
            public string Name { get; set; }
            // 手機號碼1	
            public string Mobile { get; set; }
            // 住家電話	
            public string HomePhone { get; set; }
            // 公司電話	
            public string CompanyPhone { get; set; }
            // 電子郵件	
            public string Email { get; set; }
            // Msn	
            public string Msn { get; set; }
            // 概述	
            public string Description { get; set; }
            // 生日	
            public string Birthday { get; set; }
            // 重要日期	
            public string ImportantDay { get; set; }
            // 性別	
            public string Gender { get; set; }
            // 群組
            public string Group { get; set; }
        }

        private ActionResult HandleUploadedFile(List<UploadedContact> list, UploadedFile uploadedFile)
        {
            using (var scope = this.unitOfWork.CreateTransactionScope())
            {
                var repository = this.unitOfWork.Repository<Contact>();

                var successCnt = 0;

                foreach (var model in list)
                {
                    // 姓名以及行動電話必填
                    if (string.IsNullOrEmpty(model.Name)) continue;
                    if (string.IsNullOrEmpty(model.Mobile)) continue;

                    var entity = new Contact();
                    entity.Name = model.Name;
                    entity.Mobile = model.Mobile;
                    entity.E164Mobile = MobileUtil.GetE164PhoneNumber(model.Mobile);
                    entity.Region = MobileUtil.GetRegionName(model.Mobile);
                    entity.HomePhone = model.HomePhone;
                    entity.CompanyPhone = model.CompanyPhone;
                    entity.Email = model.Email;
                    entity.Msn = model.Msn;
                    entity.Description = model.Description;
                    entity.Birthday = model.Birthday;
                    entity.ImportantDay = model.ImportantDay;
                    entity.Gender = model.Gender == "2" ? Gender.Female :
                                    model.Gender == "1" ? Gender.Male : Gender.Unknown;
                    entity.CreatedUserId = CurrentUserId;

                    var error = string.Empty;
                    var isValid = this.validationService.Validate(entity, out error);

                    if (isValid)
                    {
                        entity = repository.Insert(entity);

                        string groupDescription = model.Group.Trim();
                        if (!string.IsNullOrEmpty(groupDescription))
                        {
                            var group = this.unitOfWork.Repository<Group>().DbSet
                                            .Where(p => p.CreatedUserId == CurrentUserId && p.Name == model.Group.Trim())
                                            .FirstOrDefault();
                            if (group != null)
                            {
                                this.unitOfWork.Repository<GroupContact>().Insert(new GroupContact { 
                                    GroupId = group.Id,
                                    ContactId = entity.Id
                                });
                            }
                        }

                        successCnt++;
                    }
                }

                scope.Complete();

                string message = successCnt == list.Count
                    ? string.Format("上傳聯絡人成功，總共上傳{0}筆資料", list.Count)
                    : string.Format("上傳聯絡人成功，總共上傳{0}筆資料({1}筆成功，{2}筆失敗)", list.Count, successCnt, list.Count - successCnt);

                var result = new FileUploadResult
                {
                    FileName = uploadedFile.FileName,
                    Message = message,
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 上傳聯絡人
        /// </summary>
        [HttpPost]
        [Route("FileManagerApi/UploadContact")]
        public ActionResult UploadContact(HttpPostedFileBase attachment)
        {
            try
            {
                UploadedFile uploadedFile = SaveUploadedFile(attachment, UploadedFileType.Contact);

                List<UploadedContact> list = LoadFile<UploadedContact>(attachment);

                return HandleUploadedFile(list, uploadedFile);
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                return HttpExceptionResult("上傳聯絡人失敗", ex);
            }
        }

        #region Helpers
        
        private UploadedFile SaveUploadedFile(HttpPostedFileBase attachment, UploadedFileType uploadedFileType)
        {
            string uploadDir = Path.Combine(HostingEnvironment.MapPath("~/"), "Upload");
            string filePath = Path.Combine(uploadDir, Guid.NewGuid().ToString().Replace("-", string.Empty) + Path.GetExtension(attachment.FileName));

            // 由於未來是將伺服器放置在 Azure 中， 因此暫時沒有計畫要儲存檔案
            // attachment.SaveAs(filePath); 

            var entity = new UploadedFile();
            entity.FileName = attachment.FileName;
            entity.FilePath = filePath;
            entity.UploadedFileType = uploadedFileType;
            entity.CreatedUserId = CurrentUserId;
            entity.CreatedTime = DateTime.UtcNow;
            
            entity = this.unitOfWork.Repository<UploadedFile>().Insert(entity);

            return entity;
        }

        private string SafeString(object o)
        {
            o = o ?? string.Empty;

            return o.ToString();
        }

        private List<T> LoadZipFile<T>(Stream fileStream)
        {
            var list = new List<T>();

            var options = new ReadOptions { 
                Encoding = Encoding.GetEncoding(950) // 繁體中文
            };

            using (ZipFile zip = ZipFile.Read(fileStream, options))
            {
                foreach (var zipEntry in zip)
                {
                    using (var ms = new MemoryStream())
                    {
                        zipEntry.Extract(ms);

                        // The StreamReader will read from the current 
                        // position of the MemoryStream which is currently 
                        // set at the end of the string we just wrote to it. 
                        // We need to set the position to 0 in order to read 
                        // from the beginning.
                        ms.Position = 0;

                        var extension = Path.GetExtension(zipEntry.FileName).Replace(".", string.Empty).ToLower();

                        switch (extension)
                        {
                            case "csv":
                                {
                                    list.AddRange(LoadCsvFile<T>(ms));
                                } break;
                            case "zip":
                                {
                                    list.AddRange(LoadZipFile<T>(ms));
                                } break;
                            case "xlsx":
                                {
                                    list.AddRange(LoadExcelFile<T>(ms));
                                } break;
                        }
                    }

                    
                }
            }

            return list;
        }
        private List<T> LoadCsvFile<T>(Stream fileStream)
        {
            const bool ignoreFirstLine = true;

            var list = new List<T>();

            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            using (var streamReader = new StreamReader(fileStream, System.Text.Encoding.UTF8))
            {
                int lineNo = 0;

                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();

                    if (lineNo == 0 && ignoreFirstLine)
                    {
                        lineNo++;
                        continue;
                    }

                    string[] tokens = line.Split(new char[] { ',', '\t' }, StringSplitOptions.None);

                    var instance = (T)Activator.CreateInstance(type);

                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (i >= tokens.Length) continue;

                        PropertyInfo property = properties[i];

                        if (property.PropertyType != typeof(string))
                        {
                            string message = string.Format("在泛型的 LoadFile 函式只有使用 string 型別的 property");
                        }

                        string value = tokens[i];

                        property.SetValue(instance, value);
                    }

                    list.Add(instance);

                    lineNo++;
                }
            }

            return list;
        }

        #region LoadExcelFile
        
        private List<T> LoadExcelFile<T>(Stream fileStream)
        {
            var list = new List<T>();

            using (var package = new ExcelPackage(fileStream))
            {
                // Worksheets[1] -> 第一個 worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                int row = 2;

                Type type = typeof(T);

                PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                while (true)
                {
                    var instance = (T)Activator.CreateInstance(type);

                    bool bNonEmptyCell = false;

                    for (int i = 0; i < properties.Length; i++)
                    {
                        PropertyInfo property = properties[i];

                        if (property.PropertyType != typeof(string))
                        {
                            string message = string.Format("在泛型的 LoadFile 函式只有使用 string 型別的 property");
                        }

                        string value = SafeString(worksheet.Cells[row, (i + 1)].Value);

                        property.SetValue(instance, value);

                        if (!string.IsNullOrEmpty(value)) bNonEmptyCell = true; // 每一行至少必須有一個內容不為空
                    }

                    if (!bNonEmptyCell) break; // 如果讀到某一行，所有 Cell 都為空，則停止讀取檔案

                    list.Add(instance);

                    row++;
                }
            }
            return list;
        }
        #endregion

        private List<T> LoadFile<T>(HttpPostedFileBase attachment)
        {

            var list = new List<T>();

            var extension = Path.GetExtension(attachment.FileName).Replace(".", string.Empty).ToLower();

            switch (extension)
            {
                case "csv": 
                {
                    list = LoadCsvFile<T>(attachment.InputStream);
                } break;
                case "zip": 
                {
                    list = LoadZipFile<T>(attachment.InputStream);
                } break;
                case "xlsx":
                {
                    list = LoadExcelFile<T>(attachment.InputStream);
                } break;
            }

            return list;
        }

        //[HttpPost]
        //[Route("FileManagerApi/Upload")]
        //public ActionResult Upload(UploadedFileType uploadedFileType, HttpPostedFileBase attachment)
        //{
        //    ActionResult result = null;

        //    switch (uploadedFileType)
        //    {
        //        case UploadedFileType.SendMessage:
        //            {

        //            } break;
        //        case UploadedFileType.SendParamMessage:
        //            {

        //            } break;
        //        case UploadedFileType.Contact:
        //            {

        //            } break;
        //        case UploadedFileType.Blacklist:
        //            {
        //                result = UploadBlacklist(attachment);
        //            } break;
        //    }

        //    return result;
        //}
        #endregion
    }

    public class FileUploadResult
    {
        public string Message { get; set; }

        public string FileName { get; set; }
    }

    public class UploadedMessageReceiverListResult : FileUploadResult
    {
        /// <summary>
        /// 有效名單
        /// </summary>
        public int ValidCount { get; set; }

        /// <summary>
        /// 有效名單
        /// </summary>
        public int InvalidCount { get; set; }

        /// <summary>
        /// 由於每個允許使用者針對上傳名單，再進行新增
        /// 因此會發生沒有對應 UploadedFile 的狀況
        /// 
        /// 這時候，如何判斷是否為同一批上傳名單
        /// 我想到的方式是根據既有的 UploadedFile 的 Id 作為 識別碼
        /// </summary>
        public int UploadedSessionId { get; set; }

    }
}