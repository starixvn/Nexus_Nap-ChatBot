DELIMITER //

DROP PROCEDURE IF EXISTS NCB_SendNotification_Sel;

CREATE PROCEDURE  NCB_SendNotification_Sel(
     IN notificationID INT
    /*
		Creator: Dominic@20190417
        Param:
        
		Result Dataset:
				- 1st dataset: Main Notification Info such as title, content
                - 2nd dataset: List of ConversationID and Bot MemberID
        Revisions:
				- 20190417@Dominic: First Commit
        
    */
)
BEGIN

	#1st dataset: Main Notification Info such as title, content
	SELECT n.Title, n.Content
    FROM Notification n
    WHERE n.NotificationID = notificationID;

	#2nd dataset: List of ConversationID and Bot MemberID
	SELECT r.RecipientID, r.RecipientName
					, GroupBot.ConversationGroupID ChatGroupID
					, grp.ConversationID AS ChatGroupConversationID 
                    , bot.ConversationBotID BotID 
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
	WHERE nrl.NotificationID = notificationID;
    
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_SendNotification_Sel TO 'ncb.admin'@'%';