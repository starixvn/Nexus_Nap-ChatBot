DROP PROCEDURE IF EXISTS NCB_Common_NotificationTemplate_Sel;
DELIMITER //

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  NCB_Common_NotificationTemplate_Sel(
	IN notificationTypeID SMALLINT
    /*
		Creator: Dominic@20190401
        Param:

        Revisions:
				- 20190401@Dominic: First Commit
				- 20210513@Malcolm: Integrate Telegram - RedmineID: #151807
        
    */
)
SQL SECURITY DEFINER
BEGIN

	SELECT nt.NotificationTemplateID AS ID, nt.NotificationTemplateName AS Name, nt.Title, ntc.Content
    FROM NotificationTemplate nt
		INNER JOIN NotificationTemplateContent ntc ON ntc.NotificationTemplateID = nt.NotificationTemplateID
        INNER JOIN ConversationChannel c ON c.ChannelID = ntc.ChannelID
    WHERE nt.NotificationTypeID = notificationTypeID AND c.ChannelName = 'Skype'
    ORDER BY nt.NotificationTemplateName;
    
END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE NCB_Common_NotificationTemplate_Sel TO 'ncb.admin'@'%';