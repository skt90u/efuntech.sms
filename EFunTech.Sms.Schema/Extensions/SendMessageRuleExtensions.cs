using EFunTech.Sms.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    public static class SendMessageRuleExtensions
    {
        public static List<DateTime> GetSendTimeList(this SendMessageRule sendMessageRule)
        {
            List<DateTime> results = new List<DateTime>();

            switch (sendMessageRule.SendTimeType)
            {
                case Schema.SendTimeType.Immediately:
                    {
                        results.Add(sendMessageRule.CreatedTime);
                    } break;
                case Schema.SendTimeType.Deliver:
                    {
                        results.Add(sendMessageRule.SendDeliver.GetSendTime());
                    } break;
                case Schema.SendTimeType.Cycle:
                    {
                        List<DateTime> sendTimes = null;

                        // 週期簡訊回傳的發送時間若為NULL，表示已經不在發送起訖日期範圍內。
                        if (sendTimes == null && sendMessageRule.SendCycleEveryDay != null)
                            sendTimes = sendMessageRule.SendCycleEveryDay.GetSendTimes();

                        if (sendTimes == null && sendMessageRule.SendCycleEveryWeek != null)
                            sendTimes = sendMessageRule.SendCycleEveryWeek.GetSendTimes();

                        if (sendTimes == null && sendMessageRule.SendCycleEveryMonth != null)
                            sendTimes = sendMessageRule.SendCycleEveryMonth.GetSendTimes();

                        if (sendTimes == null && sendMessageRule.SendCycleEveryYear != null)
                            sendTimes = sendMessageRule.SendCycleEveryYear.GetSendTimes();

                        if (sendTimes != null)
                            results.AddRange(sendTimes);
                    } break;
            }

            return results;
        }

        public static string GetSendTimeString(this SendMessageRule sendMessageRule, int displaySize = Int32.MaxValue -1)
        {
            List<DateTime> results = GetSendTimeList(sendMessageRule);

            var clientTimezoneOffset = sendMessageRule.ClientTimezoneOffset;
            var sendTimeStrings = results.Select(p => Converter.ToLocalTime(p, clientTimezoneOffset).ToString(Converter.Every8d_SentTime)).ToList();

            var output = string.Join("、", sendTimeStrings.Take(Math.Min(displaySize, sendTimeStrings.Count)));

            output = "共{0}筆，" + output;

            if (sendTimeStrings.Count > displaySize)
            {
                output = output + "、...";
            }

            return output;
        }

        /// <summary>
        /// 取得發送時間UtcTime
        /// </summary>
        /// <param name="sendMessageRule">The send message rule.</param>
        /// <returns></returns>
        public static DateTime? GetSendTime(this SendMessageRule sendMessageRule)
        {
            switch (sendMessageRule.SendTimeType)
            {
                case Schema.SendTimeType.Immediately:
                    {
                        return sendMessageRule.CreatedTime; 
                    }
                case Schema.SendTimeType.Deliver:
                    {
                        return sendMessageRule.SendDeliver.GetSendTime();
                    }
                case Schema.SendTimeType.Cycle:
                    {
                        List<DateTime> sendTimes = null;

                        // 週期簡訊回傳的發送時間若為NULL，表示已經不在發送起訖日期範圍內。
                        if (sendTimes == null && sendMessageRule.SendCycleEveryDay != null)
                            sendTimes = sendMessageRule.SendCycleEveryDay.GetSendTimes();

                        if (sendTimes == null && sendMessageRule.SendCycleEveryWeek != null)
                            sendTimes = sendMessageRule.SendCycleEveryWeek.GetSendTimes();

                        if (sendTimes == null && sendMessageRule.SendCycleEveryMonth != null)
                            sendTimes = sendMessageRule.SendCycleEveryMonth.GetSendTimes();

                        if (sendTimes == null && sendMessageRule.SendCycleEveryYear != null)
                            sendTimes = sendMessageRule.SendCycleEveryYear.GetSendTimes();

                        if (sendTimes == null)
                        {
                            return null;
                        }
                        else
                        {
                            // TODO: 可能會有問題，需要再思考，是否直接回傳所有SendTimes
                            var result = sendTimes.Where(p => p <= DateTime.UtcNow).LastOrDefault(); // default is DateTime.MinValue

                            if (result != DateTime.MinValue)
                                return result;
                            else
                                return null;
                        }
                    }
            }
            return null;
        }

        public static DateTime? GetNextSendTime(this SendMessageRule sendMessageRule) {
            switch (sendMessageRule.SendTimeType)
            {
                case Schema.SendTimeType.Immediately:
                    {
                        return null;
                    }
                case Schema.SendTimeType.Deliver:
                    {
                        return null;
                    }
                case Schema.SendTimeType.Cycle:
                    {
                        List<DateTime> sendTimes = null;

                        // 週期簡訊回傳的發送時間若為NULL，表示已經不在發送起訖日期範圍內。
                        if (sendTimes == null && sendMessageRule.SendCycleEveryDay != null)
                            sendTimes = sendMessageRule.SendCycleEveryDay.GetSendTimes();

                        if (sendTimes == null && sendMessageRule.SendCycleEveryWeek != null)
                            sendTimes = sendMessageRule.SendCycleEveryWeek.GetSendTimes();

                        if (sendTimes == null && sendMessageRule.SendCycleEveryMonth != null)
                            sendTimes = sendMessageRule.SendCycleEveryMonth.GetSendTimes();

                        if (sendTimes == null && sendMessageRule.SendCycleEveryYear != null)
                            sendTimes = sendMessageRule.SendCycleEveryYear.GetSendTimes();

                        if (sendTimes == null)
                        {
                            return null;
                        }
                        else
                        {
                            sendTimes.Sort();

                            // TODO: 可能會有問題，需要再思考，是否直接回傳所有SendTimes
                            var result = sendTimes.Where(p => p > DateTime.UtcNow).FirstOrDefault(); // default is DateTime.MinValue

                            if (result != DateTime.MinValue)
                                return result;
                            else
                                return null;
                        }
                    }
            }
            return null;
        }
    }
}
