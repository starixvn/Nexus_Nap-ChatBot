DELIMITER //

DROP PROCEDURE IF EXISTS NCB_SendNotification_RecipientSentStatus_Upd;

CREATE PROCEDURE  NCB_SendNotification_RecipientSentStatus_Upd(
	IN notificationID INT
    , IN sentStatus MEDIUMTEXT CHARACTER SET utf8mb4
    , IN userID INT
	, OUT errCode SMALLINT
	, OUT errMsg VARCHAR(100)
    /*
		Creator: Dominic@20190401
        Param:
                - notificationID: ID of Notification
                - sentStatus: in JSON format
						[  
							{  
								"ChatGroupID":1,
								"ChatGroupConversationID":"19:I2NyNy5uZ3V5ZW4vJDljN2RhZjU5OGQ0ZTc4ODg=@p2p.thread.skype",
								"BotMemberID":"28:ea8d17a4-16c8-43b3-a1a8-ade5277dbd24",
								"SentTime":"2019-04-01 12:23:22.000000",
								"IsSuccess":0,
								"ErrMsg":"Bot Is Removed"
							},
							{  
								"ChatGroupID":2,
								"ChatGroupConversationID":"19:636032a5293849c59f06c8403a24b2e9@thread.skype",
								"BotMemberID":"28:5de427b9-a7f0-497e-81d3-b3348721500a",
								"SentTime":"2019-04-01 12:23:22.000000",
								"IsSuccess":0,
								"ErrMsg":"Bot Is Removed"
							}
						]
        Revisions:
				- 20190401@Dominic: First Commit
        
    */
)
NCB_SendNotification_RecipientSentStatus_Upd:BEGIN

	#Variables used to write User Logs
	DECLARE startTime 			DATETIME(3);
    DECLARE finishTime 		DATETIME(3); 
    DECLARE duration 			INT;							
    DECLARE logContent 		TEXT CHARACTER SET utf8mb4;
    DECLARE ACTION_ID 		INT DEFAULT 7;
	DECLARE iLoop INT;
    
    #Constants
    DECLARE NOTIFICATION_STATUSID_SENDING	INT DEFAULT 3;
    
    DECLARE SENT_NOTIFICATION_STATUSID	TINYINT UNSIGNED DEFAULT 1;
    DECLARE SENT_NOTIFICATION_STATUS	VARCHAR(10) DEFAULT 'Sent ';
    
    DECLARE ERROR_NOTIFICATION_STATUSID	TINYINT UNSIGNED DEFAULT 2;
    DECLARE ERROR_NOTIFICATION_STATUS	VARCHAR(10) DEFAULT 'Error';
    
    SET  startTime = UTC_TIMESTAMP(3);
    SET errCode=0, errMsg='';

	#Check param
	IF  NOT EXISTS(	SELECT 1 
									FROM Notification n
                                    WHERE n.NotificationID = notificationID
										AND n.StatusID = NOTIFICATION_STATUSID_SENDING
                                        AND n.SendImmediately = 0
                                    ) THEN
		SET errCode=701, errMsg='Invalid NotificationID';
        LEAVE NCB_SendNotification_RecipientSentStatus_Upd;
    ELSEIF IFNULL(SentStatus,'') = '' THEN
		SET errCode=702, errMsg='SentStatus must not be empty';
        LEAVE NCB_SendNotification_RecipientSentStatus_Upd;
	END IF;

    SET sentStatus = REPLACE(sentStatus,'\\','\\\\');
    
    DROP TEMPORARY TABLE IF EXISTS tmp_ChatGroupSentStatus;
	CREATE TEMPORARY TABLE tmp_ChatGroupSentStatus(
																	ChatGroupID INT
																	, ChatGroupConversationID VARCHAR(70)  CHARACTER SET utf8mb4
                                                                    , BotMemberID VARCHAR(70)  CHARACTER SET utf8mb4
                                                                    , SentTime DATETIME
                                                                    , IsSuccess TINYINT
                                                                    , ErrMsg VARCHAR(1000) CHARACTER SET utf8mb4
                                                                    , INDEX INDEX_ChatGroupID USING BTREE (ChatGroupID)
																);

	SET iLoop = 0;
	WHILE (JSON_EXTRACT(SentStatus, CONCAT('$[',iLoop, '].ChatGroupID'))  IS NOT NULL) DO
		INSERT INTO tmp_ChatGroupSentStatus(
																		ChatGroupID
                                                                        , ChatGroupConversationID
                                                                        , BotMemberID
                                                                        , SentTime
                                                                        , IsSuccess
                                                                        , ErrMsg
																	)
		SELECT JSON_EXTRACT(SentStatus, CONCAT('$[', iLoop, '].ChatGroupID')) 
						, JSON_UNQUOTE(JSON_EXTRACT(SentStatus, CONCAT('$[', iLoop, '].ChatGroupConversationID')))
                        , JSON_UNQUOTE(JSON_EXTRACT(SentStatus, CONCAT('$[', iLoop, '].BotMemberID')))
                        , JSON_UNQUOTE(JSON_EXTRACT(SentStatus, CONCAT('$[', iLoop, '].SentTime')))
                       ,  JSON_EXTRACT(SentStatus, CONCAT('$[', iLoop, '].IsSuccess'))
                        , JSON_UNQUOTE(JSON_EXTRACT(SentStatus, CONCAT('$[', iLoop, '].ErrMsg')));
                        
		SET iLoop = iLoop +1;
        
	END WHILE;
	
	UPDATE NotificationRecipientList nrl
		INNER JOIN tmp_ChatGroupSentStatus tmp
			ON tmp.ChatGroupID = nrl.BotConversationID
    SET nrl.SentTime = tmp.SentTime
			, nrl.ErrMsg = tmp.ErrMsg
            , nrl.StatusID = CASE 
										WHEN tmp.IsSuccess = 1 THEN SENT_NOTIFICATION_STATUSID
                                        ELSE ERROR_NOTIFICATION_STATUSID
									END
			 , nrl.Status = CASE 
										WHEN tmp.IsSuccess = 1 THEN SENT_NOTIFICATION_STATUS
                                        ELSE ERROR_NOTIFICATION_STATUS
									END
	WHERE nrl.NotificationID = notificationID;
    
    #Get Finish Time to calcualte execution duration
    SET finishTime = UTC_TIMESTAMP(3);
    SET duration =  TIMESTAMPDIFF(MICROSECOND
																		, startTime
                                                                        , finishTime)  / 1000; 	#miliseconds
    
    #Get Log Content, JSON format
    SET logContent = json_object(
                                         'notificationID', notificationID
                                        , 'sentStatus', '#logsentStatus#'
					) ;
     
     SET logContent = REPLACE(logContent, '"#logsentStatus#"', IFNULL(sentStatus, '[]'));
	
	#Write User Log
       CALL NCB_UserLog_RunSP_Ins(
											ACTION_ID #actionID
                                            , notificationID #referenceID
											, userID #UserID
                                            , logContent #LogContent
                                            , startTime #logtime
                                            , duration	#Duration
                                            );
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_SendNotification_RecipientSentStatus_Upd TO 'ncb.admin'@'%';