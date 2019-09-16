CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_NotificationTemplate_Sel`(
	IN notificationTemplateID INT
	/*
		Creator: Elton@20190913
        Revisions:
				- 20190910@Elton: First Commit
    */
)
NCB_NotificationTemplate_Sel:BEGIN
	SELECT nt.NotificationTemplateID
		,nt.NotificationTemplateName
        ,nt.Title
        ,nt.Content
        ,nt.NotificationTypeID
    FROM NotificationTemplate nt
	WHERE nt.NotificationTemplateID = notificationTemplateID;
END