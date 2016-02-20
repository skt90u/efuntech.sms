(1)
update Infobip_SendMessageResult set CopyId = SendMessageQueueId

(2)
update SendMessageHistories set ProviderName = 'InfobipNormalQuality' where ProviderName = 'InfobipSmsProvider'
update SendMessageHistories set ProviderName = 'Every8d' where ProviderName = 'Every8dSmsProvider'

update SendMessageStatistics set ProviderName = 'InfobipNormalQuality' where ProviderName = 'InfobipSmsProvider'
update SendMessageStatistics set ProviderName = 'Every8d' where ProviderName = 'Every8dSmsProvider'

update DeliveryReportQueues set ProviderName = 'InfobipNormalQuality' where ProviderName = 'InfobipSmsProvider'
update DeliveryReportQueues set ProviderName = 'Every8d' where ProviderName = 'Every8dSmsProvider'



(3)
SELECT * INTO Backup_Infobip_DeliveryReport FROM Infobip_DeliveryReport
SELECT * INTO Backup_Infobip_ResourceReference FROM Infobip_ResourceReference
SELECT * INTO Backup_Infobip_SendMessageResult FROM Infobip_SendMessageResult
SELECT * INTO Backup_Infobip_SendMessageResultItem FROM Infobip_SendMessageResultItem

(4)
DROP TABLE Infobip_DeliveryReport;
DROP TABLE Infobip_ResourceReference;
DROP TABLE Infobip_SendMessageResultItem;
DROP TABLE Infobip_SendMessageResult;

(5) EntityFramework Migration