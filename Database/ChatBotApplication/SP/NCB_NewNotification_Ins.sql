DELIMITER //

DROP PROCEDURE IF EXISTS NCB_NewNotification_Ins;

CREATE PROCEDURE  NCB_NewNotification_Ins(
	IN NotificationTypeID SMALLINT
	, IN ProductID INT
	, IN RecipientIDLst VARCHAR(200)
	, IN Title VARCHAR(300)
	, IN Content MEDIUMTEXT
	, IN ConfigSendTime DATETIME
	, IN Creator INT
    , OUT NotificationID INT
	, OUT ErrCode SMALLINT
	, OUT ErrMsg VARCHAR(100)
    /*
		Creator: Dominic@20190401
        Param:
				- ProductID: Applied to filter licensees who are offered a specific product
                - NotificationID: ID of created Notification
                - ErrCode:  101:
        Revisions:
				- 20190401@Dominic: First Commit
        
    */
)
NCB_NewNotification_Ins:BEGIN

	#Variables used to write User Logs
	DECLARE startTime 		DATETIME(3);
    DECLARE finishTime 		DATETIME(3); 
    DECLARE duration 			INT;							
    DECLARE logContent 		TEXT;
    DECLARE actionID 				INT DEFAULT 1;
    
    #Get Start Time to calcualte execution duration
    SET  startTime = CURRENT_TIMESTAMP(3);
    
    SET ErrCode=0, ErrMsg='';
    
	#Check param
	IF  NOT EXISTS(SELECT 1 
									FROM ChatBot.NotificationType  nt
                                    WHERE nt.NotificationTypeID = NotificationTypeID ) THEN
		SET ErrCode=101, ErrMsg='Invalid NotificationTypeID';
        LEAVE NCB_NewNotification_Ins;
    ELSEIF IFNULL(Title,'') = '' OR IFNULL(Content,'') = '' THEN
		SET ErrCode=102, ErrMsg='Title and Content must not be empty';
        LEAVE NCB_NewNotification_Ins;
	ELSEIF IFNULL(RecipientIDLst,'') = '' THEN
		SET ErrCode=103, ErrMsg='Recipient list must not be empty';
        LEAVE NCB_NewNotification_Ins;
	ELSEIF Creator IS NULL THEN
		SET ErrCode=104, ErrMsg='Creator can not be null';
        LEAVE NCB_NewNotification_Ins;
	ELSEIF ConfigSendTime IS NULL THEN
		SET ErrCode=105, ErrMsg='ConfigSendTime can not be null';
        LEAVE NCB_NewNotification_Ins;
	END IF;
	
    #Insert comma seperated list RecipientIDLst to temporary tables
    DROP TEMPORARY TABLE IF EXISTS tmp_recipientIDLst;
	CREATE TEMPORARY TABLE tmp_recipientIDLst(RecipientID INT);
    
	SET @sqlCommand = REPLACE(RecipientIDLst, ',', '),(');
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
		SET ErrCode=106;

		SELECT GROUP_CONCAT( DISTINCT re.RecipientName  ORDER BY re.RecipientName SEPARATOR ', ') 
        INTO ErrMsg
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

        LEAVE NCB_NewNotification_Ins;
    END IF;

    #Check InActive Licensees
     IF EXISTS(
						SELECT 1 
						FROM tmp_recipientIDLst main
                        INNER JOIN Recipient re 
							ON re.RecipientID = main.RecipientID 
								AND re.RecipientTypeID = 1 #RecipientType is Licensees
                        INNER JOIN Licensee l
							ON l.LicenseeId = re.SourceID
						WHERE  l.LicenseeStatusId <> 3
	) THEN
		SET ErrCode=107;
        
        SELECT GROUP_CONCAT(re.RecipientName ORDER BY re.RecipientName  SEPARATOR ', ')  
        INTO ErrMsg
        FROM Recipient re
        WHERE re.RecipientID IN (
						SELECT main. RecipientID
						FROM tmp_recipientIDLst main
                        INNER JOIN Recipient re 
							ON re.RecipientID = main.RecipientID 
								AND re.RecipientTypeID = 1 #RecipientType is Licensees
                        INNER JOIN Licensee l
							ON l.LicenseeId = re.SourceID
						WHERE  l.LicenseeStatusId <> 3
			);

        LEAVE NCB_NewNotification_Ins;
    END IF;
    
    #Check Licensees do not offered a product
    IF ProductID IS NOT NULL 
		AND EXISTS(
						SELECT 1 
						FROM tmp_recipientIDLst main
						INNER JOIN Recipient re 
							ON re.RecipientID = main.RecipientID 
								AND re.RecipientTypeID = 1 #RecipientType is Licensees
						LEFT JOIN LicenseeProduct lp 
							ON lp.LicenseeID = re.SourceID 
								AND lp.ProductID = ProductID
						WHERE  lp.LaunchDate IS NULL
							OR (
										 lp.LaunchDate IS NOT NULL 
										AND ConfigSendTime <  lp.LaunchDate
									)
							OR (
										lp.TerminationDate IS NOT NULL 
										AND ConfigSendTime > lp.TerminationDate
									)
	) THEN
		SET ErrCode=108;
        
		SELECT GROUP_CONCAT(re.RecipientName ORDER BY re.RecipientName SEPARATOR ', ') 
        INTO ErrMsg
        FROM Recipient re
        WHERE re.RecipientID IN (
						SELECT main. RecipientID
						FROM tmp_recipientIDLst main
						INNER JOIN Recipient re 
							ON re.RecipientID = main.RecipientID 
								AND re.RecipientTypeID = 1 #RecipientType is Licensees
						LEFT JOIN LicenseeProduct lp 
							ON lp.LicenseeID = re.SourceID 
								AND lp.ProductID = ProductID
						WHERE  lp.LaunchDate IS NULL
							OR (
										 lp.LaunchDate IS NOT NULL 
										AND ConfigSendTime <  lp.LaunchDate
									)
							OR (
										lp.TerminationDate IS NOT NULL 
										AND ConfigSendTime > lp.TerminationDate
									)
			);

        LEAVE NCB_NewNotification_Ins;
    END IF;
    
    #Delete exception Recipient
    DELETE main
    FROM tmp_recipientIDLst main
    WHERE main.RecipientID IN (
					SELECT ex.RecipientID 
					FROM NotificationTypeRecipientException ex
					WHERE ex.NotificationTypeID = NotificationTypeID
	);
    
    #Insert a new Notification
    INSERT INTO Notification(NotificationTypeID
											,Title
                                            ,Content
                                            ,ConfigSendTime
                                            ,CreatorID
                                            ,CreatedDate
                                            ,StatusID)
    SELECT NotificationTypeID
					, Title
                    , Content
                    , ConfigSendTime
                    , Creator
                    , NOW()
                    , 0;
    SET NotificationID = LAST_INSERT_ID();
    
    #Insert recipient list
    INSERT INTO NotificationRecipientList(NotificationID, BotConversationID
                                                                    , ChatGroupTypeID, RecipientID
                                                                    , Status, StatusID)
    SELECT NotificationID, IFNULL(gri.BotConversationID, -1)
					, IFNULL(gri.ChatGroupTypeID,-1), re.RecipientID
                    , 'Planned', 0
    FROM tmp_recipientIDLst re
		LEFT JOIN BotConversationRecipient gr 
			ON re.RecipientID = gr.RecipientID
		LEFT JOIN BotConversationExtraInfo gri 
			ON gri.BotConversationID = gr.BotConversationID
    WHERE gri.ChatGroupTypeID IN (
						SELECT cgt.ChatGroupTypeID 
						FROM NotificationChatGroupType  cgt
                        WHERE cgt.NotificationTypeID = NotificationTypeID
	);
	
    #Drop Unused temporary tables
     DROP TEMPORARY TABLE IF EXISTS tmp_recipientIDLst;
    
     #Get Finish Time to calcualte execution duration
    SET finishTime = CURRENT_TIMESTAMP(3);
    SET duration =  TIMESTAMPDIFF(MICROSECOND
																		, startTime
                                                                        , finishTime)  / 1000; 	#miliseconds
    
    #Get Log Content, JSON format
    SET logContent = json_object('NotificationTypeID', NotificationTypeID
										, 'ProductID', ProductID
                                        , 'RecipientIDLst', RecipientIDLst
										, 'Title', Title
                                        , 'Content', Content
                                        , 'ConfigSendTime', ConfigSendTime
                                        , 'NotificationID', NotificationID
					) ;
                    
		#Write User Log
        CALL NCB_UserLog_RunSP_Ins(
											actionID #actionID
											,Creator #UserID
                                            ,logContent #LogContent
                                            ,duration	#Duration
                                            );
END;