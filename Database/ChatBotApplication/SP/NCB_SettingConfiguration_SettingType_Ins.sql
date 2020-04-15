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
	DECLARE startTime	DATETIME(3) DEFAULT UTC_TIMESTAMP(3);
    DECLARE finishTime	DATETIME(3); 
    DECLARE duration	INT;							
    DECLARE logContent	TEXT CHARACTER SET utf8mb4;
    DECLARE jsonValue	JSON;
    DECLARE iLoop 		INT DEFAULT 0;
    DECLARE dataString	VARCHAR (4000);
    DECLARE actionId	INT DEFAULT 20;
    DECLARE insertTime	VARCHAR (20);

    SET errCode = 0, errMsg = '';
    
    SELECT ss.Value INTO jsonValue
    FROM SystemSetting ss
    WHERE ss.Id = 9999;

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
        SELECT 1 FROM SettingTypeTable ss
        WHERE ss.MsgContentType = type
            OR ss.Name = name
    ) THEN
        SELECT 1 INTO errCode;
        SELECT CONCAT(name,' exists') INTO errMsg;
    ELSE
		SET insertTime = DATE_FORMAT(UTC_TIMESTAMP(), '%Y-%m-%d %T' );
		SET dataString = CONCAT('{"MsgContentType": ', type, ', "Name": "', name, '", "CreatorId": ', creatorId, ', "CreatedDate" : "', insertTime, '"}');
        
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
    
        #Get Finish Time to calcualte execution duration
        SET finishTime = UTC_TIMESTAMP(3);
        SET duration =  TIMESTAMPDIFF(MICROSECOND, startTime, finishTime)  / 1000; 	#miliseconds;
        #Get Log Content, JSON format
        SELECT json_object(
            'msgContentType', type
            ,'name', name
            ,'creatorId', creatorId
            ,'createdDate', insertTime
        )
        INTO logContent;
        
        #Write User Log
        CALL NCB_UserLog_RunSP_Ins(
            actionId
            ,type 
            ,creatorId 
            ,logContent 
            ,startTime 
            ,duration
        );
    END IF;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_SettingConfiguration_SettingType_Ins TO 'ncb.admin'@'%';