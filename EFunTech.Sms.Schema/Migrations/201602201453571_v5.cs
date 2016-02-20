namespace EFunTech.Sms.Schema.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Infobip_ResourceReference", "SendMessageResultId", "dbo.Infobip_SendMessageResult");
            DropForeignKey("dbo.Infobip_SendMessageResultItem", "SendMessageResult_Id", "dbo.Infobip_SendMessageResult");
            DropForeignKey("dbo.Infobip_DeliveryReport", "MessageId", "dbo.Infobip_SendMessageResultItem");
            DropIndex("dbo.Infobip_DeliveryReport", new[] { "MessageId" });
            DropIndex("dbo.Infobip_SendMessageResultItem", new[] { "SendMessageResult_Id" });
            DropIndex("dbo.Infobip_SendMessageResult", new[] { "ClientCorrelator" });
            DropIndex("dbo.Infobip_ResourceReference", new[] { "SendMessageResultId" });
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
                        Id = c.Int(nullable: false, identity: true),
                        SourceTable = c.Int(nullable: false),
                        SourceTableId = c.Int(nullable: false),
                        ClientCorrelator = c.String(nullable: false, maxLength: 256),
                        CreatedTime = c.DateTime(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CopyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
            
            CreateIndex("dbo.Infobip_ResourceReference", "SendMessageResultId");
            CreateIndex("dbo.Infobip_SendMessageResult", "ClientCorrelator", unique: true);
            CreateIndex("dbo.Infobip_SendMessageResultItem", "SendMessageResult_Id");
            CreateIndex("dbo.Infobip_DeliveryReport", "MessageId");
            AddForeignKey("dbo.Infobip_DeliveryReport", "MessageId", "dbo.Infobip_SendMessageResultItem", "MessageId");
            AddForeignKey("dbo.Infobip_SendMessageResultItem", "SendMessageResult_Id", "dbo.Infobip_SendMessageResult", "Id");
            AddForeignKey("dbo.Infobip_ResourceReference", "SendMessageResultId", "dbo.Infobip_SendMessageResult", "Id");
        }
    }
}
