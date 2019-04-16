DELIMITER //

DROP PROCEDURE IF EXISTS NCB_EditNotification_Upd;

CREATE PROCEDURE  NCB_EditNotification_Upd(
	IN notificationID INT
	, IN notificationTypeID SMALLINT
	, IN productID INT
	, IN recipientIDLst VARCHAR(200)
	, IN title VARCHAR(300) CHARACTER SET utf8mb4
	, IN content MEDIUMTEXT CHARACTER SET utf8mb4
	, IN configSendTime DATETIME
	, IN userID INT
	, OUT errCode SMALLINT
	, OUT errMsg VARCHAR(100)
    /*
		Creator: Dominic@20190401
        Param:
				- ProductID: Applied to filter licensees who are offered a specific product
                - NotificationID: ID of created Notification
                - ConfigSendTime: UTC (GMT +0)
        Revisions:
				- 20190401@Dominic: First Commit
        
    */
)
NCB_EditNotification_Upd:BEGIN

	#Variables used to write User Logs
	DECLARE startTime 			DATETIME(3);
    DECLARE finishTime 		DATETIME(3); 
    DECLARE duration 			INT;							
    DECLARE logContent 		TEXT CHARACTER SET utf8mb4;
    DECLARE ACTION_ID 		INT DEFAULT 5;
    
    #constants
    DECLARE RECIPIENT_TYPE_LICENSEES 	SMALLINT DEFAULT 1;
    DECLARE LICENSEE_STATUS_ACTIVE 		TINYINT UNSIGNED DEFAULT 3;
    
    DECLARE NEW_NOTIFICATION_STATUSID	TINYINT UNSIGNED DEFAULT 0;
    DECLARE NEW_NOTIFICATION_STATUS	VARCHAR(10) DEFAULT 'Planned';
    
	DECLARE NOTIFICATION_STATUSID_CANCELLED	INT DEFAULT 4;
    
    DECLARE NOTIFICATION_STATUSID_EDITING	INT DEFAULT 6;
	DECLARE NOTIFICATION_STATUS_EDITING	VARCHAR(10) DEFAULT 'Editing';
    
     #Local variables
    DECLARE configSendTimeGMTPlus4 DATETIME;
    DECLARE logDeletedRecipientLst TEXT  CHARACTER SET utf8mb4;
    DECLARE logInsertedRecipientLst TEXT  CHARACTER SET utf8mb4;
	DECLARE logOldTitle TEXT CHARACTER SET utf8mb4;
	DECLARE logOldContent TEXT CHARACTER SET utf8mb4;
    DECLARE logOldconfigSendTime DATETIME;
    
    #Get Start Time to calcualte execution duration
    SET  startTime = UTC_TIMESTAMP(3);
    SET errCode=0, errMsg='';
    
	#Check param
	IF  NOT EXISTS(	SELECT 1 
									FROM Notification n
                                    WHERE n.NotificationID = notificationID
                                    ) THEN
		SET errCode=501, errMsg='Invalid NotificationID';
        LEAVE NCB_EditNotification_Upd;
	ELSEIF NOT EXISTS(SELECT 1 
									FROM Notification n
                                    WHERE n.NotificationTypeID = notificationTypeID 
										AND n.NotificationID = notificationID
                                    ) THEN
		SET errCode=502, errMsg='Invalid NotificationTypeID';
        LEAVE NCB_EditNotification_Upd;
    ELSEIF IFNULL(title,'') = '' OR IFNULL(content,'') = '' THEN
		SET errCode=503, errMsg='Title and Content must not be empty';
        LEAVE NCB_EditNotification_Upd;
	ELSEIF IFNULL(recipientIDLst,'') = '' THEN
		SET errCode=504, errMsg='Recipient list must not be empty';
        LEAVE NCB_EditNotification_Upd;
	ELSEIF userID IS NULL THEN
		SET errCode=505, errMsg='UserID can not be null';
        LEAVE NCB_EditNotification_Upd;
	ELSEIF configSendTime IS NULL THEN
		SET errCode=506, errMsg='ConfigSendTime can not be null';
        LEAVE NCB_EditNotification_Upd;
	END IF;
	
    #Insert comma seperated list RecipientIDLst to temporary tables
    DROP TEMPORARY TABLE IF EXISTS tmp_recipientIDLst;
	CREATE TEMPORARY TABLE tmp_recipientIDLst(RecipientID INT);
    
	SET @sqlCommand = REPLACE(recipientIDLst, ',', '),(');
	SET @sqlCommand = CONCAT('(', @sqlCommand, ')');
	SET @sqlCommand= CONCAT('INSERT INTO tmp_recipientIDLst VALUES'
														, @sqlCommand); 
	
    PREPARE stmt FROM  @sqlCommand;
	EXECUTE stmt;
	DEALLOCATE PREPARE stmt;
    
    #Check Not-Mapping/Removed-Bot Recipients
    IF EXISTS(
						SELECT 1 
						FROM tmp_recipientIDLst main
                        LEFT JOIN BotConversationRecipient bcr
								ON bcr.RecipientID = main.RecipientID
						LEFT JOIN BotConversationExtraInfo bcri
								ON bcri.BotConversationID = bcr.BotConversationID
						LEFT JOIN BotConversation con
								ON con.ID = bcr.BotConversationID
						LEFT JOIN BotConversationMember bot
								ON bot.ConversationID = con.ConversationID
									AND bot.IsBot = 1
									AND bot.IsActive = 1
						WHERE bcr.BotConversationID IS NULL
								OR bcri.ChatGroupTypeID IS  NULL
                                OR con.ConversationID IS NULL
                                OR bot.MemberID IS NULL
	) THEN
		SET errCode=507;

		SELECT GROUP_CONCAT( DISTINCT re.RecipientName  ORDER BY re.RecipientName SEPARATOR ', ') 
        INTO errMsg
        FROM Recipient re
        WHERE re.RecipientID IN (
						SELECT main. RecipientID
						FROM tmp_recipientIDLst main
                        LEFT JOIN BotConversationRecipient bcr
								ON bcr.RecipientID = main.RecipientID
						LEFT JOIN BotConversationExtraInfo bcri
								ON bcri.BotConversationID = bcr.BotConversationID
						LEFT JOIN BotConversation con
								ON con.ID = bcr.BotConversationID
						LEFT JOIN BotConversationMember bot
								ON bot.ConversationID = con.ConversationID
									AND IsBot = 1
									AND IsActive = 1
						WHERE bcr.BotConversationID IS NULL
								OR bcri.ChatGroupTypeID IS  NULL
                                OR con.ConversationID IS NULL
                                OR bot.MemberID IS NULL
			);

        LEAVE NCB_EditNotification_Upd;
    END IF;

    #Check InActive Licensees
     IF EXISTS(
						SELECT 1 
						FROM tmp_recipientIDLst main
                        INNER JOIN Recipient re 
							ON re.RecipientID = main.RecipientID 
								AND re.RecipientTypeID = RECIPIENT_TYPE_LICENSEES
                        INNER JOIN Licensee l
							ON l.LicenseeId = re.SourceID
						WHERE  l.LicenseeStatusId <> LICENSEE_STATUS_ACTIVE
	) THEN
		SET errCode=508;
        
        SELECT GROUP_CONCAT(re.RecipientName ORDER BY re.RecipientName  SEPARATOR ', ')  
        INTO errMsg
        FROM Recipient re
        WHERE re.RecipientID IN (
						SELECT main. RecipientID
						FROM tmp_recipientIDLst main
                        INNER JOIN Recipient re 
							ON re.RecipientID = main.RecipientID 
								AND re.RecipientTypeID = RECIPIENT_TYPE_LICENSEES
                        INNER JOIN Licensee l
							ON l.LicenseeId = re.SourceID
						WHERE  l.LicenseeStatusId <> LICENSEE_STATUS_ACTIVE
			);

        LEAVE NCB_EditNotification_Upd;
    END IF;
    
    #Check Licensees do not offered a product
    SET configSendTimeGMTPlus4 = CONVERT_TZ(UTC_TIMESTAMP(),'+00:00','-04:00');
    IF productID IS NOT NULL 
		AND EXISTS(
						SELECT 1 
						FROM tmp_recipientIDLst main
						INNER JOIN Recipient re 
							ON re.RecipientID = main.RecipientID 
								AND re.RecipientTypeID = RECIPIENT_TYPE_LICENSEES
						LEFT JOIN LicenseeProduct lp 
							ON lp.LicenseeID = re.SourceID 
								AND lp.ProductID = productID
						WHERE  lp.LaunchDate IS NULL
							OR (
										 lp.LaunchDate IS NOT NULL 
										AND configSendTimeGMTPlus4 <  lp.LaunchDate
									)
							OR (
										lp.TerminationDate IS NOT NULL 
										AND configSendTimeGMTPlus4 > lp.TerminationDate
									)
	) THEN
		SET errCode=509;
        
		SELECT GROUP_CONCAT(re.RecipientName ORDER BY re.RecipientName SEPARATOR ', ') 
        INTO errMsg
        FROM Recipient re
        WHERE re.RecipientID IN (
						SELECT main. RecipientID
						FROM tmp_recipientIDLst main
						INNER JOIN Recipient re 
							ON re.RecipientID = main.RecipientID 
								AND re.RecipientTypeID = RECIPIENT_TYPE_LICENSEES
						LEFT JOIN LicenseeProduct lp 
							ON lp.LicenseeID = re.SourceID 
								AND lp.ProductID = productID
						WHERE  lp.LaunchDate IS NULL
							OR (
										 lp.LaunchDate IS NOT NULL 
										AND configSendTimeGMTPlus4 <  lp.LaunchDate
									)
							OR (
										lp.TerminationDate IS NOT NULL 
										AND configSendTimeGMTPlus4 > lp.TerminationDate
									)
			);

        LEAVE NCB_EditNotification_Upd;
    END IF;
    
    #Check exeption recipient
    IF EXISTS(
			SELECT 1
			FROM tmp_recipientIDLst main
			WHERE main.RecipientID IN (
							SELECT ex.RecipientID 
							FROM NotificationTypeRecipientException ex
							WHERE ex.NotificationTypeID = notificationTypeID
						)
	)
    THEN
		SET errCode=510;
        
        SELECT GROUP_CONCAT(re.RecipientName ORDER BY re.RecipientName SEPARATOR ', ') 
        INTO errMsg
        FROM tmp_recipientIDLst main
        INNER JOIN Recipient re 
							ON re.RecipientID = main.RecipientID 
		WHERE main.RecipientID IN (
						SELECT ex.RecipientID 
						FROM NotificationTypeRecipientException ex
						WHERE ex.NotificationTypeID = notificationTypeID
                        );
                        
		LEAVE NCB_EditNotification_Upd;
    END IF;
    
     #Update Notification Status to 'Editing'
    UPDATE Notification n
    SET n.Status = NOTIFICATION_STATUS_EDITING
			, n.StatusID = NOTIFICATION_STATUSID_EDITING
	WHERE n.NotificationID = notificationID
		AND n.StatusID = NOTIFICATION_STATUSID_CANCELLED;
	
    IF ROW_COUNT()  = 0 THEN
		SET errCode=511, errMsg='This Notification has been updated by someone else';
        LEAVE NCB_EditNotification_Upd;
    END IF;
    
	UPDATE NotificationRecipientList rlst
    SET rlst.Status = NOTIFICATION_STATUS_EDITING
			, rlst.StatusID = NOTIFICATION_STATUSID_EDITING
    WHERE rlst.NotificationID = notificationID;
    
    #Insert new recipient list
    DROP TEMPORARY TABLE IF EXISTS tmp_newNotificationRecipientList;
	CREATE TEMPORARY TABLE tmp_newNotificationRecipientList(
																					NotificationID INT
                                                                                    , BotConversationID INT
                                                                                    , ChatGroupTypeID INT
                                                                                    , RecipientID INT
																		);
    
    INSERT INTO tmp_newNotificationRecipientList(NotificationID, BotConversationID
                                                                    , ChatGroupTypeID, RecipientID)
    SELECT notificationID, IFNULL(gri.BotConversationID, -1)
					, IFNULL(gri.ChatGroupTypeID,-1), re.RecipientID
    FROM tmp_recipientIDLst re
		LEFT JOIN BotConversationRecipient gr 
			ON re.RecipientID = gr.RecipientID
		LEFT JOIN BotConversationExtraInfo gri 
			ON gri.BotConversationID = gr.BotConversationID
    WHERE gri.ChatGroupTypeID IN (
						SELECT cgt.ChatGroupTypeID 
						FROM NotificationChatGroupType  cgt
                        WHERE cgt.NotificationTypeID = notificationTypeID
	);
    
    #Deleted notification recipients
	DROP TEMPORARY TABLE IF EXISTS tmp_deletedNotificationRecipientList;
	CREATE TEMPORARY TABLE tmp_deletedNotificationRecipientList(
																					NotificationID INT
                                                                                    , BotConversationID INT
                                                                                    , ChatGroupTypeID INT
                                                                                    , RecipientID INT
																		);
	INSERT INTO tmp_deletedNotificationRecipientList(
												NotificationID, BotConversationID
												, ChatGroupTypeID, RecipientID
											)
    SELECT nrl.NotificationID, nrl.BotConversationID
					, nrl.ChatGroupTypeID, nrl.RecipientID
    FROM NotificationRecipientList nrl
		LEFT JOIN tmp_newNotificationRecipientList newnrl
			ON newnrl.NotificationID = nrl.NotificationID
				AND newnrl.BotConversationID = nrl.BotConversationID
                AND newnrl.ChatGroupTypeID = nrl.ChatGroupTypeID
                AND newnrl.RecipientID = nrl.RecipientID
	WHERE nrl.NotificationID = notificationID
		AND newnrl.RecipientID IS NULL;
 
	#Delete removed- recipients
	DELETE nrl
    FROM NotificationRecipientList nrl
		INNER JOIN tmp_deletedNotificationRecipientList tmpDel
			ON tmpDel.NotificationID = nrl.NotificationID
				AND tmpDel.BotConversationID = nrl.BotConversationID
                AND tmpDel.ChatGroupTypeID = nrl.ChatGroupTypeID
                AND tmpDel.RecipientID = nrl.RecipientID;
                
     #Get log info for removed- recipients     
    SELECT  CONCAT('[' , GROUP_CONCAT(json_object(
														'NotificationID', tmpDel.NotificationID
                                                        , 'BotConversationID', tmpDel.BotConversationID
                                                        , 'ChatGroupTypeID', tmpDel.ChatGroupTypeID
                                                        , 'RecipientID', tmpDel.RecipientID
									) SEPARATOR ', ') ,']') A
	INTO logDeletedRecipientLst
	FROM tmp_deletedNotificationRecipientList tmpDel;
    
	 #Inserted notification recipients
	DROP TEMPORARY TABLE IF EXISTS tmp_insertedNotificationRecipientList;
	CREATE TEMPORARY TABLE tmp_insertedNotificationRecipientList(
																					NotificationID INT
                                                                                    , BotConversationID INT
                                                                                    , ChatGroupTypeID INT
                                                                                    , RecipientID INT
																		);
	INSERT INTO tmp_insertedNotificationRecipientList(
												NotificationID, BotConversationID
												, ChatGroupTypeID, RecipientID
											)
    SELECT newnrl.NotificationID, newnrl.BotConversationID
					, newnrl.ChatGroupTypeID, newnrl.RecipientID
    FROM tmp_newNotificationRecipientList newnrl
		LEFT JOIN NotificationRecipientList nrl
			ON nrl.NotificationID = newnrl.NotificationID
				AND nrl.BotConversationID = newnrl.BotConversationID
                AND nrl.ChatGroupTypeID = newnrl.ChatGroupTypeID
                AND nrl.RecipientID = newnrl.RecipientID
	WHERE nrl.StatusID IS NULL;
    
    #Insert new inserted-recipients     
    INSERT INTO NotificationRecipientList(NotificationID, BotConversationID
                                                                    , ChatGroupTypeID, RecipientID
                                                                    , Status, StatusID)
    SELECT tmpIns.NotificationID, tmpIns.BotConversationID
					, tmpIns.ChatGroupTypeID, tmpIns.RecipientID
                    , NEW_NOTIFICATION_STATUS
                    , NEW_NOTIFICATION_STATUSID
     FROM tmp_insertedNotificationRecipientList tmpIns;
     
    #Get log info for inserted-recipients     
    SELECT  CONCAT('[' , GROUP_CONCAT(json_object(
														'NotificationID', tmpIns.NotificationID
                                                        , 'BotConversationID', tmpIns.BotConversationID
                                                        , 'ChatGroupTypeID', tmpIns.ChatGroupTypeID
                                                        , 'RecipientID', tmpIns.RecipientID
									) SEPARATOR ', ') ,']') A
	INTO logInsertedRecipientLst
	FROM tmp_insertedNotificationRecipientList tmpIns;
    
    #Update status to 'Planned'
    UPDATE NotificationRecipientList nrl
	INNER JOIN tmp_newNotificationRecipientList tmpDel
			ON tmpDel.NotificationID = nrl.NotificationID
				AND tmpDel.BotConversationID = nrl.BotConversationID
                AND tmpDel.ChatGroupTypeID = nrl.ChatGroupTypeID
                AND tmpDel.RecipientID = nrl.RecipientID
	SET nrl.Status = NEW_NOTIFICATION_STATUS
			, nrl.StatusID = NEW_NOTIFICATION_STATUSID;
                
    #Get old Title and old Content for logging
    SELECT n.Title, n.Content, n.ConfigSendTime
    INTO logOldTitle, logOldContent, logOldConfigSendTime
    FROM Notification n
    WHERE n.NotificationID = notificationID;
    
	#Update Notification
    UPDATE Notification n
    SET  n.Title = title
			, n.Content = content
            , n.ConfigSendTime = configSendTime
            , n.Status = NEW_NOTIFICATION_STATUS
			, n.StatusID = NEW_NOTIFICATION_STATUSID
            , n.LastModifiedDate = UTC_TIMESTAMP()
            , n.LastModifiedBy = userID
     WHERE n.NotificationID = notificationID;
     
     
     #Get Finish Time to calcualte execution duration
    SET finishTime = UTC_TIMESTAMP(3);
    SET duration =  TIMESTAMPDIFF(MICROSECOND
																		, startTime
                                                                        , finishTime)  / 1000; 	#miliseconds
    
    #Get Log Content, JSON format
    SET logContent = json_object('notificationTypeID', notificationTypeID
										, 'productID', productID
                                        , 'recipientIDLst', recipientIDLst
										, 'oldTitle', logOldTitle
                                        , 'newTitle', title
										, 'oldContent', logOldContent
                                        , 'newContent', content
										, 'oldConfigSendTime', logOldConfigSendTime
                                        , 'newConfigSendTime', configSendTime
                                        , 'notificationID', notificationID
										, 'logDeletedRecipientLst', '#logDeletedRecipientLst#'
                                        , 'logInsertedRecipientLst',  '#logInsertedRecipientLst#'
					) ;
         
         SET logContent = REPLACE(logContent, '"#logDeletedRecipientLst#"', IFNULL(logDeletedRecipientLst, '[]'));
         SET logContent = REPLACE(logContent, '"#logInsertedRecipientLst#"', IFNULL(logInsertedRecipientLst, '[]'));
                    
		#Write User Log
        CALL NCB_UserLog_RunSP_Ins(
											ACTION_ID #actionID
                                            , notificationID #ReferenceID
											, userID #UserID
                                            , logContent #LogContent
                                            , startTime #logtime
                                            , duration	#Duration
                                            );
END;