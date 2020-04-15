DROP PROCEDURE IF EXISTS NCB_SettingConfiguration_SettingType_Del;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_SettingConfiguration_SettingType_Del`(
    IN      type        INT
    ,OUT    errCode     SMALLINT
	,OUT    errMsg      VARCHAR (100)
    /*
		Creator: 20200415@Merlin

        Revisions:
            - 20200415@Merlin: Get setting type - RedmineID: #132039
    */
)
SQL SECURITY DEFINER
BEGIN
    DECLARE jsonValue		JSON;
    DECLARE iLoop 			INT;
    DECLARE rowIndex 		INT;

    SET errCode = 0, errMsg = ''; 
    SET iLoop = 0;
    
    SELECT Value INTO jsonValue
    FROM SystemSetting
    WHERE Id = 9999;

    DROP TEMPORARY TABLE IF EXISTS SettingTypeTable;
    CREATE TEMPORARY TABLE SettingTypeTable (
		RowIndex		INT
        ,MsgContentType	INT
    );

    WHILE (
		JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].MsgContentType')) IS NOT NULL
	) DO
        INSERT INTO SettingTypeTable (
            RowIndex
            ,MsgContentType
        )
        SELECT
			iLoop
            ,JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].MsgContentType'));
            
		SET iLoop = iLoop + 1;
    END WHILE;

    IF NOT EXISTS (
        SELECT 1 FROM SettingTypeTable stt
        WHERE stt.MsgContentType = type
    ) THEN
        SELECT 1 INTO errCode;
        SELECT CONCAT('This setting type not exists') INTO errMsg;
    ELSE
        SELECT stt.RowIndex INTO rowIndex
        FROM SettingTypeTable stt
        WHERE stt.MsgContentType = type;
        
        UPDATE SystemSetting
        SET Value = JSON_REMOVE(jsonValue,
            CONCAT('$[', rowIndex, ']'))
        WHERE Id = 9999;
    END IF;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_SettingConfiguration_SettingType_Del TO 'ncb.admin'@'%';