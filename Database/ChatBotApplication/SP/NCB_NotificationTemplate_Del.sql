DROP PROCEDURE IF EXISTS NCB_NotificationTemplate_Del;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_NotificationTemplate_Del`(
	IN		notificationTemplateID	INT
	,IN		userID					INT
	,OUT	errCode					SMALLINT
	,OUT	errMsg					VARCHAR(100)
	/*
	Creator: Elton@20190913
		Revisions:
			- 20190910@Elton: First Commit
            - 20191218@Merlin.Le: Check not setting template before delete -- Redmine ID: #125707
            - 20200323@Oliver: Integrate Site Message -- Redmine ID: #129979
			- 20210513@Malcolm: Integrate Telegram - RedmineID: #151807
	*/
)
NCB_NotificationTemplate_Del:BEGIN
	#Variables used to write User Logs
	DECLARE startTime 		DATETIME(3);
	DECLARE finishTime 		DATETIME(3); 
	DECLARE duration 		INT;							
	DECLARE logContent 		TEXT CHARACTER SET utf8mb4;
	DECLARE ACTION_ID 		INT DEFAULT 14;
	DECLARE iLoop 			INT;
    DECLARE jLoop 			INT;
    DECLARE jsonSetting 	JSON;

	#Get Start Time to calcualte execution duration
	SET  startTime = UTC_TIMESTAMP(3);
	SET errCode = 0, errMsg = '';
    
	IF NOT EXISTS(SELECT 1 FROM NotificationTemplate nt WHERE nt.NotificationTemplateID = notificationTemplateID) 
	THEN
		SET errCode = 101, errMsg = 'Invalid Template Id';
		LEAVE NCB_NotificationTemplate_Del;
	END IF;

	DROP TEMPORARY TABLE IF EXISTS tmpSetting;
    CREATE TEMPORARY TABLE tmpSetting (Id INT AUTO_INCREMENT NOT NULL, SettingId INT);
    
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
		WHILE (JSON_EXTRACT(jsonSetting, CONCAT('$[',iLoop, '].NotificationTemplateID'))) DO 
			INSERT INTO tmpParseJsonTable(NotificationTypeID, NotificationTemplateID)
			VALUES (
				NULLIF(JSON_UNQUOTE(JSON_EXTRACT(jsonSetting, CONCAT('$[', iLoop, '].NotificationTypeID'))) = 'null'),
				NULLIF(JSON_UNQUOTE(JSON_EXTRACT(jsonSetting, CONCAT('$[', iLoop, '].NotificationTemplateID'))) = 'null')
			);
			
			SET iLoop := iLoop + 1;
		END WHILE;
        
        SET jLoop := jLoop + 1;
    END WHILE;

	IF EXISTS (
		SELECT 1 FROM NotificationTemplate nt
		INNER JOIN tmpParseJsonTable tmppjt ON nt.NotificationTemplateID = tmppjt.NotificationTemplateID
			AND nt.NotificationTypeID = tmppjt.NotificationTypeID
		WHERE nt.NotificationTemplateID = notificationTemplateID
	)
	THEN
		SET errCode = 102, errMsg = 'Cannot delete this template';
		LEAVE NCB_NotificationTemplate_Del;
	END IF;
    
	DELETE FROM NotificationTemplate nt
	WHERE nt.NotificationTemplateID = notificationTemplateID;
    
    DELETE FROM NotificationTemplateContent ntc
    WHERE ntc.NotificationTemplateID = NotificationTemplateID;

	#Get Finish Time to calcualte execution duration
	SET finishTime = UTC_TIMESTAMP(3);
	SET duration =  TIMESTAMPDIFF(MICROSECOND, startTime, finishTime) / 1000; 	#miliseconds;
	
	#Get Log Content, JSON format
	SET logContent = json_object('notificationTemplateID', notificationTemplateID);
    
	#Write User Log
	CALL NCB_UserLog_RunSP_Ins(ACTION_ID, notificationTemplateID, userID, logContent, startTime, duration);
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_NotificationTemplate_Del TO 'ncb.admin'@'%';