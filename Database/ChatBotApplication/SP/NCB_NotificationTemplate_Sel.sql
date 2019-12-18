DROP PROCEDURE IF EXISTS NCB_NotificationTemplate_Sel;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_NotificationTemplate_Sel`(
	IN notificationTemplateID INT
	/*
		Creator: Elton@20190913
        Revisions:
			- 20190910@Elton: First Commit
			- 20191218@Merlin.Le: Get additional IsSpecialDelete field
    */
)
NCB_NotificationTemplate_Sel:BEGIN
	DECLARE jsonValue JSON;
	SELECT Value INTO jsonValue FROM SystemSetting WHERE ID = 4 LIMIT 1;
	SELECT nt.NotificationTemplateID
		,nt.NotificationTemplateName
        ,nt.Title
        ,nt.Content
        ,nt.NotificationTypeID
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
	WHERE nt.NotificationTemplateID = notificationTemplateID;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_NotificationTemplate_Sel TO 'ncb.admin'@'%';