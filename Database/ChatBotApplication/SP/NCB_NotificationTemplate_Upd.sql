DROP PROCEDURE IF EXISTS NCB_NotificationTemplate_Upd;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_NotificationTemplate_Upd`(
	IN		notificationTemplateID		INT
	,IN		notificationTypeID			SMALLINT
	,IN		notificationTemplateName	VARCHAR(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci
	,IN		title VARCHAR(300)			CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci
	,IN		content						MEDIUMTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci
	,IN		modifierID					INT
	,OUT	errCode						SMALLINT
	,OUT	errMsg						VARCHAR(100)
	/*
		Creator: Elton@20190913
		Revisions:
			- 20190910@Elton: First Commit
			- 20191217@Merlin: Check valid content before update
	*/
)
NCB_NotificationTemplate_Upd:BEGIN
	#Variables used to write User Logs
	DECLARE startTime 		DATETIME(3);
	DECLARE finishTime 		DATETIME(3); 
	DECLARE duration 		INT;							
	DECLARE logContent 		TEXT CHARACTER SET utf8mb4;
	DECLARE ACTION_ID 		INT DEFAULT 13;
    DECLARE jsonValue		JSON;
    
	#Get Start Time to calcualte execution duration
	SET startTime = UTC_TIMESTAMP(3);
	SET errCode = 0, errMsg = '';
  
	IF EXISTS(
	SELECT 1 FROM NotificationTemplate nt
	WHERE nt.NotificationTemplateName = notificationTemplateName
		AND nt.NotificationTemplateID <> notificationTemplateID
	) 
	THEN
		SET errCode = 101, errMsg = 'The selected template name has already been taken';
		LEAVE NCB_NotificationTemplate_Upd;
	END IF;
	
	SELECT Value INTO jsonValue FROM SystemSetting WHERE ID = 4 LIMIT 1;
	IF (
		EXISTS (
			SELECT 1 FROM (			
				SELECT NotificationTypeID, NotificationTemplateID
				FROM JSON_TABLE(jsonValue, "$[*]"
					COLUMNS(
						NotificationTypeID INT PATH "$.NotificationTypeID" DEFAULT '0' ON EMPTY DEFAULT '-1' ON ERROR,
						NotificationTemplateID INT PATH "$.NotificationTemplateID" DEFAULT '0' ON EMPTY DEFAULT '-1' ON ERROR
					)
				) tmpTable
				WHERE tmpTable.NotificationTemplateID = notificationTemplateID
					AND tmpTable.NotificationTypeID = notificationTypeID
			) tmpTable
		)
        AND NOT (content LIKE '%{EN Announcement}%' AND content LIKE '%{CN Announcement}%')
    )
	THEN
		SET errCode = 102, errMsg = 'Invalid special delete template content format';
		LEAVE NCB_NotificationTemplate_Upd;
	END IF;
    
	UPDATE NotificationTemplate nt
	SET nt.NotificationTemplateName = notificationTemplateName
		,nt.NotificationTypeID = notificationTypeID
		,nt.Title = title
		,nt.Content =content
		,nt.LastModifiedDate = UTC_TIMESTAMP()
		,nt.LastModifiedBy = modifierID
	WHERE nt.NotificationTemplateID = notificationTemplateID;

	#Get Finish Time to calcualte execution duration
	SET finishTime = UTC_TIMESTAMP(3);
	SET duration = TIMESTAMPDIFF(MICROSECOND, startTime, finishTime) / 1000; 	#miliseconds;
	
	#Get Log Content, JSON format
	SELECT json_object('notificationTypeID', nt.NotificationTypeID
		,'notificationTemplateName', nt.NotificationTemplateName
		,'title', nt.Title
		,'content', nt.content
		,'lastModifiedBy', nt.LastModifiedBy
		,'lastModifiedDate', nt.LastModifiedDate
	)
	INTO logContent
	FROM NotificationTemplate nt
	WHERE nt.NotificationTemplateID = notificationTemplateID;
    
	#Write User Log
	CALL NCB_UserLog_RunSP_Ins(ACTION_ID
		,notificationTemplateID 
		,modifierID 
		,logContent 
		,startTime 
		,duration
	);
	
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_NotificationTemplate_Upd TO 'ncb.admin'@'%';