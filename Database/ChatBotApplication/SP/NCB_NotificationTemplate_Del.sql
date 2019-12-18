DROP PROCEDURE IF EXISTS NCB_NotificationTemplate_Del;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_NotificationTemplate_Del`(
	IN		notificationTemplateID	INT
	,IN		userID					INT
	,OUT	errCode					SMALLINT
	,OUT	errMsg					VARCHAR(100)
	/*
	Creator: Elton@20190913
		Revisions:
			- 20190910@Elton: First Commit
            - 20191218@Merlin.Le: Check not setting template before delete
	*/
)
NCB_NotificationTemplate_Del:BEGIN
	#Variables used to write User Logs
	DECLARE startTime 		DATETIME(3);
	DECLARE finishTime 		DATETIME(3); 
	DECLARE duration 			INT;							
	DECLARE logContent 		TEXT CHARACTER SET utf8mb4;
	DECLARE ACTION_ID 		INT DEFAULT 14;
	DECLARE jsonValue 		JSON;

	#Get Start Time to calcualte execution duration
	SET  startTime = UTC_TIMESTAMP(3);
	SET errCode = 0, errMsg = '';
    
	IF NOT EXISTS(SELECT 1 FROM NotificationTemplate nt WHERE nt.NotificationTemplateID = notificationTemplateID) 
	THEN
		SET errCode = 101, errMsg = 'Invalid Template Id';
		LEAVE NCB_NotificationTemplate_Del;
	END IF;

	SELECT Value INTO jsonValue FROM SystemSetting WHERE ID = 4 LIMIT 1;

	IF EXISTS (
		SELECT 1 FROM NotificationTemplate nt
		INNER JOIN (			
			SELECT NotificationTypeID, NotificationTemplateID
			FROM JSON_TABLE(jsonValue, "$[*]"
				COLUMNS(
					NotificationTypeID INT PATH "$.NotificationTypeID" DEFAULT '0' ON EMPTY DEFAULT '-1' ON ERROR,
					NotificationTemplateID INT PATH "$.NotificationTemplateID" DEFAULT '0' ON EMPTY DEFAULT '-1' ON ERROR
				)
			) tvt
        ) tvt ON nt.NotificationTemplateID = tvt.NotificationTemplateID
			AND nt.NotificationTypeID = tvt.NotificationTypeID
	)
	THEN
		SET errCode = 102, errMsg = 'Cannot delete this template.';
		LEAVE NCB_NotificationTemplate_Del;
	END IF;
    
	DELETE FROM NotificationTemplate nt
	WHERE nt.NotificationTemplateID = notificationTemplateID;

	#Get Finish Time to calcualte execution duration
	SET finishTime = UTC_TIMESTAMP(3);
	SET duration =  TIMESTAMPDIFF(MICROSECOND, startTime, finishTime) / 1000; 	#miliseconds;
	
	#Get Log Content, JSON format
	SET logContent = json_object('notificationTemplateID', notificationTemplateID);
    
	#Write User Log
	CALL NCB_UserLog_RunSP_Ins(ACTION_ID
		,notificationTemplateID 
		,userID 
		,logContent 
		,startTime 
		,duration
	);
END;


//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_NotificationTemplate_Del TO 'ncb.admin'@'%';