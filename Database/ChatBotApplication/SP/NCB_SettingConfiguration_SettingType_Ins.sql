DROP PROCEDURE IF EXISTS NCB_SettingConfiguration_SettingType_Ins;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_SettingConfiguration_SettingType_Ins`(
    IN      type        INT
    ,IN     name        VARCHAR (250)
    ,IN     creatorId   INT
    ,OUT    errCode     SMALLINT
	,OUT    errMsg      VARCHAR (100)
    /*
		Creator: 20200413@Merlin

        Revisions:
            - 20200413@Merlin: Create setting type - RedmineID: #132039
    */
)
SQL SECURITY DEFINER
BEGIN
    DECLARE jsonValue	JSON;
    DECLARE iLoop 		INT;
    DECLARE dataString	VARCHAR (4000);

    SET errCode = 0, errMsg = ''; 
    SET iLoop = 0;
    
    SELECT Value INTO jsonValue
    FROM SystemSetting
    WHERE Id = 9999;

    DROP TEMPORARY TABLE IF EXISTS SettingTypeTable;
    CREATE TEMPORARY TABLE SettingTypeTable (
        MsgContentType  INT
        ,Name           VARCHAR (250)
    );

    WHILE (
		JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].MsgContentType')) IS NOT NULL
	) DO
        INSERT INTO SettingTypeTable (
            MsgContentType
            ,Name
        )
        SELECT
            JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].MsgContentType'))
            ,JSON_UNQUOTE(JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].Name')));
            
		SET iLoop = iLoop + 1;
    END WHILE;

    IF EXISTS (
        SELECT 1 FROM SettingTypeTable
        WHERE MsgContentType = type
            OR Name = name
    ) THEN
        SELECT 1 INTO errCode;
        SELECT CONCAT(name,' exists') INTO errMsg;
    ELSE
		SET dataString = CONCAT('{"MsgContentType": ', type, ', "Name": "', name, '", "CreatorId": ', creatorId, ', "CreatedDate" : "',  DATE_FORMAT(UTC_TIMESTAMP(), '%Y-%m-%d %T' ), '"}');
        
        UPDATE SystemSetting
        SET
			Value = IF (
				Value IS NULL OR (JSON_TYPE(Value) != 'ARRAY'),
				JSON_ARRAY(),
				Value
			),
            Value = JSON_ARRAY_APPEND(
				Value,
                '$',
                CAST(dataString AS JSON)
            )
        WHERE Id = 9999;
    END IF;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_SettingConfiguration_SettingType_Ins TO 'ncb.admin'@'%';