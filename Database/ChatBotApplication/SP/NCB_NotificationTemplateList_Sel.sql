DROP PROCEDURE IF EXISTS NCB_NotificationTemplateList_Sel;
DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_NotificationTemplateList_Sel`(
	IN notificationTypeID INT
    ,IN pageIndex INT
    ,IN pageSize INT
	/*
        Revisions:
			- 20191218@Merlin.Le: Get additional UnRemovable field -- Redmine ID: #125707
            - 20190215@Oliver: unremovable abnormalbes settings - Redmine: #128430
            - 20200323@Oliver: Integrate Site Message -- Redmine ID: #129979
			- 20210513@Malcolm: Integrate Telegram - RedmineID: #151807
    */
)
BEGIN
	DECLARE rowStart INT;
    DECLARE iLoop    INT;
    DECLARE jLoop 	 INT;
	DECLARE NotificationSetting	JSON;
	
    SET rowStart =	pageSize * (pageIndex - 1);   
    
    SELECT COUNT(1)
    FROM NotificationTemplate nt
		INNER JOIN NotificationType t ON nt.NotificationTypeID = t.NotificationTypeID
	WHERE notificationTypeID IS NULL OR t.NotificationTypeID = notificationTypeID;
	
	DROP TEMPORARY TABLE IF EXISTS tmpSetting;
    CREATE TEMPORARY TABLE tmpSetting (ID TINYINT UNSIGNED NOT NULL AUTO_INCREMENT, SettingId INT UNSIGNED NOT NULL);
    
    INSERT INTO tmpSetting (SettingId)
    VALUES (4), (9), (14);

    DROP TEMPORARY TABLE IF EXISTS tmpParseJsonTable;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmpParseJsonTable(NotificationTypeID INT, NotificationTemplateID INT);

	#Get setting from Special Delete voiding ticket for licensee settings
    SET jLoop = 1;
    WHILE EXISTS (SELECT 1 FROM tmpSetting s WHERE s.ID = jLoop) DO
		
        SELECT Value INTO NotificationSetting
        FROM SystemSetting s
			INNER JOIN tmpSetting t ON t.SettingId = s.Id
		WHERE t.Id = jLoop;
        
		SET iLoop = 0;
		WHILE (JSON_EXTRACT(NotificationSetting, CONCAT('$[',iLoop, '].NotificationTemplateID'))) DO 
			INSERT INTO tmpParseJsonTable(NotificationTypeID, NotificationTemplateID)
			VALUES 
            (
				NULLIF(JSON_UNQUOTE(JSON_EXTRACT(NotificationSetting, CONCAT('$[', iLoop, '].NotificationTypeID'))), 'null'),
				NULLIF(JSON_UNQUOTE(JSON_EXTRACT(NotificationSetting, CONCAT('$[', iLoop, '].NotificationTemplateID'))), 'null')
			);
			SET iLoop := iLoop + 1;
		END WHILE;
        SET jLoop := jLoop + 1;
    END WHILE;

	SELECT DISTINCT nt.NotificationTemplateID
		,nt.NotificationTemplateName
        ,nt.Title
        ,ntc.Content
        ,t.NotificationTypeID
        ,t.NotificationTypeName
        ,CASE WHEN tmppjt.NotificationTemplateID IS NOT NULL THEN 1 ELSE 0 END AS UnRemovable
    FROM NotificationTemplate nt
		INNER JOIN NotificationTemplateContent ntc ON ntc.NotificationTemplateId = nt.NotificationTemplateId 
        INNER JOIN ConversationChannel cc ON cc.ChannelId = ntc.ChannelId
		INNER JOIN NotificationType t ON nt.NotificationTypeID = t.NotificationTypeID
        LEFT JOIN tmpParseJsonTable tmppjt ON tmppjt.NotificationTemplateID = nt.NotificationTemplateID
						AND tmppjt.NotificationTypeID = nt.NotificationTypeID
	WHERE (notificationTypeID IS NULL OR t.NotificationTypeID = notificationTypeID) AND cc.ChannelName = 'Skype'
    ORDER BY t.NotificationTypeName, nt.NotificationTemplateName
    LIMIT rowStart, pageSize; 
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_NotificationTemplateList_Sel TO 'ncb.admin'@'%';