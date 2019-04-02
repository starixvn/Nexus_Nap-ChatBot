DELIMITER //
CREATE PROCEDURE `NCB_ChatGroupMapping_Sel`(
		IN in_BotNames varchar(500),
        IN in_RecipientTypeIds varchar(1000),
        IN in_RecipientName varchar(100),
        IN in_MappingStatusIds varchar(1000),
		IN in_PageIndex INT, 
        IN in_PageSize INT
    )
BEGIN
	DECLARE LICENSEE_TYPE INT DEFAULT 1;
	DECLARE rowStart INT;
    SELECT 	in_PageSize * (in_PageIndex - 1) into rowStart;   
    
    IF in_BotNames is null THEN SET in_BotNames = ''; END IF;
    IF in_RecipientTypeIds is null THEN SET in_RecipientTypeIds = ''; END IF;
    IF in_RecipientName is null THEN SET in_RecipientName = ''; END IF;
    IF in_MappingStatusIds is null THEN SET in_MappingStatusIds = ''; END IF;    
    
    create temporary table AllBotConversation
    select bc.ID
    from BotConversation bc
    inner join BotConversationMember  bcm on bc.ConversationID = bcm.ConversationID
    left join BotConversationExtraInfo bce on bc.ID = bce.GroupID
    left join BotConversationRecipient bcr on bc.ID = bcr.GroupID
    left join Recipient r on bcr.RecipientId = r.RecipientId
    where bcm.IsBot = 1
		and (in_BotNames = '' or find_in_set(bcm.Name, in_BotNames) > 0)
		and (in_RecipientTypeIds = '' or find_in_set(r.RecipientTypeID, in_RecipientTypeIds) > 0)
        and (in_RecipientName = '' or r.RecipientName like CONCAT('%', in_RecipientName, '%'))
        and (in_MappingStatusIds = '' or find_in_set(case when bcm.IsActive = 1 and bce.GroupID is null then 1
										when bcm.IsActive = 1 and bce.GroupID is not null then 2
                                        else 0 end, in_MappingStatusIds) > 0)
    group by bc.ID;        
    
    create temporary table PagedBotConversation
    select bc.* 
    from AllBotConversation abc 
    inner join BotConversation bc on abc.ID = bc.ID
    order by bc.Name
    LIMIT rowStart, in_PageSize;
    
    select count(*) as Total from AllBotConversation;    
    
    select distinct pbc.Name as GroupName, pbc.ID as GroupID, cgt.ChatGroupTypeID, cgt.ChatGroupTypeName, pbc.ConversationID, bce.Remark, r.RecipientTypeID, rt.RecipientTypeName,
		case when bce.GroupID is not null then 1 else 0 end as IsMapped
    from PagedBotConversation pbc
    left join BotConversationExtraInfo bce on bce.GroupID = pbc.ID
    left join ChatGroupType cgt on bce.ChatGroupTypeID = cgt.ChatGroupTypeID
    left join BotConversationRecipient bcr on bce.GroupID = bcr.GroupID
    left join Recipient r on bcr.RecipientID = r.RecipientID
    left join RecipientType rt on r.RecipientTypeID = rt.RecipientTypeID
    where (in_RecipientTypeIds = '' or find_in_set(r.RecipientTypeID, in_RecipientTypeIds) > 0);    
    
    select bcm.Name as BotName, bcm.ConversationID, bcm.IsActive
    from BotConversationMember bcm
    inner join PagedBotConversation pbc on bcm.ConversationID = pbc.ConversationID
    where bcm.IsBot = 1 
		and (in_BotNames = '' or find_in_set(bcm.Name, in_BotNames) > 0);    
    
    select bcr.GroupID, r.RecipientID, r.RecipientName as RecipientName, ls.LicenseeStatusId as RecipientStatusID,  ls.LicenseeStatusName as RecipientStatusName
    from BotConversationRecipient bcr
    inner join PagedBotConversation pbc on bcr.GroupID = pbc.ID
    inner join Recipient r on bcr.RecipientID = r.RecipientID
    left join Licensee l on r.SourceId = l.LicenseeID and r.RecipientTypeId = LICENSEE_TYPE
    left join LicenseeStatus ls on l.LicenseeStatusId = ls.LicenseeStatusId
    where (in_RecipientName = '' or r.RecipientName like CONCAT('%', in_RecipientName, '%'));              
   
    drop temporary table AllBotConversation;
    drop temporary table PagedBotConversation;
END;