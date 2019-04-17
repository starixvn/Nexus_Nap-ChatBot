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
																					ChatGroupConversationID VARCHAR(100)
                                                                                    , BotID INT
																		);
     
     INSERT INTO tmp_ConversationIDBot (ChatGroupConversationID, BotID)
	SELECT tmp.ChatGroupConversationID, MAX(tmp.BotID) BotID
    FROM (
		SELECT grp.ConversationID AS ChatGroupConversationID , MIN(bot.ID) BotID
		FROM NotificationRecipientList nrl
			INNER JOIN BotConversation grp
				ON grp.ID = nrl.BotConversationID
			INNER JOIN BotConversationMember bot
				ON bot.ConversationID = grp.ConversationID
					AND bot.IsBot = 1
		WHERE nrl.NotificationID = notificationID
		GROUP BY grp.ConversationID
        UNION ALL
        SELECT grp.ConversationID AS ChatGroupConversationID , MIN(bot.ID) BotID
		FROM NotificationRecipientList nrl
			INNER JOIN BotConversation grp
				ON grp.ID = nrl.BotConversationID
			INNER JOIN BotConversationMember bot
				ON bot.ConversationID = grp.ConversationID
					AND bot.IsBot = 1 AND bot.IsActive = 1
		WHERE nrl.NotificationID = notificationID
		GROUP BY grp.ConversationID
    ) AS tmp
    GROUP BY tmp.ChatGroupConversationID;

	#2nd dataset: List of ConversationID and Bot MemberID
    SELECT tmp.ChatGroupConversationID AS ChatGroupID
				, bot.MemberID AS BotMemberID
    FROM tmp_ConversationIDBot tmp
		INNER JOIN BotConversationMember bot
			ON bot.ID = tmp.BotID;
END;