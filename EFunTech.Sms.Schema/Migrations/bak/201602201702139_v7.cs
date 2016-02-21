namespace EFunTech.Sms.Schema.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v7 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Every8d_DeliveryReport", "SendMessageResult_SendMessageQueueId", "dbo.Every8d_SendMessageResult");
            DropForeignKey("dbo.Every8d_SendMessageResult", "SendMessageQueueId", "dbo.SendMessageQueues");
            DropIndex("dbo.Every8d_DeliveryReport", new[] { "SendMessageResult_SendMessageQueueId" });
            DropIndex("dbo.Every8d_SendMessageResult", new[] { "SendMessageQueueId" });
            DropIndex("dbo.Every8d_SendMessageResult", new[] { "BATCH_ID" });
            DropTable("dbo.Every8d_DeliveryReport");
            DropTable("dbo.Every8d_SendMessageResult");
            //DropTable("dbo.SendMessageQueues");
        }
        
        public override void Down()
        {
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
            
            CreateIndex("dbo.Every8d_SendMessageResult", "BATCH_ID", unique: true);
            CreateIndex("dbo.Every8d_SendMessageResult", "SendMessageQueueId");
            CreateIndex("dbo.Every8d_DeliveryReport", "SendMessageResult_SendMessageQueueId");
            AddForeignKey("dbo.Every8d_SendMessageResult", "SendMessageQueueId", "dbo.SendMessageQueues", "Id");
            AddForeignKey("dbo.Every8d_DeliveryReport", "SendMessageResult_SendMessageQueueId", "dbo.Every8d_SendMessageResult", "SendMessageQueueId");
        }
    }
}
