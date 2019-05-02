DELIMITER //

DROP PROCEDURE IF EXISTS NCB_SendNotification_UnLockAfterSending_Upd;

CREATE PROCEDURE  NCB_SendNotification_UnLockAfterSending_Upd(
	 IN notificationID INT
	, IN userID INT
	, OUT errCode SMALLINT
	, OUT errMsg VARCHAR(100)
    /*
		Creator: Dominic@20190417
        Param:

        Revisions:
				- 20190417@Dominic: First Commit
        
    */
)
NCB_SendNotification_UnLockAfterSending_Upd:BEGIN

	#Variables used to write User Logs
	DECLARE startTime 			DATETIME(3);
    DECLARE finishTime 		DATETIME(3); 
    DECLARE duration 			INT;							
    DECLARE logContent 		TEXT;
    DECLARE ACTION_ID 		INT DEFAULT 8;
    DECLARE hasError			BIT DEFAULT 0;
    
    #Constants
	DECLARE NOTIFICATION_STATUSID_SENDING	INT DEFAULT 3;
	DECLARE NOTIFICATION_STATUS_SENDING	VARCHAR(10) DEFAULT 'Sending';
	
	DECLARE NOTIFICATION_STATUSID_SENT	INT DEFAULT 1;
	DECLARE NOTIFICATION_STATUS_SENT	VARCHAR(10) DEFAULT 'Sent';
    
	DECLARE NOTIFICATION_STATUSID_ERROR	INT DEFAULT 2;
	DECLARE NOTIFICATION_STATUS_ERROR	VARCHAR(10) DEFAULT 'Error';
    
    
     
    
    #Get Start Time to calcualte execution duration
    SET  startTime = UTC_TIMESTAMP(3);
    
    SET errCode=0, errMsg='';
    
    #Check param
	IF  NOT EXISTS(SELECT 1 
									FROM Notification  n
                                    WHERE n.NotificationID = notificationID) THEN
		SET errCode=801, errMsg='Invalid NotificationID';
        LEAVE NCB_SendNotification_UnLockAfterSending_Upd;
     ELSEIF userID IS NULL THEN
		SET errCode=802, errMsg='UserID can not be null';
        LEAVE NCB_SendNotification_UnLockAfterSending_Upd;
	END IF;
	
    IF EXISTS(SELECT 1 
						FROM NotificationRecipientList rlst
                        WHERE rlst.NotificationID = notificationID
							AND rlst.StatusID NOT IN (
									NOTIFICATION_STATUSID_SENT
                                    , NOTIFICATION_STATUSID_ERROR
								)
                        )
	THEN
			SET errCode=803, errMsg='This Notification is being sent, can not update';
			LEAVE NCB_SendNotification_UnLockAfterSending_Upd;
	END IF;
    
    IF EXISTS(SELECT 1 
						FROM NotificationRecipientList rlst
                        WHERE rlst.NotificationID = notificationID
							AND rlst.StatusID = NOTIFICATION_STATUSID_ERROR
                        )
	THEN
			SET hasError = 1;
	END IF;
    
    #Update Notification Status to 'Cancelled'
    UPDATE Notification n
    SET n.Status = CASE 
												WHEN hasError = 1 THEN NOTIFICATION_STATUS_ERROR
                                                ELSE NOTIFICATION_STATUS_SENT
									END
			, n.StatusID = CASE 
												WHEN hasError = 1 THEN NOTIFICATION_STATUSID_ERROR
                                                ELSE NOTIFICATION_STATUSID_SENT
									END
	WHERE n.NotificationID = notificationID
		AND n.StatusID = NOTIFICATION_STATUSID_SENDING;
	
    IF ROW_COUNT()  = 0 THEN
		SET errCode=804, errMsg='This Notification has been updated by someone else';
        LEAVE NCB_SendNotification_UnLockAfterSending_Upd;
    END IF;
    
     #Get Finish Time to calcualte execution duration
    SET finishTime = UTC_TIMESTAMP(3);
    SET duration =  TIMESTAMPDIFF(MICROSECOND
																		, startTime
                                                                        , finishTime)  / 1000; 	#miliseconds
    
    #Get Log Content, JSON format
    SET logContent = json_object(
							'notificationID', notificationID
					) ;
                    
		#Write User Log
        CALL NCB_UserLog_RunSP_Ins(
											ACTION_ID #actionID
                                            , notificationID #referenceID
											, userID #UserID
                                            , logContent #LogContent
                                            , startTime #logtime
                                            , duration	#Duration
                                            );
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_SendNotification_UnLockAfterSending_Upd TO 'ncb.admin'@'%';