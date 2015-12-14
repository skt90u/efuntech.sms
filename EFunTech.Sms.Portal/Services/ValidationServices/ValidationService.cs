using EFunTech.Sms.Schema;
using JUtilSharp.Database;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EFunTech.Sms.Portal
{
    public class ValidationService
    {
        protected IUnitOfWork unitOfWork;
        protected ILogService logService;

        public ValidationService(IUnitOfWork unitOfWork, ILogService logService)
        {
            this.unitOfWork = unitOfWork;
            this.logService = logService;
        }

        public bool Validate(MessageReceiver entity, out string error)
        {
            List<string> errors = new List<string>();

            // Mobile
            if (string.IsNullOrEmpty(entity.Mobile))
            {
                errors.Add("手機門號必填");
            }
            else
            {
                if (!MobileUtil.IsPossibleNumber(entity.Mobile))
                {
                    errors.Add("手機門號格式錯誤");
                }
                else
                {
                    // 檢驗是否為黑名單
                    if (this.unitOfWork.Repository<Blacklist>().Any(p => p.Mobile == entity.Mobile))
                    {
                        errors.Add("黑名單");
                    }
                }
            }

            // Email
            if (!string.IsNullOrEmpty(entity.Email))
            {
                Regex rgx = new Regex(RegularExpressionPatterns.Email);

                if (!rgx.IsMatch(entity.Email))
                {
                    errors.Add("電子郵件格式錯誤");
                }
            }

            error = string.Join(", ", errors);
            
            return string.IsNullOrEmpty(error);
        }

        public bool Validate(UploadedMessageReceiver entity, out string error)
        {
            List<string> errors = new List<string>();

            // Mobile
            if (string.IsNullOrEmpty(entity.Mobile))
            {
                errors.Add("手機門號必填");
            }
            else
            {
                if (!MobileUtil.IsPossibleNumber(entity.Mobile))
                {
                    errors.Add("手機門號格式錯誤");
                }
                else
                {
                    // 檢驗是否為黑名單
                    if (this.unitOfWork.Repository<Blacklist>().Any(p => p.Mobile == entity.Mobile))
                    {
                        errors.Add("黑名單");
                    }
                }
            }

            // Email
            if (!string.IsNullOrEmpty(entity.Email))
            {
                Regex rgx = new Regex(RegularExpressionPatterns.Email);

                if (!rgx.IsMatch(entity.Email))
                {
                    errors.Add("電子郵件格式錯誤");
                }
            }

            if (!string.IsNullOrEmpty(entity.SendTimeString))
            {
                if (entity.SendTime.HasValue)
                {
                    if (entity.SendTime.Value < DateTime.UtcNow)
                    {
                        errors.Add("指定時間必須大於目前時間");
                    }
                }
                else
                {
                    // 不為空的時間字串必須格式正確
                    if (!string.IsNullOrEmpty(entity.SendTimeString))
                    {
                        errors.Add("時間格式錯誤");
                    }
                }
            }
            
            error = string.Join(", ", errors);
            entity.IsValid = string.IsNullOrEmpty(error);
            entity.InvalidReason = error;
            return string.IsNullOrEmpty(error);
        }

        public bool Validate(Blacklist entity, out string error)
        {
            List<string> errors = new List<string>();

            // Mobile
            if (string.IsNullOrEmpty(entity.Mobile))
            {
                errors.Add("手機門號必填");
            }
            else
            {
                if (!MobileUtil.IsPossibleNumber(entity.Mobile))
                {
                    errors.Add("手機門號格式錯誤");
                }
            }

            error = string.Join(", ", errors);
            return string.IsNullOrEmpty(error);
        }

        public bool Validate(Contact entity, out string error)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(entity.Name))
            {
                errors.Add("姓名必填");
            }

            if (string.IsNullOrEmpty(entity.Mobile))
            {
                errors.Add("手機門號必填");
            }
            else
            {
                if (!MobileUtil.IsPossibleNumber(entity.Mobile))
                {
                    errors.Add("手機門號格式錯誤");
                }
            }

            // HomePhone
            // CompanyPhone
            // Email
            // Msn
            // Description
            // Birthday
            // ImportantDay

            error = string.Join(", ", errors);
            return string.IsNullOrEmpty(error);
        }
    }
}