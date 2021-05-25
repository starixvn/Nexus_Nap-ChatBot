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
            - 20210513@Malcolm: Integrate Telegram - RedmineID: #151807

    */
)
NCB_NotificationTemplate_Sel:BEGIN
	DECLARE jsonSetting	JSON;
	DECLARE iLoop   INT;
	DECLARE jLoop   INT;
    
	DROP TEMPORARY TABLE IF EXISTS tmpSetting;
    CREATE TEMPORARY TABLE tmpSetting (Id INT AUTO_INCREMENT NOT NULL, SettingId INT UNSIGNED NOT NULL, PRIMARY KEY (ID));
    INSERT INTO tmpSetting (SettingId)
    VALUES (4), (9), (14);
    
    DROP TEMPORARY TABLE IF EXISTS tmpParseJsonTable;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmpParseJsonTable(NotificationTypeID INT, NotificationTemplateID INT);
    
    SET jLoop = 1;
    WHILE (SELECT 1 FROM tmpSetting WHERE Id = jLoop) DO
		SELECT s.Value INTO jsonSetting
        FROM SystemSetting s 
			INNER JOIN tmpSetting t ON t.SettingId = s.Id 
		WHERE t.Id = jLoop;
        
		SET iLoop = 0;
		WHILE (JSON_EXTRACT(jsonSetting, CONCAT('$[',iLoop, '].NotificationTypeID'))) DO 
			INSERT INTO tmpParseJsonTable(NotificationTypeID, NotificationTemplateID)
			VALUES 
            (
				 NULLIF(JSON_UNQUOTE(JSON_EXTRACT(jsonSetting, CONCAT('$[', iLoop, '].NotificationTypeID'))), 'null'),
				 NULLIF(JSON_UNQUOTE(JSON_EXTRACT(jsonSetting, CONCAT('$[', iLoop, '].NotificationTemplateID'))), 'null')
			);
			
			SET iLoop := iLoop + 1;
		END WHILE;
        
        SET jLoop := jLoop + 1;
	END WHILE;

	SELECT nt.NotificationTemplateID
		,nt.NotificationTemplateName
        ,nt.Title
        ,ntc.Content
        ,nt.NotificationTypeID
        ,CASE WHEN (m.NotificationTemplateID IS NOT NULL) THEN 1 ELSE 0 END AS UnRemovable
    FROM NotificationTemplate nt
		INNER JOIN NotificationTemplateContent ntc ON ntc.NotificationTemplateID = nt.NotificationTemplateID
        INNER JOIN ConversationChannel c ON c.ChannelID = ntc.ChannelID
        LEFT JOIN (SELECT DISTINCT NotificationTypeID, NotificationTemplateID FROM tmpParseJsonTable) m 
			ON m.NotificationTemplateID = nt.NotificationTemplateID AND m.NotificationTypeID = nt.NotificationTypeID
	WHERE nt.NotificationTemplateID = notificationTemplateID AND c.ChannelName = 'Skype';
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_NotificationTemplate_Sel TO 'ncb.admin'@'%';