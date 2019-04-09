DELIMITER //

DROP PROCEDURE IF EXISTS NCB_EditNotification_Upd;

CREATE PROCEDURE  NCB_EditNotification_Upd(
	IN notificationID INT
	, IN notificationTypeID SMALLINT
	, IN productID INT
	, IN recipientIDLst VARCHAR(200)
	, IN title VARCHAR(300)
	, IN content MEDIUMTEXT
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
    DECLARE logContent 		TEXT;
    DECLARE ACTION_ID 		INT DEFAULT 5;
    
    #constants
    DECLARE RECIPIENT_TYPE_LICENSEES 	SMALLINT DEFAULT 1;
    DECLARE LICENSEE_STATUS_ACTIVE 		TINYINT UNSIGNED DEFAULT 3;
    DECLARE NEW_NOTIFICATION_STATUSID	TINYINT UNSIGNED DEFAULT 0;
    DECLARE NEW_NOTIFICATION_STATUS	VARCHAR(10) DEFAULT 'Planned';
	DECLARE NOTIFICATION_STATUSID_CANCELLED	INT DEFAULT 4;
	DECLARE NOTIFICATION_STATUS_CANCELLED	VARCHAR(10) DEFAULT 'Cancelled';
    
     #Local variables
    DECLARE configSendTimeGMTPlus4 DATETIME;
    
    #Get Start Time to calcualte execution duration
    SET  startTime = UTC_TIMESTAMP(3);
    SET errCode=0, errMsg='';
    
	#Check param
	IF  NOT EXISTS(	SELECT 1 
									FROM Notification n
                                    WHERE n.NotificationID = notificationID
										AND n.StatusID = NOTIFICATION_STATUSID_CANCELLED
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
    
    #Insert recipient list
    update/delete NotificationRecipientList
    INSERT INTO NotificationRecipientList(NotificationID, BotConversationID
                                                                    , ChatGroupTypeID, RecipientID
                                                                    , Status, StatusID)
    SELECT NotificationID, IFNULL(gri.BotConversationID, -1)
					, IFNULL(gri.ChatGroupTypeID,-1), re.RecipientID
                    , NEW_NOTIFICATION_STATUS
                    , NEW_NOTIFICATION_STATUSID
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
    
    
      #Update Notification
    UPDATE Notification n
    SET  n.Title = title
			, n.Content = content
            , n.ConfigSendTime = configSendTime
            , n.Status = NEW_NOTIFICATION_STATUS
			, n.StatusID = NEW_NOTIFICATION_STATUSID
            #, n.CreatedDate = UTC_TIMESTAMP()
            #, n.CreatorID = userID
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
										, 'title', title
                                        , 'content', content
                                        , 'configSendTime', configSendTime
                                        , 'notificationID', notificationID
					) ;
                    
		#Write User Log
        CALL NCB_UserLog_RunSP_Ins(
											ACTION_ID #actionID
											, creatorID #UserID
                                            , logContent #LogContent
                                            , startTime #logtime
                                            , duration	#Duration
                                            );
END;