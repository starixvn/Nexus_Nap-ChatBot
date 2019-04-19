DELIMITER //

DROP PROCEDURE IF EXISTS NCB_ChatGroupMapping_Sel;

CREATE PROCEDURE `NCB_ChatGroupMapping_Sel`(
		IN botNames VARCHAR(500) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' ,
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
					- licenseeStatusIds: Licensee Status Ids, ex: "1,2", | -1000 for N/A
					- mappingStatusIds: Mapping Status Ids, ex: "1,2" | BotRemoved = 0, NotMapped = 1, Mapped = 2     
					- pageIndex: Page Index for paging
					- pageSize: Page Size for paging
			Revisions:
					- 20190404@Scofield: Create        
                    - 20190418@Dominic: Improve performance
		*/
    )
    
BEGIN
	
	-- Constant
	DECLARE RECIPIENT_TYPE_LICENSEE INT DEFAULT 1;
	DECLARE RECIPIENT_TYPE_3RD_PRODUCT INT DEFAULT 2;
	DECLARE LICENSEE_STATUSID_NA INT DEFAULT -1000;
    DECLARE LICENSEE_STATUS_NA VARCHAR(100) DEFAULT 'N/A';
    
    DECLARE MAPPING_STATUS_BOT_REMOVED INT DEFAULT 0;
    DECLARE MAPPING_STATUS_NOT_MAPPED INT DEFAULT 1;
    DECLARE MAPPING_STATUS_MAPPED INT DEFAULT 2;
	
    DECLARE rowStart INT;
    SET rowStart =  pageSize * (pageIndex - 1);
    
    SET SESSION TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ;
    
	-- Split recipientTypeIds into temporary table tmp_RecipientTypeIds
    DROP TEMPORARY TABLE IF EXISTS tmp_BotNames;
	CREATE TEMPORARY TABLE tmp_BotNames(
									BotName VARCHAR(50) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' 
							);
    
    IF IFNULL(botNames, '') <> '' THEN
		SET @SqlCommand = REPLACE(botNames, ',', '''),(''');
		SET @SqlCommand = CONCAT('(''', @SqlCommand, ''')');
		SET @SqlCommand= CONCAT('INSERT INTO tmp_BotNames VALUES', @SqlCommand); 
		
		PREPARE Stmt FROM  @SqlCommand;
		EXECUTE Stmt;
		DEALLOCATE PREPARE Stmt;
	END IF;
     
	-- Split recipientTypeIds into temporary table tmp_RecipientTypeIds
    DROP TEMPORARY TABLE IF EXISTS tmp_RecipientTypeIds;
	CREATE TEMPORARY TABLE tmp_RecipientTypeIds(RecipientTypeId INT);
    
    IF IFNULL(recipientTypeIds, '') <> '' THEN
		SET @SqlCommand = REPLACE(recipientTypeIds, ',', '),(');
		SET @SqlCommand = CONCAT('(', @SqlCommand, ')');
		SET @SqlCommand= CONCAT('INSERT INTO tmp_RecipientTypeIds VALUES', @SqlCommand); 
		
		PREPARE Stmt FROM  @SqlCommand;
		EXECUTE Stmt;
		DEALLOCATE PREPARE Stmt;
	END IF;
	
	-- Split licenseeStatusIds into temporary table tmp_LicenseeStatusIds
    DROP TEMPORARY TABLE IF EXISTS tmp_LicenseeStatusIds;
	CREATE TEMPORARY TABLE tmp_LicenseeStatusIds(LicenseeStatusId INT);
    
    IF IFNULL(licenseeStatusIds, '') <> '' THEN
		SET @SqlCommand = REPLACE(licenseeStatusIds, ',', '),(');
		SET @SqlCommand = CONCAT('(', @SqlCommand, ')');
		SET @SqlCommand= CONCAT('INSERT INTO tmp_LicenseeStatusIds VALUES', @SqlCommand); 
		
		PREPARE Stmt FROM  @SqlCommand;
		EXECUTE Stmt;
		DEALLOCATE PREPARE Stmt;
	END IF;
    
    -- Split mappingStatusIds into temporary table tmp_LicenseeStatusIds
    DROP TEMPORARY TABLE IF EXISTS tmp_MappingStatusIDs;
	CREATE TEMPORARY TABLE tmp_MappingStatusIDs(MappingStatusID INT);
    
    IF IFNULL(mappingStatusIds, '') <> '' THEN
		SET @SqlCommand = REPLACE(mappingStatusIds, ',', '),(');
		SET @SqlCommand = CONCAT('(', @SqlCommand, ')');
		SET @SqlCommand= CONCAT('INSERT INTO tmp_MappingStatusIDs VALUES', @SqlCommand); 
		
		PREPARE Stmt FROM  @SqlCommand;
		EXECUTE Stmt;
		DEALLOCATE PREPARE Stmt;
	END IF;

	DROP TEMPORARY TABLE IF EXISTS tmp_RecipientIDByLicenseeStatus;
	CREATE TEMPORARY TABLE tmp_RecipientIDByLicenseeStatus(RecipientID INT);
    
    INSERT INTO tmp_RecipientIDByLicenseeStatus(RecipientID)
    SELECT r.RecipientID
    FROM Recipient r
    INNER JOIN Licensee l ON r.SourceId = l.LicenseeID 
			AND r.RecipientTypeId = RECIPIENT_TYPE_LICENSEE
    WHERE l.LicenseeStatusId IN (
														SELECT tls.LicenseeStatusId 
                                                        FROM tmp_LicenseeStatusIds tls 
                                                        WHERE tls.LicenseeStatusId <> LICENSEE_STATUSID_NA
													);
	
    IF EXISTS(
						SELECT 1 
						FROM tmp_LicenseeStatusIds tls 
                        WHERE tls.LicenseeStatusId = LICENSEE_STATUSID_NA
                        )
	THEN
		INSERT INTO tmp_RecipientIDByLicenseeStatus(RecipientID)
		SELECT r.RecipientID
		FROM Recipient r
        WHERE r.RecipientTypeId = RECIPIENT_TYPE_3RD_PRODUCT;
    END IF;
    
    DROP TEMPORARY TABLE IF EXISTS tmp_ConversationIDBot;
	CREATE TEMPORARY TABLE tmp_ConversationIDBot(
																					ChatGroupConversationID INT
                                                                                    , BotID INT
																		);
     
     
	INSERT INTO tmp_ConversationIDBot (ChatGroupConversationID, BotID)
	SELECT grp.ID AS ChatGroupConversationID , MIN(bot.ID) BotID
	FROM BotConversation grp
			INNER JOIN BotConversationMember bot
				ON bot.ConversationID = grp.ConversationID
					AND bot.IsBot = 1
	WHERE bot.Name IN (SELECT tbn.BotName FROM tmp_BotNames tbn)
    GROUP BY grp.ID;
        
	INSERT INTO tmp_ConversationIDBot (ChatGroupConversationID, BotID)
	SELECT grp.ID AS ChatGroupConversationID , MIN(bot.ID) BotID
	FROM BotConversation grp
			INNER JOIN BotConversationMember bot
				ON bot.ConversationID = grp.ConversationID
					AND bot.IsBot = 1 AND bot.IsActive = 1
	WHERE bot.Name IN (SELECT tbn.BotName FROM tmp_BotNames tbn)
    GROUP BY grp.ID;
        
	
    DROP TEMPORARY TABLE IF EXISTS tmp_ConversationID_FIRSTBot;
	CREATE TEMPORARY TABLE tmp_ConversationID_FIRSTBot(
																					ChatGroupConversationID INT
                                                                                    , BotID INT
																		);
     
	INSERT INTO tmp_ConversationID_FIRSTBot (ChatGroupConversationID, BotID)
	SELECT tmp.ChatGroupConversationID, MAX(tmp.BotID) BotID
    FROM tmp_ConversationIDBot tmp
    GROUP BY tmp.ChatGroupConversationID;
    
        
	-- Get all BotConversation IDs that satisfy filtering conditions
    DROP TEMPORARY TABLE IF EXISTS tmp_AllBotConversation;
    CREATE TEMPORARY TABLE tmp_AllBotConversation
    SELECT DISTINCT bc.Name AS GroupName
				, bc.ID AS GroupID
                , cgt.ChatGroupTypeID
                , cgt.ChatGroupTypeName
                , bc.ConversationID
                , bce.Remark
                , rt.RecipientTypeName
                , CASE 
						WHEN r.RecipientName IS NOT NULL  AND bce.ChatGroupTypeID IS NOT NULL THEN 1
                        ELSE 0
					END IsMapped
				, bcm.Name BotName
                , bcm.MemberID BotMemberID
                , bcm.IsActive
                , r.RecipientTypeID
    FROM BotConversation bc
    INNER JOIN tmp_ConversationID_FIRSTBot  FirstBot ON bc.ID = FirstBot.ChatGroupConversationID
    INNER JOIN BotConversationMember  bcm ON bcm.ID = FirstBot.BotID
    LEFT JOIN BotConversationExtraInfo bce ON bc.ID = bce.BotConversationID
    LEFT JOIN ChatGroupType cgt ON bce.ChatGroupTypeID = cgt.ChatGroupTypeID
    LEFT JOIN BotConversationRecipient bcr ON bc.ID = bcr.BotConversationID
    LEFT JOIN Recipient r ON bcr.RecipientId = r.RecipientId
    LEFT JOIN RecipientType rt ON r.RecipientTypeID = rt.RecipientTypeID
    WHERE bcm.IsBot = 1
        AND r.RecipientId IN (SELECT rbls.RecipientId FROM tmp_RecipientIDByLicenseeStatus rbls)
		AND r.RecipientTypeID  IN (SELECT trt.RecipientTypeId FROM tmp_RecipientTypeIds trt)
        AND r.RecipientName LIKE CONCAT(recipientName, '%')
        AND EXISTS (
			SELECT 1 
            FROM tmp_MappingStatusIDs t1 
            WHERE t1.MappingStatusID = CASE 
																	WHEN bcm.IsActive = 0 
																		THEN MAPPING_STATUS_BOT_REMOVED
																	 WHEN r.RecipientName IS NULL  OR  bce.ChatGroupTypeID IS NULL 
																		THEN MAPPING_STATUS_NOT_MAPPED
																	WHEN r.RecipientName IS NOT NULL  AND bce.ChatGroupTypeID IS NOT NULL 
																		THEN MAPPING_STATUS_MAPPED
                                                            END
		) ;
    
	-- Get BotConversations for current paging page
    DROP TEMPORARY TABLE IF EXISTS tmp_PagedBotConversation;
    CREATE TEMPORARY TABLE tmp_PagedBotConversation
    SELECT tmp.GroupName, 
		tmp.GroupID, 
        tmp.ChatGroupTypeID, 
        tmp.ChatGroupTypeName, 
        tmp.ConversationID, 
        tmp.Remark, 
        tmp.RecipientTypeID, 
        tmp.RecipientTypeName,
		tmp.IsMapped,
        tmp.BotName,
        tmp.BotMemberID,
        tmp.IsActive
	FROM tmp_AllBotConversation tmp
    ORDER BY tmp.GroupName
    LIMIT rowStart, pageSize;
    
	-- Get Total number of items
    SELECT COUNT(1) AS Total 
    FROM tmp_AllBotConversation tmp;    
    
	-- Get BotConversations for current paging page with some extra infos
	-- and satisfy RecipientTypeIds filtering condition
    SELECT tmp.GroupName, 
		tmp.GroupID, 
        tmp.ChatGroupTypeID, 
        tmp.ChatGroupTypeName, 
        tmp.ConversationID, 
        tmp.Remark, 
        tmp.RecipientTypeID, 
        tmp.RecipientTypeName,
		tmp.IsMapped
	FROM tmp_PagedBotConversation tmp
    ORDER BY tmp.GroupName;
    
	-- Get BotConversation Members that belong to the current paging page of BotConversations 
	-- and satisfy BotName filtering condition
    SELECT tmp.BotName, 
		tmp.ConversationID, 
        tmp.IsActive
    FROM tmp_PagedBotConversation tmp;
    
	-- Get BotConversationRecipient that belongs to the current paging page of BotConversations
	-- and satisfy RecipientName filtering condition
    SELECT bcr.BotConversationID AS GroupID, 
		r.RecipientID, 
        r.RecipientName, 
        IFNULL(ls.LicenseeStatusId, LICENSEE_STATUSID_NA) AS RecipientStatusID,  
        IFNULL(ls.LicenseeStatusName, LICENSEE_STATUS_NA) AS RecipientStatusName
    FROM BotConversationRecipient bcr
		INNER JOIN tmp_PagedBotConversation pbc ON bcr.BotConversationID = pbc.GroupID
		INNER JOIN Recipient r ON bcr.RecipientID = r.RecipientID
		LEFT JOIN Licensee l ON r.SourceId = l.LicenseeID AND r.RecipientTypeId = RECIPIENT_TYPE_LICENSEE
		LEFT JOIN LicenseeStatus ls ON l.LicenseeStatusId = ls.LicenseeStatusId;
    
	-- Drop temporary tables
    DROP TEMPORARY TABLE IF EXISTS tmp_AllBotConversation;
    DROP TEMPORARY TABLE IF EXISTS tmp_PagedBotConversation;
    
END;