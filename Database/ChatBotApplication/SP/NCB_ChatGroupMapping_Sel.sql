CREATE DEFINER=`scofield.huynh`@`%` PROCEDURE `NCB_ChatGroupMapping_Sel`(IN pageIndex INT, IN pageSize INT)
BEGIN
	DECLARE rowStart INT;
    SELECT 	pageSize * (pageIndex - 1) into rowStart;     
    
    create temporary table PagedBotConversation
    select * 
    from BotConversation
    order by Name
    LIMIT rowStart, pageSize;
    
    select count(*) as Total from BotConversation;    
    
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
   
    drop temporary table PagedBotConversation;
END