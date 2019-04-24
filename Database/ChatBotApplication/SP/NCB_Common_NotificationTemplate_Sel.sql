DELIMITER //

DROP PROCEDURE IF EXISTS NCB_Common_NotificationTemplate_Sel;

CREATE PROCEDURE  NCB_Common_NotificationTemplate_Sel(
	IN notificationTypeID SMALLINT
    /*
		Creator: Dominic@20190401
        Param:

        Revisions:
				- 20190401@Dominic: First Commit
        
    */
)
BEGIN

	SELECT nt.NotificationTemplateID AS ID, nt.NotificationTemplateName AS Name, nt.Title, nt.Content
    FROM NotificationTemplate nt
    WHERE nt.NotificationTypeID = notificationTypeID
    ORDER BY nt.NotificationTemplateName;
    
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_Common_NotificationTemplate_Sel TO 'ncb.admin'@'%';