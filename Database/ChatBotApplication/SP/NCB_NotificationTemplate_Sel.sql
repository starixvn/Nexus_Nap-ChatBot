DROP PROCEDURE IF EXISTS NCB_NotificationTemplate_Sel;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_NotificationTemplate_Sel`(
	IN notificationTemplateID INT
	/*
		Creator: Elton@20190913
        Revisions:
			- 20190910@Elton: First Commit
			- 20191218@Merlin.Le: Get additional UnRemovable field -- Redmine ID: #125707
            - 20190215@Oliver: unremovable abnormalbes settings - Redmine: #128430

    */
)
NCB_NotificationTemplate_Sel:BEGIN
	DECLARE jsonValue	JSON;
    DECLARE indexLoop   INT;
	
	SELECT Value INTO jsonValue FROM SystemSetting WHERE ID = 4 LIMIT 1;
    DROP TEMPORARY TABLE IF EXISTS tmpParseJsonTable;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmpParseJsonTable(NotificationTypeID INT, NotificationTemplateID INT);
    SET indexLoop = 0;
    WHILE (JSON_EXTRACT(jsonValue, CONCAT('$[',indexLoop, '].NotificationTemplateID'))) DO 
		INSERT INTO tmpParseJsonTable(NotificationTypeID, NotificationTemplateID)
        VALUES (
            CASE
                WHEN JSON_UNQUOTE(JSON_EXTRACT(jsonValue, CONCAT('$[', indexLoop, '].NotificationTypeID'))) = 'null'
                THEN NULL
                ELSE JSON_UNQUOTE(JSON_EXTRACT(jsonValue, CONCAT('$[', indexLoop, '].NotificationTypeID')))
            END,
            CASE
                WHEN JSON_UNQUOTE(JSON_EXTRACT(jsonValue, CONCAT('$[', indexLoop, '].NotificationTemplateID'))) = 'null'
                THEN NULL
                ELSE JSON_UNQUOTE(JSON_EXTRACT(jsonValue, CONCAT('$[', indexLoop, '].NotificationTemplateID')))
            END
		);
        
        SET indexLoop := indexLoop + 1;
    END WHILE;

	SELECT nt.NotificationTemplateID
		,nt.NotificationTemplateName
        ,nt.Title
        ,nt.Content
        ,nt.NotificationTypeID
        ,CASE
			WHEN (
				EXISTS (
					SELECT 1 FROM tmpParseJsonTable tmppjt
					WHERE tmppjt.NotificationTemplateID = nt.NotificationTemplateID
						AND tmppjt.NotificationTypeID = nt.NotificationTypeID
				) 
            ) THEN 1
			ELSE 0
		END AS UnRemovable
    FROM NotificationTemplate nt
	WHERE nt.NotificationTemplateID = notificationTemplateID;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_NotificationTemplate_Sel TO 'ncb.admin'@'%';