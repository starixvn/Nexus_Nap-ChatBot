DROP PROCEDURE IF EXISTS NCB_NewNotification_GetTemplates;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_NewNotification_GetTemplates`(
	IN notificationTypeID SMALLINT
	/*
        Revisions:
			- 20210513@Malcolm: Integrate Telegram - RedmineID: #151807
    */
)
NCB_NewNotification_GetTemplates:BEGIN
	SELECT nt.NotificationTemplateId
		,nt.NotificationTemplateName
        ,nt.Title
        ,ntc.Content
    FROM NotificationTemplate nt
		INNER JOIN NotificationTemplateContent ntc ON ntc.NotificationTemplateID = nt.NotificationTemplateID
        INNER JOIN ConversationChannel c ON c.ChannelID = ntc.ChannelID
    WHERE nt.NotificationTypeID = notificationTypeID AND c.ChannelName = 'Skype'
    ORDER BY nt.NotificationTemplateName;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_NewNotification_GetTemplates TO 'ncb.admin'@'%';