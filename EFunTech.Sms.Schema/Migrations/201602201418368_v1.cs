namespace EFunTech.Sms.Schema.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Infobip_SendMessageResult", "SendMessageQueueId", "dbo.SendMessageQueues");
            DropForeignKey("dbo.Infobip_ResourceReference", "SendMessageResultId", "dbo.Infobip_SendMessageResult");
            DropForeignKey("dbo.Infobip_SendMessageResultItem", "SendMessageResultId", "dbo.Infobip_SendMessageResult");
            DropIndex("dbo.Infobip_SendMessageResult", new[] { "SendMessageQueueId" });
            RenameColumn(table: "dbo.Infobip_SendMessageResultItem", name: "SendMessageResult_SendMessageQueueId", newName: "SendMessageResultId");
            RenameIndex(table: "dbo.Infobip_SendMessageResultItem", name: "IX_SendMessageResult_SendMessageQueueId", newName: "IX_SendMessageResultId");
            DropPrimaryKey("dbo.Infobip_SendMessageResult");
            AddColumn("dbo.DeliveryReportQueues", "SourceTableId", c => c.Int(nullable: false));
            AddColumn("dbo.DeliveryReportQueues", "SourceTable", c => c.Int(nullable: false));
            AddColumn("dbo.Infobip_SendMessageResult", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Infobip_SendMessageResult", "SourceTable", c => c.Int(nullable: false));
            AddColumn("dbo.Infobip_SendMessageResult", "SourceTableId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Infobip_SendMessageResult", "Id");
            AddForeignKey("dbo.Infobip_ResourceReference", "SendMessageResultId", "dbo.Infobip_SendMessageResult", "Id");
            AddForeignKey("dbo.Infobip_SendMessageResultItem", "SendMessageResultId", "dbo.Infobip_SendMessageResult", "Id");
            DropColumn("dbo.DeliveryReportQueues", "SendMessageQueueId");
            DropColumn("dbo.Infobip_SendMessageResult", "SendMessageQueueId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Infobip_SendMessageResult", "SendMessageQueueId", c => c.Int(nullable: false));
            AddColumn("dbo.DeliveryReportQueues", "SendMessageQueueId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Infobip_SendMessageResultItem", "SendMessageResultId", "dbo.Infobip_SendMessageResult");
            DropForeignKey("dbo.Infobip_ResourceReference", "SendMessageResultId", "dbo.Infobip_SendMessageResult");
            DropPrimaryKey("dbo.Infobip_SendMessageResult");
            DropColumn("dbo.Infobip_SendMessageResult", "SourceTableId");
            DropColumn("dbo.Infobip_SendMessageResult", "SourceTable");
            DropColumn("dbo.Infobip_SendMessageResult", "Id");
            DropColumn("dbo.DeliveryReportQueues", "SourceTable");
            DropColumn("dbo.DeliveryReportQueues", "SourceTableId");
            AddPrimaryKey("dbo.Infobip_SendMessageResult", "SendMessageQueueId");
            RenameIndex(table: "dbo.Infobip_SendMessageResultItem", name: "IX_SendMessageResultId", newName: "IX_SendMessageResult_SendMessageQueueId");
            RenameColumn(table: "dbo.Infobip_SendMessageResultItem", name: "SendMessageResultId", newName: "SendMessageResult_SendMessageQueueId");
            CreateIndex("dbo.Infobip_SendMessageResult", "SendMessageQueueId");
            AddForeignKey("dbo.Infobip_SendMessageResultItem", "SendMessageResultId", "dbo.Infobip_SendMessageResult", "Id");
            AddForeignKey("dbo.Infobip_ResourceReference", "SendMessageResultId", "dbo.Infobip_SendMessageResult", "Id");
            AddForeignKey("dbo.Infobip_SendMessageResult", "SendMessageQueueId", "dbo.SendMessageQueues", "Id");
        }
    }
}
