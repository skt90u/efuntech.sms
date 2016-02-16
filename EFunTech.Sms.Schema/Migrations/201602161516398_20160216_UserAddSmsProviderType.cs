namespace EFunTech.Sms.Schema.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20160216_UserAddSmsProviderType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "SmsProviderType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "SmsProviderType");
        }
    }
}
