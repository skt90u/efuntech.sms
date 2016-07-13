namespace EFunTech.Sms.Schema.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20160713 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Infobip_SendMessageResultItem", "Email", c => c.String());
            AddColumn("dbo.SendMessageHistories", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SendMessageHistories", "Email");
            DropColumn("dbo.Infobip_SendMessageResultItem", "Email");
        }
    }
}
