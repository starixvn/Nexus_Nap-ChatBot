CREATE PROCEDURE `NCB_ChatGroupMapping_Sel`()
BEGIN
	select bcm.Name as BotName, bc.ConversationID, bc.Name as GroupName, 1 as MappingStatus, null as RecipientTypeID 
	from BotConversation bc
	inner join BotCoversationMember bcm on bc.ConversationID = bcm.ConversationID
	where bcm.IsActive = 1 
		and bcm.Name is not null 
		and bc.ConversationId not in (select ConversationID from Conversation)
		
	UNION 
	
	select BotName, ConversationID, GroupName, MappingStatus, RecipientTypeID
	from Conversation;
END