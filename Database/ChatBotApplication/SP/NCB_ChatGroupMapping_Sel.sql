DELIMITER //
CREATE PROCEDURE `NCB_ChatGroupMapping_Sel`(
		IN in_BotNames VARCHAR(500),
        IN in_RecipientTypeIds VARCHAR(1000),
        IN in_RecipientName VARCHAR(100),
        IN in_MappingStatusIds VARCHAR(1000),
		IN in_PageIndex INT, 
        IN in_PageSize INT
    )
BEGIN
	DECLARE LICENSEE_TYPE INT DEFAULT 1;
	DECLARE rowStart INT;
    SELECT 	in_PageSize * (in_PageIndex - 1) INTO rowStart;   
    
    IF in_BotNames IS NULL THEN SET in_BotNames = ''; END IF;
    IF in_RecipientTypeIds IS NULL THEN SET in_RecipientTypeIds = ''; END IF;
    IF in_RecipientName IS NULL THEN SET in_RecipientName = ''; END IF;
    IF in_MappingStatusIds IS NULL THEN SET in_MappingStatusIds = ''; END IF;    
    
    CREATE TEMPORARY TABLE AllBotConversation
    SELECT bc.ID
    FROM BotConversation bc
    INNER JOIN BotConversationMember  bcm ON bc.ConversationID = bcm.ConversationID
    LEFT JOIN BotConversationExtraInfo bce ON bc.ID = bce.GroupID
    LEFT JOIN BotConversationRecipient bcr ON bc.ID = bcr.GroupID
    LEFT JOIN Recipient r ON bcr.RecipientId = r.RecipientId
    WHERE bcm.IsBot = 1
		AND (in_BotNames = '' OR find_in_set(bcm.Name, in_BotNames) > 0)
		AND (in_RecipientTypeIds = '' OR find_in_set(r.RecipientTypeID, in_RecipientTypeIds) > 0)
        AND (in_RecipientName = '' OR r.RecipientName LIKE CONCAT('%', in_RecipientName, '%'))
        AND (in_MappingStatusIds = '' OR find_in_set(CASE WHEN bcm.IsActive = 1 AND bce.GroupID IS NULL THEN 1
										WHEN bcm.IsActive = 1 AND bce.GroupID IS NOT NULL THEN 2
                                        ELSE 0 END, in_MappingStatusIds) > 0)
    GROUP BY bc.ID;        
    
    CREATE TEMPORARY TABLE PagedBotConversation
    SELECT bc.* 
    FROM AllBotConversation abc 
    INNER JOIN BotConversation bc ON abc.ID = bc.ID
    ORDER BY bc.Name
    LIMIT rowStart, in_PageSize;
    
    SELECT COUNT(*) AS Total FROM AllBotConversation;    
    
    SELECT DISTINCT pbc.Name AS GroupName, pbc.ID AS GroupID, cgt.ChatGroupTypeID, cgt.ChatGroupTypeName, pbc.ConversationID, bce.Remark, r.RecipientTypeID, rt.RecipientTypeName,
		CASE WHEN bce.GroupID IS NOT NULL THEN 1 ELSE 0 END AS IsMapped
    FROM PagedBotConversation pbc
    LEFT JOIN BotConversationExtraInfo bce ON bce.GroupID = pbc.ID
    LEFT JOIN ChatGroupType cgt ON bce.ChatGroupTypeID = cgt.ChatGroupTypeID
    LEFT JOIN BotConversationRecipient bcr ON bce.GroupID = bcr.GroupID
    LEFT JOIN Recipient r ON bcr.RecipientID = r.RecipientID
    LEFT JOIN RecipientType rt ON r.RecipientTypeID = rt.RecipientTypeID
    WHERE (in_RecipientTypeIds = '' OR find_in_set(r.RecipientTypeID, in_RecipientTypeIds) > 0);    
    
    SELECT bcm.Name AS BotName, bcm.ConversationID, bcm.IsActive
    FROM BotConversationMember bcm
    INNER JOIN PagedBotConversation pbc ON bcm.ConversationID = pbc.ConversationID
    WHERE bcm.IsBot = 1 
		AND (in_BotNames = '' OR find_in_set(bcm.Name, in_BotNames) > 0);    
    
    SELECT bcr.GroupID, r.RecipientID, r.RecipientName AS RecipientName, ls.LicenseeStatusId AS RecipientStatusID,  ls.LicenseeStatusName AS RecipientStatusName
    FROM BotConversationRecipient bcr
    INNER JOIN PagedBotConversation pbc ON bcr.GroupID = pbc.ID
    INNER JOIN Recipient r ON bcr.RecipientID = r.RecipientID
    LEFT JOIN Licensee l ON r.SourceId = l.LicenseeID AND r.RecipientTypeId = LICENSEE_TYPE
    LEFT JOIN LicenseeStatus ls ON l.LicenseeStatusId = ls.LicenseeStatusId
    WHERE (in_RecipientName = '' OR r.RecipientName LIKE CONCAT('%', in_RecipientName, '%'));              
   
    DROP TEMPORARY TABLE AllBotConversation;
    DROP TEMPORARY TABLE PagedBotConversation;
END;