using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFunTech.Sms.Core;

namespace EFunTech.Sms.Schema
{
    public static class SendTimeParamsExtensions
    {
        /// <summary>
        /// 預定發送時間
        /// </summary>
        public static DateTime GetSendTime(this SendDeliver sendDeliver)
        {
            return sendDeliver.SendTime;
        }

        public static List<DateTime> GetSendTimes(this SendCycleEveryDay sendCycleEveryDay)
        {
            var result = new List<DateTime>();

            DateTime startDate = sendCycleEveryDay.StartDate;
            DateTime endDate = sendCycleEveryDay.EndDate;
            
            DateTime sendTime = sendCycleEveryDay.SendTime; 
            /* SendTimeModalCtrl.js
             * 
            var SendTime = new Date(
                StartDate.getFullYear(),
                StartDate.getMonth(),
                StartDate.getDate(),
                Hour,
                Minute,
                0); // 必須設為 0，否則 Hangfire 會發生一分鐘的誤差
             */

            while (sendTime <= endDate)
            {
                result.Add(sendTime);

                sendTime = sendTime.AddDays(1);
            }

            return result;
        }

        public static List<DateTime> GetSendTimes(this SendCycleEveryWeek sendCycleEveryWeek)
        {
            var result = new List<DateTime>();

            DateTime startDate = sendCycleEveryWeek.StartDate;
            DateTime endDate = sendCycleEveryWeek.EndDate;
            List<DayOfWeek> dayOfWeeks = sendCycleEveryWeek.GetDayOfWeeks();

            DateTime sendTime = sendCycleEveryWeek.SendTime;
            /* SendTimeModalCtrl.js
             * 
            var SendTime = new Date(
                StartDate.getFullYear(),
                StartDate.getMonth(),
                StartDate.getDate(),
                Hour,
                Minute,
                0); // 必須設為 0，否則 Hangfire 會發生一分鐘的誤差
             */

            while (sendTime <= endDate)
            {
                if (dayOfWeeks.Contains(sendTime.DayOfWeek))
                {
                    result.Add(sendTime);
                }

                sendTime = sendTime.AddDays(1);
            }

            return result;
        }

        public static List<DateTime> GetSendTimes(this SendCycleEveryMonth sendCycleEveryMonth)
        {
            var result = new List<DateTime>();

            DateTime startDate = sendCycleEveryMonth.StartDate;
            DateTime endDate = sendCycleEveryMonth.EndDate;

            DateTime sendTime = sendCycleEveryMonth.SendTime;
            /* SendTimeModalCtrl.js
             * 
            var SendTime = new Date(
                StartDate.getFullYear(),
                StartDate.getMonth(),
                StartDate.getDate(),
                Hour,
                Minute,
                0); // 必須設為 0，否則 Hangfire 會發生一分鐘的誤差
             */

            while (sendTime <= endDate)
            {
                if (sendTime.Day == sendCycleEveryMonth.SendTime.Day)
                {
                    result.Add(sendTime);
                }

                sendTime = sendTime.AddDays(1);
            }

            return result;
        }

        public static List<DateTime> GetSendTimes(this SendCycleEveryYear sendCycleEveryYear)
        {
            var result = new List<DateTime>();

            DateTime dt = sendCycleEveryYear.StartDate;
            DateTime startDate = sendCycleEveryYear.StartDate;
            DateTime endDate = sendCycleEveryYear.EndDate;

            DateTime sendTime = sendCycleEveryYear.SendTime;
            /* SendTimeModalCtrl.js
             * 
            var SendTime = new Date(
                StartDate.getFullYear(),
                Month - 1, 
                Day,
                Hour,
                Minute,
                0); // 必須設為 0，否則 Hangfire 會發生一分鐘的誤差
             */

            while (sendTime <= endDate)
            {
                result.Add(sendTime);

                sendTime = sendTime.AddYears(1);
            }

            return result;
        }
        
    }
}
