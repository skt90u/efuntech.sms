namespace EFunTech.Sms.Schema.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RetrySMS_step02 : DbMigration
    {
        public override void Up()
        {
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
                        SendMessageResult_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Every8d_SendMessageResult", t => t.SendMessageResult_Id)
                .Index(t => t.SendMessageResult_Id);
            
            CreateTable(
                "dbo.Every8d_SendMessageResult",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceTable = c.Int(nullable: false),
                        SourceTableId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.Id)
                .Index(t => t.BATCH_ID, unique: true);
            
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
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.Infobip_SendMessageResultItem", t => t.MessageId)
                .Index(t => t.MessageId);
            
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
                        SendMessageResult_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.Infobip_SendMessageResult", t => t.SendMessageResult_Id)
                .Index(t => t.SendMessageResult_Id);
            
            CreateTable(
                "dbo.Infobip_SendMessageResult",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceTable = c.Int(nullable: false),
                        SourceTableId = c.Int(nullable: false),
                        ClientCorrelator = c.String(nullable: false, maxLength: 256),
                        CreatedTime = c.DateTime(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ClientCorrelator, unique: true);
            
            CreateTable(
                "dbo.Infobip_ResourceReference",
                c => new
                    {
                        SendMessageResultId = c.Int(nullable: false),
                        ResourceURL = c.String(),
                    })
                .PrimaryKey(t => t.SendMessageResultId)
                .ForeignKey("dbo.Infobip_SendMessageResult", t => t.SendMessageResultId)
                .Index(t => t.SendMessageResultId);
            
            //CreateTable(
            //    "dbo.SendMessageQueues",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            SendMessageType = c.Int(nullable: false),
            //            SendTime = c.DateTime(nullable: false),
            //            SendTitle = c.String(),
            //            SendBody = c.String(nullable: false),
            //            SendCustType = c.Int(nullable: false),
            //            TotalReceiverCount = c.Int(nullable: false),
            //            TotalMessageCost = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            SendMessageRuleId = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Infobip_DeliveryReport", "MessageId", "dbo.Infobip_SendMessageResultItem");
            DropForeignKey("dbo.Infobip_SendMessageResultItem", "SendMessageResult_Id", "dbo.Infobip_SendMessageResult");
            DropForeignKey("dbo.Infobip_ResourceReference", "SendMessageResultId", "dbo.Infobip_SendMessageResult");
            DropForeignKey("dbo.Every8d_DeliveryReport", "SendMessageResult_Id", "dbo.Every8d_SendMessageResult");
            DropIndex("dbo.Infobip_ResourceReference", new[] { "SendMessageResultId" });
            DropIndex("dbo.Infobip_SendMessageResult", new[] { "ClientCorrelator" });
            DropIndex("dbo.Infobip_SendMessageResultItem", new[] { "SendMessageResult_Id" });
            DropIndex("dbo.Infobip_DeliveryReport", new[] { "MessageId" });
            DropIndex("dbo.Every8d_SendMessageResult", new[] { "BATCH_ID" });
            DropIndex("dbo.Every8d_DeliveryReport", new[] { "SendMessageResult_Id" });
            DropTable("dbo.SendMessageQueues");
            DropTable("dbo.Infobip_ResourceReference");
            DropTable("dbo.Infobip_SendMessageResult");
            DropTable("dbo.Infobip_SendMessageResultItem");
            DropTable("dbo.Infobip_DeliveryReport");
            DropTable("dbo.Every8d_SendMessageResult");
            DropTable("dbo.Every8d_DeliveryReport");
        }
    }
}
