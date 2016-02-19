update SendMessageHistories set ProviderName = 'InfobipNormalQuality' where ProviderName = 'InfobipSmsProvider'
update SendMessageHistories set ProviderName = 'Every8d' where ProviderName = 'Every8dSmsProvider'

update SendMessageStatistics set ProviderName = 'InfobipNormalQuality' where ProviderName = 'InfobipSmsProvider'
update SendMessageStatistics set ProviderName = 'Every8d' where ProviderName = 'Every8dSmsProvider'

update DeliveryReportQueues set ProviderName = 'InfobipNormalQuality' where ProviderName = 'InfobipSmsProvider'
update DeliveryReportQueues set ProviderName = 'Every8d' where ProviderName = 'Every8dSmsProvider'