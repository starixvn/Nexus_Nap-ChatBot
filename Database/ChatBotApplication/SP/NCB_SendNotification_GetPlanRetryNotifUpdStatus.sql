DELIMITER //

DROP PROCEDURE IF EXISTS NCB_SendNotification_GetPlanRetryNotifUpdStatus;

CREATE PROCEDURE  NCB_SendNotification_GetPlanRetryNotifUpdStatus(
    /*
		Creator: Dominic@20190417
        Param:
		Result Dataset:
				 #1st dataset: Main Notification Info such as title, content
				 #2nd dataset: List of recipient,bot,conversation
        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
NCB_SendNotification_GetPlanRetryNotifUpdStatus:BEGIN
	#Constant
	DECLARE NOTIFICATION_STATUSID_PLANNED	TINYINT UNSIGNED DEFAULT 0;  #Planned
    DECLARE NOTIFICATION_STATUSID_SENDING	INT DEFAULT 3;
	DECLARE NOTIFICATION_STATUS_SENDING	VARCHAR(10) DEFAULT 'Sending';
    DECLARE NOTIFICATION_STATUSID_RETRYING	INT DEFAULT 8;
	DECLARE NOTIFICATION_STATUS_RETRYING	VARCHAR(10) DEFAULT 'Retrying';
    
	DROP TEMPORARY TABLE IF EXISTS tmp_PlannedNotification;
	CREATE TEMPORARY TABLE tmp_PlannedNotification(NotificationID INT, LockSuccess BIT);
    
    #Get planned notifications
    INSERT INTO tmp_PlannedNotification(NotificationID, LockSuccess)
	SELECT n.NotificationID, 0
	FROM Notification n
	WHERE n.StatusID = NOTIFICATION_STATUSID_PLANNED
		AND n.ConfigSendTime <= UTC_TIMESTAMP()
        AND n.SendImmediately = 0;
     
     #Lock planned notifications to send
	 UPDATE Notification AS n
     INNER JOIN  tmp_PlannedNotification AS t ON n.NotificationID = t.NotificationID
     SET n.Status = NOTIFICATION_STATUS_SENDING
			, n.StatusID = NOTIFICATION_STATUSID_SENDING
            , t.LockSuccess = 1
	 WHERE n.StatusID = NOTIFICATION_STATUSID_PLANNED;
		
	DROP TEMPORARY TABLE IF EXISTS tmp_TryingNotification;
	CREATE TEMPORARY TABLE tmp_TryingNotification(NotificationID INT, RecipientID INT, ConversationGroupID INT
																					, LockSuccess BIT);
    
    #Get Trying notifications
    INSERT INTO tmp_TryingNotification(NotificationID, RecipientID, ConversationGroupID, LockSuccess)
	SELECT n.NotificationID, n.RecipientID, n.ConversationGroupID, 0
	FROM NotificationRecipientList n
	WHERE n.StatusID = NOTIFICATION_STATUSID_RETRYING;
    
    #Lock Trying notifications to send
	 UPDATE NotificationRecipientList AS n
     INNER JOIN  tmp_TryingNotification AS t 
			ON n.NotificationID = t.NotificationID AND n.RecipientID = t.RecipientID  AND n.ConversationGroupID = t.ConversationGroupID
     SET n.Status = NOTIFICATION_STATUS_SENDING
			, n.StatusID = NOTIFICATION_STATUSID_SENDING
            , t.LockSuccess = 1
	 WHERE n.StatusID = NOTIFICATION_STATUSID_RETRYING;
    
    #1st dataset: Main Notification Info such as title, content
	SELECT n.NotificationID, n.Title, n.Content
    FROM Notification n
    WHERE n.NotificationID IN (
		SELECT t1.NotificationID FROM tmp_PlannedNotification t1
        UNION ALL
        SELECT t1.NotificationID FROM tmp_TryingNotification t1
    );
    
    #2nd dataset: List of recipient,bot,conversation
	SELECT nrl.NotificationID AS NotificationID
					, r.RecipientID, r.RecipientName
					, GroupBot.ConversationGroupID AS ChatGroupID
					, grp.ConversationID AS ChatGroupConversationID 
                    , bot.ConversationBotID AS BotID 
                    , bot.Name AS BotName
                    , bot.ConversationID AS BotConversationID 
	 FROM NotificationRecipientList nrl
	 INNER JOIN ConversationGroup grp
			ON grp.ConversationGroupID = nrl.ConversationGroupID
     INNER JOIN Recipient r ON r.RecipientID = nrl.RecipientID
     INNER JOIN ConversationGroupBot GroupBot 
			ON GroupBot.ConversationGroupID = nrl.ConversationGroupID
	 INNER JOIN (
				SELECT grb.ConversationGroupID, MIN(grb.CreatedTime)  MaxTime
				FROM ConversationGroupBot grb 
				GROUP BY grb.ConversationGroupID
			) tmp ON GroupBot.ConversationGroupID = tmp.ConversationGroupID AND GroupBot.CreatedTime = tmp. MaxTime
	INNER JOIN ConversationBot bot ON bot.ConversationBotID = GroupBot.ConversationBotID
	WHERE nrl.NotificationID IN (SELECT t1.NotificationID FROM tmp_PlannedNotification t1)
    #Trying Nofitication List
    UNION ALL
    SELECT nrl.NotificationID AS NotificationID
					, r.RecipientID, r.RecipientName
					, GroupBot.ConversationGroupID AS ChatGroupID
					, grp.ConversationID AS ChatGroupConversationID 
                    , bot.ConversationBotID AS BotID 
                    , bot.Name AS BotName
                    , bot.ConversationID AS BotConversationID 
	 FROM NotificationRecipientList nrl
     INNER JOIN tmp_TryingNotification tnlst
			ON nrl.NotificationID = tnlst.NotificationID AND nrl.RecipientID = tnlst.RecipientID  AND nrl.ConversationGroupID = tnlst.ConversationGroupID
	 INNER JOIN ConversationGroup grp
			ON grp.ConversationGroupID = nrl.ConversationGroupID
     INNER JOIN Recipient r ON r.RecipientID = nrl.RecipientID
     INNER JOIN ConversationGroupBot GroupBot 
			ON GroupBot.ConversationGroupID = nrl.ConversationGroupID
	 INNER JOIN (
				SELECT grb.ConversationGroupID, MIN(grb.CreatedTime)  MaxTime
				FROM ConversationGroupBot grb 
				GROUP BY grb.ConversationGroupID
			) tmp ON GroupBot.ConversationGroupID = tmp.ConversationGroupID AND GroupBot.CreatedTime = tmp. MaxTime
	INNER JOIN ConversationBot bot ON bot.ConversationBotID = GroupBot.ConversationBotID;
    
    
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_SendNotification_GetPlanRetryNotifUpdStatus TO 'ncb.admin'@'%';