DROP PROCEDURE IF EXISTS NCB_SettingConfiguration_SettingType_Udp;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_SettingConfiguration_SettingType_Udp`(
    IN      type        INT
    ,IN		name		VARCHAR (250)
    ,in		modifiedBy	INT
    ,OUT    errCode     SMALLINT
	,OUT    errMsg      VARCHAR (100)
    /*
		Creator: 20200414@Merlin

        Revisions:
            - 20200414@Merlin: Get setting type - RedmineID: #132039
    */
)
SQL SECURITY DEFINER
BEGIN
    DECLARE jsonValue		JSON;
    DECLARE iLoop 			INT;
    DECLARE rowNumber 		INT;

    SET errCode = 0, errMsg = ''; 
    SET iLoop = 0;
    
    SELECT Value INTO jsonValue
    FROM SystemSetting
    WHERE Id = 9999;

    DROP TEMPORARY TABLE IF EXISTS SettingTypeTable;
    CREATE TEMPORARY TABLE SettingTypeTable (
		RowNumber		INT
        ,MsgContentType	INT
        ,Name           VARCHAR (250)
    );

    WHILE (
		JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].MsgContentType')) IS NOT NULL
	) DO
        INSERT INTO SettingTypeTable (
            RowNumber
            ,MsgContentType
            ,Name
        )
        SELECT
			iLoop
            ,JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].MsgContentType'))
            ,JSON_UNQUOTE(JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].Name')));
            
		SET iLoop = iLoop + 1;
    END WHILE;

    IF NOT EXISTS (
        SELECT 1 FROM SettingTypeTable
        WHERE MsgContentType = type
    ) THEN
        SELECT 1 INTO errCode;
        SELECT CONCAT('This setting type not exists') INTO errMsg;
    ELSE
		IF EXISTS (
			SELECT 1 FROM SettingTypeTable
            WHERE MsgContentType != type
				AND Name = name
        ) THEN
			SELECT 2 INTO errCode;
			SELECT CONCAT(name, ' exists') INTO errMsg;
        END IF;
        
		SELECT RowNumber INTO rowNumber
        FROM SettingTypeTable
        WHERE MsgContentType = type;
        
        UPDATE SystemSetting
        SET Value = JSON_SET(jsonValue,
			CONCAT('$[', rowNumber, '].Name'), name
            ,CONCAT('$[', rowNumber, '].ModifiedBy'), modifiedBy
            ,CONCAT('$[', rowNumber, '].ModifiedDate'), DATE_FORMAT(UTC_TIMESTAMP(), '%Y-%m-%d %T' ))
        WHERE Id = 9999;
    END IF;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_SettingConfiguration_SettingType_Udp TO 'ncb.admin'@'%';