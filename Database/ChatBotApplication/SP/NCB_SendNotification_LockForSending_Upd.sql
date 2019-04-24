DELIMITER //

DROP PROCEDURE IF EXISTS NCB_SendNotification_LockForSending_Upd;

CREATE PROCEDURE  NCB_SendNotification_LockForSending_Upd(
	 IN notificationID INT
	, OUT errCode SMALLINT
	, OUT errMsg VARCHAR(100)
    /*
		Creator: Dominic@20190417
        Param:

        Revisions:
				- 20190417@Dominic: First Commit
        
    */
)
NCB_SendNotification_LockForSending_Upd:BEGIN

	#Variables used to write User Logs
	DECLARE startTime 			DATETIME(3);
    DECLARE finishTime 		DATETIME(3); 
    DECLARE duration 			INT;							
    DECLARE logContent 		TEXT;
    DECLARE ACTION_ID 		INT DEFAULT 6;
    
    #Constants
	DECLARE NOTIFICATION_STATUSID_SENDING	INT DEFAULT 3;
	DECLARE NOTIFICATION_STATUS_SENDING	VARCHAR(10) DEFAULT 'Sending';
	DECLARE NOTIFICATION_STATUSID_PLANNED	INT DEFAULT 0;
     
    
    #Get Start Time to calcualte execution duration
    SET  startTime = UTC_TIMESTAMP(3);
    
    SET errCode=0, errMsg='';
    
    #Check param
	IF  NOT EXISTS(SELECT 1 
									FROM Notification  n
                                    WHERE n.NotificationID = notificationID ) THEN
		SET errCode=601, errMsg='Invalid NotificationID';
        LEAVE NCB_SendNotification_LockForSending_Upd;
	END IF;
	
    #Update Notification Status to 'Cancelled'
    UPDATE Notification n
    SET n.Status = NOTIFICATION_STATUS_SENDING
			, n.StatusID = NOTIFICATION_STATUSID_SENDING
	WHERE n.NotificationID = notificationID
		AND n.StatusID = NOTIFICATION_STATUSID_PLANNED
        AND n.SendImmediately = 0;
	
    IF ROW_COUNT()  = 0 THEN
		SET errCode=602, errMsg='This Notification has been updated by someone else';
        LEAVE NCB_SendNotification_LockForSending_Upd;
    END IF;
    
    UPDATE NotificationRecipientList rlst
    SET rlst.Status = NOTIFICATION_STATUS_SENDING
			, rlst.StatusID = NOTIFICATION_STATUSID_SENDING
    WHERE rlst.NotificationID = notificationID;
    
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
											, 0 #UserID
                                            , logContent #LogContent
                                            , startTime #logtime
                                            , duration	#Duration
                                            );
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_SendNotification_LockForSending_Upd TO 'ncb.admin'@'%';