namespace EFunTech.Sms.Schema.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20160219 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SendMessageHistories", "SendMessageQueueId", c => c.Int(nullable: false));
            DropColumn("dbo.SendMessageHistories", "SourceTableType");
            DropColumn("dbo.SendMessageHistories", "SourceTableId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SendMessageHistories", "SourceTableId", c => c.Int(nullable: false));
            AddColumn("dbo.SendMessageHistories", "SourceTableType", c => c.Int(nullable: false));
            DropColumn("dbo.SendMessageHistories", "SendMessageQueueId");
            
        }
    }
}
