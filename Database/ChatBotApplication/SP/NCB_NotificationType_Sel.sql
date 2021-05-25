DROP PROCEDURE IF EXISTS NCB_NotificationType_Sel;
DELIMITER //

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE NCB_NotificationType_Sel(
	IN pageIndex INT, 
	IN pageSize INT,
    OUT totalItems INT
	/*
		Creator: Oliver@20190820
			Params:
				- pageIndex: Page Index for paging
				- pageSize: Page Size for paging
			Revisions:
				- 20190729@Oliver: Revised total items to output    
				- 20191223@Merlin.Le: Get additional UnRemovable field -- Redmine ID: #125707
            	- 20190215@Oliver: unremovable abnormalbes settings - Redmine: #128430
				- 20200323@Oliver: Integrate Site Message -- Redmine ID: #129979
				- 20210423@Malcolm: Integrate abnormal account -- Redmine ID: #152175
                - 20210513@Malcolm: Integrate Telegram - RedmineID: #151807
    */
	)
SQL SECURITY DEFINER
BEGIN
	DECLARE rowStart	INT;
    DECLARE iLoop   	INT;
    DECLARE jLoop		INT;
	DECLARE jsonSetting JSON;
    
	DECLARE _ID 			INT;
    SET rowStart =	pageSize * (pageIndex - 1);  
    
    SET totalItems = (SELECT COUNT(1) FROM NotificationType nt);
	
    DROP TEMPORARY TABLE IF EXISTS tmpSetting;
    CREATE TEMPORARY TABLE tmpSetting (Id INT AUTO_INCREMENT NOT NULL, SettingId INT UNSIGNED NOT NULL, PRIMARY KEY (Id));
    INSERT INTO tmpSetting (SettingId)
    VALUES (4), (9), (14);
	SELECT Value
	FROM SystemSetting WHERE ID IN (4, 9, 14);

    DROP TEMPORARY TABLE IF EXISTS tmpParseJsonTable;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmpParseJsonTable(NotificationTypeID INT);

	SET jLoop = 1;
    WHILE (SELECT 1 FROM tmpSetting WHERE Id = jLoop) DO
		 SELECT s.Value INTO jsonSetting
         FROM SystemSetting s 
			INNER JOIN tmpSetting t ON t.SettingId = s.Id
		WHERE t.Id = JLoop;
        
		SET iLoop = 0;
		WHILE (JSON_EXTRACT(jsonSetting, CONCAT('$[',iLoop, '].NotificationTypeID'))) DO 
			INSERT INTO tmpParseJsonTable(NotificationTypeID)
			VALUES (NULLIF(JSON_UNQUOTE(JSON_EXTRACT(jsonSetting, CONCAT('$[', iLoop, '].NotificationTypeID'))), 'null'));
			
			SET iLoop := iLoop + 1;
		END WHILE;
        
        SET jLoop := jLoop + 1;
	END WHILE;

    SELECT
		nt.NotificationTypeID AS Id, 
		nt.NotificationTypeName AS Name, 
		s.SystemID AS SystemId,
		s.SystemName AS SystemName,
        CASE WHEN (tmppjt.NotificationTypeID IS NOT NULL) THEN 1 ELSE 0 END AS UnRemovable
	FROM NotificationType nt
		INNER JOIN `System` s ON nt.SystemID = s.SystemId
		LEFT JOIN (SELECT DISTINCT NotificationTypeID FROM tmpParseJsonTable) tmppjt ON nt.NotificationTypeID = tmppjt.NotificationTypeID
    ORDER BY nt.NotificationTypeID
	LIMIT rowStart, pageSize;
    
    SELECT 
		nrt.RecipientTypeID AS Id,
		rt.RecipientTypeName AS Name,
		nrt.NotificationTypeID AS NotificationTypeId
    FROM NotificationRecipientType nrt 
		INNER JOIN RecipientType rt ON nrt.RecipientTypeID = rt.RecipientTypeId;
	
	SELECT 
		cgt.ChatGroupTypeID AS Id,
		cgt.ChatGroupTypeName AS Name,
		nct.NotificationTypeID AS NotificationTypeId
	FROM NotificationChatGroupType nct
		INNER JOIN ChatGroupType cgt ON cgt.ChatGroupTypeID = nct.ChatGroupTypeID;
END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE NCB_NotificationType_Sel TO 'ncb.admin'@'%';