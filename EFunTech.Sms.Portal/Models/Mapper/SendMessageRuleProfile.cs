using AutoMapper;
using EFunTech.Sms.Core;
using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
    public class SendMessageRuleProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<SendMessageRule, SendMessageRuleModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.SendTitle, opt => opt.MapFrom(src => src.SendTitle))
                .ForMember(dst => dst.SendBody, opt => opt.MapFrom(src => src.SendBody))
                .ForMember(dst => dst.RecipientFromType, opt => opt.MapFrom(src => src.RecipientFromType))
                .ForMember(dst => dst.RecipientFromFileUpload, opt => opt.MapFrom(src => src.RecipientFromFileUpload))
                .ForMember(dst => dst.RecipientFromCommonContact, opt => opt.MapFrom(src => src.RecipientFromCommonContact))
                .ForMember(dst => dst.RecipientFromGroupContact, opt => opt.MapFrom(src => src.RecipientFromGroupContact))
                .ForMember(dst => dst.RecipientFromManualInput, opt => opt.MapFrom(src => src.RecipientFromManualInput))
                .ForMember(dst => dst.SendTimeType, opt => opt.MapFrom(src => src.SendTimeType))
                .ForMember(dst => dst.SendDeliver, opt => opt.MapFrom(src => src.SendDeliver))
                .ForMember(dst => dst.SendCycleEveryDay, opt => opt.MapFrom(src => src.SendCycleEveryDay))
                .ForMember(dst => dst.SendCycleEveryWeek, opt => opt.MapFrom(src => src.SendCycleEveryWeek))
                .ForMember(dst => dst.SendCycleEveryMonth, opt => opt.MapFrom(src => src.SendCycleEveryMonth))
                .ForMember(dst => dst.SendCycleEveryYear, opt => opt.MapFrom(src => src.SendCycleEveryYear))
                .ForMember(dst => dst.SendCustType, opt => opt.MapFrom(src => src.SendCustType))
                .ForMember(dst => dst.UseParam, opt => opt.MapFrom(src => src.UseParam))
                .ForMember(dst => dst.SendMessageType, opt => opt.MapFrom(src => src.SendMessageType))
                .ForMember(dst => dst.TotalReceiverCount, opt => opt.MapFrom(src => src.TotalReceiverCount))
                .ForMember(dst => dst.TotalMessageCost, opt => opt.MapFrom(src => src.TotalMessageCost))
                .ForMember(dst => dst.RemainingSmsBalance, opt => opt.MapFrom(src => src.RemainingSmsBalance))
                .ForMember(dst => dst.SendMessageRuleStatus, opt => opt.MapFrom(src => src.SendMessageRuleStatus))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dst => dst.ClientTimezoneOffset, opt => opt.MapFrom(src => src.ClientTimezoneOffset))
                .ForMember(dst => dst.SenderAddress, opt => opt.MapFrom(src => src.SenderAddress))
                //.ForMember(dst => dst.SendTime, opt => opt.MapFrom(src => GetSendTime(src)))
                //.ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => GetStartDate(src)))
                //.ForMember(dst => dst.EndDate, opt => opt.MapFrom(src => GetEndDate(src)))
                //.ForMember(dst => dst.CycleString, opt => opt.MapFrom(src => GetCycleString(src)))
                ;
            
            CreateMap<SendMessageRuleModel, SendMessageRule>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.SendTitle, opt => opt.MapFrom(src => src.SendTitle))
                .ForMember(dst => dst.SendBody, opt => opt.MapFrom(src => src.SendBody))
                .ForMember(dst => dst.RecipientFromType, opt => opt.MapFrom(src => src.RecipientFromType))
                .ForMember(dst => dst.RecipientFromFileUpload, opt => opt.MapFrom(src => src.RecipientFromFileUpload))
                .ForMember(dst => dst.RecipientFromCommonContact, opt => opt.MapFrom(src => src.RecipientFromCommonContact))
                .ForMember(dst => dst.RecipientFromGroupContact, opt => opt.MapFrom(src => src.RecipientFromGroupContact))
                .ForMember(dst => dst.RecipientFromManualInput, opt => opt.MapFrom(src => src.RecipientFromManualInput))
                .ForMember(dst => dst.SendTimeType, opt => opt.MapFrom(src => src.SendTimeType))
                .ForMember(dst => dst.SendDeliver, opt => opt.MapFrom(src => src.SendDeliver))
                .ForMember(dst => dst.SendCycleEveryDay, opt => opt.MapFrom(src => src.SendCycleEveryDay))
                .ForMember(dst => dst.SendCycleEveryWeek, opt => opt.MapFrom(src => src.SendCycleEveryWeek))
                .ForMember(dst => dst.SendCycleEveryMonth, opt => opt.MapFrom(src => src.SendCycleEveryMonth))
                .ForMember(dst => dst.SendCycleEveryYear, opt => opt.MapFrom(src => src.SendCycleEveryYear))
                .ForMember(dst => dst.SendCustType, opt => opt.MapFrom(src => src.SendCustType))
                .ForMember(dst => dst.UseParam, opt => opt.MapFrom(src => src.UseParam))
                .ForMember(dst => dst.SendMessageType, opt => opt.MapFrom(src => src.SendMessageType))
                .ForMember(dst => dst.TotalReceiverCount, opt => opt.MapFrom(src => src.TotalReceiverCount))
                .ForMember(dst => dst.TotalMessageCost, opt => opt.MapFrom(src => src.TotalMessageCost))
                .ForMember(dst => dst.RemainingSmsBalance, opt => opt.MapFrom(src => src.RemainingSmsBalance))
                .ForMember(dst => dst.SendMessageRuleStatus, opt => opt.MapFrom(src => src.SendMessageRuleStatus))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dst => dst.ClientTimezoneOffset, opt => opt.MapFrom(src => src.ClientTimezoneOffset))
                .ForMember(dst => dst.SenderAddress, opt => opt.MapFrom(src => src.SenderAddress))
                ;
        }

        public static SendMessageRuleModel ConvertModel(SendMessageRuleModel model)
        {
            model.SendTime = GetSendTime(model);
            model.StartDate = GetStartDate(model);
            model.EndDate = GetEndDate(model);
            model.CycleString = GetCycleString(model);

            return model;
        }

        public static DateTime? GetSendTime(SendMessageRuleModel model)
        {
            if (model.SendTimeType == SendTimeType.Deliver)
            {
                var rule = AutoMapper.Mapper.Map<SendMessageRuleModel, SendMessageRule>(model);

                return rule.GetSendTime();
            }
            else
            {
                return null;
            }
        }



        public static DateTime? GetStartDate(SendMessageRuleModel model)
        {
            if (model.SendTimeType == SendTimeType.Cycle)
            {
                if (model.SendCycleEveryDay != null)
                {
                    return model.SendCycleEveryDay.StartDate;
                }
                else if (model.SendCycleEveryWeek != null)
                {
                    return model.SendCycleEveryWeek.StartDate;
                }
                else if (model.SendCycleEveryMonth != null)
                {
                    return model.SendCycleEveryMonth.StartDate;
                }
                else if (model.SendCycleEveryYear != null)
                {
                    return model.SendCycleEveryYear.StartDate;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static DateTime? GetEndDate(SendMessageRuleModel model)
        {
            if (model.SendTimeType == SendTimeType.Cycle)
            {
                if (model.SendCycleEveryDay != null)
                {
                    return model.SendCycleEveryDay.EndDate;
                }
                else if (model.SendCycleEveryWeek != null)
                {
                    return model.SendCycleEveryWeek.EndDate;
                }
                else if (model.SendCycleEveryMonth != null)
                {
                    return model.SendCycleEveryMonth.EndDate;
                }
                else if (model.SendCycleEveryYear != null)
                {
                    return model.SendCycleEveryYear.EndDate;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static string GetDayOfWeekString(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday: return "週日";
                case DayOfWeek.Monday: return "週一";
                case DayOfWeek.Tuesday: return "週二";
                case DayOfWeek.Wednesday: return "週三";
                case DayOfWeek.Thursday: return "週四";
                case DayOfWeek.Friday: return "週五";
                case DayOfWeek.Saturday: return "週六";
                default: return dayOfWeek.ToString();
            }
        }

        public static string GetCycleString(SendMessageRuleModel model)
        {
            DateTime utcNow = DateTime.UtcNow;

            if (model.SendTimeType == SendTimeType.Cycle)
            {
                if (model.SendCycleEveryDay != null)
                {
                    DateTime cycleDate = Converter.ToLocalTime(
                        model.SendCycleEveryDay.SendTime, 
                        model.SendCycleEveryDay.ClientTimezoneOffset);

                    return string.Format("每天的{0}點{1}分",
                        cycleDate.ToString("HH"),
                        cycleDate.ToString("mm"));
                }
                else if (model.SendCycleEveryWeek != null)
                {
                    DateTime cycleDate = Converter.ToLocalTime(
                        model.SendCycleEveryWeek.SendTime,
                        model.SendCycleEveryWeek.ClientTimezoneOffset);

                    List<DayOfWeek> dayOfWeeks = Converter.ToLocalDayOfWeeks(
                        model.SendCycleEveryWeek.SendTime,
                        model.SendCycleEveryWeek.GetDayOfWeeks(),
                        model.SendCycleEveryWeek.ClientTimezoneOffset);

                    // http://254698001.blog.51cto.com/2521548/711940
                    //return string.Format("每周({0})的{1}",
                    //    // 取得 DayOfWeek 的中文名稱(C#)
                    //    // http://zip.nvp.com.tw/forum.php?mod=viewthread&tid=1150
                    //    string.Join("、", dayOfWeeks.Select(p => System.Globalization.DateTimeFormatInfo.CurrentInfo.DayNames[(int)p])),
                    //    cycleDate.ToString("HH:mm"));

                    return string.Format("{0}的{1}點{2}分",
                        // 取得 DayOfWeek 的中文名稱(C#)
                        // http://zip.nvp.com.tw/forum.php?mod=viewthread&tid=1150
                        string.Join("、", dayOfWeeks.Select(p => GetDayOfWeekString(p))),
                        cycleDate.ToString("HH"),
                        cycleDate.ToString("mm"));
                }
                else if (model.SendCycleEveryMonth != null)
                {
                    DateTime cycleDate = Converter.ToLocalTime(
                        model.SendCycleEveryMonth.SendTime,
                        model.SendCycleEveryMonth.ClientTimezoneOffset);

                    return string.Format("每月的{0}號{1}點{2}分",
                        cycleDate.ToString("dd"),
                        cycleDate.ToString("HH"),
                        cycleDate.ToString("mm"));
                }
                else if (model.SendCycleEveryYear != null)
                {
                    DateTime cycleDate = Converter.ToLocalTime(
                        model.SendCycleEveryYear.SendTime,
                        model.SendCycleEveryYear.ClientTimezoneOffset);

                    return string.Format("每年的{0}月{1}號{2}點{3}分",
                        cycleDate.ToString("MM"),
                        cycleDate.ToString("dd"),
                        cycleDate.ToString("HH"),
                        cycleDate.ToString("mm"));
                }
                else
                {
                    return string.Empty;
                }

            }
            else
            {
                return string.Empty;
            }
        }
    }
}
