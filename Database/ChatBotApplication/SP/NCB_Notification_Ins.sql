DELIMITER //

DROP PROCEDURE IF EXISTS NCB_Notification_Ins;

CREATE PROCEDURE  NCB_Notification_Ins(
	IN in_NotificationTypeID SMALLINT
	,IN in_ProductID INT
	,IN in_RecipientIDLst VARCHAR(200)
	,IN in_Title VARCHAR(300)
	,IN in_Content MEDIUMTEXT
	,IN in_ConfigSendTime DATETIME
	,IN in_Creator INT
	,OUT out_ErrCode SMALLINT
	,OUT out_ErrMsg VARCHAR(100)
    /*
		Creator: Dominic@20190401
        Param:
				- ProductIDLst: Applied to filter licensees who are offered a specific product
        Revisions:
				- 20190401@Dominic: First Commit
        
    */
)
NCB_Notification_Ins:BEGIN

	#Variables used to write User Logs
	DECLARE v_SPStartTime 		DATETIME(3);
    DECLARE v_SPFinishTime 		DATETIME(3); 
    DECLARE v_SPDuration 			INT;							
    DECLARE v_SPLogContent 		TEXT;
    DECLARE v_ActionID 				INT DEFAULT 1;
    
    #New NotificationID 
    DECLARE v_NotificationID 		INT;
    
    #Get Start Time to calcualte execution duration
    SELECT  CURRENT_TIMESTAMP(3) INTO v_SPStartTime;
    
    SET out_ErrCode=0, out_ErrMsg='';
    
	#Check param
	IF  NOT EXISTS(SELECT 1 FROM ChatBot.NotificationType  WHERE NotificationTypeID = in_NotificationTypeID) THEN
		SET out_ErrCode=1, out_ErrMsg='Invalid NotificationTypeID';
        LEAVE NCB_Notification_Ins;
    ELSEIF IFNULL(in_Title,'') = '' OR IFNULL(in_Content,'') = '' THEN
		SET out_ErrCode=2, out_ErrMsg='Title and Content must not be empty';
        LEAVE NCB_Notification_Ins;
	ELSEIF IFNULL(in_RecipientIDLst,'') = '' THEN
		SET out_ErrCode=3, out_ErrMsg='Recipient list must not be empty';
        LEAVE NCB_Notification_Ins;
	ELSEIF in_Creator IS NULL THEN
		SET out_ErrCode=4, out_ErrMsg='Creator can not be null';
        LEAVE NCB_Notification_Ins;
	ELSEIF in_ConfigSendTime IS NULL THEN
		SET out_ErrCode=5, out_ErrMsg='ConfigSendTime can not be null';
        LEAVE NCB_Notification_Ins;
	END IF;
	
    #Insert comma seperated list RecipientIDLst to temporary tables
     CREATE TEMPORARY TABLE tbl_RecipientID(RecipientID INT);
	SET @SQLCommand = REPLACE(in_RecipientIDLst, ',', '),(');
	SET @SQLCommand = CONCAT('(', @SQLCommand, ')');
	SET @SQLCommand= CONCAT('INSERT INTO tbl_RecipientID VALUES', @SQLCommand); 
	
    PREPARE stmt FROM  @SQLCommand;
	EXECUTE stmt;
	DEALLOCATE PREPARE stmt;
    
    #Filter Recipient by offered products
    IF in_ProductID IS NOT NULL THEN
		DELETE main
        FROM tbl_RecipientID main
        INNER JOIN Recipient re ON re.RecipientID = main.RecipientID AND re.RecipientTypeID = 1
        LEFT JOIN LicenseeProduct lp ON lp.LicenseeID = re.SourceID AND lp.ProductID = in_ProductID
        WHERE LaunchDate IS NULL
        OR (LaunchDate IS NOT NULL AND in_ConfigSendTime < LaunchDate)
		OR (TerminationDate IS NOT NULL AND in_ConfigSendTime > TerminationDate);
    END IF;
    
    #Insert a new Notification
    INSERT INTO Notification(NotificationTypeID
											,Title
                                            ,Content
                                            ,ConfigSendTime
                                            ,CreatorID
                                            ,CreatedDate
                                            ,StatusID)
    SELECT in_NotificationTypeID
					, in_Title
                    , in_Content
                    , in_ConfigSendTime
                    , in_Creator
                    , NOW()
                    , 0;
    SET v_NotificationID = LAST_INSERT_ID();
    
    #Insert recipient list
    INSERT INTO NotificationRecipientList(NotificationID, BotConversationID, ChatGroupTypeID, RecipientID, Status, StatusID)
    SELECT v_NotificationID, gri.BotConversationID, gri.ChatGroupTypeID, re.RecipientID, 'Planned', 0
    FROM tbl_RecipientID re
    INNER JOIN BotConversationRecipient gr ON re.RecipientID = gr.RecipientID
    INNER JOIN BotConversationExtraInfo gri ON gri.BotConversationID = gr.BotConversationID
    WHERE gri.ChatGroupTypeID IN (SELECT ChatGroupTypeID FROM NotificationChatGroupType WHERE NotificationTypeID = in_NotificationTypeID);
	
    #Drop Unused temporary tables
    DROP TEMPORARY TABLE tbl_RecipientID;
    
     #Get Finish Time to calcualte execution duration
    SELECT  CURRENT_TIMESTAMP(3) INTO v_SPFinishTime;
    SET v_SPDuration =  TIMESTAMPDIFF(MICROSECOND,v_SPStartTime,v_SPFinishTime)  / 1000; 	#miliseconds
    
    #Get Log Content, JSON format
    SET v_SPLogContent = json_object('in_NotificationTypeID', in_NotificationTypeID
										, 'in_ProductID', in_ProductID
                                        , 'in_RecipientIDLst', in_RecipientIDLst
										, 'in_Title', in_Title
                                        , 'in_Content', in_Content
                                        , 'in_ConfigSendTime', in_ConfigSendTime
                                        , 'v_NotificationID', v_NotificationID
					) ;
                    
		#Write User Log
        CALL NCB_UserLog_RunSP_Ins(
											v_ActionID #ActionID
											,in_Creator #UserID
                                            ,v_SPLogContent #LogContent
                                            ,v_SPDuration	#Duration
                                            );
END;