-- SELECT * FROM SendMessageRules Order By Id DESC

-- SELECT * FROM RecipientFromFileUploads  ORDER BY SendMessageRuleId DESC
--SELECT * FROM RecipientFromCommonContacts ORDER BY SendMessageRuleId DESC
--SELECT * FROM RecipientFromGroupContacts ORDER BY SendMessageRuleId DESC
--SELECT * FROM RecipientFromManualInputs ORDER BY SendMessageRuleId DESC

-- UPDATE RecipientFromGroupContacts SET ContactIds = '' 

-- SELECT * FROM SendDelivers

--SELECT * FROM SendCycleEveryDays
--SELECT * FROM SendCycleEveryWeeks
--SELECT * FROM SendCycleEveryMonths
SELECT * FROM SendCycleEveryYears
