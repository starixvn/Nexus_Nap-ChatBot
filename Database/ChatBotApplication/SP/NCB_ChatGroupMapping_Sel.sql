DELIMITER //
DROP PROCEDURE `NCB_ChatGroupMapping_Sel`;
CREATE PROCEDURE `NCB_ChatGroupMapping_Sel`(
		IN botNames VARCHAR(500),
        IN recipientTypeIds VARCHAR(1000),
        IN recipientName VARCHAR(100),
		IN licenseeStatusIds VARCHAR(1000),
        IN mappingStatusIds VARCHAR(1000),
		IN pageIndex INT, 
        IN pageSize INT
		 /*
			Creator: Scofield@20190404
			Params:
					- botNames: Bot names, ex: "bot name 1,bot name 2"
					- recipientTypeIds: Recipient Type Ids, ex: "1,2"
					- recipientName: Recipient Name
					- licenseeStatusIds: Licensee Status Ids, ex: "1,2", 1000 for Products
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
	DECLARE PRODUCT_TYPE INT DEFAULT 2;
	DECLARE PRODUCT_STATUS_ID INT DEFAULT 1000;
	DECLARE rowStart INT;
    SELECT 	pageSize * (pageIndex - 1) INTO rowStart;   
    
	-- Check if params are NULL
    IF botNames IS NULL THEN SET botNames = ''; END IF;
    IF recipientTypeIds IS NULL THEN SET recipientTypeIds = ''; END IF;
    IF recipientName IS NULL THEN SET recipientName = ''; END IF;
    IF mappingStatusIds IS NULL THEN SET mappingStatusIds = ''; END IF;    
	IF licenseeStatusIds IS NULL THEN SET licenseeStatusIds = ''; END IF;    
	
	-- Split recipientTypeIds into temporary table tmp_RecipientTypeIds
    DROP TEMPORARY TABLE IF EXISTS tmp_RecipientTypeIds;
	CREATE TEMPORARY TABLE tmp_RecipientTypeIds(RecipientTypeId INT);
    
	SET @SqlCommand = REPLACE(recipientTypeIds, ',', '),(');
	SET @SqlCommand = CONCAT('(', @SqlCommand, ')');
	SET @SqlCommand= CONCAT('INSERT INTO tmp_RecipientTypeIds VALUES', @SqlCommand); 
	
    PREPARE Stmt FROM  @SqlCommand;
	EXECUTE Stmt;
	DEALLOCATE PREPARE Stmt;
	
	-- Split licenseeStatusIds into temporary table tmp_LicenseeStatusIds
    DROP TEMPORARY TABLE IF EXISTS tmp_LicenseeStatusIds;
	CREATE TEMPORARY TABLE tmp_LicenseeStatusIds(LicenseeStatusId INT);
    
	SET @SqlCommand = REPLACE(licenseeStatusIds, ',', '),(');
	SET @SqlCommand = CONCAT('(', @SqlCommand, ')');
	SET @SqlCommand= CONCAT('INSERT INTO tmp_LicenseeStatusIds VALUES', @SqlCommand); 
	
    PREPARE Stmt FROM  @SqlCommand;
	EXECUTE Stmt;
	DEALLOCATE PREPARE Stmt;
	    
	-- Get all BotConversation IDs that satisfy filtering conditions
    DROP TEMPORARY TABLE IF EXISTS tmp_AllBotConversation;
    CREATE TEMPORARY TABLE tmp_AllBotConversation
    SELECT bc.ID
    FROM BotConversation bc
    INNER JOIN BotConversationMember  bcm ON bc.ConversationID = bcm.ConversationID
    LEFT JOIN BotConversationExtraInfo bce ON bc.ID = bce.BotConversationID
    LEFT JOIN BotConversationRecipient bcr ON bc.ID = bcr.BotConversationID
    LEFT JOIN Recipient r ON bcr.RecipientId = r.RecipientId
	LEFT JOIN Licensee l ON r.SourceId = l.LicenseeID AND r.RecipientTypeId = LICENSEE_TYPE    
	LEFT JOIN tmp_RecipientTypeIds trt ON trt.RecipientTypeId = r.RecipientTypeID
	LEFT JOIN tmp_LicenseeStatusIds tls ON l.LicenseeStatusId = tls.LicenseeStatusId
    WHERE bcm.IsBot = 1
		AND (botNames = '' OR find_in_set(bcm.Name, botNames) > 0)
		AND (recipientTypeIds = '' OR trt.RecipientTypeId IS NOT NULL)
        AND (recipientName = '' OR r.RecipientName LIKE CONCAT('%', recipientName, '%'))
		AND (licenseeStatusIds = '' 
			OR (r.RecipientTypeId = PRODUCT_TYPE AND find_in_set(PRODUCT_STATUS_ID, licenseeStatusIds))
			OR tls.LicenseeStatusId IS NOT NULL)
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
	LEFT JOIN tmp_RecipientTypeIds trt ON trt.RecipientTypeId = r.RecipientTypeID
    WHERE (recipientTypeIds = '' OR trt.RecipientTypeId IS NOT NULL);    
    
	-- Get BotConversation Members that belong to the current paging page of BotConversations 
	-- and satisfy BotName filtering condition
    SELECT bcm.Name AS BotName, 
		bcm.ConversationID, 
        bcm.IsActive
    FROM BotConversationMember bcm
    INNER JOIN tmp_PagedBotConversation pbc ON bcm.ConversationID = pbc.ConversationID
    WHERE bcm.IsBot = 1 and bcm.IsActive = 1
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
	LEFT JOIN tmp_LicenseeStatusIds tls ON l.LicenseeStatusId = tls.LicenseeStatusId
    WHERE (recipientName = '' OR r.RecipientName LIKE CONCAT('%', recipientName, '%'))
		AND (licenseeStatusIds = '' 
			OR (r.RecipientTypeId = PRODUCT_TYPE AND find_in_set(PRODUCT_STATUS_ID, licenseeStatusIds)) 
			OR tls.LicenseeStatusId IS NOT NULL);
    
	-- Drop temporary tables
    DROP TEMPORARY TABLE IF EXISTS tmp_AllBotConversation;
    DROP TEMPORARY TABLE IF EXISTS tmp_PagedBotConversation;
END;