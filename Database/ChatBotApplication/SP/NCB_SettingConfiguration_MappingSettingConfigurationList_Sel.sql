DROP PROCEDURE IF EXISTS NCB_SettingConfiguration_MappingSettingConfigurationList_Sel;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_SettingConfiguration_MappingSettingConfigurationList_Sel`(
    IN      jobId       INT
    ,IN     pageIndex   INT
    ,IN     pageSize    INT
    /*
		Creator: 20200417@Merlin

        Revisions:
            - 20200417@Merlin: Get job info - RedmineID: #132039
    */
)
SQL SECURITY DEFINER
BEGIN
    DECLARE jsonValue   JSON;
    DECLARE iLoop 		INT DEFAULT 0;

    SELECT ss.Value INTO jsonValue
    FROM SystemSetting ss
    WHERE ss.Description LIKE '%Notification Template ID / MsgContentsType / NotificationType%'
        AND ss.JobIdentify = jobId;

    DROP TEMPORARY TABLE IF EXISTS MappingSettingConfigurationTable;
    CREATE TEMPORARY TABLE MappingSettingConfigurationTable (
        IsTest                  INT
        ,MsgContentsType        INT
        ,NotificationTemplateID INT
        ,NotificationTypeID     INT
        ,TemplateRequiredParams VARCHAR (4000)
    );

    WHILE (
		JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].MsgContentsType')) IS NOT NULL
	) DO
        INSERT INTO MappingSettingConfigurationTable (
            IsTest
            ,MsgContentsType
            ,NotificationTemplateID
            ,NotificationTypeID
            ,TemplateRequiredParams
        )
        SELECT
            JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].IsTest'))
            ,JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].MsgContentsType'))
            ,JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].NotificationTemplateID'))
            ,JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].NotificationTypeID'))
            ,JSON_UNQUOTE(JSON_EXTRACT(jsonValue, CONCAT('$[', iLoop, '].TemplateRequiredParams')));
            
		SET iLoop = iLoop + 1;
    END WHILE;
    
    SELECT COUNT(msct.MsgContentsType) AS Total
    FROM MappingSettingConfigurationTable msct;

    SELECT 
        msct.IsTest
        ,msct.MsgContentsType
        ,msct.NotificationTemplateID
        ,msct.TemplateRequiredParams
        ,nt.NotificationTypeID
        ,nt.NotificationTypeName
        ,s.SystemID
        ,s.SystemName
    FROM MappingSettingConfigurationTable msct
    INNER JOIN NotificationType nt ON nt.NotificationTypeID = msct.NotificationTypeID
    INNER JOIN System s ON s.SystemID = nt.SystemID;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_SettingConfiguration_MappingSettingConfigurationList_Sel TO 'ncb.admin'@'%';