CREATE DEFINER=`scofield.huynh`@`%` PROCEDURE `NCB_ChatGroupMapping_Sel`(
		IN botNames varchar(500),
        IN recipientTypeIds varchar(1000),
        IN recipientName varchar(100),
        IN mappingStatusIds varchar(1000),
		IN pageIndex INT, 
        IN pageSize INT
    )
BEGIN
	DECLARE rowStart INT;
    SELECT 	pageSize * (pageIndex - 1) into rowStart;   
    
    IF botNames is null THEN SET botNames = ''; END IF;
    IF recipientTypeIds is null THEN SET recipientTypeIds = ''; END IF;
    IF recipientName is null THEN SET recipientName = ''; END IF;
    IF mappingStatusIds is null THEN SET mappingStatusIds = ''; END IF;    
    
    create temporary table AllBotConversation
    select bc.GroupID
    from BotConversation bc
    inner join BotConversationMember  bcm on bc.ConversationID = bcm.ConversationID
    left join Conversation c on bc.GroupID = c.GroupID
    left join ConversationRecipient cr on bc.GroupID = cr.GroupID
    left join Recipient r on cr.RecipientId = r.RecipientId
    where bcm.IsBot = 1
		and (botNames = '' or find_in_set(bcm.Name, botNames) > 0)
		and (recipientTypeIds = '' or find_in_set(c.RecipientTypeID, recipientTypeIds) > 0)
        and (recipientName = '' or r.SourceName like CONCAT('%', recipientName, '%'))
        and (mappingStatusIds = '' or find_in_set(case when bcm.IsActive = 1 and c.GroupID is null then 1
										when bcm.IsActive = 1 and c.GroupID is not null then 2
                                        else 0 end, mappingStatusIds) > 0)
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
    left join RecipientType rt on rt.RecipientTypeID = c.RecipientTypeID
    where (recipientTypeIds = '' or find_in_set(c.RecipientTypeID, recipientTypeIds) > 0);    
    
    select bcm.Name as BotName, bcm.ConversationID, bcm.IsActive
    from BotConversationMember bcm
    inner join PagedBotConversation pbc on bcm.ConversationID = pbc.ConversationID
    where bcm.IsBot = 1 
		and (botNames = '' or find_in_set(bcm.Name, botNames) > 0);    
    
    select cr.GroupID, r.RecipientID, r.SourceName as RecipientName, r.SourceStatusId as RecipientStatusID,  r.SourceStatusName as RecipientStatusName
    from ConversationRecipient cr
    inner join PagedBotConversation pbc on cr.GroupID = pbc.GroupID
    inner join Recipient r on cr.RecipientID = r.RecipientID
    where (recipientName = '' or r.SourceName like CONCAT('%', recipientName, '%'));              
   
    drop temporary table AllBotConversation;
    drop temporary table PagedBotConversation;
END