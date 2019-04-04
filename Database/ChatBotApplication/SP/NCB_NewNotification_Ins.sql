DELIMITER //

DROP PROCEDURE IF EXISTS NCB_NewNotification_Ins;

CREATE PROCEDURE  NCB_NewNotification_Ins(
	IN NotificationTypeID SMALLINT
	,IN ProductID INT
	,IN RecipientIDLst VARCHAR(200)
	,IN Title VARCHAR(300)
	,IN Content MEDIUMTEXT
	,IN ConfigSendTime DATETIME
	,IN Creator INT
	,OUT ErrCode SMALLINT
	,OUT ErrMsg VARCHAR(100)
    /*
		Creator: Dominic@20190401
        Param:
				- ProductIDLst: Applied to filter licensees who are offered a specific product
        Revisions:
				- 20190401@Dominic: First Commit
        
    */
)
NCB_NewNotification_Ins:BEGIN

	#Variables used to write User Logs
	DECLARE SPStartTime 		DATETIME(3);
    DECLARE SPFinishTime 		DATETIME(3); 
    DECLARE SPDuration 			INT;							
    DECLARE SPLogContent 		TEXT;
    DECLARE ActionID 				INT DEFAULT 1;
    
    #New NotificationID 
    DECLARE NotificationID 		INT;
    
    #Get Start Time to calcualte execution duration
    SET  SPStartTime = CURRENT_TIMESTAMP(3);
    
    SET ErrCode=0, ErrMsg='';
    
	#Check param
	IF  NOT EXISTS(SELECT 1 FROM ChatBot.NotificationType  WHERE NotificationTypeID = NotificationTypeID) THEN
		SET ErrCode=1, ErrMsg='Invalid NotificationTypeID';
        LEAVE NCB_NewNotification_Ins;
    ELSEIF IFNULL(Title,'') = '' OR IFNULL(Content,'') = '' THEN
		SET ErrCode=2, ErrMsg='Title and Content must not be empty';
        LEAVE NCB_NewNotification_Ins;
	ELSEIF IFNULL(RecipientIDLst,'') = '' THEN
		SET ErrCode=3, ErrMsg='Recipient list must not be empty';
        LEAVE NCB_NewNotification_Ins;
	ELSEIF Creator IS NULL THEN
		SET ErrCode=4, ErrMsg='Creator can not be null';
        LEAVE NCB_NewNotification_Ins;
	ELSEIF ConfigSendTime IS NULL THEN
		SET ErrCode=5, ErrMsg='ConfigSendTime can not be null';
        LEAVE NCB_NewNotification_Ins;
	END IF;
	
    #Insert comma seperated list RecipientIDLst to temporary tables
    DROP TEMPORARY TABLE IF EXISTS tmp_RecipientID;
	CREATE TEMPORARY TABLE tmp_RecipientID(RecipientID INT);
	SET @SqlCommand = REPLACE(RecipientIDLst, ',', '),(');
	SET @SqlCommand = CONCAT('(', @SqlCommand, ')');
	SET @SqlCommand= CONCAT('INSERT INTO tmp_RecipientID VALUES', @SqlCommand); 
	
    PREPARE stmt FROM  @SqlCommand;
	EXECUTE stmt;
	DEALLOCATE PREPARE stmt;
    
    #Filter Recipient by offered products
    IF ProductID IS NOT NULL THEN
		DELETE main
        FROM tmp_RecipientID main
        INNER JOIN Recipient re ON re.RecipientID = main.RecipientID AND re.RecipientTypeID = 1 #RecipientType is Licensees
        LEFT JOIN LicenseeProduct lp ON lp.LicenseeID = re.SourceID AND lp.ProductID = ProductID
        WHERE LaunchDate IS NULL
        OR (LaunchDate IS NOT NULL AND ConfigSendTime < LaunchDate)
		OR (TerminationDate IS NOT NULL AND ConfigSendTime > TerminationDate);
    END IF;
    
    #Delete exception Recipient
    DELETE main
    FROM tmp_RecipientID main
    WHERE RecipientID IN (SELECT RecipientID FROM NotificationTypeRecipientException WHERE NotificationTypeID = NotificationTypeID);
    
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
    INSERT INTO NotificationRecipientList(NotificationID, BotConversationID, ChatGroupTypeID, RecipientID, Status, StatusID)
    SELECT NotificationID, IFNULL(gri.BotConversationID, -1), IFNULL(gri.ChatGroupTypeID,-1), re.RecipientID, 'Planned', 0
    FROM tmp_RecipientID re
    LEFT JOIN BotConversationRecipient gr ON re.RecipientID = gr.RecipientID
    LEFT JOIN BotConversationExtraInfo gri ON gri.BotConversationID = gr.BotConversationID
    WHERE gri.ChatGroupTypeID IN (SELECT ChatGroupTypeID FROM NotificationChatGroupType WHERE NotificationTypeID = NotificationTypeID);
	
    #Drop Unused temporary tables
     DROP TEMPORARY TABLE IF EXISTS tmp_RecipientID;
    
     #Get Finish Time to calcualte execution duration
    SET SPFinishTime = CURRENT_TIMESTAMP(3);
    SET SPDuration =  TIMESTAMPDIFF(MICROSECOND,SPStartTime,SPFinishTime)  / 1000; 	#miliseconds
    
    #Get Log Content, JSON format
    SET SPLogContent = json_object('NotificationTypeID', NotificationTypeID
										, 'ProductID', ProductID
                                        , 'RecipientIDLst', RecipientIDLst
										, 'Title', Title
                                        , 'Content', Content
                                        , 'ConfigSendTime', ConfigSendTime
                                        , 'NotificationID', NotificationID
					) ;
                    
		#Write User Log
        CALL NCB_UserLog_RunSP_Ins(
											ActionID #ActionID
											,Creator #UserID
                                            ,SPLogContent #LogContent
                                            ,SPDuration	#Duration
                                            );
END;