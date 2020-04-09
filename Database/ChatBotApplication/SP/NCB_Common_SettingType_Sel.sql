DROP PROCEDURE IF EXISTS NCB_Common_SettingType_Sel;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_Common_SettingType_Sel`(
    /*
		Creator: 20200409@Merlin

        Revisions:
            - 20200409@Oliver: Get setting type - RedmineID: #132039
    */
)
SQL SECURITY DEFINER
BEGIN
    DECLARE jsonValue	JSON;
    DECLARE iLoop 		INT;
    SET iLoop = 0;
    
    SELECT Value INTO jsonValue
    FROM SystemSetting
    WHERE Id = 9999;

    DROP TEMPORARY TABLE IF EXISTS MsgContentTypeTable;
    CREATE TEMPORARY TABLE MsgContentTypeTable (
        MsgContentType  INT
        ,Name           VARCHAR (250)
    );

    WHILE (
		JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].MsgContentType')) IS NOT NULL
	) DO
        INSERT INTO MsgContentTypeTable (
            MsgContentType
            ,Name
        )
        SELECT
            JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].MsgContentType'))
            ,JSON_UNQUOTE(JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].Name')));
            
		SET iLoop = iLoop + 1;
    END WHILE;

    SELECT
        MsgContentType
        ,Name
    FROM MsgContentTypeTable;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_Common_SettingType_Sel TO 'ncb.admin'@'%';