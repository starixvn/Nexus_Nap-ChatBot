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
    
    DROP TEMPORARY TABLE IF EXISTS tmp_ConversationIDBot;
	CREATE TEMPORARY TABLE tmp_ConversationIDBot(
																					ConversationID VARCHAR(100)
                                                                                    , BotID INT
																		);
     
     INSERT INTO tmp_ConversationIDBot (ConversationID, BotID)
	SELECT tmp.ConversationID, MAX(tmp.BotID) BotID
    FROM (
		SELECT nrl.ConversationID AS ChatGroupConversationID , MIN(bot.ID) BotID
		FROM NotificationRecipientList nrl
			INNER JOIN BotConversationMember bot
				ON bot.ConversationID = nrl.ConversationID
					AND bot.IsBot = 1
		WHERE nrl.NotificationID = notificationID
		GROUP BY nrl.ConversationID
		UNION ALL
		SELECT nrl.ConversationID AS ChatGroupConversationID , MIN(bot.ID) BotID
		FROM NotificationRecipientList nrl
			INNER JOIN BotConversationMember bot
				ON bot.ConversationID = nrl.ConversationID
					AND bot.IsBot = 1 AND bot.IsActive = 1
		WHERE nrl.NotificationID = notificationID
		GROUP BY nrl.ConversationID
    ) AS tmp
    GROUP BY tmp.ConversationID;

	#2nd dataset: List of ConversationID and Bot MemberID
    SELECT tmp.ConversationID AS ChatGroupID
				, bot.MemberID AS BotMemberID
    FROM tmp_ConversationIDBot tmp
		INNER JOIN BotConversationMember bot
			ON bot.ID = tmp.BotID;
END;