CREATE DEFINER=`scofield.huynh`@`%` PROCEDURE `NCB_ChatGroupMapping_Sel`(IN pageIndex INT, IN pageSize INT)
BEGIN
	DECLARE rowStart INT;
    SELECT 	pageSize * (pageIndex - 1) into rowStart;     
    
    create temporary table AllBotConversation
    select bc.GroupID
    from BotConversation bc
    inner join BotConversationMember  bcm on bc.ConversationID = bcm.ConversationID
    left join Conversation c on bc.GroupID = c.GroupID
    left join ConversationRecipient cr on bc.GroupID = cr.GroupID
    left join Recipient r on cr.RecipientId = r.RecipientId
    where (botNames = '' or find_in_set(bcm.Name, botNames) > 0)
		and (recipientTypeIds = '' or find_in_set(c.RecipientTypeID, recipientTypeIds) > 0)
        and (recipientName = '' or r.SourceName = recipientName)
        and (mappingStatusIds = '' or find_in_set(case when bcm.IsBot = 1 and c.GroupID is null then 0
										when bcm.IsBot = 1 and c.GroupID is not null then 1
                                        else 2 end, mappingStatusIds) > 0)
    group by bc.GroupID;    
    
    create temporary table PagedBotConversation
    select bc.* 
    from AllBotConversation abc 
    inner join BotConversation bc on abc.GroupID = bc.GroupID
    order by bc.Name
    LIMIT rowStart, pageSize;
    
    select count(*) as Total from AllBotConversation;    
    
    select pbc.Name as GroupName, pbc.GroupID, pbc.ConversationID, c.Remark, c.RecipientTypeId, rt.RecipientTypeName,
		case when c.GroupID is not null then 1 else 0 end as IsMapped
    from PagedBotConversation pbc
    left join Conversation c on c.GroupID = pbc.GroupID
    left join RecipientType rt on rt.RecipientTypeID = c.RecipientTypeID;    
    
    select bcm.Name as BotName, bcm.ConversationID, bcm.IsActive
    from BotConversationMember bcm
    inner join PagedBotConversation pbc on bcm.ConversationID = pbc.ConversationID
    where bcm.IsBot = 1;    
    
    select cr.GroupID, r.RecipientID, r.SourceName as RecipientName, r.SourceStatusId as RecipientStatusID,  r.SourceStatusName as RecipientStatusName
    from ConversationRecipient cr
    inner join PagedBotConversation pbc on cr.GroupID = pbc.GroupID
    inner join Recipient r on cr.RecipientID = r.RecipientID;              
   
    drop temporary table AllBotConversation;
    drop temporary table PagedBotConversation;
END