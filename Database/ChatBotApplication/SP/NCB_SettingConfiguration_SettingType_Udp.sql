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
	DECLARE startTime	DATETIME(3) DEFAULT UTC_TIMESTAMP(3);
    DECLARE finishTime	DATETIME(3); 
    DECLARE duration	INT;							
    DECLARE logContent	TEXT CHARACTER SET utf8mb4;
    DECLARE actionId	INT DEFAULT 21;
    DECLARE updateTime	VARCHAR (20);
    DECLARE jsonValue	JSON;
    DECLARE iLoop 		INT;
    DECLARE rowIndex 	INT;

    SET errCode = 0, errMsg = ''; 
    SET iLoop = 0;
    
    SELECT ss.Value INTO jsonValue
    FROM SystemSetting ss
    WHERE ss.Id = 9999;

    DROP TEMPORARY TABLE IF EXISTS SettingTypeTable;
    CREATE TEMPORARY TABLE SettingTypeTable (
		RowIndex		INT
        ,MsgContentType	INT
        ,Name           VARCHAR (250)
    );

    WHILE (
		JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].MsgContentType')) IS NOT NULL
	) DO
        INSERT INTO SettingTypeTable (
            RowIndex
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
        SELECT 1 FROM SettingTypeTable stt
        WHERE stt.MsgContentType = type
    ) THEN
        SELECT 1 INTO errCode;
        SELECT CONCAT('This setting type not exists') INTO errMsg;
    ELSE
		IF EXISTS (
			SELECT 1 FROM SettingTypeTable stt
            WHERE stt.MsgContentType != type
				AND stt.Name = name
        ) THEN
			SELECT 2 INTO errCode;
			SELECT CONCAT(name, ' exists') INTO errMsg;
        ELSE
            SELECT stt.RowIndex INTO rowIndex
            FROM SettingTypeTable stt
            WHERE stt.MsgContentType = type;
            
            SET updateTime = DATE_FORMAT(UTC_TIMESTAMP(), '%Y-%m-%d %T' );
            
            UPDATE SystemSetting
            SET Value = JSON_SET(jsonValue,
                CONCAT('$[', rowIndex, '].Name'), name
                ,CONCAT('$[', rowIndex, '].ModifiedBy'), modifiedBy
                ,CONCAT('$[', rowIndex, '].ModifiedDate'), updateTime)
            WHERE Id = 9999;
    
			#Get Finish Time to calcualte execution duration
			SET finishTime = UTC_TIMESTAMP(3);
			SET duration =  TIMESTAMPDIFF(MICROSECOND, startTime, finishTime)  / 1000; 	#miliseconds;
            
			#Get Log Content, JSON format
			SELECT json_object(
                'msgContentType', type
				,'name', name
				,'modifiedBy', modifiedBy
				,'modifiedDate', updateTime
			)
			INTO logContent;
			
			#Write User Log
			CALL NCB_UserLog_RunSP_Ins(
				actionId
				,type 
				,modifiedBy 
				,logContent 
				,startTime 
				,duration
			);
        END IF;
    END IF;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_SettingConfiguration_SettingType_Udp TO 'ncb.admin'@'%';