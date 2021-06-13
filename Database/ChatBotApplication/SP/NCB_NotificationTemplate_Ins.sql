DROP PROCEDURE IF EXISTS NCB_NotificationTemplate_Ins;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_NotificationTemplate_Ins`(
	IN notificationTypeID SMALLINT
    ,IN notificationTemplateName VARCHAR(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci
    ,IN title VARCHAR(300)  CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci
    ,IN ContentLst JSON
    ,IN creatorID INT
    ,OUT notificationTemplateID INT
	,OUT errCode SMALLINT
	,OUT errMsg VARCHAR(100)
	/*
		Creator: Elton@20190910
        Revisions:
				- 20190910@Elton: First Commit
                - 20210513@Malcolm: Integrate Telegram - RedmineID: #151807
		Param's Explanation:
		- ContentLst:
				[
					{"ChannelId" : "1", "Content" : "Content_skype"}
                    ,{"ChannelId" : "2", "Content" : "Content_telegram"}
                ]
    */
)
NCB_NotificationTemplate_Ins:BEGIN
	#Variables used to write User Logs
	DECLARE startTime 		DATETIME(3);
    DECLARE finishTime 		DATETIME(3); 
    DECLARE duration 		INT;		
    DECLARE iLoop 			INT;
    DECLARE logContent 		TEXT CHARACTER SET utf8mb4;
    DECLARE ACTION_ID 		INT DEFAULT 12;
     #Get Start Time to calcualte execution duration
    SET  startTime = UTC_TIMESTAMP(3);
    SET errCode=0, errMsg='';
    
    IF  EXISTS(SELECT 1 FROM NotificationTemplate nt WHERE nt.NotificationTemplateName = notificationTemplateName) 
    THEN
		SET errCode=101, errMsg='The selected template name has already been taken';
        LEAVE NCB_NotificationTemplate_Ins;
    END IF;
    
    INSERT INTO NotificationTemplate (`NotificationTemplateName`, `NotificationTypeID`, `Title`, `CreatorID`, `CreatedDate`)
	VALUES (notificationTemplateName, notificationTypeID, title, creatorID, UTC_TIMESTAMP());
    
    SET notificationTemplateID = LAST_INSERT_ID();

    DROP TEMPORARY TABLE IF EXISTS tmpContent;
    CREATE TEMPORARY TABLE tmpContent (ChannelId TINYINT UNSIGNED NOT NULL, Content MEDIUMTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci);
    
    SET iLoop = 0;
    WHILE (JSON_EXTRACT(ContentLst, CONCAT('$[', iLoop, '].ChannelId')) IS NOT NULL) DO
		INSERT INTO tmpContent (ChannelId, Content)
        VALUES 
        (
			JSON_UNQUOTE(JSON_EXTRACT(ContentLst, CONCAT('$[', iLoop, '].ChannelId'))),
            JSON_UNQUOTE(JSON_EXTRACT(ContentLst, CONCAT('$[', iLoop, '].Content')))
        );
        SET iLoop := iLoop + 1;
    END WHILE;
    
    INSERT INTO NotificationTemplateContent (`NotificationTemplateId`, `ChannelId`, `Content`)
	SELECT notificationTemplateID, ChannelId, Content
    FROM tmpContent;
    
	#Get Finish Time to calcualte execution duration
    SET finishTime = UTC_TIMESTAMP(3);
    SET duration =  TIMESTAMPDIFF(MICROSECOND, startTime, finishTime)  / 1000; 	#miliseconds;
    
    #Get Log Content, JSON format
    SELECT json_object ('notificationTypeID', nt.NotificationTypeID ,'notificationTemplateName', nt.NotificationTemplateName ,'title', nt.Title, 'ContentLst',ContentLst ,'creatorID', nt.CreatorID ,'createdDate', nt.CreatedDate)
	INTO logContent
	FROM NotificationTemplate nt
    WHERE nt.NotificationTemplateID = notificationTemplateID;
    
	#Write User Log
	CALL NCB_UserLog_RunSP_Ins(ACTION_ID, notificationTemplateID, creatorID, logContent, startTime, duration);
END;
//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_NotificationTemplate_Ins TO 'ncb.admin'@'%';
