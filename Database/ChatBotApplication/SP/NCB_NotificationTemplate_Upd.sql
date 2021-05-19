DROP PROCEDURE IF EXISTS NCB_NotificationTemplate_Upd;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_NotificationTemplate_Upd`(
	IN		notificationTemplateID		INT
	,IN		notificationTypeID			SMALLINT
	,IN		notificationTemplateName	VARCHAR(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci
	,IN		title VARCHAR(300)			CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci
	,IN		ContentLst					JSON
	,IN		modifierID					INT
	,OUT	errCode						SMALLINT
	,OUT	errMsg						VARCHAR(100)
	/*
		Creator: Elton@20190913
		Revisions:
			- 20190910@Elton: First Commit
			- 20191217@Merlin: Check valid content before update -- Redmine ID: #125707
            - 20190215@Oliver: unremovable abnormalbes settings - Redmine: #128430
			- 20200323@Oliver: Integrate Site Message -- Redmine ID: #129979
            - 20210513@Malcolm: Integrate Telegram - RedmineID: #151807
		Param's Explanation:
		- ContentLst:
				[
					{"ChannelId" : "1", "Content" : "Content_skype"}
                    ,{"ChannelId" : "2", "Content" : "Content_telegram"}
                ]
	*/
)
NCB_NotificationTemplate_Upd:BEGIN
	#Variables used to write User Logs
	DECLARE startTime 		DATETIME(3);
	DECLARE finishTime 		DATETIME(3); 
	DECLARE duration 		INT;							
	DECLARE logContent 		TEXT CHARACTER SET utf8mb4;
	DECLARE ACTION_ID 		INT DEFAULT 13;
    DECLARE jsonValue		JSON;
	DECLARE indexLoop		INT;
    DECLARE jLoop 			INT;
	DECLARE requiredParams  TEXT;
    
	#Get Start Time to calcualte execution duration
	SET startTime = UTC_TIMESTAMP(3);
	SET errCode = 0, errMsg = '';
  
	IF EXISTS(
	SELECT 1 FROM NotificationTemplate nt
	WHERE nt.NotificationTemplateName = notificationTemplateName
		AND nt.NotificationTemplateID <> notificationTemplateID
	) 
	THEN
		SET errCode = 101, errMsg = 'The selected template name has already been taken';
		LEAVE NCB_NotificationTemplate_Upd;
	END IF;
	
    DROP TEMPORARY TABLE IF EXISTS Tmp_Setting;
    CREATE TEMPORARY TABLE Tmp_Setting (ID INT AUTO_INCREMENT NOT NULL,SettingId INT UNSIGNED NOT NULL);
    INSERT INTO Tmp_Setting (SettingId)
    VALUES (4), (9), (14);
    
	#Check params for template voiding tickets for licensee
	SELECT Value INTO jsonValue FROM SystemSetting WHERE ID = 4 LIMIT 1;
    DROP TEMPORARY TABLE IF EXISTS tmpSettingData;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmpSettingData(NotificationTypeID INT, NotificationTemplateID INT,TemplateRequiredParams TEXT);
    
    SET jLoop = 1;
    WHILE (SELECT 1 FROM Tmp_Setting WHERE Id = jLoop) DO
    
		SELECT s.Value INTO jsonValue 
        FROM SystemSetting s 
			INNER JOIN Tmp_Setting t ON t.SettingId = s.Id 
		WHERE t.ID = jLoop
        LIMIT 1;
        
		SET indexLoop = 0;
		WHILE (JSON_EXTRACT(jsonValue, CONCAT('$[',indexLoop, '].NotificationTemplateID'))) DO 
			INSERT INTO tmpSettingData(NotificationTypeID, NotificationTemplateID, TemplateRequiredParams)
			VALUES 
            (
				NULLIF(JSON_UNQUOTE(JSON_EXTRACT(jsonValue, CONCAT('$[', indexLoop, '].NotificationTypeID'))), 'null'),
				NULLIF(JSON_UNQUOTE(JSON_EXTRACT(jsonValue, CONCAT('$[', indexLoop, '].NotificationTemplateID'))), 'null'),
				NULLIF(JSON_UNQUOTE(JSON_EXTRACT(jsonValue, CONCAT('$[', indexLoop, '].TemplateRequiredParams'))), 'null')
			);
			SET indexLoop := indexLoop + 1;
		END WHILE;
    END WHILE;

	SELECT tmppjt.TemplateRequiredParams INTO requiredParams
	FROM tmpSettingData tmppjt
	WHERE tmppjt.NotificationTemplateID = notificationTemplateID
		AND tmppjt.NotificationTypeID = notificationTypeID LIMIT 1;

	DROP TEMPORARY TABLE IF EXISTS tmpParams;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmpParams(Param VARCHAR(50));
    
    IF IFNULL(requiredParams, '') <> '' THEN
		SET @SqlCommand = REPLACE(requiredParams, ',', '''),(''');
		SET @SqlCommand = CONCAT('(''', @SqlCommand, ''')');
		SET @SqlCommand= CONCAT('INSERT INTO tmpParams VALUES', @SqlCommand); 
		
		PREPARE Stmt FROM  @SqlCommand;
		EXECUTE Stmt;
		DEALLOCATE PREPARE Stmt;
	END IF;

	IF (
		EXISTS (
			SELECT 1 FROM tmpSettingData tmppjt
			WHERE tmppjt.NotificationTemplateID = notificationTemplateID
				AND tmppjt.NotificationTypeID = notificationTypeID
		)
        AND EXISTS (
			SELECT 1 FROM tmpParams 
			WHERE LOCATE(TRIM(Param), Content) = 0)
    )
	THEN
		SET errCode = 102, errMsg = 'Not allow to delete defined params';
		LEAVE NCB_NotificationTemplate_Upd;
	END IF;
    
	UPDATE NotificationTemplate nt
	SET nt.NotificationTemplateName = notificationTemplateName
		,nt.NotificationTypeID = notificationTypeID
		,nt.Title = title
		,nt.Content =content
		,nt.LastModifiedDate = UTC_TIMESTAMP()
		,nt.LastModifiedBy = modifierID
	WHERE nt.NotificationTemplateID = notificationTemplateID;

	#Get Finish Time to calcualte execution duration
	SET finishTime = UTC_TIMESTAMP(3);
	SET duration = TIMESTAMPDIFF(MICROSECOND, startTime, finishTime) / 1000; 	#miliseconds;
	
	#Get Log Content, JSON format
	SELECT json_object('notificationTypeID', nt.NotificationTypeID
		,'notificationTemplateName', nt.NotificationTemplateName
		,'title', nt.Title
		,'content', nt.content
		,'lastModifiedBy', nt.LastModifiedBy
		,'lastModifiedDate', nt.LastModifiedDate
	)
	INTO logContent
	FROM NotificationTemplate nt
	WHERE nt.NotificationTemplateID = notificationTemplateID;
    
	#Write User Log
	CALL NCB_UserLog_RunSP_Ins(ACTION_ID, notificationTemplateID, modifierID, logContent, startTime, duration);	
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_NotificationTemplate_Upd TO 'ncb.admin'@'%';