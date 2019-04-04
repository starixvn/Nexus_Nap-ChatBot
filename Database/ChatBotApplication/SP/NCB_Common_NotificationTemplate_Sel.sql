DELIMITER //

DROP PROCEDURE IF EXISTS NCB_Common_NotificationTemplate_Sel;

CREATE PROCEDURE  NCB_Common_NotificationTemplate_Sel(
	IN NotificationTypeID SMALLINT
    /*
		Creator: Dominic@20190401
        Param:

        Revisions:
				- 20190401@Dominic: First Commit
        
    */
)
BEGIN

	SELECT nt.NotificationTemplateID, nt.NotificationTemplateName
					, nt.Title, nt.Content
    FROM NotificationTemplate nt
    WHERE nt.NotificationTypeID = NotificationTypeID
    ORDER BY nt.NotificationTemplateName;
    
END;