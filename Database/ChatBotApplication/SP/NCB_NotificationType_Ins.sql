DELIMITER //

DROP PROCEDURE IF EXISTS NCB_NotificationType_Ins;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE NCB_NotificationType_Ins(
	IN notificationTypeName nvarchar(100), 
    IN recipientTypeIds nvarchar(500), 
    IN chatGroupTypeIds nvarchar(500), 
    IN systemId INT,
    OUT errCode SMALLINT,
	OUT errMsg VARCHAR(100)
    /*
		Creator: Scofield@20190912
				Params:
						- pageIndex: Page Index for paging
						- pageSize: Page Size for paging
				Revisions:
						- 20190912@Scofield: Create
    */
   )
SQL SECURITY DEFINER
BEGIN		
    DECLARE notificationTypeId INT;
        
	IF EXISTS(SELECT * FROM NotificationType nt WHERE nt.NotificationTypeName =  notificationTypeName) THEN
		SELECT 1 INTO errCode;
        SELECT "Notification Type Name exists" INTO errMsg;
    ELSE
		INSERT INTO NotificationType(NotificationTypeName, SystemId)
        VALUES (notificationTypeName, systemId);
        
        SET notificationTypeId = LAST_INSERT_ID();
        
        -- Update NotificationRecipientType
		DROP TEMPORARY TABLE IF EXISTS tmp_RecipientTypes;
		CREATE TEMPORARY TABLE tmp_RecipientTypes(RecipientTypeID INT);
		
		SET @SqlCommand = REPLACE(recipientTypeIds, ',', '),(');
		SET @SqlCommand = CONCAT('(', @SqlCommand, ')');
		SET @SqlCommand= CONCAT('INSERT INTO tmp_RecipientTypes VALUES'
															, @SqlCommand); 
		
		PREPARE Stmt FROM  @SqlCommand;
		EXECUTE Stmt;
		DEALLOCATE PREPARE Stmt;        
       
        INSERT INTO NotificationRecipientType(NotificationTypeID, RecipientTypeID)
        SELECT notificationTypeId, tmp.RecipientTypeID
        FROM tmp_RecipientTypes tmp;		        
        
        -- Update NotificationRecipientType
		DROP TEMPORARY TABLE IF EXISTS tmp_ChatGroupTypes;
		CREATE TEMPORARY TABLE tmp_ChatGroupTypes(ChatGroupTypeID INT);
		
		SET @SqlCommand = REPLACE(chatGroupTypeIds, ',', '),(');
		SET @SqlCommand = CONCAT('(', @SqlCommand, ')');
		SET @SqlCommand= CONCAT('INSERT INTO tmp_ChatGroupTypes VALUES'
															, @SqlCommand); 
		
		PREPARE Stmt FROM  @SqlCommand;
		EXECUTE Stmt;
		DEALLOCATE PREPARE Stmt;       
               
        INSERT INTO NotificationChatGroupType(NotificationTypeID, ChatGroupTypeID)
        SELECT notificationTypeId, tmp.ChatGroupTypeID
        FROM tmp_ChatGroupTypes tmp;		        
        
        -- Drop temp tables
        DROP TEMPORARY TABLE IF EXISTS tmp_RecipientTypes;
        DROP TEMPORARY TABLE IF EXISTS tmp_ChatGroupTypes;
       
		-- Update error code
        SELECT 0 INTO errCode;
        SELECT "" INTO errMsg;
    END IF;   
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_NotificationType_Ins TO 'ncb.admin'@'%';