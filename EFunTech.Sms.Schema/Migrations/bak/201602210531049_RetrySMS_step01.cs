namespace EFunTech.Sms.Schema.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RetrySMS_step01 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Every8d_DeliveryReport", "SendMessageResult_SendMessageQueueId", "dbo.Every8d_SendMessageResult");
            DropForeignKey("dbo.Every8d_SendMessageResult", "SendMessageQueueId", "dbo.SendMessageQueues");
            DropForeignKey("dbo.Infobip_ResourceReference", "SendMessageResultId", "dbo.Infobip_SendMessageResult");
            DropForeignKey("dbo.Infobip_SendMessageResult", "SendMessageQueueId", "dbo.SendMessageQueues");
            DropForeignKey("dbo.Infobip_SendMessageResultItem", "SendMessageResult_SendMessageQueueId", "dbo.Infobip_SendMessageResult");
            DropForeignKey("dbo.Infobip_DeliveryReport", "MessageId", "dbo.Infobip_SendMessageResultItem");
            DropIndex("dbo.Every8d_DeliveryReport", new[] { "SendMessageResult_SendMessageQueueId" });
            DropIndex("dbo.Every8d_SendMessageResult", new[] { "SendMessageQueueId" });
            DropIndex("dbo.Every8d_SendMessageResult", new[] { "BATCH_ID" });
            DropIndex("dbo.Infobip_DeliveryReport", new[] { "MessageId" });
            DropIndex("dbo.Infobip_SendMessageResultItem", new[] { "SendMessageResult_SendMessageQueueId" });
            DropIndex("dbo.Infobip_SendMessageResult", new[] { "SendMessageQueueId" });
            DropIndex("dbo.Infobip_SendMessageResult", new[] { "ClientCorrelator" });
            DropIndex("dbo.Infobip_ResourceReference", new[] { "SendMessageResultId" });
            CreateTable(
                "dbo.SendMessageRetryHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SendMessageHistoryId = c.Int(nullable: false),
                        RequestId = c.String(nullable: false, maxLength: 256),
                        ProviderName = c.String(nullable: false, maxLength: 256),
                        MessageId = c.String(maxLength: 256),
                        MessageStatus = c.Int(nullable: false),
                        MessageStatusString = c.String(),
                        SenderAddress = c.String(),
                        DestinationAddress = c.String(),
                        SendMessageResultCreatedTime = c.DateTime(nullable: false),
                        SentDate = c.DateTime(),
                        DoneDate = c.DateTime(),
                        DeliveryStatus = c.Int(nullable: false),
                        DeliveryStatusString = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveryReportCreatedTime = c.DateTime(),
                        Delivered = c.Boolean(nullable: false),
                        CreatedTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CreatedTime);
            
            AddColumn("dbo.AspNetUsers", "SmsProviderType", c => c.Int(nullable: false));
            //AddColumn("dbo.DeliveryReportQueues", "SourceTableId", c => c.Int(nullable: false));
            AddColumn("dbo.DeliveryReportQueues", "SourceTable", c => c.Int(nullable: false));
            AddColumn("dbo.SendMessageHistories", "RetryMaxTimes", c => c.Int(nullable: false));
            AddColumn("dbo.SendMessageHistories", "RetryTotalTimes", c => c.Int(nullable: false));
            AddColumn("dbo.SendMessageHistories", "SendMessageRetryHistoryId", c => c.Int());
            CreateIndex("dbo.SendMessageHistories", "SendMessageRetryHistoryId");
            AddForeignKey("dbo.SendMessageHistories", "SendMessageRetryHistoryId", "dbo.SendMessageRetryHistories", "Id");
            //DropColumn("dbo.DeliveryReportQueues", "SendMessageQueueId");
            RenameColumn("dbo.DeliveryReportQueues", "SendMessageQueueId", "SourceTableId");
            DropTable("dbo.Every8d_DeliveryReport");
            DropTable("dbo.Every8d_SendMessageResult");
            //DropTable("dbo.SendMessageQueues");
            DropTable("dbo.Infobip_DeliveryReport");
            DropTable("dbo.Infobip_SendMessageResultItem");
            DropTable("dbo.Infobip_SendMessageResult");
            DropTable("dbo.Infobip_ResourceReference");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Infobip_ResourceReference",
                c => new
                    {
                        SendMessageResultId = c.Int(nullable: false),
                        ResourceURL = c.String(),
                    })
                .PrimaryKey(t => t.SendMessageResultId);
            
            CreateTable(
                "dbo.Infobip_SendMessageResult",
                c => new
                    {
                        SendMessageQueueId = c.Int(nullable: false),
                        ClientCorrelator = c.String(nullable: false, maxLength: 256),
                        CreatedTime = c.DateTime(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.SendMessageQueueId);
            
            CreateTable(
                "dbo.Infobip_SendMessageResultItem",
                c => new
                    {
                        MessageId = c.String(nullable: false, maxLength: 256),
                        MessageStatus = c.Int(nullable: false),
                        MessageStatusString = c.String(),
                        SenderAddress = c.String(),
                        DestinationAddress = c.String(),
                        DestinationName = c.String(),
                        SendMessageResult_SendMessageQueueId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MessageId);
            
            CreateTable(
                "dbo.Infobip_DeliveryReport",
                c => new
                    {
                        MessageId = c.String(nullable: false, maxLength: 256),
                        RequestId = c.String(nullable: false),
                        SentDate = c.DateTime(nullable: false),
                        DoneDate = c.DateTime(nullable: false),
                        StatusString = c.String(),
                        Status = c.Int(nullable: false),
                        Price = c.Decimal(precision: 18, scale: 2),
                        CreatedTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MessageId);
            
            CreateTable(
                "dbo.SendMessageQueues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SendMessageType = c.Int(nullable: false),
                        SendTime = c.DateTime(nullable: false),
                        SendTitle = c.String(),
                        SendBody = c.String(nullable: false),
                        SendCustType = c.Int(nullable: false),
                        TotalReceiverCount = c.Int(nullable: false),
                        TotalMessageCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SendMessageRuleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Every8d_SendMessageResult",
                c => new
                    {
                        SendMessageQueueId = c.Int(nullable: false),
                        SendTime = c.DateTime(nullable: false),
                        Subject = c.String(),
                        Content = c.String(),
                        CreatedTime = c.DateTime(nullable: false),
                        CREDIT = c.Double(),
                        SENDED = c.Int(),
                        COST = c.Double(),
                        UNSEND = c.Int(),
                        BATCH_ID = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.SendMessageQueueId);
            
            CreateTable(
                "dbo.Every8d_DeliveryReport",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RequestId = c.String(),
                        CreatedTime = c.DateTime(nullable: false),
                        CODE = c.Int(nullable: false),
                        DESCRIPTION = c.String(),
                        NAME = c.String(),
                        MOBILE = c.String(),
                        SENT_TIME = c.String(),
                        COST = c.String(),
                        STATUS = c.String(),
                        SendMessageResult_SendMessageQueueId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DeliveryReportQueues", "SendMessageQueueId", c => c.Int(nullable: false));
            DropForeignKey("dbo.SendMessageHistories", "SendMessageRetryHistoryId", "dbo.SendMessageRetryHistories");
            DropIndex("dbo.SendMessageRetryHistories", new[] { "CreatedTime" });
            DropIndex("dbo.SendMessageHistories", new[] { "SendMessageRetryHistoryId" });
            DropColumn("dbo.SendMessageHistories", "SendMessageRetryHistoryId");
            DropColumn("dbo.SendMessageHistories", "RetryTotalTimes");
            DropColumn("dbo.SendMessageHistories", "RetryMaxTimes");
            DropColumn("dbo.DeliveryReportQueues", "SourceTable");
            DropColumn("dbo.DeliveryReportQueues", "SourceTableId");
            DropColumn("dbo.AspNetUsers", "SmsProviderType");
            DropTable("dbo.SendMessageRetryHistories");
            CreateIndex("dbo.Infobip_ResourceReference", "SendMessageResultId");
            CreateIndex("dbo.Infobip_SendMessageResult", "ClientCorrelator", unique: true);
            CreateIndex("dbo.Infobip_SendMessageResult", "SendMessageQueueId");
            CreateIndex("dbo.Infobip_SendMessageResultItem", "SendMessageResult_SendMessageQueueId");
            CreateIndex("dbo.Infobip_DeliveryReport", "MessageId");
            CreateIndex("dbo.Every8d_SendMessageResult", "BATCH_ID", unique: true);
            CreateIndex("dbo.Every8d_SendMessageResult", "SendMessageQueueId");
            CreateIndex("dbo.Every8d_DeliveryReport", "SendMessageResult_SendMessageQueueId");
            AddForeignKey("dbo.Infobip_DeliveryReport", "MessageId", "dbo.Infobip_SendMessageResultItem", "MessageId");
            AddForeignKey("dbo.Infobip_SendMessageResultItem", "SendMessageResult_SendMessageQueueId", "dbo.Infobip_SendMessageResult", "SendMessageQueueId");
            AddForeignKey("dbo.Infobip_SendMessageResult", "SendMessageQueueId", "dbo.SendMessageQueues", "Id");
            AddForeignKey("dbo.Infobip_ResourceReference", "SendMessageResultId", "dbo.Infobip_SendMessageResult", "SendMessageQueueId");
            AddForeignKey("dbo.Every8d_SendMessageResult", "SendMessageQueueId", "dbo.SendMessageQueues", "Id");
            AddForeignKey("dbo.Every8d_DeliveryReport", "SendMessageResult_SendMessageQueueId", "dbo.Every8d_SendMessageResult", "SendMessageQueueId");
        }
    }
}
