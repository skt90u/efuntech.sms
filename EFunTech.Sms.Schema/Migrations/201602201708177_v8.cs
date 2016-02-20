namespace EFunTech.Sms.Schema.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v8 : DbMigration
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Every8d_DeliveryReport", "SendMessageResult_Id", "dbo.Every8d_SendMessageResult");
            DropIndex("dbo.Every8d_SendMessageResult", new[] { "BATCH_ID" });
            DropIndex("dbo.Every8d_DeliveryReport", new[] { "SendMessageResult_Id" });
            DropTable("dbo.Every8d_SendMessageResult");
            DropTable("dbo.Every8d_DeliveryReport");
        }
    }
}
