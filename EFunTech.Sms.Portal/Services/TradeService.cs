using EFunTech.Sms.Schema;
using JUtilSharp.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Practices.ServiceLocation;
using Hangfire;
using System.Linq;

namespace EFunTech.Sms.Portal
{
    public class TradeService
    {
        private IUnitOfWork unitOfWork;
        private ILogService logService;

        public TradeService(IUnitOfWork unitOfWork, ILogService logService)
        {
            this.unitOfWork = unitOfWork;
            this.logService = logService;
        }

        #region 判斷是否需要預先扣除

        public bool ShouldWithhold(SendTimeType SendTimeType)
        {
            return SendTimeType == SendTimeType.Immediately ||
                   SendTimeType == SendTimeType.Deliver;
        }

        #endregion

        #region 發送扣點

        /// <summary>
        /// 扣除目前使用者點數，並檢驗點數預警
        /// </summary>
        /// <param name="bCheckCredit">是否執行點數預警</param>
        public void CreateSendMessageRule(SendMessageRule sendMessageRule, bool bCheckCredit)
        {
            DateTime utcNow = DateTime.UtcNow;

            if (ShouldWithhold(sendMessageRule.SendTimeType))
            {
                // 扣點
                decimal point = sendMessageRule.TotalMessageCost;
                var userRepository = this.unitOfWork.Repository<ApplicationUser>();
                var user = sendMessageRule.CreatedUser;
                user.SmsBalance -= point;
                userRepository.Update(user);

                // 寫入交易明細
                var tradeDetailRepository = this.unitOfWork.Repository<TradeDetail>();
                TradeDetail tradeDetail = new TradeDetail
                {
                    TradeTime = utcNow,
                    TradeType = TradeType.DeductionOfSendMessage,
                    Point = -1 * point,
                    OwnerId = user.Id,
                    TargetId = sendMessageRule.Id.ToString(),
                    Remark = string.Format("{0}(簡訊編號：{1})，建立完成(共{2}筆收訊人)，扣除{3}點",
                        AttributeHelper.GetColumnDescription(sendMessageRule.SendTimeType),
                        sendMessageRule.Id,
                        this.unitOfWork.Repository<MessageReceiver>().Count(p => p.SendMessageRuleId == sendMessageRule.Id),
                        point)
                };
                tradeDetailRepository.Insert(tradeDetail);

                // 自動補點與點數預警
                if (bCheckCredit) CheckCredit(user);
            }
        }

        public void CreateSendMessageQueue(SendMessageRule sendMessageRule, SendMessageQueue sendMessageQueue)
        {
            DateTime utcNow = DateTime.UtcNow;

            // 只針對非預先扣除的簡訊類型，對於打算要發送的內容進行扣點
            if (!ShouldWithhold(sendMessageRule.SendTimeType))
            {
                // 扣點
                decimal point = sendMessageQueue.TotalMessageCost;
                var userRepository = this.unitOfWork.Repository<ApplicationUser>();
                var user = sendMessageRule.CreatedUser;
                user.SmsBalance -= point;
                userRepository.Update(user);

                // 寫入交易明細
                var tradeDetailRepository = this.unitOfWork.Repository<TradeDetail>();
                TradeDetail tradeDetail = new TradeDetail
                {
                    TradeTime = utcNow,
                    TradeType = TradeType.DeductionOfSendMessage,
                    Point = -1 * point,
                    OwnerId = user.Id,
                    TargetId = sendMessageRule.Id.ToString(),
                    Remark = string.Format("{0}(簡訊編號：{1}，序列編號:{2})，建立完成(共{3}筆收訊人)，扣除{4}點",
                        AttributeHelper.GetColumnDescription(sendMessageRule.SendTimeType),
                        sendMessageRule.Id,
                        sendMessageQueue.Id,
                        this.unitOfWork.Repository<MessageReceiver>().Count(p => p.SendMessageRuleId == sendMessageRule.Id),
                        point)
                };
                tradeDetailRepository.Insert(tradeDetail);

                // 自動補點與點數預警
                CheckCredit(user);
            }
        }

        #endregion

        #region 追加差額 | 回補差額

        public void UpdateSendMessageRule(SendMessageRule sendMessageRule, decimal beforeTotalMessageCost, decimal afterTotalMessageCost)
        {
            DateTime utcNow = DateTime.UtcNow;

            // 只針對預先扣除的簡訊類型，對於打算要發送的內容進行扣點
            if (ShouldWithhold(sendMessageRule.SendTimeType))
            {
                // 扣點或補點
                decimal point = afterTotalMessageCost - beforeTotalMessageCost;
                if (point == 0) return;
                var userRepository = this.unitOfWork.Repository<ApplicationUser>();
                var user = sendMessageRule.CreatedUser;
                user.SmsBalance -= point;
                userRepository.Update(user);

                // 寫入交易明細
                TradeType tradeType = (point > 0) ? TradeType.DeductionOfSendMessage : TradeType.CoverOfSendMessage;
                string remark = (point > 0)
                    ? string.Format("{0}(簡訊編號：{1})，更新發送內容，追加差額點數{2}點",
                        AttributeHelper.GetColumnDescription(sendMessageRule.SendTimeType),
                        sendMessageRule.Id,
                        Math.Abs(point))
                    : string.Format("{0}(簡訊編號：{1})，更新發送內容，回補差額點數{2}點",
                        AttributeHelper.GetColumnDescription(sendMessageRule.SendTimeType),
                        sendMessageRule.Id,
                        Math.Abs(point));
                var tradeDetailRepository = this.unitOfWork.Repository<TradeDetail>();
                TradeDetail tradeDetail = new TradeDetail
                {
                    TradeTime = utcNow,
                    TradeType = tradeType,
                    Point = -1 * point,
                    OwnerId = user.Id,
                    TargetId = sendMessageRule.Id.ToString(),
                    Remark = remark
                };
                tradeDetailRepository.Insert(tradeDetail);

                // 自動補點與點數預警
                CheckCredit(user);
            }
        }

        #endregion

        #region 發送回補

        public void DeleteSendMessageRule(SendMessageRule sendMessageRule)
        {
            DateTime utcNow = DateTime.UtcNow;

            if (ShouldWithhold(sendMessageRule.SendTimeType))
            {
                // 補點
                decimal point = sendMessageRule.TotalMessageCost;
                var userRepository = this.unitOfWork.Repository<ApplicationUser>();
                var user = sendMessageRule.CreatedUser;
                user.SmsBalance += point;
                userRepository.Update(user);

                // 寫入交易明細
                var tradeDetailRepository = this.unitOfWork.Repository<TradeDetail>();
                TradeDetail tradeDetail = new TradeDetail
                {
                    TradeTime = utcNow,
                    TradeType = TradeType.CoverOfSendMessage,
                    Point = point,
                    OwnerId = user.Id,
                    TargetId = sendMessageRule.Id.ToString(),
                    Remark = string.Format("{0}(簡訊編號：{1})，取消發送(共{2}筆收訊人)，回補點數{3}點",
                        AttributeHelper.GetColumnDescription(sendMessageRule.SendTimeType),
                        sendMessageRule.Id,
                        this.unitOfWork.Repository<MessageReceiver>().Count(p => p.SendMessageRuleId == sendMessageRule.Id),
                        point)
                };
                tradeDetailRepository.Insert(tradeDetail);

                // 自動補點與點數預警
                CheckCredit(user);
            }
        }

        /// <summary>
        /// 針對預先扣點的發送時間類型，進行黑名單的收訊者 - 退還點數
        /// </summary>
        /// <param name="sendMessageRule"></param>
        public void HandleReceiversInBlackList(SendMessageRule sendMessageRule, IEnumerable<MessageReceiver> receiversInBlackList)
        {
            if (!receiversInBlackList.Any()) return;

            DateTime utcNow = DateTime.UtcNow;

            if (ShouldWithhold(sendMessageRule.SendTimeType))
            {
                // 補點
                decimal point = receiversInBlackList.Sum(p => p.MessageCost);
                var userRepository = this.unitOfWork.Repository<ApplicationUser>();
                var user = sendMessageRule.CreatedUser;
                user.SmsBalance += point;
                userRepository.Update(user);

                // 寫入交易明細
                var tradeDetailRepository = this.unitOfWork.Repository<TradeDetail>();
                TradeDetail tradeDetail = new TradeDetail
                {
                    TradeTime = utcNow,
                    TradeType = TradeType.CoverOfSendMessage,
                    Point = point,
                    OwnerId = user.Id,
                    TargetId = sendMessageRule.Id.ToString(),
                    Remark = string.Format("{0}(簡訊編號：{1})，退還黑名單點數(共{2}筆收訊人屬於黑名單)，回補{3}點",
                        AttributeHelper.GetColumnDescription(sendMessageRule.SendTimeType),
                        sendMessageRule.Id,
                        receiversInBlackList.Count(),
                        point)
                };
                tradeDetailRepository.Insert(tradeDetail);

                // 自動補點與點數預警
                CheckCredit(user);
            }
        }

        /// <summary>
        /// 處理取回派回結果後，根據派送成功或者失敗，決定是否回補點數。
        /// </summary>
        private void HandleSendMessageHistory(
            SendMessageRule sendMessageRule, 
            SendMessageQueue sendMessageQueue, 
            SendMessageHistory sendMessageHistory)
        {
            if(sendMessageHistory.Delivered)return;
            if (sendMessageHistory.Price != 0) return; // 如果簡訊商就算傳送失敗，但有收取簡訊費用，就不回補點數
            if (sendMessageHistory.ProviderName == SmsProviderType.Every8d.ToString()) return; // Every8d 會重送，不需要回補點數
            
            DateTime utcNow = DateTime.UtcNow;

            // 補點
            decimal point = sendMessageHistory.MessageCost;
            var userRepository = this.unitOfWork.Repository<ApplicationUser>();
            var user = sendMessageRule.CreatedUser;
            user.SmsBalance += point;
            userRepository.Update(user);

            // 寫入交易明細
            var tradeDetailRepository = this.unitOfWork.Repository<TradeDetail>();
            TradeDetail tradeDetail = new TradeDetail
            {
                TradeTime = utcNow,
                TradeType = TradeType.CoverOfSendMessage,
                Point = point,
                OwnerId = user.Id,
                TargetId = sendMessageHistory.Id.ToString(),
                Remark = string.Format("{0}(簡訊編號：{1}，序列編號:{2})，收訊門號{3}發送失敗({4})，回補點數{5}點",
                    AttributeHelper.GetColumnDescription(sendMessageRule.SendTimeType),
                    sendMessageRule.Id,
                    sendMessageQueue.Id,
                    sendMessageHistory.DestinationAddress,
                    //AttributeHelper.GetColumnDescription(sendMessageHistory.DeliveryStatus),
                    sendMessageHistory.DeliveryStatusString,
                    point)
            };
            tradeDetailRepository.Insert(tradeDetail);

            // 自動補點與點數預警 // 回補點數，不用自動補點與點數預警
            //CheckCredit(user);
        }

        /// <summary>
        /// 刪除收訊人回補點數
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="messageReceiver">The message receiver.</param>
        public void DeleteMessageReceiver(SendMessageRule sendMessageRule, MessageReceiver messageReceiver)
        {
            DateTime utcNow = DateTime.UtcNow;

            if (ShouldWithhold(sendMessageRule.SendTimeType))
            {
                decimal point = messageReceiver.MessageCost;

                var userRepository = this.unitOfWork.Repository<ApplicationUser>();
                var user = sendMessageRule.CreatedUser;
                user.SmsBalance += point;
                userRepository.Update(user);

                var sendMessageRuleRepository = this.unitOfWork.Repository<SendMessageRule>();
                sendMessageRule.TotalReceiverCount -= 1;
                sendMessageRule.TotalMessageCost -= point;
                sendMessageRule.RemainingSmsBalance += point;
                sendMessageRuleRepository.Update(sendMessageRule);

                // 寫入交易明細
                var tradeDetailRepository = this.unitOfWork.Repository<TradeDetail>();
                TradeDetail tradeDetail = new TradeDetail
                {
                    TradeTime = utcNow,
                    TradeType = TradeType.CoverOfSendMessage,
                    Point = point,
                    OwnerId = user.Id,
                    TargetId = messageReceiver.Id.ToString(),
                    Remark = string.Format("{0}(簡訊編號：{1})，刪除收訊人{2}，回補{3}點",
                        AttributeHelper.GetColumnDescription(sendMessageRule.SendTimeType),
                        sendMessageRule.Id,
                        messageReceiver.Mobile,
                        messageReceiver.MessageCost)
                };
                tradeDetailRepository.Insert(tradeDetail);

                // 自動補點與點數預警
                CheckCredit(user);
            }
        }

        #endregion

        #region 刪除子帳號 回補點數

        public void DeleteUser(ApplicationUser user)
        {
            DateTime utcNow = DateTime.UtcNow;

            ApplicationUser parentUser = user.Parent;
            if (parentUser == null)return;

            // 回補點數
            decimal point = user.SmsBalance;
            if (point == 0) return;

            var userRepository = this.unitOfWork.Repository<ApplicationUser>();
            parentUser.SmsBalance += point;
            userRepository.Update(parentUser);

            // 寫入交易明細
            var tradeDetailRepository = this.unitOfWork.Repository<TradeDetail>();
            TradeDetail tradeDetail = new TradeDetail
            {
                TradeTime = utcNow,
                TradeType = TradeType.Cover,
                Point = point,
                OwnerId = parentUser.Id,
                TargetId = user.Id,
                Remark = string.Format("刪除子帳號{0}，將該帳號所有點數(共{1}點)，回補給您",
                    user.UserName,
                    user.SmsBalance)
            };
            tradeDetailRepository.Insert(tradeDetail);

            // 自動補點與點數預警(不需要)
            // CheckCredit(user);
        }

        [Obsolete("DeleteChildUser 已經過時, 請使用 DeleteUser 取代")]
        private void DeleteChildUser(ApplicationUser user, ApplicationUser childUser)
        {
            DateTime utcNow = DateTime.UtcNow;

            // 回補點數
            decimal point = childUser.SmsBalance;
            var userRepository = this.unitOfWork.Repository<ApplicationUser>();
            user.SmsBalance += point;
            userRepository.Update(user);

            // 寫入交易明細
            var tradeDetailRepository = this.unitOfWork.Repository<TradeDetail>();
            TradeDetail tradeDetail = new TradeDetail
            {
                TradeTime = utcNow,
                TradeType = TradeType.Cover,
                Point = point,
                OwnerId = user.Id,
                TargetId = childUser.Id,
                Remark = string.Format("刪除子帳號{0}，將該帳號所有點數(共{1}點)，回補給您",
                    childUser.UserName,
                    childUser.SmsBalance)
            };
            tradeDetailRepository.Insert(tradeDetail);

            // 自動補點與點數預警
            CheckCredit(user);
        }

        #endregion

        #region 撥點作業(手動撥點、低於下限自動撥點、每月定時撥點)

        /// <summary>
        /// 手動撥點
        /// </summary>
        public void AllotPoint(ApplicationUser src, List<ApplicationUser> dsts, Decimal point)
        {
            if (src.SmsBalance < dsts.Count * point)
                throw new Exception(string.Format("點數不足，使用者{0}目前點數{1}點，撥給所需點數{2}點",
                    src.UserName,
                    src.SmsBalance,
                    dsts.Count * point));

            foreach (var dst in dsts)
            {
                AllotPoint(src, dst, point, bSrcCheckCredit: true, bDstCheckCredit: true);
            }
        }

        /// <summary>
        /// 撤銷撥點
        /// </summary>
        public void DismissAllot(TradeDetail tradeDetail)
        {
            var tradeDetailRepository = this.unitOfWork.Repository<TradeDetail>();
            var userRepository = this.unitOfWork.Repository<ApplicationUser>();

            if(tradeDetail.TradeType != TradeType.ExportPoints){
                throw new Exception("只有交易類型為【點數匯出】，才能執行撤銷撥點");
            }

            ApplicationUser dst = userRepository.GetById(tradeDetail.TargetId);
            ApplicationUser src = userRepository.GetById(tradeDetail.OwnerId);
            if(dst == null)return; // 使用者已經被刪除

            decimal point = Math.Abs(tradeDetail.Point); /* tradeDetail.Point 為負數 */
            decimal actualPoint = Math.Min(dst.SmsBalance, point);
            if (actualPoint != point)
            {
                // 使用者已經使用部分點數
                this.logService.Error("使用者 {0} 針對 {1} 撤銷撥點作業只完成部分，預期撤銷點數 {2}，實際撤銷點數 {3}",
                    src.UserName,
                    dst.UserName,
                    point,
                    actualPoint);
            }

            dst.SmsBalance -= actualPoint;
            src.SmsBalance += actualPoint;
            userRepository.Update(dst);
            userRepository.Update(src);

            // 刪除兩筆 TradeDetail
            TradeDetail anotherTradeDetail = tradeDetailRepository.Get(p =>
                p.TradeTime == tradeDetail.TradeTime &&
                p.TradeType == TradeType.ImportPoints &&
                p.OwnerId == tradeDetail.TargetId &&
                p.TargetId == tradeDetail.OwnerId);

            if (anotherTradeDetail != null) tradeDetailRepository.Delete(anotherTradeDetail);
            if (tradeDetail != null) tradeDetailRepository.Delete(tradeDetail);
        }

        /// <summary>
        /// 低於下限時自動撥點
        /// </summary>
        private void LimitAllotPoint(ApplicationUser user)
        {
            DateTime utcNow = DateTime.UtcNow;

            if (user.AllotSetting == null) return;
            if (user.AllotSetting.MonthlyAllot) return;
            if (user.SmsBalance > user.AllotSetting.LimitMinPoint) return;
            if (user.Parent == null) return;

            // 父帳號所需提供點數
            var point = user.AllotSetting.LimitMaxPoint - user.SmsBalance;

            if (user.Parent.SmsBalance < point)
            {
                // 父帳號點數不足以提供子帳號補點

                string subject = "低於下限時，自動補點失敗";

                string body = string.Format("使用者 {0} 點數不足以執行子帳號 {1} 自動補點作業。子帳號目前點數 {2} 已達指定下限點數 {3}，需自動補點至指定點數 {4}，目前點數{5}，但須要{6} 點",
                                user.Parent.UserName,
                                user.UserName,
                                user.SmsBalance,
                                user.AllotSetting.LimitMinPoint,
                                user.AllotSetting.LimitMaxPoint,
                                user.Parent.SmsBalance,
                                point);

                string[] destinations = new string[] { user.Parent.Email };

                // 紀錄LogService
                this.logService.Error(body);

                BackgroundJob.Enqueue<CommonMailService>(x => x.Send(subject, body, destinations));
            }
            else
            {
                user.AllotSetting.LastAllotTime = utcNow;
                this.unitOfWork.Repository<AllotSetting>().Update(user.AllotSetting);

                AllotPoint(user.Parent, user, point, 
                    bSrcCheckCredit: true, 
                    bDstCheckCredit: false /* 避免遞迴呼叫 */);
            }
        }

        /// <summary>
        /// 每月定時撥點
        /// </summary>
        public void MonthlyAllotPoint(ApplicationUser user)
        {
            DateTime utcNow = DateTime.UtcNow;

            if (user.AllotSetting == null) return;
            if (user.AllotSetting.MonthlyAllot == false) return;
            if (user.AllotSetting.MonthlyAllotDay == utcNow.Day) return;
            if (user.Parent == null) return;
            // 指定日期已經撥點完畢
            if (user.AllotSetting.LastAllotTime != null &&
                user.AllotSetting.LastAllotTime.Value - utcNow < TimeSpan.FromDays(1)) return;

            // 父帳號所需提供點數
            var point = user.AllotSetting.MonthlyAllotPoint;

            // 父帳號點數不足以提供子帳號補點
            if (user.Parent.SmsBalance < point)
            {
                string subject = "每月定時撥點，失敗";
                string body = string.Format("使用者 {0} 點數不足以執行子帳號 {1} 自動補點作業。目前點數{2}，但須要{3} 點",
                                user.Parent.UserName,
                                user.UserName,
                                user.Parent.SmsBalance,
                                point);
                string[] destinations = new string[] { user.Parent.Email };

                // 紀錄LogService
                this.logService.Error(body);

                BackgroundJob.Enqueue<CommonMailService>(x => x.Send(subject, body, destinations));
            }
            else
            {
                user.AllotSetting.LastAllotTime = utcNow;
                this.unitOfWork.Repository<AllotSetting>().Update(user.AllotSetting);

                AllotPoint(user.Parent, user, point,
                    bSrcCheckCredit: true,
                    bDstCheckCredit: false /* 避免遞迴呼叫 */);
            }
        }

        /// <summary>
        /// 每月定時撥點
        /// </summary>
        //private void AutoAllotPoint(ApplicationUser user, Decimal point)
        //{
        //    if (user.Parent == null) return;

        //    // 父帳號點數不足以提供子帳號補點
        //    if (user.Parent.SmsBalance < point)
        //    {
        //        string subject = "自動補點失敗";
        //        string body = !user.AllotSetting.MonthlyAllot
        //                    ? string.Format("使用者 {0} 點數不足以執行子帳號 {1} 自動補點作業。子帳號目前點數 {2} 已達指定下限點數 {3}，需自動補點至指定點數 {4}，目前點數{5}，但須要{6} 點",
        //                        user.Parent.UserName,
        //                        user.UserName,
        //                        user.SmsBalance,
        //                        user.AllotSetting.LimitMinPoint,
        //                        user.AllotSetting.LimitMaxPoint,
        //                        user.Parent.SmsBalance,
        //                        point)
        //                    : string.Format("使用者 {0} 點數不足以執行子帳號 {1} 自動補點作業。目前點數{2}，但須要{3} 點",
        //                        user.Parent.UserName,
        //                        user.UserName,
        //                        user.Parent.SmsBalance,
        //                        point);
        //        string[] destinations = new string[] { user.Parent.Email };

        //        BackgroundJob.Enqueue<GMailService>(x => x.Send(subject, body, destinations));

        //        // 紀錄LogService
        //        this.logService.Error(body);
        //    }
        //    else
        //    {
        //        AllotPoint(user.Parent, user, point, bSrcCheckCredit: true, bDstCheckCredit: false);

        //        user.AllotSetting.LastAllotTime = now;
        //        this.unitOfWork.Repository<AllotSetting>().Update(user.AllotSetting);
        //    }
        //}

        /// <summary>
        /// 每月定時撥點
        /// </summary>
        //public void CheckMonthlyAllot()
        //{
        //    DateTime utcNow = DateTime.UtcNow;

        //    var users = this.unitOfWork.Repository<ApplicationUser>().GetMany(p =>
        //        p.AllotSetting != null &&
        //        p.AllotSetting.MonthlyAllot == true &&
        //        p.AllotSetting.MonthlyAllotDay == utcNow.Day).ToList();

        //    foreach (var user in users)
        //    {
        //        if (user.AllotSetting.LastAllotTime != null &&
        //            user.AllotSetting.LastAllotTime.Value - utcNow < TimeSpan.FromDays(1)) continue;

        //        AutoAllotPoint(user, user.AllotSetting.MonthlyAllotPoint);
        //    }
        //}

        public void AllotPoint(ApplicationUser src, ApplicationUser dst, Decimal point, bool bSrcCheckCredit, bool bDstCheckCredit)
        {
            DateTime utcNow = DateTime.UtcNow;
            
            if (src.SmsBalance - point < 0)
            {
                throw new Exception(string.Format("點數不足，使用者{0}目前點數{1}點，無法提撥{2}點給使用者{3}",
                    src.UserName,
                    src.SmsBalance,
                    point,
                    dst.UserName));
            }

            // 撥點
            var userRepository = this.unitOfWork.Repository<ApplicationUser>();
            dst.SmsBalance += point;
            src.SmsBalance -= point;
            userRepository.Update(dst);
            userRepository.Update(src);

            // 寫入交易明細
            var tradeDetailRepository = this.unitOfWork.Repository<TradeDetail>();
            TradeDetail srcTradeDetail = new TradeDetail
            {
                TradeTime = utcNow,
                TradeType = TradeType.ExportPoints,
                Point = -1 * point,
                OwnerId = src.Id,
                TargetId = dst.Id,
                Remark = string.Format("點數轉出至{0}", dst.UserName)
            };
            TradeDetail dstTradeDetail = new TradeDetail
            {
                TradeTime = utcNow,
                TradeType = TradeType.ImportPoints,
                Point = point,
                OwnerId = dst.Id,
                TargetId = src.Id,
                Remark = string.Format("{0}撥款給您", src.UserName)
            };
            tradeDetailRepository.Insert(srcTradeDetail);
            tradeDetailRepository.Insert(dstTradeDetail);

            // 自動補點與點數預警

            if (bDstCheckCredit) CheckCredit(dst);
            if (bSrcCheckCredit) CheckCredit(src);
        }

        #endregion

        #region 警戒點數偵測

        private void CheckCredit(ApplicationUser user)
        {
            DateTime utcNow = DateTime.UtcNow;

            // 低於下限時自動撥點
            LimitAllotPoint(user);

            // 點數預警設定
            var creditWarning = user.CreditWarning;
            if (creditWarning == null) return;
            if (creditWarning.Enabled == false) return;
            if (creditWarning.LastNotifiedTime.HasValue &&
                creditWarning.NotifiedInterval > (utcNow - creditWarning.LastNotifiedTime.Value).TotalSeconds) return;

            string Email = user.Email;
            string Mobile = user.PhoneNumber;

            if (creditWarning.ByEmail)
            {
                string subject = "點數預警";
                string body = string.Format("目前點數 {0}，低於點數預警 {1}", user.SmsBalance, creditWarning.SmsBalance);
                string[] destinations = new string[] { Email };

                bool notifyMe = creditWarning.SmsBalance >= (user.SmsBalance);
                if (notifyMe)
                {
                    // 在發送通知之前，先更新預警通知時間，可以避免遞迴問題
                    creditWarning.LastNotifiedTime = utcNow;
                    this.unitOfWork.Repository<CreditWarning>().Update(creditWarning);

                    // 發送通知
                    BackgroundJob.Enqueue<CommonMailService>(x => x.Send(subject, body, destinations));
                }
            }

            if (creditWarning.BySmsMessage)
            {
                string subject = "點數預警";
                string body = string.Format("目前點數 {0}，低於點數預警 {1}", user.SmsBalance, creditWarning.SmsBalance);
                string[] destinations = new string[] { Mobile };

                var messageCostInfo = new MessageCostInfo(body, Mobile);

                // 以簡訊提供預警通知，花費點數為使用者自己的點數(messageCostInfo.MessageCost)
                bool notifyMe = creditWarning.SmsBalance >= (user.SmsBalance + messageCostInfo.MessageCost);

                if (notifyMe)
                {
                    // 在發送通知之前，先更新預警通知時間，可以避免遞迴問題
                    creditWarning.LastNotifiedTime = utcNow;
                    this.unitOfWork.Repository<CreditWarning>().Update(creditWarning);

                    // 發送通知
                    SendMessageRuleService sendMessageRuleService = new SendMessageRuleService(this.unitOfWork, this.logService);
                    sendMessageRuleService.CreateCreditWarningSendMessageRule(user, subject, body, destinations);
                }
            }
        }

        #endregion

        /// <summary>
        /// 回收所有點數
        /// </summary>
        /// <param name="currentUser">目前操作[回收所有點數]動作的使用者</param>
        /// <param name="user">要被回收點數的使用者</param>
        public void RecoveryPoint(ApplicationUser currentUser, ApplicationUser user)
        {
            DateTime utcNow = DateTime.UtcNow;

            ApplicationUser src = user;
            ApplicationUser dst = user.Parent;
            decimal point = src.SmsBalance;

            if (dst == null) return;

            // 撥點
            var userRepository = this.unitOfWork.Repository<ApplicationUser>();
            dst.SmsBalance += point;
            src.SmsBalance -= point;
            userRepository.Update(dst);
            userRepository.Update(src);

            // 寫入交易明細
            var tradeDetailRepository = this.unitOfWork.Repository<TradeDetail>();
            TradeDetail srcTradeDetail = new TradeDetail
            {
                TradeTime = utcNow,
                TradeType = TradeType.ExportRecoveryPoints,
                Point = -1 * point,
                OwnerId = src.Id,
                TargetId = dst.Id,
                Remark = string.Format("使用者{0}執行回收點數，所有點數回收至{1}", currentUser.UserName, dst.UserName)
            };
            TradeDetail dstTradeDetail = new TradeDetail
            {
                TradeTime = utcNow,
                TradeType = TradeType.ImportRecoveryPoints,
                Point = point,
                OwnerId = dst.Id,
                TargetId = src.Id,
                Remark = string.Format("使用者{0}執行回收點數，回收所有{1}點數", currentUser.UserName, src.UserName)
            };
            tradeDetailRepository.Insert(srcTradeDetail);
            tradeDetailRepository.Insert(dstTradeDetail);

            // 回收點數，不需要點數預警
            //if (bDstCheckCredit) CheckCredit(dst);
            //if (bSrcCheckCredit) CheckCredit(src);
        }

        
    }
}