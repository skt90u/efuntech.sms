namespace EFunTech.Sms.Schema.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.MenuItems", name: "WebAuthorization_Id", newName: "WebAuthorizationId");
            RenameIndex(table: "dbo.MenuItems", name: "IX_WebAuthorization_Id", newName: "IX_MenuItem_WebAuthorizationId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.MenuItems", name: "IX_MenuItem_WebAuthorizationId", newName: "IX_WebAuthorization_Id");
            RenameColumn(table: "dbo.MenuItems", name: "WebAuthorizationId", newName: "WebAuthorization_Id");
        }
    }
}
