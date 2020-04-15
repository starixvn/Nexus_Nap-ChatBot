DROP PROCEDURE IF EXISTS NCB_SettingConfiguration_SettingType_Del;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_SettingConfiguration_SettingType_Del`(
    IN      type        INT
    ,IN     modifiedBy  INT
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
	DECLARE startTime	DATETIME(3) DEFAULT UTC_TIMESTAMP(3);
    DECLARE finishTime	DATETIME(3); 
    DECLARE duration	INT;							
    DECLARE logContent	TEXT CHARACTER SET utf8mb4;
    DECLARE actionId	INT DEFAULT 22;
    DECLARE jsonValue	JSON;
    DECLARE iLoop 		INT;
    DECLARE rowIndex 	INT;
    DECLARE isUsing 	BIT DEFAULT 0;

    SET errCode = 0, errMsg = ''; 
    SET iLoop = 0;
    
    SELECT ss.Value INTO jsonValue
    FROM SystemSetting ss
    WHERE ss.Id = 9999;

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
        SELECT 'This setting type not exists' INTO errMsg;
	END IF;
    
	DROP TEMPORARY TABLE IF EXISTS UsingSystemSettingValueTable;
	CREATE TEMPORARY TABLE UsingSystemSettingValueTable (
		Value JSON
	);

	INSERT INTO UsingSystemSettingValueTable
	SELECT ss.Value FROM SystemSetting ss
	WHERE Id IN (4, 9, 14);

	WHILE (
		EXISTS (
			SELECT 1 FROM UsingSystemSettingValueTable
		)
		AND NOT isUsing
	) DO
		SET iLoop = 0;

		myLoop: WHILE (
			JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].MsgContentType')) IS NOT NULL
		) DO
			IF (JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].MsgContentType')) = type) THEN
				SELECT 2 INTO errCode;
				SELECT 'Unable to delete the Msg Contents Type because it is used.' INTO errMsg;
				SET isUsing = 1;
				LEAVE myLoop;
			END IF;

			SET iLoop = iLoop + 1;
		END WHILE myLoop;
	END WHILE;

	IF NOT isUsing THEN
		SELECT stt.RowIndex INTO rowIndex
		FROM SettingTypeTable stt
		WHERE stt.MsgContentType = type;
		
		UPDATE SystemSetting
		SET Value = JSON_REMOVE(jsonValue,
			CONCAT('$[', rowIndex, ']'))
		WHERE Id = 9999;
	
		#Get Finish Time to calcualte execution duration
		SET finishTime = UTC_TIMESTAMP(3);
		SET duration =  TIMESTAMPDIFF(MICROSECOND, startTime, finishTime)  / 1000; 	#miliseconds;
		
		#Get Log Content, JSON format
		SELECT json_object('msgContentType', type)
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
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_SettingConfiguration_SettingType_Del TO 'ncb.admin'@'%';