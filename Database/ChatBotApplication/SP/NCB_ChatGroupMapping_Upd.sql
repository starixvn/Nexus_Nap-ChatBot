DELIMITER //

DROP PROCEDURE IF EXISTS NCB_ChatGroupMapping_Upd;

CREATE PROCEDURE `NCB_ChatGroupMapping_Upd`(
	IN groupID INT,
    IN recipientTypeID INT, 
    IN recipientIDs VARCHAR(1000), 
    IN chatGroupTypeID INT,
    IN remark VARCHAR(1000),
    IN creatorID INT,
    OUT errorCode SMALLINT,
	OUT errorMsg VARCHAR(100)
	/*
		Creator: Scofield@20190404
        Params:
				- groupID: Bot Conversation ID
                - recipientTypeID: Recipient Type ID
                - recipientIDs: Recipient IDs, ex: "1,2"
                - chatGroupTypeID: Chat Group Type ID
                - remark: Bot Conversation Remark
                - creatorID: Creator ID
        Revisions:
				- 20190404@Scofield: Create        
    */
)
NCB_ChatGroupMapping_Upd:BEGIN
	-- Declare variables
	DECLARE strLen    INT DEFAULT 0;
	DECLARE subStrLen INT DEFAULT 0;
	DECLARE ACTION_ID INT DEFAULT 2;
	DECLARE startTime DATETIME(3);
    DECLARE finishTime DATETIME(3); 
	DECLARE duration INT;							
	DECLARE logContent TEXT;
	
	-- Get Start Time to calcualte execution duration
    SET  startTime = UTC_TIMESTAMP(3);

	-- Set default values for errorCode and errorMsg
    SET errorCode=0, errorMsg='';
    
    -- Check if params are VALID
	IF  groupID IS NULL THEN
		SET errorCode=101, errorMsg='Invalid GroupID';
        LEAVE NCB_ChatGroupMapping_Upd;
	ELSEIF recipientTypeID IS NULL THEN
		SET errorCode=102, errorMsg='Invalid RecipientTypeID';
        LEAVE NCB_ChatGroupMapping_Upd;
	ELSEIF chatGroupTypeID IS NULL THEN
		SET errorCode=103, errorMsg='Invalid ChatGroupTypeID';
        LEAVE NCB_ChatGroupMapping_Upd;
	ELSEIF creatorID IS NULL THEN
		SET errorCode=104, errorMsg='Invalid CreatorID';
        LEAVE NCB_ChatGroupMapping_Upd;
	ELSEIF IFNULL(recipientIDs,'') = '' THEN
		SET errorCode=105, errorMsg='Recipients can not be empty';
        LEAVE NCB_ChatGroupMapping_Upd;	
	END IF;   
    
	-- If groupID exists in BotConversationExtraInfo then Update BotConversationExtraInfo
	-- Else Insert into BotConversationExtraInfo
	IF EXISTS(SELECT 1 FROM BotConversationExtraInfo bce WHERE bce.BotConversationID = groupID) THEN
		UPDATE BotConversationExtraInfo bce
        SET bce.ChatGroupTypeID = chatGroupTypeID, bce.Remark = remark
        WHERE bce.BotConversationID = groupID;
    ELSE
		INSERT INTO BotConversationExtraInfo(BotConversationID, ChatGroupTypeID, Remark)
        VALUES (groupID, chatGroupTypeID, remark);
    END IF;    
    
    -- Split recipientIDs into temporary table tmp_RecipientID
    DROP TEMPORARY TABLE IF EXISTS tmp_RecipientID;
	CREATE TEMPORARY TABLE tmp_RecipientID(RecipientID INT);
    
	SET @SqlCommand = REPLACE(recipientIDs, ',', '),(');
	SET @SqlCommand = CONCAT('(', @SqlCommand, ')');
	SET @SqlCommand= CONCAT('INSERT INTO tmp_RecipientID VALUES'
														, @SqlCommand); 
	
    PREPARE Stmt FROM  @SqlCommand;
	EXECUTE Stmt;
	DEALLOCATE PREPARE Stmt;
    
	-- Update BotConversationRecipient with values from tmp_RecipientID
    DELETE FROM BotConversationRecipient 
    WHERE BotConversationID = groupID;
    
    INSERT INTO BotConversationRecipient(BotConversationID, RecipientID)
    SELECT groupID AS BotConversationID, tri.RecipientID
    FROM tmp_RecipientID tri;    
	
	-- Drop temporary tables
    DROP TEMPORARY TABLE IF EXISTS tmp_RecipientID;        
	
	-- Get Finish Time to calcualte execution duration
    SET finishTime = UTC_TIMESTAMP(3);
    SET duration =  TIMESTAMPDIFF(MICROSECOND, startTime, finishTime)  / 1000;
	
	-- Get Log Content, JSON format
    SET logContent = json_object('groupID', groupID
								, 'recipientTypeID', recipientTypeID
								, 'recipientIDs', recipientIDs
								, 'chatGroupTypeID', chatGroupTypeID
								, 'remark', remark
								, 'creatorID', creatorID);
										
	-- Write User Log
	CALL NCB_UserLog_RunSP_Ins(ACTION_ID, creatorID, logContent, startTime, duration);
END;