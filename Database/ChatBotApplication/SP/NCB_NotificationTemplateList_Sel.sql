DROP PROCEDURE IF EXISTS NCB_NotificationTemplateList_Sel;

DELIMITER //


CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_NotificationTemplateList_Sel`(
	IN notificationTypeID INT
    ,IN pageIndex INT
    ,IN pageSize INT
	/*
        Revisions:
			- 20191218@Merlin.Le: Get additional UnRemovable field -- Redmine ID: #125707
    */
)
BEGIN
	DECLARE rowStart    INT;
	DECLARE jsonValue   JSON;
    DECLARE indexLoop   INT;
    SET rowStart =	pageSize * (pageIndex - 1);   
    
    SELECT COUNT(1)
    FROM NotificationTemplate nt
		INNER JOIN NotificationType t ON nt.NotificationTypeID = t.NotificationTypeID
	WHERE notificationTypeID IS NULL OR t.NotificationTypeID = notificationTypeID;
    
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
        ,t.NotificationTypeID
        ,t.NotificationTypeName
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
		INNER JOIN NotificationType t ON nt.NotificationTypeID = t.NotificationTypeID
	WHERE notificationTypeID IS NULL OR t.NotificationTypeID = notificationTypeID
    ORDER BY t.NotificationTypeName, nt.NotificationTemplateName
    LIMIT rowStart, pageSize; 
END;


//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_NotificationTemplateList_Sel TO 'ncb.admin'@'%';