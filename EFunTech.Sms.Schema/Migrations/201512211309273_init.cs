namespace EFunTech.Sms.Schema.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Blacklists", name: "CreatedUser_Id", newName: "CreatedUserId");
            RenameColumn(table: "dbo.Blacklists", name: "UploadedFile_Id", newName: "UploadedFileId");
            RenameColumn(table: "dbo.CommonMessages", name: "CreatedUser_Id", newName: "CreatedUserId");
            RenameColumn(table: "dbo.Contacts", name: "CreatedUser_Id", newName: "CreatedUserId");
            RenameColumn(table: "dbo.SendMessageRules", name: "CreatedUser_Id", newName: "CreatedUserId");
            RenameColumn(table: "dbo.Signatures", name: "CreatedUser_Id", newName: "CreatedUserId");
            RenameColumn(table: "dbo.UploadedFiles", name: "CreatedUser_Id", newName: "CreatedUserId");
            RenameColumn(table: "dbo.Departments", name: "CreatedUser_Id", newName: "CreatedUserId");
            RenameColumn(table: "dbo.MessageReceivers", name: "CreatedUser_Id", newName: "CreatedUserId");
            RenameColumn(table: "dbo.SystemAnnouncements", name: "CreatedUser_Id", newName: "CreatedUserId");
            RenameColumn(table: "dbo.UploadedMessageReceivers", name: "CreatedUser_Id", newName: "CreatedUserId");
            RenameColumn(table: "dbo.UploadedMessageReceivers", name: "UploadedFile_Id", newName: "UploadedFileId");
            RenameIndex(table: "dbo.Blacklists", name: "IX_CreatedUser_Id", newName: "IX_Blacklist_CreatedUserId");
            RenameIndex(table: "dbo.Blacklists", name: "IX_UploadedFile_Id", newName: "IX_UploadedFileId");
            RenameIndex(table: "dbo.CommonMessages", name: "IX_CreatedUser_Id", newName: "IX_CommonMessage_CreatedUserId");
            RenameIndex(table: "dbo.Contacts", name: "IX_CreatedUser_Id", newName: "IX_Contact_CreatedUserId");
            RenameIndex(table: "dbo.Departments", name: "IX_CreatedUser_Id", newName: "IX_Department_CreatedUserId");
            RenameIndex(table: "dbo.SendMessageRules", name: "IX_CreatedUser_Id", newName: "IX_SendMessageRule_CreatedUserId");
            RenameIndex(table: "dbo.UploadedFiles", name: "IX_CreatedUser_Id", newName: "IX_UploadedFile_CreatedUserId");
            RenameIndex(table: "dbo.Signatures", name: "IX_CreatedUser_Id", newName: "IX_CommonMessage_CreatedUserId");
            RenameIndex(table: "dbo.MessageReceivers", name: "IX_CreatedUser_Id", newName: "IX_MessageReceiver_CreatedUserId");
            RenameIndex(table: "dbo.SystemAnnouncements", name: "IX_CreatedUser_Id", newName: "IX_SystemAnnouncement_CreatedUserId");
            RenameIndex(table: "dbo.UploadedMessageReceivers", name: "IX_CreatedUser_Id", newName: "IX_UploadedMessageReceiver_CreatedUserId");
            RenameIndex(table: "dbo.UploadedMessageReceivers", name: "IX_UploadedFile_Id", newName: "IX_UploadedFileId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.UploadedMessageReceivers", name: "IX_UploadedFileId", newName: "IX_UploadedFile_Id");
            RenameIndex(table: "dbo.UploadedMessageReceivers", name: "IX_UploadedMessageReceiver_CreatedUserId", newName: "IX_CreatedUser_Id");
            RenameIndex(table: "dbo.SystemAnnouncements", name: "IX_SystemAnnouncement_CreatedUserId", newName: "IX_CreatedUser_Id");
            RenameIndex(table: "dbo.MessageReceivers", name: "IX_MessageReceiver_CreatedUserId", newName: "IX_CreatedUser_Id");
            RenameIndex(table: "dbo.Signatures", name: "IX_CommonMessage_CreatedUserId", newName: "IX_CreatedUser_Id");
            RenameIndex(table: "dbo.UploadedFiles", name: "IX_UploadedFile_CreatedUserId", newName: "IX_CreatedUser_Id");
            RenameIndex(table: "dbo.SendMessageRules", name: "IX_SendMessageRule_CreatedUserId", newName: "IX_CreatedUser_Id");
            RenameIndex(table: "dbo.Departments", name: "IX_Department_CreatedUserId", newName: "IX_CreatedUser_Id");
            RenameIndex(table: "dbo.Contacts", name: "IX_Contact_CreatedUserId", newName: "IX_CreatedUser_Id");
            RenameIndex(table: "dbo.CommonMessages", name: "IX_CommonMessage_CreatedUserId", newName: "IX_CreatedUser_Id");
            RenameIndex(table: "dbo.Blacklists", name: "IX_UploadedFileId", newName: "IX_UploadedFile_Id");
            RenameIndex(table: "dbo.Blacklists", name: "IX_Blacklist_CreatedUserId", newName: "IX_CreatedUser_Id");
            RenameColumn(table: "dbo.UploadedMessageReceivers", name: "UploadedFileId", newName: "UploadedFile_Id");
            RenameColumn(table: "dbo.UploadedMessageReceivers", name: "CreatedUserId", newName: "CreatedUser_Id");
            RenameColumn(table: "dbo.SystemAnnouncements", name: "CreatedUserId", newName: "CreatedUser_Id");
            RenameColumn(table: "dbo.MessageReceivers", name: "CreatedUserId", newName: "CreatedUser_Id");
            RenameColumn(table: "dbo.Departments", name: "CreatedUserId", newName: "CreatedUser_Id");
            RenameColumn(table: "dbo.UploadedFiles", name: "CreatedUserId", newName: "CreatedUser_Id");
            RenameColumn(table: "dbo.Signatures", name: "CreatedUserId", newName: "CreatedUser_Id");
            RenameColumn(table: "dbo.SendMessageRules", name: "CreatedUserId", newName: "CreatedUser_Id");
            RenameColumn(table: "dbo.Contacts", name: "CreatedUserId", newName: "CreatedUser_Id");
            RenameColumn(table: "dbo.CommonMessages", name: "CreatedUserId", newName: "CreatedUser_Id");
            RenameColumn(table: "dbo.Blacklists", name: "UploadedFileId", newName: "UploadedFile_Id");
            RenameColumn(table: "dbo.Blacklists", name: "CreatedUserId", newName: "CreatedUser_Id");
        }
    }
}
