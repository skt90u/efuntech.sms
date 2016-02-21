(0) 備份資料庫

(1)
SELECT * INTO Backup_Infobip_DeliveryReport FROM Infobip_DeliveryReport
GO

SELECT * INTO Backup_Infobip_ResourceReference FROM Infobip_ResourceReference
GO

SELECT * INTO Backup_Infobip_SendMessageResult FROM Infobip_SendMessageResult
GO

SELECT * INTO Backup_Infobip_SendMessageResultItem FROM Infobip_SendMessageResultItem
GO

SELECT * INTO Backup_Every8d_DeliveryReport FROM Every8d_DeliveryReport
GO

SELECT * INTO Backup_Every8d_SendMessageResult FROM Every8d_SendMessageResult
GO

SELECT * INTO Backup_SendMessageQueues FROM SendMessageQueues
GO

(2) EntityFramework Migration 01 (刪除)
        // sms: infobip
        // public DbSet<Infobip_SendMessageResult> Infobip_SendMessageResults { get; set; }
        // public DbSet<Infobip_ResourceReference> Infobip_ResourceReferences { get; set; }
        // public DbSet<Infobip_SendMessageResultItem> Infobip_SendMessageResultItems { get; set; }
        // public DbSet<Infobip_DeliveryReport> Infobip_DeliveryReports { get; set; }
		
		// sms: every8d
        //public DbSet<Every8d_DeliveryReport> Every8d_DeliveryReports { get; set; }
        //public DbSet<Every8d_SendMessageResult> Every8d_SendMessageResults { get; set; }		
		
		//public DbSet<SendMessageQueue> SendMessageQueues { get; set; }
		
****************************************************
VERY IMPORTANT: 確保 Ef Migration 沒有刪除 SendMessageQueues

//AddColumn("dbo.DeliveryReportQueues", "SourceTableId", c => c.Int(nullable: false));
//DropColumn("dbo.DeliveryReportQueues", "SendMessageQueueId");
RenameColumn("dbo.DeliveryReportQueues", "SendMessageQueueId", "SourceTableId");
			
//DropTable("dbo.SendMessageQueues");
****************************************************
		
(3) EntityFramework Migration 02 (重建)
        // sms: infobip
         public DbSet<Infobip_SendMessageResult> Infobip_SendMessageResults { get; set; }
         public DbSet<Infobip_ResourceReference> Infobip_ResourceReferences { get; set; }
         public DbSet<Infobip_SendMessageResultItem> Infobip_SendMessageResultItems { get; set; }
         public DbSet<Infobip_DeliveryReport> Infobip_DeliveryReports { get; set; }		
		 
		// sms: every8d
          public DbSet<Every8d_DeliveryReport> Every8d_DeliveryReports { get; set; }
          public DbSet<Every8d_SendMessageResult> Every8d_SendMessageResults { get; set; }				 
		  
		  public DbSet<SendMessageQueue> SendMessageQueues { get; set; }
(4) EntityFramework Migration 03 (執行以下sql)		 

SET IDENTITY_INSERT Infobip_SendMessageResult ON
GO

-- Infobip_SendMessageResult
INSERT INTO Infobip_SendMessageResult(Id, SourceTable, SourceTableId, ClientCorrelator, CreatedTime, Balance)
SELECT 
	SendMessageQueueId as Id,
	0 as SourceTable,
	SendMessageQueueId as SourceTableId,
	ClientCorrelator,
	CONVERT(DATETIME, CreatedTime) as CreatedTime,
	Balance
FROM 
	Backup_Infobip_SendMessageResult
GO

-- Infobip_ResourceReference
INSERT INTO Infobip_ResourceReference(SendMessageResultId, ResourceURL)
SELECT SendMessageResultId, ResourceURL FROM Backup_Infobip_ResourceReference
GO
	
-- Infobip_SendMessageResultItem	
INSERT INTO Infobip_SendMessageResultItem(MessageId, MessageStatus, MessageStatusString, SenderAddress, DestinationAddress, DestinationName, SendMessageResult_Id) 
SELECT 
	   MessageId
      ,MessageStatus
      ,MessageStatusString
      ,SenderAddress
      ,DestinationAddress
      ,DestinationName
      ,SendMessageResult_SendMessageQueueId AS SendMessageResult_Id
FROM 
	Backup_Infobip_SendMessageResultItem
GO
	
-- Infobip_DeliveryReport
INSERT INTO Infobip_DeliveryReport(MessageId, SentDate, DoneDate, StatusString, Status, Price, CreatedTime, RequestId)
SELECT 
	   MessageId
      ,SentDate
      ,DoneDate
      ,StatusString
      ,Status
      ,Price
      ,CreatedTime
      ,RequestId
FROM 
	Backup_Infobip_DeliveryReport
GO

SET IDENTITY_INSERT Infobip_SendMessageResult OFF
GO

update SendMessageHistories set ProviderName = 'InfobipNormalQuality' where ProviderName = 'InfobipSmsProvider'
GO
update SendMessageHistories set ProviderName = 'Every8d' where ProviderName = 'Every8dSmsProvider'
GO

update SendMessageStatistics set ProviderName = 'InfobipNormalQuality' where ProviderName = 'InfobipSmsProvider'
GO
update SendMessageStatistics set ProviderName = 'Every8d' where ProviderName = 'Every8dSmsProvider'
GO

update DeliveryReportQueues set ProviderName = 'InfobipNormalQuality' where ProviderName = 'InfobipSmsProvider'
GO
update DeliveryReportQueues set ProviderName = 'Every8d' where ProviderName = 'Every8dSmsProvider'
GO
