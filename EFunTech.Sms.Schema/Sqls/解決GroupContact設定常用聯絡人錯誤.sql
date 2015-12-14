select
	WrongGroupContacts.WrongGroupId,
	WrongGroupContacts.ContactId,
	WrongGroupContacts.CreatedUser_Id,
	Groups.Id as CorrectGroupId
into #temp
from
(
	select 
		GroupContacts.GroupId As WrongGroupId,
		GroupContacts.ContactId,
		Contacts.CreatedUser_Id
	from  
		GroupContacts 
		inner join Contacts on Contacts.Id = GroupContacts.ContactId
	where 
		GroupContacts.GroupId = 1
) as WrongGroupContacts
	inner join Groups on Groups.Name = N'常用聯絡人' and Groups.CreatedUserId = WrongGroupContacts.CreatedUser_Id

update GroupContacts
set
	GroupContacts.GroupId = #temp.CorrectGroupId
from
	#temp
where
	#temp.WrongGroupId <> CorrectGroupId
	and #temp.WrongGroupId = GroupContacts.GroupId
	and #temp.ContactId = GroupContacts.ContactId