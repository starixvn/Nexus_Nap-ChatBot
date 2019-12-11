DROP PROCEDURE IF EXISTS NCB_SpecialDeleteMsg_Import;
DELIMITER //

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  NCB_SpecialDeleteMsg_Import(
	IN messageHeader JSON,
	IN messageDetail JSON,
    OUT errCode INT,
    OUT errMsg VARCHAR(200)

    /*
		Creator: Dominic@20191211
        Task: Chatbot For Licensee - Phase 2: Integrate Special Delete - Redmine:#122905
        Param:
				-  @MessageHeader:
							[  
								{ "MessageID":885128, "MsgContentsType":101, "Content_en":"Attn:[Soccer] Due to bets er...", "Content_cs":"請注意:[籃球] 由於中文隊名顯示.." }
								, { "MessageID":885131,"MsgContentsType":101,"Content_en":"Attn:[Soccer] Due to bets er...", "Content_cs":"請注意:[籃球] 由於中文隊名顯示..."}
							]
				- @MessageDetail
							[  
								{ "MessageID":885128, "Site":"IBO" }
								, { "MessageID":885128, "Site":"XTD3" }
								, { "MessageID":885131,"Site":"mansion88"}
								, { "MessageID":885131, "Site":"haifa"}
							]
        Revisions:
				- 20191211@Dominic: First Commit
        
    */
)
SQL SECURITY DEFINER
NCB_SpecialDeleteMsg_Import:BEGIN
    # Variables
	DECLARE iLoop INT;
    DECLARE startTime 			DATETIME(3);
    DECLARE finishTime 		DATETIME(3); 
    DECLARE duration 			INT;							
    DECLARE logContent 		TEXT CHARACTER SET utf8mb4;
    DECLARE templateNotifcationType JSON;

    #Constants
	DECLARE ACTION_ID 	INT DEFAULT 18;
    DECLARE VALUE_4_RUNNING 	TEXT  DEFAULT 'Running';
    DECLARE VALUE_4_DONE 	TEXT DEFAULT 'Done';
    DECLARE RECIPIENT_TYPE_LICENSEE INT DEFAULT 1;
    DECLARE STARIXIT_CUSTID 	INT DEFAULT 10278938;
    
    DECLARE NEW_NOTIFICATION_STATUSID	TINYINT UNSIGNED DEFAULT 0;
    DECLARE NEW_NOTIFICATION_STATUS	VARCHAR(10) DEFAULT 'Planned';
    DECLARE CREATING_NOTIFICATION_STATUSID	TINYINT UNSIGNED DEFAULT 7;
    DECLARE CREATING_NOTIFICATION_STATUS	VARCHAR(10) DEFAULT 'Creating';

	SET  startTime = UTC_TIMESTAMP(3);
    SET errCode=0, errMsg='';
    
    DROP TEMPORARY TABLE IF EXISTS tmp_MsgHeader;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmp_MsgHeader(
            MessageID INT, 
            MsgContentsType INT,
            Content_en TEXT, 
            Content_cs TEXT,
            CheckMsg VARCHAR(1000),
            UNIQUE INDEX Index_MessageID(MessageID)
            );
    
    SET iLoop = 0;
    WHILE (JSON_EXTRACT(messageHeader, CONCAT('$[',iLoop, '].MessageID'))  IS NOT NULL) DO 
		INSERT INTO tmp_MsgHeader(MessageID, MsgContentsType, Content_en, Content_cs)
        VALUES (
			JSON_UNQUOTE(JSON_EXTRACT(messageHeader, CONCAT('$[', iLoop, '].MessageID'))), 
            JSON_UNQUOTE(JSON_EXTRACT(messageHeader, CONCAT('$[', iLoop, '].MsgContentsType'))),
            JSON_UNQUOTE(JSON_EXTRACT(messageHeader, CONCAT('$[', iLoop, '].Content_en'))),
            JSON_UNQUOTE(JSON_EXTRACT(messageHeader, CONCAT('$[', iLoop, '].Content_cs')))
		);
        
        SET iLoop := iLoop + 1;
    END WHILE;    

	DROP TEMPORARY TABLE IF EXISTS tmp_MsgDetail;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmp_MsgDetail(
            MessageID INT, 
            Site TEXT,
            RecipientID INT,
            INDEX Index_MessageID(MessageID)
            );
    
    SET iLoop = 0;
    WHILE (JSON_EXTRACT(messageDetail, CONCAT('$[',iLoop, '].MessageID'))  IS NOT NULL) DO 
		INSERT INTO tmp_MsgDetail(MessageID, Site)
        VALUES (
			JSON_UNQUOTE(JSON_EXTRACT(messageDetail, CONCAT('$[', iLoop, '].MessageID'))), 
            JSON_UNQUOTE(JSON_EXTRACT(messageDetail, CONCAT('$[', iLoop, '].Site')))
		);
        
        SET iLoop := iLoop + 1;
    END WHILE;    
    
    SELECT Value INTO templateNotifcationType 
    FROM SystemSetting ss
    WHERE ss.ID = 4; 
    
    DROP TEMPORARY TABLE IF EXISTS tmp_MsgContentTypeTemplate;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmp_MsgContentTypeTemplate(
				MsgContentsType INT
                , NotificationTypeID INT
                , NotificationTemplateID INT
                , TemplateTitle VARCHAR(300)
                , TemplateContent MEDIUMTEXT
            );
   SET iLoop = 0;
   WHILE (JSON_EXTRACT(templateNotifcationType, CONCAT('$[',iLoop, '].MsgContentsType'))  IS NOT NULL) DO 
		INSERT INTO tmp_MsgContentTypeTemplate(MsgContentsType, NotificationTypeID, NotificationTemplateID)
        VALUES (
			JSON_UNQUOTE(JSON_EXTRACT(templateNotifcationType, CONCAT('$[', iLoop, '].MsgContentsType')))
            , JSON_UNQUOTE(JSON_EXTRACT(templateNotifcationType, CONCAT('$[', iLoop, '].NotificationTypeID')))
            , JSON_UNQUOTE(JSON_EXTRACT(templateNotifcationType, CONCAT('$[', iLoop, '].NotificationTemplateID')))
		);
        
        SET iLoop := iLoop + 1;
    END WHILE;    
    
    #Get template content
    UPDATE tmp_MsgContentTypeTemplate tmp
    INNER JOIN NotificationTemplate nt ON tmp.NotificationTemplateID = nt.NotificationTemplateID
	SET tmp.TemplateContent = nt.Content
			, tmp.TemplateTitle = nt.Title;
    
    #Check if there are any template IDs are not existing
    IF  EXISTS(SELECT 1 
									FROM tmp_MsgContentTypeTemplate  nt
                                    WHERE nt.TemplateContent IS NULL ) THEN
		SET errCode = 1801;
		SELECT GROUP_CONCAT( DISTINCT nt.NotificationTemplateID  ORDER BY nt.NotificationTemplateID SEPARATOR ', ') 
        INTO errMsg
        FROM tmp_MsgContentTypeTemplate  nt
		WHERE nt.TemplateContent IS NULL;
        
		SET errMsg = CONCAT('Notification template is not existing (IDs): ' , errMsg);
        LEAVE NCB_SpecialDeleteMsg_Import;
        
	END IF;
    
    #Check if there are any template IDs which do not have required params
     IF  EXISTS(SELECT 1 
									FROM tmp_MsgContentTypeTemplate  nt
                                    WHERE nt.TemplateContent NOT LIKE '%{EN Announcement}%' 
										OR nt.TemplateContent NOT LIKE '%{CN Announcement}%'  ) THEN
		SET errCode = 1802;
		SELECT GROUP_CONCAT( DISTINCT nt.NotificationTemplateID  ORDER BY nt.NotificationTemplateID SEPARATOR ', ') 
        INTO errMsg
        FROM tmp_MsgContentTypeTemplate  nt
		WHERE nt.TemplateContent NOT LIKE '%{EN Announcement}%' 
										OR nt.TemplateContent NOT LIKE '%{CN Announcement}%' ;
        
		SET errMsg = CONCAT('Notification template is not existing (IDs): ' , errMsg);
        LEAVE NCB_SpecialDeleteMsg_Import;
        
	END IF;
    
    DELETE FROM tmp_MsgHeader h WHERE h.MessageID IN (SELECT MessageID FROM NotificationSystemMsg);
    DELETE FROM tmp_MsgDetail d WHERE d.MessageID IN (SELECT MessageID FROM NotificationSystemMsg);
    
    #Update flag to 'running'
    UPDATE SystemSetting 
    SET Value = VALUE_4_RUNNING
    WHERE ID = 1 AND Value = VALUE_4_DONE;
    
     IF ROW_COUNT()  = 0 THEN
        LEAVE NCB_SpecialDeleteMsg_Import;
    END IF;
     
     #Get RecipientID
    UPDATE tmp_MsgDetail tmp
    INNER JOIN Licensee l ON l.RegisteredLicenseeName = tmp.Site
    INNER JOIN Recipient r ON r.SourceID = l.LicenseeId AND r.RecipientTypeID = RECIPIENT_TYPE_LICENSEE
	SET tmp.RecipientID = r.RecipientID;
    
    #Check if there are any sites which can not be mapped with any recipients
    UPDATE tmp_MsgHeader h
    SET h.CheckMsg = (
		SELECT GROUP_CONCAT(d.Site  ORDER BY d.Site SEPARATOR ', ') 
		FROM tmp_MsgDetail d 
		WHERE d.RecipientID IS NULL AND d.MessageID = h.MessageID
    );
    
    UPDATE   tmp_MsgHeader h
    SET h.CheckMsg = CONCAT('- Can not map to any Recipient(s): ' , h.CheckMsg)
    WHERE h.CheckMsg  IS NOT NULL;
    
	#Insert  NotificationSystemMsg
    INSERT INTO NotificationSystemMsg(MessageID, NotificationID, CheckMsg, CreatedDate)
    SELECT h.MessageID, NULL NotificationID, h.CheckMsg, startTime
    FROM tmp_MsgHeader h
    WHERE h.MessageID NOT IN (SELECT MessageID FROM NotificationSystemMsg);
    
    #Insert Notification
     INSERT INTO Notification(NotificationTypeID
											, ProductID
											, Title
                                            , Content
                                            , ConfigSendTime
                                            , SendImmediately
                                            , CreatorID
                                            , CreatedDate
                                            , Status
                                            , StatusID
                                            , LastModifiedBy)
         SELECT DISTINCT ct.NotificationTypeID
						, NULL ProductID
                        , ct.TemplateTitle Title
						, REPLACE(REPLACE(ct.TemplateContent, '{EN Announcement}', h.Content_en )
												, '{CN Announcement}', h.Content_cs) Content
                        , startTime ConfigSendTime
                        , 0 SendImmediately
                        , STARIXIT_CUSTID CreatorID
                        , startTime CreatedDate
                        , CREATING_NOTIFICATION_STATUS Status
                        , CREATING_NOTIFICATION_STATUSID StatusID
                        , h.MessageID LastModifiedBy
         FROM tmp_MsgHeader h
         INNER JOIN tmp_MsgContentTypeTemplate ct ON ct.MsgContentsType = h.MsgContentsType
         INNER JOIN tmp_MsgDetail d  ON h.MessageID = d.MessageID AND d.RecipientID IS NOT NULL;
         
         #Update NotificationSystemMsg
         UPDATE NotificationSystemMsg nm
         INNER JOIN Notification n ON n.StatusID = CREATING_NOTIFICATION_STATUSID 
																AND n.ConfigSendTime = startTime
                                                                AND n.LastModifiedBy = nm.MessageID
		SET nm.NotificationID = n.NotificationID
		WHERE nm.MessageID IN (SELECT h.MessageID FROM tmp_MsgHeader h);
         
		#Insert recipient list
        INSERT INTO NotificationRecipientList(NotificationID, ConversationGroupID
																		, ChatGroupTypeID, RecipientID
																		, Status, StatusID)
        SELECT nm.NotificationID
					, IFNULL(gri.ConversationGroupID, -1)
					, IFNULL(gri.ChatGroupTypeID,-1)
                    , d.RecipientID
					, NEW_NOTIFICATION_STATUS
					, NEW_NOTIFICATION_STATUSID
        FROM NotificationSystemMsg nm
        INNER JOIN tmp_MsgDetail d ON d.MessageID = nm.MessageID
			LEFT JOIN ConversationGroupRecipient gr 
				ON d.RecipientID = gr.RecipientID
			LEFT JOIN ConversationGroupExtraInfo gri 
				ON gri.ConversationGroupID = gr.ConversationGroupID
        WHERE nm.MessageID IN (SELECT h.MessageID FROM tmp_MsgHeader h)
			AND d.RecipientID IS NOT NULL;
        
        #Update Status for Notification
        UPDATE Notification n
        INNER JOIN NotificationSystemMsg nm ON nm.NotificationID = n.NotificationID
        SET n.Status = NEW_NOTIFICATION_STATUS
				, n.StatusID = NEW_NOTIFICATION_STATUSID
                , n.LastModifiedBy = NULL
		WHERE nm.MessageID IN (SELECT h.MessageID FROM tmp_MsgHeader h);
        
        #Update flag to 'Done'
		UPDATE SystemSetting 
		SET Value = VALUE_4_DONE
		WHERE ID = 1 AND Value = VALUE_4_RUNNING;
        
	 #Get Finish Time to calcualte execution duration
    SET finishTime = UTC_TIMESTAMP(3);
    SET duration =  TIMESTAMPDIFF(MICROSECOND
																		, startTime
                                                                        , finishTime)  / 1000; 	#miliseconds
    
    #Get Log Content, JSON format
    SET logContent = json_object('MessageHeader', messageHeader
										, 'MessageDetail', messageDetail
					) ;
                    
	#Write User Log
    IF EXISTS (SELECT 1 FROM tmp_MsgHeader) THEN
		CALL NCB_UserLog_RunSP_Ins(
												ACTION_ID #actionID
												, -1  #referenceID
												, STARIXIT_CUSTID #UserID
												, logContent #LogContent
												, startTime #logtime
												, duration	#Duration
												);
	END IF;
END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE NCB_SpecialDeleteMsg_Import TO 'ncb.admin'@'%';