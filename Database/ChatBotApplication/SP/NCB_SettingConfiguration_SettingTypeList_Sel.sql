DROP PROCEDURE IF EXISTS NCB_SettingConfiguration_SettingTypeList_Sel;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_SettingConfiguration_SettingTypeList_Sel`(
    IN      PageIndex   INT
    ,IN     PageSize    INT
    /*
		Creator: 20200410@Merlin

        Revisions:
            - 20200410@Merlin: Get setting type - RedmineID: #132039
    */
)
SQL SECURITY DEFINER
BEGIN
    DECLARE jsonValue	JSON;
    DECLARE iLoop 		INT;
	DECLARE rowStart    INT;

    SET rowStart =	pageSize * (pageIndex - 1);   
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
    
    SELECT COUNT(MsgContentType) AS Total
    FROM MsgContentTypeTable;

    SELECT
        MsgContentType AS Type
        ,Name
    FROM MsgContentTypeTable
    LIMIT rowStart, pageSize;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_SettingConfiguration_SettingTypeList_Sel TO 'ncb.admin'@'%';

call NCB_SettingConfiguration_SettingTypeList_Sel(1, 10)