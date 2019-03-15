CREATE DEFINER=`scofield.huynh`@`%` PROCEDURE `NCB_ChatGroupMapping_Sel`(IN pageIndex INT, IN pageSize INT)
BEGIN
	declare NOT_MAPPED_STATUS int default 1;
    declare MAPPED_STATUS int default 2;
    declare REMOVED_STATUS int default 3;
    declare LICENSEE_TYPE int default 1;
    declare PRODUCT_TYPE int default 2;
    
    DECLARE rowStart INT;
    SELECT 	pageSize * (pageIndex - 1) into rowStart; 
    
	create temporary table tmpResult
    select * From 
	(
        select min(bcm.Name) as BotName, bc.ConversationID, bc.Name as GroupName, 
			NOT_MAPPED_STATUS as MappingStatusId, -1 as RecipientTypeID, '' as Remark, -1 as RecipientID
		from BotConversation bc
		inner join BotCoversationMember bcm on bc.ConversationID = bcm.ConversationID
		where bcm.IsActive = 1 
			and bcm.IsBot = 1
			and bc.GroupID not in (select GroupID from Conversation)
		group by bc.ConversationID, bc.Name 
			
		UNION 
		
		select bcm.Name as BotName, bc.ConversationID, bc.Name as GroupName, 
			case when bcm.IsActive = 1 then MAPPED_STATUS else REMOVED_STATUS end as MappingStatusId, 
			c.RecipientTypeID, c.Remark, cr.RecipientID
		from Conversation c
		inner join BotConversation bc on c.GroupID = bc.GroupID
		inner join BotCoversationMember bcm on c.ConversationID = bcm.ConversationID
		inner join ConversationRecipient cr on c.GroupID = c.GroupID
	) as tmp;
    
    select count(*) as Total From tmpResult;
    
    select BotName, ConversationID, GroupName, ls.LicenseeStatusName as LicenseeStatus,
		tr.MappingStatusId, ss.SettingValue as MappingStatusName, tr.RecipientTypeID, rt.RecipientTypeName, 
		Remark, RecipientID, case when tr.RecipientTypeId = LICENSEE_TYPE then l.LicenseeName else p.ProductName end as RecipientName
    from tmpResult tr
    inner join SystemSetting ss on tr.MappingStatusId = ss.SettingId and ss.SettingType = 'MappingStatus'
    left join RecipientType rt on tr.RecipientTypeID = rt.RecipientTypeID
    left join Licensee l on tr.RecipientID = l.LicenseeID and tr.RecipientTypeId = LICENSEE_TYPE
    left join Product p on tr.RecipientID = p.ProductID and tr.RecipientTypeId = PRODUCT_TYPE
    left join LicenseeStatus ls on l.LicenseeStatusId = ls.LicenseeStatusID and tr.RecipientTypeId = LICENSEE_TYPE
    LIMIT rowStart, pageSize;
    
    drop temporary table tmpResult;
END