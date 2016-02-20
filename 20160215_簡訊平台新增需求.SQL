http://www.w3schools.com/sql/sql_foreignkey.asp

EXEC sp_fkeys 'Infobip_SendMessageResultItem' -- FK_NAME

ALTER TABLE Infobip_SendMessageResultItem DROP CONSTRAINT FK_dbo.Infobip_DeliveryReport_dbo.Infobip_SendMessageResultItem_MessageId

(1)
SELECT * INTO Backup_Infobip_DeliveryReport FROM Infobip_DeliveryReport
SELECT * INTO Backup_Infobip_ResourceReference FROM Infobip_ResourceReference
SELECT * INTO Backup_Infobip_SendMessageResult FROM Infobip_SendMessageResult
SELECT * INTO Backup_Infobip_SendMessageResultItem FROM Infobip_SendMessageResultItem

(2)
update SendMessageHistories set ProviderName = 'InfobipNormalQuality' where ProviderName = 'InfobipSmsProvider'
update SendMessageHistories set ProviderName = 'Every8d' where ProviderName = 'Every8dSmsProvider'

update SendMessageStatistics set ProviderName = 'InfobipNormalQuality' where ProviderName = 'InfobipSmsProvider'
update SendMessageStatistics set ProviderName = 'Every8d' where ProviderName = 'Every8dSmsProvider'

update DeliveryReportQueues set ProviderName = 'InfobipNormalQuality' where ProviderName = 'InfobipSmsProvider'
update DeliveryReportQueues set ProviderName = 'Every8d' where ProviderName = 'Every8dSmsProvider'

(3) EntityFramework Migration 01 (刪除)
        // sms: infobip
        // public DbSet<Infobip_SendMessageResult> Infobip_SendMessageResults { get; set; }
        // public DbSet<Infobip_ResourceReference> Infobip_ResourceReferences { get; set; }
        // public DbSet<Infobip_SendMessageResultItem> Infobip_SendMessageResultItems { get; set; }

        // public DbSet<Infobip_DeliveryReport> Infobip_DeliveryReports { get; set; }
		
(4) EntityFramework Migration 02 (重建)
        // sms: infobip
         public DbSet<Infobip_SendMessageResult> Infobip_SendMessageResults { get; set; }
         public DbSet<Infobip_ResourceReference> Infobip_ResourceReferences { get; set; }
         public DbSet<Infobip_SendMessageResultItem> Infobip_SendMessageResultItems { get; set; }

         public DbSet<Infobip_DeliveryReport> Infobip_DeliveryReports { get; set; }		
		 
(5) TODO: EntityFramework Migration 03 (執行以下sql)		 

SET IDENTITY_INSERT Infobip_SendMessageResult ON

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

-- Infobip_ResourceReference
INSERT INTO Infobip_ResourceReference(SendMessageResultId, ResourceURL)
SELECT SendMessageResultId, ResourceURL FROM Backup_Infobip_ResourceReference
	
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

SET IDENTITY_INSERT Infobip_SendMessageResult OFF