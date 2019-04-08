DELIMITER //
CREATE PROCEDURE `NCB_ChatGroupMapping_Sel`(
		IN botNames VARCHAR(500),
        IN recipientTypeIds VARCHAR(1000),
        IN recipientName VARCHAR(100),
        IN mappingStatusIds VARCHAR(1000),
		IN pageIndex INT, 
        IN pageSize INT
		 /*
			Creator: Scofield@20190404
			Params:
					- botNames: Bot names, ex: "bot name 1,bot name 2"
					- recipientTypeIds: Recipient Type Ids, ex: "1,2"
					- recipientName: Recipient Name
					- mappingStatusIds: Mapping Status Ids, ex: "1,2"
					- pageIndex: Page Index for paging
					- pageSize: Page Size for paging
			Revisions:
					- 20190404@Scofield: Create        
		*/
    )
    
BEGIN
	-- Delare variables
	DECLARE LICENSEE_TYPE INT DEFAULT 1;
	DECLARE rowStart INT;
    SELECT 	pageSize * (pageIndex - 1) INTO rowStart;   
    
	-- Check if params are NULL
    IF botNames IS NULL THEN SET botNames = ''; END IF;
    IF recipientTypeIds IS NULL THEN SET recipientTypeIds = ''; END IF;
    IF recipientName IS NULL THEN SET recipientName = ''; END IF;
    IF mappingStatusIds IS NULL THEN SET mappingStatusIds = ''; END IF;    
    
	-- Get all BotConversation IDs that satisfy filtering conditions
    DROP TEMPORARY TABLE IF EXISTS tmp_AllBotConversation;
    CREATE TEMPORARY TABLE tmp_AllBotConversation
    SELECT bc.ID
    FROM BotConversation bc
    INNER JOIN BotConversationMember  bcm ON bc.ConversationID = bcm.ConversationID
    LEFT JOIN BotConversationExtraInfo bce ON bc.ID = bce.BotConversationID
    LEFT JOIN BotConversationRecipient bcr ON bc.ID = bcr.BotConversationID
    LEFT JOIN Recipient r ON bcr.RecipientId = r.RecipientId
    WHERE bcm.IsBot = 1
		AND (botNames = '' OR find_in_set(bcm.Name, botNames) > 0)
		AND (recipientTypeIds = '' OR find_in_set(r.RecipientTypeID, recipientTypeIds) > 0)
        AND (recipientName = '' OR r.RecipientName LIKE CONCAT('%', recipientName, '%'))
        AND (mappingStatusIds = '' OR find_in_set(CASE WHEN bcm.IsActive = 1 AND bce.BotConversationID IS NULL THEN 1
										WHEN bcm.IsActive = 1 AND bce.BotConversationID IS NOT NULL THEN 2
                                        ELSE 0 END, mappingStatusIds) > 0)
    GROUP BY bc.ID;        
    
	-- Get BotConversations for current paging page
    DROP TEMPORARY TABLE IF EXISTS tmp_PagedBotConversation;
    CREATE TEMPORARY TABLE tmp_PagedBotConversation
    SELECT bc.* 
    FROM tmp_AllBotConversation abc 
    INNER JOIN BotConversation bc ON abc.ID = bc.ID
    ORDER BY bc.Name
    LIMIT rowStart, pageSize;
    
	-- Get Total number of items
    SELECT COUNT(*) AS Total FROM tmp_AllBotConversation;    
    
	-- Get BotConversations for current paging page with some extra infos
	-- and satisfy RecipientTypeIds filtering condition
    SELECT DISTINCT pbc.Name AS GroupName, 
		pbc.ID AS GroupID, 
        cgt.ChatGroupTypeID, 
        cgt.ChatGroupTypeName, 
        pbc.ConversationID, 
        bce.Remark, 
        r.RecipientTypeID, 
        rt.RecipientTypeName,
		CASE WHEN bce.BotConversationID IS NOT NULL THEN 1 ELSE 0 END AS IsMapped
    FROM tmp_PagedBotConversation pbc
    LEFT JOIN BotConversationExtraInfo bce ON bce.BotConversationID = pbc.ID
    LEFT JOIN ChatGroupType cgt ON bce.ChatGroupTypeID = cgt.ChatGroupTypeID
    LEFT JOIN BotConversationRecipient bcr ON bce.BotConversationID = bcr.BotConversationID
    LEFT JOIN Recipient r ON bcr.RecipientID = r.RecipientID
    LEFT JOIN RecipientType rt ON r.RecipientTypeID = rt.RecipientTypeID
    WHERE (recipientTypeIds = '' OR find_in_set(r.RecipientTypeID, recipientTypeIds) > 0);    
    
	-- Get BotConversation Members that belong to the current paging page of BotConversations 
	-- and satisfy BotName filtering condition
    SELECT bcm.Name AS BotName, 
		bcm.ConversationID, 
        bcm.IsActive
    FROM BotConversationMember bcm
    INNER JOIN tmp_PagedBotConversation pbc ON bcm.ConversationID = pbc.ConversationID
    WHERE bcm.IsBot = 1 
		AND (botNames = '' OR find_in_set(bcm.Name, botNames) > 0);    
    
	-- Get BotConversationRecipient that belongs to the current paging page of BotConversations
	-- and satisfy RecipientName filtering condition
    SELECT bcr.BotConversationID AS GroupID, 
		r.RecipientID, 
        r.RecipientName, 
        ls.LicenseeStatusId AS RecipientStatusID,  
        ls.LicenseeStatusName AS RecipientStatusName
    FROM BotConversationRecipient bcr
    INNER JOIN tmp_PagedBotConversation pbc ON bcr.BotConversationID = pbc.ID
    INNER JOIN Recipient r ON bcr.RecipientID = r.RecipientID
    LEFT JOIN Licensee l ON r.SourceId = l.LicenseeID AND r.RecipientTypeId = LICENSEE_TYPE
    LEFT JOIN LicenseeStatus ls ON l.LicenseeStatusId = ls.LicenseeStatusId
    WHERE (recipientName = '' OR r.RecipientName LIKE CONCAT('%', recipientName, '%'));                 
    
	-- Drop temporary tables
    DROP TEMPORARY TABLE IF EXISTS tmp_AllBotConversation;
    DROP TEMPORARY TABLE IF EXISTS tmp_PagedBotConversation;
END;