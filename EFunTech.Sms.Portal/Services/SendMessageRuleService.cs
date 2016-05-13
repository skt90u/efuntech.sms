using AutoMapper;
using EFunTech.Sms.Core;
using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Portal.Models.Mapper;
using EFunTech.Sms.Portal.Services;
using EFunTech.Sms.Schema;
using Hangfire;
using JUtilSharp.Database;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using EntityFramework.BulkInsert.Extensions;
using System.Data.Entity;

namespace EFunTech.Sms.Portal
{
    public class SendMessageRuleService
    {
        private IUnitOfWork unitOfWork;
        private ILogService logService;
        private TradeService tradeService;
        private IRepository<SendMessageRule> repository;

        public SendMessageRuleService(IUnitOfWork unitOfWork, ILogService logService)
        {
            this.unitOfWork = unitOfWork;
            this.logService = logService;
            this.tradeService = new TradeService(unitOfWork, logService);
            this.repository = this.unitOfWork.Repository<SendMessageRule>();
        }

        #region 建立簡訊規則

        /// <summary>
        /// 為點數預警建立一個立即發送的簡訊規則
        /// </summary>
        public SendMessageRule CreateCreditWarningSendMessageRule(ApplicationUser user, string subject, string body, string[] destinations)
        {
            var model = new SendMessageRuleModel();
            model.SendTitle = subject;
            model.SendBody = body;

            model.RecipientFromType = RecipientFromType.ManualInput;
            model.RecipientFromFileUpload = null;
            model.RecipientFromCommonContact = null;
            model.RecipientFromGroupContact = null;
            model.RecipientFromManualInput = new RecipientFromManualInputModel
            {
                PhoneNumbers = string.Join(",", destinations)
            };

            model.SendTimeType = SendTimeType.Immediately;
            model.SendDeliver = null;
            model.SendCycleEveryDay = null;
            model.SendCycleEveryWeek = null;
            model.SendCycleEveryMonth = null;
            model.SendCycleEveryYear = null;

            model.SendCustType = SendCustType.OneWay;
            model.UseParam = false;
            model.SendMessageType = SendMessageType.SmsMessage;

            return CreateSendMessageRule(user, model, bCheckCredit: false /* 為點數預警所建立的簡訊規則，不能再次檢驗Credit，否則會造成遞迴呼叫 */);
        }

        /// <summary>
        /// 使用來自網頁輸入的資料，建立簡訊發送規則
        /// </summary>
        public List<SendMessageRule> CreateSendMessageRuleFromWeb(ApplicationUser user, SendMessageRuleModel model)
        {
            var rules = new List<SendMessageRule>();

            switch (model.RecipientFromType)
            {
                case RecipientFromType.FileUpload: // 上傳收訊人名單另外處理
                    {
                        var uploadedFileRepository = this.unitOfWork.Repository<UploadedFile>();
                        var uploadedMessageReceiverRepository = this.unitOfWork.Repository<UploadedMessageReceiver>();
                        var recipientFromFileUploadRepository = this.unitOfWork.Repository<RecipientFromFileUpload>();

                        // 
                        List<DateTime?> sendTimes = uploadedMessageReceiverRepository.GetMany(p =>
                            p.IsValid == true &&
                            p.UploadedSessionId == model.RecipientFromFileUpload.UploadedFileId).Select(p => p.SendTime).Distinct().ToList();

                        var _models = new List<SendMessageRuleModel>();

                        foreach(var sendTime in sendTimes)
                        {
                            // 如果收訊人來源是來自上傳名單，則分成兩種處理方式
                            // 1. 有指定時間的收訊人
                            //  根據不同發送時間，設定不同預約簡訊規則
                            // 2. 無指定時間的收訊人
                            //  根據發送時間設定

                            var _model = (SendMessageRuleModel) ObjectCopier.DeepCopy(model);
                            
                            // 找出 sendTime 對應資料, 並建立新的上傳資料

                            RecipientFromFileUploadModel recipientFromFileUpload = SplitUploadedMessageReceivers(model.RecipientFromFileUpload, sendTime);
                            _model.RecipientFromFileUpload = recipientFromFileUpload;

                            if (sendTime.HasValue)
                            {
                                _model.SendTimeType = SendTimeType.Deliver;
                                _model.SendDeliver = new SendDeliverModel
                                {
                                    SendTime = sendTime.Value,
                                    ClientTimezoneOffset = _model.ClientTimezoneOffset,
                                };
                                _model.SendCycleEveryDay = null;
                                _model.SendCycleEveryWeek = null;
                                _model.SendCycleEveryMonth = null;
                                _model.SendCycleEveryYear = null;
                            }

                            _models.Add(_model);
                        }

                        var totalMessageCost = (decimal)0.0;

                        // 檢查所有 _model 所需費用是否足夠

                        foreach (var _model in _models)
                        {
                            
                            var totalMessageCostInfo = GrottyHacks.Cast(GetTotalMessageCostInfo(user, model),
                                new
                                {
                                    TotalReceiverCount = (int)0,
                                    TotalMessageCost = (decimal)0.0,
                                    RemainingSmsBalance = (decimal)0.0,
                                }
                            );

                            if (this.tradeService.ShouldWithhold(model.SendTimeType))
                            {
                                totalMessageCost += totalMessageCostInfo.TotalMessageCost;
                            }
                        }

                        var remainingSmsBalance = user.SmsBalance - totalMessageCost;

                        if (remainingSmsBalance < 0)
                        {
                            throw new Exception(string.Format("點數不足，目前點數 {0} 點，發送所需點數 {1} 點",
                                user.SmsBalance,
                                totalMessageCost));
                        }

                        // 對所有 _model，建立對應的SendMessageRule

                        foreach (var _model in _models)
                            rules.Add(CreateSendMessageRule(user, _model, bCheckCredit: true));

                        // 刪除原本的 uploadedFile & uploadedMessageReceivers

                        // 先刪除 UploadedMessageReceiver (FK_dbo.UploadedMessageReceivers_dbo.UploadedFiles_UploadedFile_Id)
                        uploadedMessageReceiverRepository.Delete(p => p.UploadedSessionId == model.RecipientFromFileUpload.UploadedFileId);

                        // 再刪除 UploadedFile
                        uploadedFileRepository.Delete(p => p.Id == model.RecipientFromFileUpload.UploadedFileId);

                    }break;
                default: 
                    {
                        rules.Add(CreateSendMessageRule(user, model, bCheckCredit: true));
                    } break;
            }
            
            return rules;
        }

        /// <summary>
        /// 找出 sendTime 對應資料, 並建立新的上傳資料
        /// </summary>
        private RecipientFromFileUploadModel SplitUploadedMessageReceivers(RecipientFromFileUploadModel recipientFromFileUpload, DateTime? sendTime)
        {
            var uploadedFileRepository = this.unitOfWork.Repository<UploadedFile>();
            var uploadedMessageReceiverRepository = this.unitOfWork.Repository<UploadedMessageReceiver>();
            var recipientFromFileUploadRepository = this.unitOfWork.Repository<RecipientFromFileUpload>();

            // Simulate SaveUploadedFile

            var uploadedFile = uploadedFileRepository.GetById(recipientFromFileUpload.UploadedFileId);

            var entityUploadedFile = new UploadedFile(); // (UploadedFile)uploadedFile.DeepCopy(); // Entity 使用 DeepCopy 會失敗

            entityUploadedFile.FileName = uploadedFile.FileName;
            entityUploadedFile.FilePath = uploadedFile.FilePath;
            entityUploadedFile.UploadedFileType = uploadedFile.UploadedFileType;
            entityUploadedFile.CreatedUser = uploadedFile.CreatedUser;
            entityUploadedFile.CreatedTime = uploadedFile.CreatedTime;

            entityUploadedFile = uploadedFileRepository.Insert(entityUploadedFile);

            // Simulate HandleUploadedFile
            var entities = uploadedMessageReceiverRepository
                .GetMany(p => p.IsValid == true &&
                              p.UploadedSessionId == recipientFromFileUpload.UploadedFileId &&
                              p.SendTime == sendTime)
                .ToList()
                .Select((uploadedMessageReceiver, i) => new UploadedMessageReceiver
                {
                    RowNo = i + 1,
                    Name = uploadedMessageReceiver.Name,
                    Mobile = uploadedMessageReceiver.Mobile,
                    Email = uploadedMessageReceiver.Email,

                    SendTime = uploadedMessageReceiver.SendTime,
                    ClientTimezoneOffset = uploadedMessageReceiver.ClientTimezoneOffset,
                    SendTimeString = uploadedMessageReceiver.SendTimeString,
                    UseParam = uploadedMessageReceiver.UseParam,
                    Param1 = uploadedMessageReceiver.Param1,
                    Param2 = uploadedMessageReceiver.Param2,
                    Param3 = uploadedMessageReceiver.Param3,
                    Param4 = uploadedMessageReceiver.Param4,
                    Param5 = uploadedMessageReceiver.Param5,
                    CreatedUserId = uploadedMessageReceiver.CreatedUserId,
                    CreatedTime = uploadedMessageReceiver.CreatedTime,
                    UploadedFile = entityUploadedFile,
                    UploadedSessionId = entityUploadedFile.Id,
                    IsValid = true,
                }).ToList();

            DbContext context = this.unitOfWork.DbContext;
            context.BulkInsert(entities);
            context.MySaveChanges();

            var result = new RecipientFromFileUploadModel();

            result.SendMessageRuleId = default(int);
            result.UploadedFileId = entityUploadedFile.Id;
            result.AddSelfToMessageReceiverList = recipientFromFileUpload.AddSelfToMessageReceiverList;

            return result;
        }

        /// <summary>
        /// 建立簡訊規則
        /// </summary>
        private SendMessageRule CreateSendMessageRule(ApplicationUser user, SendMessageRuleModel model, bool bCheckCredit)
        {
            // string output = JsonConvert.SerializeObject(model);

            // (1.1) 檢查目前使用者是否有足夠點數去發送簡訊
            //      - 發送通數 = COUNT(所有名單)
            //      - 花費點數 = SUM(每則簡訊所需點數)
            //      - 剩餘點數 = 是否為週期簡訊 ? 使用者目前可用點數 : 使用者目前可用點數 - 花費點數
            // (1.2) 如果關閉國際簡訊發送，則不允許發送國際簡訊
            // (2) 新增簡訊規則，並指定簡訊規則狀態為 Prepare (正在建立簡訊規則以及相關資料)
            // (3) 新增 收訊人來源 (RecipientFromFileUpload, RecipientFromCommonContact, RecipientFromGroupContact, RecipientFromManualInput)
            // (4) 新增 簡訊發送時間 (SendDeliver, SendCycleEveryDay, SendCycleEveryWeek, SendCycleEveryMonth, SendCycleEveryYear)
            // (5) 新增 簡訊收訊人 (MessageReceiver)
            // (6) 扣除目前使用者點數
            // (7) 簡訊規則狀態為 Ready // 簡訊規則以及相關資料已經備妥，可以準備發送

            ////////////////////////////////////////
            // (1.1) 檢查目前使用者是否有足夠點數去發送簡訊
            ////////////////////////////////////////

            var totalMessageCostInfo = GrottyHacks.Cast(GetTotalMessageCostInfo(user, model),
                new
                {
                    TotalReceiverCount = (int)0,
                    TotalMessageCost = (decimal)0.0,
                    RemainingSmsBalance = (decimal)0.0,
                });

            if (this.tradeService.ShouldWithhold(model.SendTimeType))
            {
                if (totalMessageCostInfo.RemainingSmsBalance < 0)
                {
                    throw new Exception(string.Format("點數不足，目前點數 {0} 點，發送所需點數 {1} 點",
                        user.SmsBalance,
                        totalMessageCostInfo.TotalMessageCost));
                }
            }

            // (1.2) 如果關閉國際簡訊發送，則不允許發送國際簡訊
            if (!user.ForeignSmsEnabled)
            {
                List<string> internationalMobiles = GetInternationalMobiles(user, model);

                if (internationalMobiles.Count != 0)
                {
                    throw new Exception(string.Format("目前使用者不允許國際簡訊發送，請移除以下門號 {0}", string.Join(",", internationalMobiles)));
                }
            }

            ////////////////////////////////////////
            // (2) 新增簡訊規則，並指定簡訊規則狀態為 Prepare (正在建立簡訊規則以及相關資料)
            ////////////////////////////////////////

            var entity = new SendMessageRule();

            entity.ClientTimezoneOffset = model.ClientTimezoneOffset;

            entity.SendTitle = model.SendTitle;
            entity.SendBody = model.SendBody;

            entity.SendCustType = model.SendCustType;
            entity.UseParam = model.UseParam;
            entity.SendMessageType = model.SendMessageType;
            entity.TotalReceiverCount = totalMessageCostInfo.TotalReceiverCount;
            entity.TotalMessageCost = this.tradeService.ShouldWithhold(model.SendTimeType) ? totalMessageCostInfo.TotalMessageCost : 0;
            entity.RemainingSmsBalance = this.tradeService.ShouldWithhold(model.SendTimeType) ? totalMessageCostInfo.RemainingSmsBalance : user.SmsBalance;

            entity.CreatedTime = DateTime.UtcNow;
            entity.CreatedUserId = user.Id;
            entity.SendMessageRuleStatus = SendMessageRuleStatus.Prepare; // 正在建立簡訊規則以及相關資料已經備妥

            // 20151024 Norman, 讓使用者有能力修改發送來源
            entity.SenderAddress = model.SenderAddress;
            if (string.IsNullOrEmpty(entity.SenderAddress))
                entity.SenderAddress = SendMessageRule.DefaultSenderAddress;
            
            entity = this.repository.Insert(entity);

            ////////////////////////////////////////
            // (3) 新增 收訊人來源 (RecipientFromFileUpload, RecipientFromCommonContact, RecipientFromGroupContact, RecipientFromManualInput)
            ////////////////////////////////////////

            #region 設定 收訊人來源

            entity.RecipientFromType = model.RecipientFromType;

            switch (model.RecipientFromType)
            {
                case RecipientFromType.FileUpload:
                    {
                        RecipientFromFileUpload _entity = Mapper.Map<RecipientFromFileUploadModel, RecipientFromFileUpload>(model.RecipientFromFileUpload);
                        _entity.SendMessageRuleId = entity.Id;
                        _entity.UploadedFile = this.unitOfWork.Repository<UploadedFile>().GetById(model.RecipientFromFileUpload.UploadedFileId);
                        _entity = this.unitOfWork.Repository<RecipientFromFileUpload>().Insert(_entity);
                        entity.RecipientFromFileUpload = _entity;


                    } break;

                case RecipientFromType.CommonContact:
                    {
                        RecipientFromCommonContact _entity = Mapper.Map<RecipientFromCommonContactModel, RecipientFromCommonContact>(model.RecipientFromCommonContact);
                        _entity.SendMessageRuleId = entity.Id;
                        _entity = this.unitOfWork.Repository<RecipientFromCommonContact>().Insert(_entity);
                        entity.RecipientFromCommonContact = _entity;
                    } break;

                case RecipientFromType.GroupContact:
                    {
                        RecipientFromGroupContact _entity = Mapper.Map<RecipientFromGroupContactModel, RecipientFromGroupContact>(model.RecipientFromGroupContact);
                        _entity.SendMessageRuleId = entity.Id;
                        _entity = this.unitOfWork.Repository<RecipientFromGroupContact>().Insert(_entity);
                        entity.RecipientFromGroupContact = _entity;
                    } break;

                case RecipientFromType.ManualInput:
                    {
                        RecipientFromManualInput _entity = Mapper.Map<RecipientFromManualInputModel, RecipientFromManualInput>(model.RecipientFromManualInput);
                        _entity.SendMessageRuleId = entity.Id;
                        _entity = this.unitOfWork.Repository<RecipientFromManualInput>().Insert(_entity);
                        entity.RecipientFromManualInput = _entity;
                    } break;
                default:
                    {
                        throw new Exception(string.Format("Unknown RecipientFromType ({0})", model.RecipientFromType));
                    };
            }
            #endregion

            ////////////////////////////////////////
            // (4) 新增 簡訊發送時間 (SendDeliver, SendCycleEveryDay, SendCycleEveryWeek, SendCycleEveryMonth, SendCycleEveryYear)
            ////////////////////////////////////////

            #region 設定 簡訊發送時間

            entity.SendTimeType = model.SendTimeType;

            switch (model.SendTimeType)
            {
                case SendTimeType.Immediately:
                    {
                    } break;

                case SendTimeType.Deliver:
                    {
                        SendDeliver _entity = Mapper.Map<SendDeliverModel, SendDeliver>(model.SendDeliver);
                        _entity.SendMessageRuleId = entity.Id;
                        _entity = this.unitOfWork.Repository<SendDeliver>().Insert(_entity);
                        entity.SendDeliver = _entity;
                    } break;

                case SendTimeType.Cycle:
                    {
                        if (model.SendCycleEveryDay != null)
                        {
                            SendCycleEveryDay _entity = Mapper.Map<SendCycleEveryDayModel, SendCycleEveryDay>(model.SendCycleEveryDay);
                            _entity.SendMessageRuleId = entity.Id;
                            _entity = this.unitOfWork.Repository<SendCycleEveryDay>().Insert(_entity);
                            entity.SendCycleEveryDay = _entity;
                        }

                        if (model.SendCycleEveryWeek != null)
                        {
                            SendCycleEveryWeek _entity = Mapper.Map<SendCycleEveryWeekModel, SendCycleEveryWeek>(model.SendCycleEveryWeek);
                            _entity.SendMessageRuleId = entity.Id;
                            _entity = this.unitOfWork.Repository<SendCycleEveryWeek>().Insert(_entity);
                            entity.SendCycleEveryWeek = _entity;
                        }

                        if (model.SendCycleEveryMonth != null)
                        {
                            SendCycleEveryMonth _entity = Mapper.Map<SendCycleEveryMonthModel, SendCycleEveryMonth>(model.SendCycleEveryMonth);
                            _entity.SendMessageRuleId = entity.Id;
                            _entity = this.unitOfWork.Repository<SendCycleEveryMonth>().Insert(_entity);
                            entity.SendCycleEveryMonth = _entity;
                        }

                        if (model.SendCycleEveryYear != null)
                        {
                            SendCycleEveryYear _entity = Mapper.Map<SendCycleEveryYearModel, SendCycleEveryYear>(model.SendCycleEveryYear);
                            _entity.SendMessageRuleId = entity.Id;
                            _entity = this.unitOfWork.Repository<SendCycleEveryYear>().Insert(_entity);
                            entity.SendCycleEveryYear = _entity;
                        }
                    } break;
                default:
                    {
                        throw new Exception(string.Format("Unknown SendTimeType ({0})", model.SendTimeType));
                    };

            }
            #endregion

            int updated = this.repository.Update(entity); // (設定 收訊人來源) + (設定 簡訊發送時間)

            ////////////////////////////////////////
            // (5) 新增 簡訊收訊人 (MessageReceiver)
            ////////////////////////////////////////

            CreateMessageReceivers(user, entity);

            ////////////////////////////////////////
            // (6) 扣除目前使用者點數，並檢驗點數預警
            ////////////////////////////////////////

            this.tradeService.CreateSendMessageRule(entity, bCheckCredit: bCheckCredit);

            ////////////////////////////////////////
            // (7) 簡訊規則狀態為 Ready // 簡訊規則以及相關資料已經備妥，可以準備發送
            ////////////////////////////////////////

            entity.SendMessageRuleStatus = SendMessageRuleStatus.Ready;
            this.repository.Update(entity);

            if (entity.SendTimeType == SendTimeType.Immediately)
            {
                var utcNow = DateTime.UtcNow;

                BackgroundJob.Enqueue<CommonSmsService>(x => x.SendSMS(entity.Id, utcNow));
            }

            ////////////////////////////////////////
            // (8) 紀錄LOG
            ////////////////////////////////////////

            var sb = new StringBuilder();

            sb.AppendFormat("建立簡訊規則(");

            sb.AppendFormat("簡訊編號：{0}，", entity.Id);
            sb.AppendFormat("主旨：{0}，", entity.SendTitle);
            sb.AppendFormat("發送內容：{0}，", entity.SendBody);
            sb.AppendFormat("收訊人來源：{0}，", AttributeHelper.GetColumnDescription(entity.RecipientFromType));
            sb.AppendFormat("發送名單：{0}，", GetBriefInformation_MessageReceiver(entity)); // 發送名單：共10筆，[0921859698、0921859698、...]
            sb.AppendFormat("發送時間類型：{0}，", AttributeHelper.GetColumnDescription(entity.SendTimeType));
            sb.AppendFormat("發送時間：{0}，", GetBriefInformation_SendTime(entity)); // 發送時間：共10筆，[2015/10/10 12:00:00、2015/10/11 12:00:00、...]
            sb.AppendFormat("發送通數：{0}，", entity.TotalReceiverCount);
            sb.AppendFormat("花費點數：{0}，", entity.TotalMessageCost);
            sb.AppendFormat("剩餘點數：{0}，", entity.RemainingSmsBalance);
            sb.AppendFormat("ClientTimezoneOffset：{0}", entity.ClientTimezoneOffset);

            sb.AppendFormat(")");

            this.logService.Debug(sb.ToString());

            //List<string> detailInformations = new List<string>();
            //detailInformations.AddRange(GetDetailInformation_MessageReceiver(entity));
            //detailInformations.AddRange(GetDetailInformation_SendTime(entity));
            //foreach (var detailInformation in detailInformations)
            //{
            //    this.logService.Debug(detailInformation);
            //}

            return entity;
        }

        //private DateTime DebugGetSendTime(SendDeliver sendDeliver)
        //{
        //    /*
        //    DebugGetSendTime, utcTime: 10/29/2015 4:00:00 PM
        //    DebugGetSendTime, clientTimezoneOffset: 08:00:00
        //    DebugGetSendTime, localTime: (Unspecified) 2015/10/30 00:00:00
        //    DebugGetSendTime, sendDeliver.Date: (Local) 2015/10/30 00:00:00
        //     */
        //    this.logService.Debug("DebugGetSendTime, sendDeliver.Date: {0}", Converter.DebugString(sendDeliver.Date));

        //    DateTime localTime = new DateTime(sendDeliver.Date.Year, sendDeliver.Date.Month, sendDeliver.Date.Day, sendDeliver.Hour, sendDeliver.Minute, 0  /* 必須設為 0，否則 Hangfire 會發生一分鐘的誤差 */, DateTimeKind.Local);
        //    this.logService.Debug("DebugGetSendTime, localTime: {0}", Converter.DebugString(localTime));

        //    TimeSpan clientTimezoneOffset = sendDeliver.ClientTimezoneOffset;
        //    DateTime utcTime = Converter.ToUniversalTime(localTime, clientTimezoneOffset);

        //    this.logService.Debug("DebugGetSendTime, clientTimezoneOffset: {0}", clientTimezoneOffset);
        //    this.logService.Debug("DebugGetSendTime, utcTime: {0}", Converter.DebugString(utcTime));

        //    return utcTime;
        //}

        #region 避免產生過長訊息，造成輸入Excel發生問題

        #region GetBriefInformation

        private string GetBriefInformation_MessageReceiver(SendMessageRule entity)
        {
            var data = this.unitOfWork.Repository<MessageReceiver>()
                .GetMany(p => p.SendMessageRuleId == entity.Id)
                .Select(p => p.Mobile)
                .ToList();

            return ExcelBugFix.GetInformation(data);
        }

        private string GetBriefInformation_SendTime(SendMessageRule entity)
        {
            var data = entity.GetSendTimeList()
                .Select(p => Converter.ToLocalTime(p, entity.ClientTimezoneOffset).ToString(Converter.Every8d_SentTime))
                .ToList();

            return ExcelBugFix.GetInformation(data);
        }

        #endregion // GetBriefInformation

        #region GetDetailInformation

        //private List<string> GetDetailInformation_MessageReceiver(SendMessageRule entity)
        //{
        //    var data = this.unitOfWork.Repository<MessageReceiver>()
        //        .GetMany(p => p.SendMessageRuleId == entity.Id)
        //        .Select(p => p.Mobile)
        //        .ToList();

        //    List<string> outputs = ExcelBugFix.GetDetailInformation(data)
        //        .Select(p => string.Format("簡訊編號：{0}，發送名單(共{1}筆)：{2}", entity.Id, data.Count, p))
        //        .ToList();

        //    return outputs;
        //}

        //private List<string> GetDetailInformation_SendTime(SendMessageRule entity)
        //{
        //    var data = entity.GetSendTimeList()
        //        .Select(p => Converter.ToLocalTime(p, entity.ClientTimezoneOffset).ToString(Converter.Every8d_SentTime))
        //        .ToList();

        //    List<string> outputs = ExcelBugFix.GetDetailInformation(data)
        //        .Select(p => string.Format("簡訊編號：{0}，發送時間(共{1}筆)：{2}", entity.Id, data.Count, p))
        //        .ToList();

        //    return outputs;
        //}

        #endregion // GetDetailInformation

        #endregion // 避免產生過長訊息，造成輸入Excel發生問題

        private MessageCostInfo GetMessageCostInfo(SendMessageRule entity, UploadedMessageReceiver item)
        {
            MessageCostInfo messageCostInfo = entity.UseParam
                                ? new MessageCostInfo(entity.SendBody, item.Mobile, new Dictionary<string, string>{
                                    {"@space1@", item.Param1},
                                    {"@space2@", item.Param2},
                                    {"@space3@", item.Param3},
                                    {"@space4@", item.Param4},
                                    {"@space5@", item.Param5},
                                })
                                : new MessageCostInfo(entity.SendBody, item.Mobile);

            return messageCostInfo;
        }

        private void CreateMessageReceivers(ApplicationUser user, SendMessageRule entity)
        {
            DateTime utcNow = DateTime.UtcNow;

            switch (entity.RecipientFromType)
            {
                case RecipientFromType.FileUpload:
                    {
                        var uploadedMessageReceiverRepository = this.unitOfWork.Repository<UploadedMessageReceiver>();

                        //IQueryable<UploadedMessageReceiver> result = uploadedMessageReceiverRepository.GetMany(p => p.UploadedSessionId == entity.RecipientFromFileUpload.UploadedFileId); 
                        // 發生錯誤：已經開啟一個與這個 Command 相關的 DataReader，必須先將它關閉
                        // 解決方式：http://readily-notes.blogspot.tw/2014/01/aspnet-mvc-4-webapi-command-datareader.html
                        var result = uploadedMessageReceiverRepository.GetMany(p => p.IsValid == true && p.UploadedSessionId == entity.RecipientFromFileUpload.UploadedFileId).ToList();

                        var messageCostInfos = result.Select(item => GetMessageCostInfo(entity, item)).ToList();

                        var _entities = result.Select((item, i) => new MessageReceiver
                        {
                            SendMessageRuleId = entity.Id,
                            RowNo = item.RowNo,
                            Name = item.Name,
                            Mobile = item.Mobile,
                            E164Mobile = MobileUtil.GetE164PhoneNumber(item.Mobile, throwException: true),
                            Region = MobileUtil.GetRegionName(item.Mobile),
                            Email = item.Email,
                            SendTime = item.SendTime,

                            SendTitle = entity.SendTitle,
                            SendBody = messageCostInfos[i].SendBody,
                            SendMessageType = entity.SendMessageType,
                            RecipientFromType = entity.RecipientFromType,
                            CreatedUserId = user.Id,
                            CreatedTime = utcNow,
                            UpdatedTime = utcNow,

                            MessageLength = messageCostInfos[i].MessageLength,
                            MessageNum = messageCostInfos[i].MessageNum,
                            MessageCost = messageCostInfos[i].MessageCost,
                            MessageFormatError = messageCostInfos[i].MessageFormatError,
                        }).ToList();

                        DbContext context = this.unitOfWork.DbContext;
                        context.BulkInsert(_entities);
                        context.MySaveChanges();

                        if (entity.RecipientFromFileUpload.AddSelfToMessageReceiverList)
                        {
                            var _repository = this.unitOfWork.Repository<MessageReceiver>();

                            var messageCostInfo = new MessageCostInfo(entity.SendBody, user.PhoneNumber);

                            var _entity = new MessageReceiver();

                            _entity.SendMessageRuleId = entity.Id;
                            _entity.RowNo = result.Count() + 1;
                            _entity.Name = user.FullName;
                            _entity.Mobile = user.PhoneNumber;
                            _entity.E164Mobile = MobileUtil.GetE164PhoneNumber(user.PhoneNumber, throwException: true);
                            _entity.Region = MobileUtil.GetRegionName(user.PhoneNumber);
                            _entity.Email = user.Email;
                            _entity.SendTime = null;

                            _entity.SendTitle = entity.SendTitle;
                            _entity.SendBody = messageCostInfo.SendBody;
                            _entity.SendMessageType = entity.SendMessageType;
                            _entity.RecipientFromType = entity.RecipientFromType;
                            _entity.CreatedUserId = user.Id;
                            _entity.CreatedTime = utcNow;
                            _entity.UpdatedTime = utcNow;

                            _entity.MessageLength = messageCostInfo.MessageLength;
                            _entity.MessageNum = messageCostInfo.MessageNum;
                            _entity.MessageCost = messageCostInfo.MessageCost;
                            _entity.MessageFormatError = messageCostInfo.MessageFormatError;

                            _entity = _repository.Insert(_entity);
                        }

                    } break;

                case RecipientFromType.CommonContact:
                    {
                        var _repository = this.unitOfWork.Repository<MessageReceiver>();

                        var contactIds = entity.RecipientFromCommonContact.ContactIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p));
                        var contactRepository = this.unitOfWork.Repository<Contact>();
                        //IQueryable<Contact> result = contactRepository.GetMany(p => contactIds.Contains(p.Id));
                        // 發生錯誤：已經開啟一個與這個 Command 相關的 DataReader，必須先將它關閉
                        // 解決方式：http://readily-notes.blogspot.tw/2014/01/aspnet-mvc-4-webapi-command-datareader.html
                        var result = contactRepository.GetMany(p => contactIds.Contains(p.Id)).ToList();

                        var messageCostInfos = result.Select(item => new MessageCostInfo(entity.SendBody, item.Mobile)).ToList();

                        var _entities = result.Select((item, i) => new MessageReceiver
                        {
                            SendMessageRuleId = entity.Id,
                            RowNo = i + 1,
                            Name = item.Name,
                            Mobile = item.Mobile,
                            E164Mobile = MobileUtil.GetE164PhoneNumber(item.Mobile, throwException: true),
                            Region = MobileUtil.GetRegionName(item.Mobile),
                            Email = item.Email,
                            SendTime = null,

                            SendTitle = entity.SendTitle,
                            SendBody = messageCostInfos[i].SendBody,
                            SendMessageType = entity.SendMessageType,
                            RecipientFromType = entity.RecipientFromType,
                            CreatedUserId = user.Id,
                            CreatedTime = utcNow,
                            UpdatedTime = utcNow,

                            MessageLength = messageCostInfos[i].MessageLength,
                            MessageNum = messageCostInfos[i].MessageNum,
                            MessageCost = messageCostInfos[i].MessageCost,
                            MessageFormatError = messageCostInfos[i].MessageFormatError,
                        }).ToList();

                        DbContext context = this.unitOfWork.DbContext;
                        context.BulkInsert(_entities);
                        context.MySaveChanges();
                        
                    } break;

                case RecipientFromType.GroupContact:
                    {
                        var _repository = this.unitOfWork.Repository<MessageReceiver>();

                        var contactIds = entity.RecipientFromGroupContact.ContactIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p));
                        var contactRepository = this.unitOfWork.Repository<Contact>();
                        //IQueryable<Contact> result = contactRepository.GetMany(p => contactIds.Contains(p.Id));
                        // 發生錯誤：已經開啟一個與這個 Command 相關的 DataReader，必須先將它關閉
                        // 解決方式：http://readily-notes.blogspot.tw/2014/01/aspnet-mvc-4-webapi-command-datareader.html
                        var result = contactRepository.GetMany(p => contactIds.Contains(p.Id)).ToList();

                        var messageCostInfos = result.Select(item => new MessageCostInfo(entity.SendBody, item.Mobile)).ToList();

                        var _entities = result.Select((item, i) => new MessageReceiver
                        {
                            SendMessageRuleId = entity.Id,
                            RowNo = i + 1,
                            Name = item.Name,
                            Mobile = item.Mobile,
                            E164Mobile = MobileUtil.GetE164PhoneNumber(item.Mobile, throwException: true),
                            Region = MobileUtil.GetRegionName(item.Mobile),
                            Email = item.Email,
                            SendTime = null,

                            SendTitle = entity.SendTitle,
                            SendBody = messageCostInfos[i].SendBody,
                            SendMessageType = entity.SendMessageType,
                            RecipientFromType = entity.RecipientFromType,
                            CreatedUserId = user.Id,
                            CreatedTime = utcNow,
                            UpdatedTime = utcNow,

                            MessageLength = messageCostInfos[i].MessageLength,
                            MessageNum = messageCostInfos[i].MessageNum,
                            MessageCost = messageCostInfos[i].MessageCost,
                            MessageFormatError = messageCostInfos[i].MessageFormatError,
                        }).ToList();

                        DbContext context = this.unitOfWork.DbContext;
                        context.BulkInsert(_entities);
                        context.MySaveChanges();
                    } break;

                case RecipientFromType.ManualInput:
                    {
                        var _repository = this.unitOfWork.Repository<MessageReceiver>();

                        var phoneNumbers = entity.RecipientFromManualInput.PhoneNumbers.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        var messageCostInfos = phoneNumbers.Select(phoneNumber => new MessageCostInfo(entity.SendBody, phoneNumber)).ToList();

                        var _entities = phoneNumbers.Select((phoneNumber, i) => new MessageReceiver
                        {
                            SendMessageRuleId = entity.Id,
                            RowNo = i + 1,
                            Name = string.Empty,
                            Mobile = phoneNumber,
                            E164Mobile = MobileUtil.GetE164PhoneNumber(phoneNumber, throwException: true),
                            Region = MobileUtil.GetRegionName(phoneNumber),
                            Email = string.Empty,
                            SendTime = null,

                            SendTitle = entity.SendTitle,
                            SendBody = messageCostInfos[i].SendBody,
                            SendMessageType = entity.SendMessageType,
                            RecipientFromType = entity.RecipientFromType,
                            CreatedUserId = user.Id,
                            CreatedTime = utcNow,
                            UpdatedTime = utcNow,

                            MessageLength = messageCostInfos[i].MessageLength,
                            MessageNum = messageCostInfos[i].MessageNum,
                            MessageCost = messageCostInfos[i].MessageCost,
                            MessageFormatError = messageCostInfos[i].MessageFormatError,
                        }).ToList();

                        DbContext context = this.unitOfWork.DbContext;
                        context.BulkInsert(_entities);
                        context.MySaveChanges();

                    } break;
                default:
                    {
                        throw new Exception(string.Format("Unknown RecipientFromType ({0})", entity.RecipientFromType));
                    };
            }
        }

        private List<string> GetInternationalMobiles(ApplicationUser user, SendMessageRuleModel model)
        {
            var internationalMobiles = new List<string>();

            switch (model.RecipientFromType)
            {
                case RecipientFromType.FileUpload:
                    {
                        var uploadedMessageReceiverRepository = this.unitOfWork.Repository<UploadedMessageReceiver>();

                        //IQueryable<UploadedMessageReceiver> result = uploadedMessageReceiverRepository.GetMany(p => p.UploadedSessionId == entity.RecipientFromFileUpload.UploadedFileId); 
                        // 發生錯誤：已經開啟一個與這個 Command 相關的 DataReader，必須先將它關閉
                        // 解決方式：http://readily-notes.blogspot.tw/2014/01/aspnet-mvc-4-webapi-command-datareader.html
                        var result = uploadedMessageReceiverRepository.GetMany(p => p.IsValid == true && p.UploadedSessionId == model.RecipientFromFileUpload.UploadedFileId).ToList();

                        foreach (var item in result)
                        {
                            if (MobileUtil.IsInternationPhoneNumber(item.Mobile)) internationalMobiles.Add(item.Mobile);
                        }

                    } break;

                case RecipientFromType.CommonContact:
                    {
                        var contactIds = model.RecipientFromCommonContact.ContactIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p));
                        var contactRepository = this.unitOfWork.Repository<Contact>();
                        //IQueryable<Contact> result = contactRepository.GetMany(p => contactIds.Contains(p.Id));
                        // 發生錯誤：已經開啟一個與這個 Command 相關的 DataReader，必須先將它關閉
                        // 解決方式：http://readily-notes.blogspot.tw/2014/01/aspnet-mvc-4-webapi-command-datareader.html
                        var result = contactRepository.GetMany(p => contactIds.Contains(p.Id)).ToList();

                        foreach (var item in result)
                        {
                            if (MobileUtil.IsInternationPhoneNumber(item.Mobile)) internationalMobiles.Add(item.Mobile);
                        }

                    } break;

                case RecipientFromType.GroupContact:
                    {
                        var contactIds = model.RecipientFromGroupContact.ContactIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p));
                        var contactRepository = this.unitOfWork.Repository<Contact>();
                        //IQueryable<Contact> result = contactRepository.GetMany(p => contactIds.Contains(p.Id));
                        // 發生錯誤：已經開啟一個與這個 Command 相關的 DataReader，必須先將它關閉
                        // 解決方式：http://readily-notes.blogspot.tw/2014/01/aspnet-mvc-4-webapi-command-datareader.html
                        var result = contactRepository.GetMany(p => contactIds.Contains(p.Id)).ToList();

                        foreach (var item in result)
                        {
                            if (MobileUtil.IsInternationPhoneNumber(item.Mobile)) internationalMobiles.Add(item.Mobile);
                        }

                    } break;

                case RecipientFromType.ManualInput:
                    {
                        var phoneNumbers = model.RecipientFromManualInput.PhoneNumbers.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var phoneNumber in phoneNumbers)
                        {
                            if (MobileUtil.IsInternationPhoneNumber(phoneNumber)) internationalMobiles.Add(phoneNumber);
                        }

                    } break;
                default:
                    {
                        throw new Exception(string.Format("Unknown RecipientFromType ({0})", model.RecipientFromType));
                    };
            }

            return internationalMobiles;
        }

        /// <summary>
        /// 在不考慮發送時間類型的狀況下，計算執行一次簡訊規則，發送到相關收訊人所需的花費
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        private object GetTotalMessageCostInfo(ApplicationUser user, SendMessageRuleModel model)
        {
            // 發送通數 = COUNT(所有名單)
            int TotalReceiverCount = 0;
            // 花費點數 = SUM(每則簡訊所需點數)
            decimal TotalMessageCost = 0;
            // 剩餘點數(新增簡訊規則當下剩餘點數) 是否為週期簡訊 ? 使用者目前可用點數 : 使用者目前可用點數 - 花費點數
            decimal RemainingSmsBalance = 0;

            switch (model.RecipientFromType)
            {
                case RecipientFromType.FileUpload:
                    {
                        var uploadedMessageReceiverRepository = this.unitOfWork.Repository<UploadedMessageReceiver>();

                        //IQueryable<UploadedMessageReceiver> result = uploadedMessageReceiverRepository.GetMany(p => p.UploadedSessionId == entity.RecipientFromFileUpload.UploadedFileId); 
                        // 發生錯誤：已經開啟一個與這個 Command 相關的 DataReader，必須先將它關閉
                        // 解決方式：http://readily-notes.blogspot.tw/2014/01/aspnet-mvc-4-webapi-command-datareader.html
                        var result = uploadedMessageReceiverRepository.GetMany(p => p.IsValid == true && p.UploadedSessionId == model.RecipientFromFileUpload.UploadedFileId).ToList();

                        foreach (var item in result)
                        {
                            MessageCostInfo messageCostInfo = model.UseParam
                                ? new MessageCostInfo(model.SendBody, item.Mobile, new Dictionary<string, string>{
                                    {"@space1@", item.Param1},
                                    {"@space2@", item.Param2},
                                    {"@space3@", item.Param3},
                                    {"@space4@", item.Param4},
                                    {"@space5@", item.Param5},
                                })
                            : new MessageCostInfo(model.SendBody, item.Mobile);

                            TotalMessageCost += messageCostInfo.MessageCost;
                            TotalReceiverCount += 1;
                        }

                        if (model.RecipientFromFileUpload.AddSelfToMessageReceiverList)
                        {
                            var messageCostInfo = new MessageCostInfo(model.SendBody, user.PhoneNumber);

                            TotalMessageCost += messageCostInfo.MessageCost;
                            TotalReceiverCount += 1;
                        }

                        RemainingSmsBalance = this.tradeService.ShouldWithhold(model.SendTimeType) ? user.SmsBalance - TotalMessageCost : user.SmsBalance;

                    } break;

                case RecipientFromType.CommonContact:
                    {
                        var contactIds = model.RecipientFromCommonContact.ContactIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p));
                        var contactRepository = this.unitOfWork.Repository<Contact>();
                        //IQueryable<Contact> result = contactRepository.GetMany(p => contactIds.Contains(p.Id));
                        // 發生錯誤：已經開啟一個與這個 Command 相關的 DataReader，必須先將它關閉
                        // 解決方式：http://readily-notes.blogspot.tw/2014/01/aspnet-mvc-4-webapi-command-datareader.html
                        var result = contactRepository.GetMany(p => contactIds.Contains(p.Id)).ToList();

                        foreach (var item in result)
                        {
                            var messageCostInfo = new MessageCostInfo(model.SendBody, item.Mobile);

                            TotalMessageCost += messageCostInfo.MessageCost;
                            TotalReceiverCount += 1;
                        }

                        RemainingSmsBalance = this.tradeService.ShouldWithhold(model.SendTimeType) ? user.SmsBalance - TotalMessageCost : user.SmsBalance;

                    } break;

                case RecipientFromType.GroupContact:
                    {
                        var contactIds = model.RecipientFromGroupContact.ContactIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt32(p));
                        var contactRepository = this.unitOfWork.Repository<Contact>();
                        //IQueryable<Contact> result = contactRepository.GetMany(p => contactIds.Contains(p.Id));
                        // 發生錯誤：已經開啟一個與這個 Command 相關的 DataReader，必須先將它關閉
                        // 解決方式：http://readily-notes.blogspot.tw/2014/01/aspnet-mvc-4-webapi-command-datareader.html
                        var result = contactRepository.GetMany(p => contactIds.Contains(p.Id)).ToList();

                        foreach (var item in result)
                        {
                            var messageCostInfo = new MessageCostInfo(model.SendBody, item.Mobile);

                            TotalMessageCost += messageCostInfo.MessageCost;
                            TotalReceiverCount += 1;
                        }

                        RemainingSmsBalance = this.tradeService.ShouldWithhold(model.SendTimeType) ? user.SmsBalance - TotalMessageCost : user.SmsBalance;

                    } break;

                case RecipientFromType.ManualInput:
                    {
                        var phoneNumbers = model.RecipientFromManualInput.PhoneNumbers.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var phoneNumber in phoneNumbers)
                        {
                            var messageCostInfo = new MessageCostInfo(model.SendBody, phoneNumber);

                            TotalMessageCost += messageCostInfo.MessageCost;
                            TotalReceiverCount += 1;
                        }

                        RemainingSmsBalance = this.tradeService.ShouldWithhold(model.SendTimeType) ? user.SmsBalance - TotalMessageCost : user.SmsBalance;

                    } break;
                default:
                    {
                        throw new Exception(string.Format("Unknown RecipientFromType ({0})", model.RecipientFromType));
                    };
            }

            return new
            {
                TotalReceiverCount = TotalReceiverCount,
                TotalMessageCost = TotalMessageCost,
                RemainingSmsBalance = RemainingSmsBalance,
            };
        }

        #endregion

        #region 更新簡訊規則

        public void UpdateSendMessageRule(ApplicationUser user, SendMessageRuleModel model)
        {
            decimal beforeTotalMessageCost = 0.0M;
            decimal beforeRemainingSmsBalance = 0.0M;
            decimal afterTotalMessageCost = 0.0M;
            decimal afterRemainingSmsBalance = 0.0M;

            //--------------------
            // 只能異動
            //--------------------
            // SendTitle
            // SendBody
            //--------------------
            // 不能異動
            //--------------------
            // RecipientFromType
            //  RecipientFromFileUpload
            //  RecipientFromCommonContact
            //  RecipientFromGroupContact
            //  RecipientFromManualInput
            //
            // SendTimeType
            //  SendDeliver
            //  SendCycleEveryDay
            //  SendCycleEveryWeek
            //  SendCycleEveryMonth
            //  SendCycleEveryYear
            //
            // SendCustType
            // UseParam
            // SendMessageType

            // (1) 只能更新自己所建立的簡訊規則
            // (2) 檢查目前簡訊狀態為 Ready 才可以執行更新動作
            // (3) 設定目前簡訊狀態為更新中
            // (4) 重新計算發送所需點數
            // (5) 更新簡訊規則
            // (6) 更新簡訊收訊人 (MessageReceiver)
            // (7) 簡訊規則狀態為 Ready // 簡訊規則以及相關資料已經備妥，可以準備發送

            ////////////////////////////////////////
            // (1) 只能更新自己所建立的簡訊規則
            ////////////////////////////////////////

            if (!this.repository.Any(p => p.CreatedUserId == user.Id && p.Id == model.Id))
                throw new Exception(string.Format("使用者{0}，沒有指定簡訊規則({1})", user.UserName, model.Id));

            SendMessageRule entity = this.repository.GetById(model.Id); // 函式傳遞過來的 entity 是透過 AutoMapper 轉換的，我要的是來自資料庫的資料
            beforeTotalMessageCost = entity.TotalMessageCost;
            beforeRemainingSmsBalance = entity.RemainingSmsBalance;

            ////////////////////////////////////////
            // (2) 檢查目前簡訊狀態為 Ready 才可以執行更新動作
            ////////////////////////////////////////

            if (entity.SendMessageRuleStatus != SendMessageRuleStatus.Ready)
            {
                var description = AttributeHelper.GetColumnDescription(entity.SendMessageRuleStatus);
                throw new Exception(description);
            }

            ////////////////////////////////////////
            // (3) 設定目前簡訊狀態為更新中
            ////////////////////////////////////////

            entity.SendMessageRuleStatus = SendMessageRuleStatus.Updating;
            this.repository.Update(entity);

            ////////////////////////////////////////
            // (4) 重新計算發送所需點數
            ////////////////////////////////////////

            var totalMessageCostInfo = GrottyHacks.Cast(GetTotalMessageCostInfo(user, model),
                new
                {
                    TotalReceiverCount = (int)0,
                    TotalMessageCost = (decimal)0.0,
                    RemainingSmsBalance = (decimal)0.0,
                }
            );

            ////////////////////////////////////////
            // (5) 更新簡訊規則
            ////////////////////////////////////////

            entity.SendTitle = model.SendTitle;
            entity.SendBody = model.SendBody;
            entity.TotalReceiverCount = totalMessageCostInfo.TotalReceiverCount;
            entity.TotalMessageCost = this.tradeService.ShouldWithhold(model.SendTimeType) ? totalMessageCostInfo.TotalMessageCost : 0;
            entity.RemainingSmsBalance = this.tradeService.ShouldWithhold(model.SendTimeType) ? totalMessageCostInfo.RemainingSmsBalance : user.SmsBalance;

            afterTotalMessageCost = entity.TotalMessageCost;
            afterRemainingSmsBalance = entity.RemainingSmsBalance;

            ////////////////////////////////////////
            // (6) 更新簡訊收訊人 (MessageReceiver)
            ////////////////////////////////////////

            UpdateMessageReceivers(model);

            ////////////////////////////////////////
            // (6) 扣除目前使用者點數
            ////////////////////////////////////////

            this.tradeService.UpdateSendMessageRule(entity, beforeTotalMessageCost, afterTotalMessageCost);

            ////////////////////////////////////////
            // (7) 簡訊規則狀態為 Ready // 簡訊規則以及相關資料已經備妥，可以準備發送
            ////////////////////////////////////////

            entity.SendMessageRuleStatus = SendMessageRuleStatus.Ready;
            this.repository.Update(entity);
        }

        private void UpdateMessageReceivers(SendMessageRuleModel model)
        {
            DateTime utcNow = DateTime.UtcNow;

            var _repository = this.unitOfWork.Repository<MessageReceiver>();

            var _entities = _repository.GetMany(p => p.SendMessageRuleId == model.Id).ToList();

            foreach (var _entity in _entities)
            {
                var messageCostInfo = new MessageCostInfo(model.SendBody, _entity.Mobile);

                _entity.SendTitle = model.SendTitle;
                _entity.UpdatedTime = utcNow;

                _entity.SendBody = messageCostInfo.SendBody;
                _entity.MessageLength = messageCostInfo.MessageLength;
                _entity.MessageNum = messageCostInfo.MessageNum;
                _entity.MessageCost = messageCostInfo.MessageCost;
                _entity.MessageFormatError = messageCostInfo.MessageFormatError;

                _repository.Update(_entity);
            }
        }

        #endregion

        #region 刪除簡訊規則

        public void RemoveSendMessageRule(ApplicationUser user, int id)
        {
            // (1) 只能刪除自己所建立的簡訊規則
            // (2) 檢查目前簡訊狀態為 Ready 才可以執行刪除動作
            // (3) 設定目前簡訊狀態為刪除中
            // (4) 刪除 收訊人來源 (RecipientFromFileUpload, RecipientFromCommonContact, RecipientFromGroupContact, RecipientFromManualInput)
            // (5) 刪除 簡訊發送時間 (SendDeliver, SendCycleEveryDay, SendCycleEveryWeek, SendCycleEveryMonth, SendCycleEveryYear)
            // (6) 回補目前使用者點數 (回補點數會回填簡訊收訊人資料到 TradeDetail，因此必須在刪除簡訊收訊人之前進行回補點數作業)
            // (7) 刪除 簡訊收訊人 (MessageReceiver)
            // (8) 相關資料已經刪除完畢，可以刪除簡訊規則了

            ////////////////////////////////////////
            // (1) 只能刪除自己所建立的簡訊規則
            ////////////////////////////////////////

            if (!this.repository.Any(p => p.CreatedUserId == user.Id && p.Id == id))
                throw new Exception(string.Format("使用者{0}，沒有指定簡訊規則({1})", user.UserName, id));

            SendMessageRule entity = this.repository.GetById(id);

            ////////////////////////////////////////
            // (2) 檢查目前簡訊狀態為 Ready 才可以執行刪除動作
            ////////////////////////////////////////

            if (entity.SendMessageRuleStatus != SendMessageRuleStatus.Ready)
            {
                var description = AttributeHelper.GetColumnDescription(entity.SendMessageRuleStatus);
                throw new Exception(description);
            }

            ////////////////////////////////////////
            // (3) 設定目前簡訊狀態為刪除中
            ////////////////////////////////////////

            entity.SendMessageRuleStatus = SendMessageRuleStatus.Deleting;
            this.repository.Update(entity);

            ////////////////////////////////////////
            // (4) 刪除 收訊人來源 (RecipientFromFileUpload, RecipientFromCommonContact, RecipientFromGroupContact, RecipientFromManualInput)
            ////////////////////////////////////////


            #region 刪除 收訊人來源

            switch (entity.RecipientFromType)
            {
                case RecipientFromType.FileUpload:
                    {
                        var UploadedFileIds = this.unitOfWork.Repository<RecipientFromFileUpload>().GetMany(p => p.SendMessageRuleId == entity.Id).Select(p => p.UploadedFileId).ToList();

                        this.unitOfWork.Repository<RecipientFromFileUpload>().Delete(p => p.SendMessageRuleId == entity.Id);
                        this.unitOfWork.Repository<UploadedMessageReceiver>().Delete(p => UploadedFileIds.Contains(p.UploadedSessionId));
                        this.unitOfWork.Repository<UploadedFile>().Delete(p => UploadedFileIds.Contains(p.Id));
                    } break;
                case RecipientFromType.CommonContact:
                    {
                        this.unitOfWork.Repository<RecipientFromCommonContact>().Delete(p => p.SendMessageRuleId == entity.Id);
                    } break;
                case RecipientFromType.GroupContact:
                    {
                        this.unitOfWork.Repository<RecipientFromGroupContact>().Delete(p => p.SendMessageRuleId == entity.Id);
                    } break;
                case RecipientFromType.ManualInput:
                    {
                        this.unitOfWork.Repository<RecipientFromManualInput>().Delete(p => p.SendMessageRuleId == entity.Id);
                    } break;
                default:
                    {
                        throw new Exception(string.Format("Unknown RecipientFromType ({0})", entity.RecipientFromType));
                    };
            }

            #endregion

            ////////////////////////////////////////
            // (5) 刪除 簡訊發送時間 (SendDeliver, SendCycleEveryDay, SendCycleEveryWeek, SendCycleEveryMonth, SendCycleEveryYear)
            ////////////////////////////////////////


            #region 刪除 簡訊發送時間

            this.unitOfWork.Repository<SendDeliver>().Delete(p => p.SendMessageRuleId == entity.Id);
            this.unitOfWork.Repository<SendCycleEveryDay>().Delete(p => p.SendMessageRuleId == entity.Id);
            this.unitOfWork.Repository<SendCycleEveryWeek>().Delete(p => p.SendMessageRuleId == entity.Id);
            this.unitOfWork.Repository<SendCycleEveryMonth>().Delete(p => p.SendMessageRuleId == entity.Id);
            this.unitOfWork.Repository<SendCycleEveryYear>().Delete(p => p.SendMessageRuleId == entity.Id);

            #endregion

            ////////////////////////////////////////
            // (6) 回補目前使用者點數 (回補點數會回填簡訊收訊人資料到 TradeDetail，因此必須在刪除簡訊收訊人之前進行回補點數作業)
            ////////////////////////////////////////

            this.tradeService.DeleteSendMessageRule(entity);

            ////////////////////////////////////////
            // (7) 刪除 簡訊收訊人 (MessageReceiver)
            ////////////////////////////////////////

            this.unitOfWork.Repository<MessageReceiver>().Delete(p => p.SendMessageRuleId == entity.Id);

            

            ////////////////////////////////////////
            // (8) 相關資料已經刪除完畢，可以刪除簡訊規則了
            ////////////////////////////////////////

            this.repository.Delete(entity);
        }

        #endregion
    }
}