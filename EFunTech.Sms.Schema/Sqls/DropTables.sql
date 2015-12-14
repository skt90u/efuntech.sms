drop table sendmessagerules
drop table sharedcontactgroups;
drop table uploadedfiles;
drop table signatures;
drop table contactgroupmaps;
drop table blacklists;
drop table contacts;
drop table commonmessages;
drop table contactgroups;
drop table users;
drop table departments;
drop table sendsmsresults
drop table sendmessageresults
drop table resourcereferences
drop table deliveryreports

drop table __migrationhistory

/*
ALTER DATABASE EFunTechSms SET SINGLE_USER WITH ROLLBACK IMMEDIATE 
GO 
SP_RENAMEDB 'EFunTechSms','EFunTechSms_NEW'
Go 
ALTER DATABASE EFunTechSms  SET MULTI_USER -- set back to multi user 
GO 
*/