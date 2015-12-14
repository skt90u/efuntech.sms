-- 93cbe58c-b5b4-47cd-8e11-3d1084a3be94
-- 37b72bba-ea25-49fb-aacf-03574c514941
-- a356c6b1-baed-46bd-81b2-62b4bbdc336d
-- 8411c49e-9239-46d5-b34c-d401c520391b
-- select distinct CreatedUserId from SendMessageHistories 

-- 8325
-- select count(1) from SendMessageHistories 

-- 2797
-- select count(1) from SendMessageHistories where CreatedUserId = 'a356c6b1-baed-46bd-81b2-62b4bbdc336d'

-- select distinct Every8d_StatusString from SendMessageHistories where CreatedUserId = 'a356c6b1-baed-46bd-81b2-62b4bbdc336d'

編號	
	
	
	
	
	
成功接收	
傳送中通數	
預約傳送通數	
預約取消通數	
逾期收訊	
回覆通數	
發送扣點

select 
	SendMessageQueueId, 
	SendMessageType AS 簡訊類別,
	SendTime AS 發送時間,
	SendTitle AS 簡訊類別描述,
	SendBody AS 發送內容,
	COUNT(1) AS 發送通數,
	SUM(MessageCost) AS 發送扣點,
	COUNT(CASE WHEN (Infobip_Delivered = '1' OR Infobip_Delivered = '1') THEN 1 END) AS Infobip發送成功,
	COUNT(CASE WHEN Infobip_Delivered = '0' THEN 1 END) AS Infobip發送失敗,
	COUNT(CASE WHEN (MessageStatusString = 'MessageAccepted') OR 
	                (MessageStatusString = 'MessageSent') OR  
					(MessageStatusString = 'MessageWaitingForDelivery') OR 
					(MessageStatusString = 'MessageDelivered')
		  THEN 1 END) AS InfobipBookingOk,
	COUNT(CASE WHEN (MessageStatusString = 'Unknown') OR 
	                (MessageStatusString = 'MessageNotSent') OR  
					(MessageStatusString = 'MessageNotDelivered') OR 
					(MessageStatusString = 'NetworkNotAllowed') OR 
					(MessageStatusString = 'NetworkNotAvailable') OR 
					(MessageStatusString = 'InvalidDestinationAddress') OR 
					(MessageStatusString = 'MessageDeliveryUnknown') OR 
					(MessageStatusString = 'RouteNotAvailable') OR 
					(MessageStatusString = 'InvalidSourceAddress') OR 
					(MessageStatusString = 'NotEnoughCredits') OR 
					(MessageStatusString = 'MessageRejected') OR 
					(MessageStatusString = 'MessageExpired') OR 
					(MessageStatusString = 'SystemError')
		  THEN 1 END) AS InfobipBookingError,
	COUNT(CASE WHEN (StatusString = 'DeliveredToTerminal') OR
	                (StatusString = 'MessageWaiting') OR  
	                (StatusString = 'DeliveredToNetwork')
		  THEN 1 END) AS InfobipDeliveryOK,
	COUNT(CASE WHEN (StatusString = 'Unknown') OR 
	                (StatusString = 'DeliveryUncertain') OR  
	                (StatusString = 'DeliveryImpossible')
		  THEN 1 END) AS InfobipDeliveryError,
	SUM(MessageCost) AS 發送扣點

--	SUM(case when Infobip_Delivered = '0' then valor end) as OK,
--    SUM(case when Infobip_Delivered = '1' then valor end) as KO,
	
FROM SendMessageHistories WHERE 
	CreatedUserId = 'a356c6b1-baed-46bd-81b2-62b4bbdc336d'
	-- AND SendTime (依發送時間查詢)
	-- AND DestinationAddress (依接收門號查詢)
	-- AND STATUS (依接收狀態查詢)
GROUP BY 
	SendMessageQueueId, 
	SendMessageType,
	SendTime,
	SendTitle,
	SendBody

/*	
成功接收	
傳送中通數	
預約傳送通數	
預約取消通數	
逾期收訊	
回覆通數	
發送扣點

發送通數
INFOBIP發送成功
INFOBIP發送失敗
*/
