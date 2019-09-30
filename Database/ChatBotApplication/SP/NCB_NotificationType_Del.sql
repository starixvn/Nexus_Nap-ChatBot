DROP PROCEDURE IF EXISTS NCB_NotificationType_Del;
DELIMITER //

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE NCB_NotificationType_Del(
	IN notificationTypeId INT,    
    IN userId INT,
    OUT errCode SMALLINT,
	OUT errMsg VARCHAR(100)
	/*
		Creator: Scofield@20190912
				Params:
						- notificationTypeId: notification type id
				Revisions:
						- 20190912@Scofield: Create
    */
   )
SQL SECURITY DEFINER
BEGIN	     
	-- Variables used to write User Logs
	DECLARE startTime 			DATETIME(3);
    DECLARE finishTime 		DATETIME(3); 
    DECLARE duration 			INT;							
    DECLARE logContent 		TEXT CHARACTER SET utf8mb4;
    DECLARE ACTION_ID 		INT DEFAULT 17;
	
	-- Get Start Time to calcualte execution duration
    SET startTime = UTC_TIMESTAMP(3);
    SET errCode = 0, errMsg = '';
    
    IF (EXISTS(SELECT * FROM Notification WHERE Notification.NotificationTypeID =  notificationTypeId) 
		OR EXISTS(SELECT * FROM NotificationTemplate WHERE NotificationTemplate.NotificationTypeID =  notificationTypeId)) THEN
			SELECT 1 INTO errCode;
			SELECT "Unable to delete the notification type because one or many assigned notification(s) exist(s)" INTO errMsg;
    ELSE
		DELETE FROM NotificationType
		WHERE NotificationType.notificationTypeId = notificationTypeId;
		
		-- Get Finish Time to calcualte execution duration
		SET finishTime = UTC_TIMESTAMP(3);
		SET duration =  TIMESTAMPDIFF(MICROSECOND, startTime,finishTime) / 1000;
		
		-- Get Log Content, JSON format
		SET logContent = json_object('notificationTypeId', notificationTypeId);
						
		-- Write User Log
		CALL NCB_UserLog_RunSP_Ins(ACTION_ID, notificationTypeId, userId, logContent, startTime, duration);
    END IF;
END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE NCB_NotificationType_Del TO 'ncb.admin'@'%';