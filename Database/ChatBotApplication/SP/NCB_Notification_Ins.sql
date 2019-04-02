DELIMITER //

DROP PROCEDURE IF EXISTS NCB_Notification_Ins;

CREATE PROCEDURE  NCB_Notification_Ins(
	IN in_NotificationTypeID SMALLINT
	,IN in_ProductIDLst VARCHAR(200)
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
    DECLARE v_StringSplitValue	VARCHAR(1000);
    
    #New NotificationID 
    DECLARE v_NotificationID 		INT;
    
    #Create necessary temporary tables
    CREATE TEMPORARY TABLE tbl_ProductID(ProductID INT);
    CREATE TEMPORARY TABLE tbl_RecipientID(RecipientID INT);
    
    #Get Start Time to calcualte execution duration
    SELECT  CURRENT_TIMESTAMP(3) INTO v_SPStartTime;
    
	#Check param
	IF  NOT EXISTS(SELECT 1 FROM ChatBot.NotificationType  WHERE NotificationTypeID = in_NotificationTypeID) THEN
		SET out_ErrCode=1, out_ErrMsg='Invalid NotificationTypeID';
        LEAVE NCB_Notification_Ins;
    ELSEIF IFNULL(in_Title,'') = '' OR IFNULL(in_Content,'') = '' THEN
		SET out_ErrCode=2, out_ErrMsg='Title and Content must not be empty';
        LEAVE NCB_Notification_Ins;
	ELSEIF in_Creator IS NULL THEN
		SET out_ErrCode=3, out_ErrMsg='Creator can not be null';
        LEAVE NCB_Notification_Ins;
	ELSEIF in_ConfigSendTime IS NULL THEN
		SET out_ErrCode=4, out_ErrMsg='ConfigSendTime can not be null';
        LEAVE NCB_Notification_Ins;
	END IF;

    
    #Drop Unused temporary tables
    DROP TEMPORARY TABLE tbl_ProductID;
    DROP TEMPORARY TABLE tbl_RecipientID;
    
     #Get Finish Time to calcualte execution duration
    SELECT  CURRENT_TIMESTAMP(3) INTO v_SPFinishTime;
    SET v_SPDuration =  TIMESTAMPDIFF(MICROSECOND,v_SPStartTime,v_SPFinishTime)  / 1000; 	#miliseconds
    
    #Get Log Content, JSON format
    SET v_SPLogContent = json_object('in_NotificationTypeID', in_NotificationTypeID
										, 'in_ProductIDLst', in_ProductIDLst
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