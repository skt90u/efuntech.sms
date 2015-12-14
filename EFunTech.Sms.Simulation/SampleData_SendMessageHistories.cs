using AutoPoco;
using AutoPoco.DataSources;
using AutoPoco.Engine;
using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Validation;
using EFunTech.Sms.Simulation.DataSources;
using EFunTech.Sms.Simulation.Comparision;
using System.Linq.Expressions;
using System.Data.Entity;

namespace EFunTech.Sms.Simulation
{
    public class SampleData_SendMessageHistories : SampleData 
    {
        //private static IGenerationSessionFactory factory;
        //private static IRandomNumberGenerator randomNumberGenerator;
        //private static IGenerationSession session;

        static SampleData_SendMessageHistories()
        {
            /*
            DateTime now = DateTime.UtcNow;

            factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.UseDefaultConventions();
                });

                x.AddFromAssemblyContainingType<SendMessageHistory>();

                x.Include<SendMessageHistory>()
                    // CreatedUserId
                    // DepartmentId
                    // SendMessageRuleId
                    // SendMessageQueueId
                    .Setup(p => p.SendMessageType).Use<EnumSource<SendMessageType>>()
                    //.Setup(p => p.SendTime).Use<DateTimeSource>(now.AddYears(-1), now.AddYears(1))
                    //.Setup(p => p.SendTitle).Use<SubjectDataSource>()
                    //.Setup(p => p.SendBody).Use<ContentDataSource>()
                    .Setup(p => p.SendCustType).Use<EnumSource<SendCustType>>()
                    .Setup(p => p.RequestId).Use<GuidDataSource>()
                    .Setup(p => p.MessageId).Use<GuidDataSource>()
                    // MessageStatusString
                    .Setup(p => p.MessageStatus).Use<EnumSource<Infobip_MessageStatus>>()
                    // SenderAddress
                    .Setup(p => p.DestinationAddress).Use<E164MobileDataSource>()
                    .Setup(p => p.SentDate).Use<DateTimeSource>(now.AddYears(-10), now.AddYears(10))
                    .Setup(p => p.DoneDate).Use<DateTimeSource>(now.AddYears(-10), now.AddYears(10))
                    // StatusString
                    .Setup(p => p.Status).Use<EnumSource<Infobip_DeliveryReportStatus>>()
                    // Price
                    .Setup(p => p.DeliveryReportCreatedTimeow.AddYears(-10), now.AddYears(10))
                    .Setup(p => p.Infobip_DeliveryReport_CreatedTime).Use<DateTimeSource>(now.AddYears(-10), now.AddYears(10))
                    // MessageCost
                    .Setup(p => p.Delivered).Use<BooleanSource>()
                    // RetryTime
                    // Every8d_Credit
                    // Every8d_Sended
                    // Every8d_Cost
                    // Every8d_Unsend
                    .Setup(p => p.Every8d_BatchId).Use<GuidDataSource>()
                    // Every8d_SentTime
                    .Setup(p => p.Every8d_Status).Use<EnumSource<Every8d_DeliveryReportStatus>>()
                    // Every8d_StatusString
                    .Setup(p => p.Every8d_Delivered).Use<BooleanSource>()
                    ;
            });

            session = factory.CreateSession();

            randomNumberGenerator = new RandomNumberGenerator();
            */
        }

        private static Dictionary<string, int> dictMinSendMessageRuleId = new Dictionary<string, int> { 
            {"Administrator", 1 + 0 * maxSizeSendMessageRuleId},
            {"Eric",          1 + 1 * maxSizeSendMessageRuleId},
            {"Dino",          1 + 2 * maxSizeSendMessageRuleId},
            {"Norman",        1 + 3 * maxSizeSendMessageRuleId},
        };
        public const int maxSizeSendMessageRuleId = 10;

        protected override void SeedEntity(ApplicationDbContext context, ApplicationUser user)
        {
            /*
            DateTime now = DateTime.UtcNow;
            string CreatedUserId = user.Id;
            
            int? DepartmentId = null;
            if (user.Department != null)
                DepartmentId = user.Department.Id;

            DateTime RetryTime = now;
            string Every8d_BatchId = Guid.NewGuid().ToString().Replace("-", string.Empty);

            DateTime Every8d_SentTime = now;
            
            // 已經有測試資料
            if (context.SendMessageHistorys.Where(p => p.CreatedUserId == CreatedUserId).Count() != 0) return;

            var SendMessageHistorys = session.List<SendMessageHistory>(randomNumberGenerator.Next(1000, 3000))
                .Impose(p => p.CreatedUserId, CreatedUserId)
                .Impose(p => p.DepartmentId, DepartmentId)
                //.Impose(p => p.SendMessageRuleId, randomNumberGenerator.Next(1, 10))
                //.Impose(p => p.SendMessageQueueId, randomNumberGenerator.Next(1, 30))
                .Impose(p => p.SenderAddress, SendMessageRule.DefaultSenderAddress)
                //.Impose(p => p.Price, (decimal)randomNumberGenerator.Next(1, 30))
                //.Impose(p => p.MessageCost, (decimal)randomNumberGenerator.Next(1, 30))
                .Impose(p => p.Every8d_SendMessageQueue, null)
                //.Impose(p => p.RetryTime, RetryTime)
                //.Impose(p => p.Every8d_Credit, (double)randomNumberGenerator.Next(1, 30))
                //.Impose(p => p.Every8d_Sended, (int)randomNumberGenerator.Next(1, 30))
                //.Impose(p => p.Every8d_Cost, (double)randomNumberGenerator.Next(1, 30))
                //.Impose(p => p.Every8d_Unsend, (int)randomNumberGenerator.Next(1, 30))
                //.Impose(p => p.Every8d_SentTime, Every8d_SentTime)
                .Get();
            
            foreach (var SendMessageHistory in SendMessageHistorys)
            {
                var sizeSendMessageRuleId = randomNumberGenerator.Next(1, 10);
                var minSendMessageRuleId = dictMinSendMessageRuleId[user.UserName];
                var maxSendMessageRuleId = minSendMessageRuleId + sizeSendMessageRuleId - 1;

                SendMessageHistory.SendMessageRuleId = randomNumberGenerator.Next(minSendMessageRuleId, maxSendMessageRuleId);
                SendMessageHistory.SendMessageQueueId = SendMessageHistory.SendMessageRuleId;

                SendMessageHistory.SendTime = now;
                SendMessageHistory.SendTitle = "好野手難選 桃猿挑走王柏融";
                SendMessageHistory.SendBody = "（中央社記者李宇政台北29日電）大滿貫等級的草地賽事溫網今天點燃戰火，台灣女將詹皓晴今天在臉書上表示，「溫布頓終身會員證到手啦！」她去年在混雙殺進決賽，今年又是新的開始。";

                SendMessageHistory.Price = (decimal)randomNumberGenerator.Next(1, 30);
                SendMessageHistory.MessageCost = (decimal)randomNumberGenerator.Next(1, 30);

                SendMessageHistory.Every8d_Credit = (double)randomNumberGenerator.Next(1, 30);
                SendMessageHistory.Every8d_Sended = (int)randomNumberGenerator.Next(1, 30);
                SendMessageHistory.Every8d_Cost = (double)randomNumberGenerator.Next(1, 30);
                SendMessageHistory.Every8d_Unsend = (int)randomNumberGenerator.Next(1, 30);

                SendMessageHistory.MessageStatusString = SendMessageHistory.MessageStatus.ToString();
                SendMessageHistory.StatusString = SendMessageHistory.Status.ToString();
                SendMessageHistory.Every8d_StatusString = SendMessageHistory.Every8d_Status.ToString();

                if (randomNumberGenerator.Next(minSendMessageRuleId, maxSendMessageRuleId) % 2 == 0)
                {
                    SendMessageHistory.RetryTime = now;
                    SendMessageHistory.Every8d_Credit = (double)randomNumberGenerator.Next(1, 30);
                    SendMessageHistory.Every8d_Sended = (int)randomNumberGenerator.Next(1, 30);
                    SendMessageHistory.Every8d_Cost = (double)randomNumberGenerator.Next(1, 30);
                    SendMessageHistory.Every8d_Unsend = (int)randomNumberGenerator.Next(1, 30);
                    SendMessageHistory.Every8d_BatchId = Every8d_BatchId;
                    SendMessageHistory.Every8d_SentTime = now;
                }
                else
                {
                    SendMessageHistory.RetryTime = null;
                    SendMessageHistory.Every8d_Credit = null;
                    SendMessageHistory.Every8d_Sended = null;
                    SendMessageHistory.Every8d_Cost = null;
                    SendMessageHistory.Every8d_Unsend = null;
                    SendMessageHistory.Every8d_BatchId = null;
                    SendMessageHistory.Every8d_SentTime = null;
                }
                
            }
            context.SendMessageHistorys.AddRange(SendMessageHistorys);

            context.SaveChanges();
            */
        }
    }
}
