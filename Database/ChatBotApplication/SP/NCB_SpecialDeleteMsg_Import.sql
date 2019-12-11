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
	DECLARE ACTION_ID 		INT DEFAULT 18;

    DROP TEMPORARY TABLE IF EXISTS tmp_MsgHeader;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmp_MsgHeader(
            MessageID INT, 
            MsgContentsType INT,
            Content_en TEXT, 
            Content_cs TEXT,
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
    
    UPDATE tmp_MsgContentTypeTemplate tmp
    INNER JOIN NotificationTemplate nt ON tmp.NotificationTemplateID = nt.NotificationTemplateID
	SET tmp.TemplateContent = nt.Content;
    
    IF  EXISTS(SELECT 1 
									FROM tmp_MsgContentTypeTemplate  nt
                                    WHERE nt.TemplateContent IS NULL ) THEN
    
		SELECT GROUP_CONCAT( DISTINCT nt.NotificationTemplateID  ORDER BY nt.NotificationTemplateID SEPARATOR ', ') 
        INTO errMsg
        FROM tmp_MsgContentTypeTemplate  nt
		WHERE nt.TemplateContent IS NULL;
        
		SET errMsg = CONCAT('Notification template is not existing (IDs): ' , errMsg);
        LEAVE NCB_SpecialDeleteMsg_Import;
        
	END IF;
    
     IF  EXISTS(SELECT 1 
									FROM tmp_MsgContentTypeTemplate  nt
                                    WHERE nt.TemplateContent NOT LIKE '%{EN Announcement}%' 
										OR nt.TemplateContent NOT LIKE '%{CN Announcement}%'  ) THEN
    
		SELECT GROUP_CONCAT( DISTINCT nt.NotificationTemplateID  ORDER BY nt.NotificationTemplateID SEPARATOR ', ') 
        INTO errMsg
        FROM tmp_MsgContentTypeTemplate  nt
		WHERE nt.TemplateContent NOT LIKE '%{EN Announcement}%' 
										OR nt.TemplateContent NOT LIKE '%{CN Announcement}%' ;
        
		SET errMsg = CONCAT('Notification template is not existing (IDs): ' , errMsg);
        LEAVE NCB_SpecialDeleteMsg_Import;
        
	END IF;
    
	SELECT * FROM tmp_MsgHeader;
    SELECT * FROM tmp_MsgDetail;
	SELECT * FROM tmp_MsgContentTypeTemplate;
END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE NCB_SpecialDeleteMsg_Import TO 'ncb.admin'@'%';