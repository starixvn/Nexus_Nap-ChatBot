DROP PROCEDURE IF EXISTS NCB_NotificationTemplateList_Sel;

DELIMITER //


CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_NotificationTemplateList_Sel`(
	IN notificationTypeID INT
    ,IN pageIndex INT
    ,IN pageSize INT
	/*
        Revisions:
			- 20191218@Merlin.Le: Get additional IsSpecialDelete field
    */
)
BEGIN
	DECLARE rowStart INT;
	DECLARE jsonValue JSON;
    SET rowStart =	pageSize * (pageIndex - 1);   
    
    SELECT COUNT(1)
    FROM NotificationTemplate nt
		INNER JOIN NotificationType t ON nt.NotificationTypeID = t.NotificationTypeID
	WHERE notificationTypeID IS NULL OR t.NotificationTypeID = notificationTypeID;
    
	SELECT Value INTO jsonValue FROM SystemSetting WHERE ID = 4 LIMIT 1;
	SELECT nt.NotificationTemplateID
		,nt.NotificationTemplateName
        ,nt.Title
        ,nt.Content
        ,t.NotificationTypeID
        ,t.NotificationTypeName
        ,CASE
			WHEN (
				EXISTS (
					SELECT 1 FROM (			
						SELECT NotificationTypeID, NotificationTemplateID
						FROM JSON_TABLE(jsonValue, "$[*]"
							COLUMNS(
								NotificationTypeID INT PATH "$.NotificationTypeID" DEFAULT '0' ON EMPTY DEFAULT '-1' ON ERROR,
								NotificationTemplateID INT PATH "$.NotificationTemplateID" DEFAULT '0' ON EMPTY DEFAULT '-1' ON ERROR
							)
						) tmpTable
                        WHERE tmpTable.NotificationTemplateID = nt.NotificationTemplateID
							AND tmpTable.NotificationTypeID = nt.NotificationTypeID
					) tmpTable
				) 
            ) THEN 1
			ELSE 0
		END AS IsSpecialDelete
    FROM NotificationTemplate nt
		INNER JOIN NotificationType t ON nt.NotificationTypeID = t.NotificationTypeID
	WHERE notificationTypeID IS NULL OR t.NotificationTypeID = notificationTypeID
    ORDER BY t.NotificationTypeName, nt.NotificationTemplateName
    LIMIT rowStart, pageSize; 
END;


//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_NotificationTemplateList_Sel TO 'ncb.admin'@'%';