DELIMITER //

DROP PROCEDURE IF EXISTS NCB_SendNotification_ScanWaitingList_Sel;

CREATE PROCEDURE  NCB_SendNotification_ScanWaitingList_Sel(
    /*
		Creator: Dominic@20190417
        Param:
		Result Dataset:
				- 1st dataset: List of planned NotificationID
        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
BEGIN
	#Constant
	DECLARE NOTIFICATION_STATUSID_PLANNED	TINYINT UNSIGNED DEFAULT 0;  #Planned
    
	SELECT n.NotificationID
	FROM Notification n
	WHERE n.StatusID = NOTIFICATION_STATUSID_PLANNED
		AND n.ConfigSendTime <= UTC_TIMESTAMP()
        AND n.SendImmediately = 0;
END;