namespace EFunTech.Sms.Schema.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SendMessageRetryHistoryEmail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SendMessageRetryHistories", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SendMessageRetryHistories", "Email");
        }
    }
}
