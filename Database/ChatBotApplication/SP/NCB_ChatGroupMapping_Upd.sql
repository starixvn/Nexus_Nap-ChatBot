DELIMITER //
CREATE PROCEDURE `NCB_ChatGroupMapping_Upd`(
	IN groupID INT,
    IN recipientTypeID INT, 
    IN recipientIDs VARCHAR(1000), 
    IN chatGroupTypeID INT,
    IN remark VARCHAR(1000),
    IN creatorID INT    
)
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
BEGIN
	DECLARE strLen    INT DEFAULT 0;
	DECLARE subStrLen INT DEFAULT 0;
    
	IF EXISTS(SELECT 1 FROM BotConversationExtraInfo bce WHERE bce.BotConversationID = groupID) THEN
		UPDATE BotConversationExtraInfo bce
        SET bce.ChatGroupTypeID = chatGroupTypeID, bce.Remark = remark
        WHERE bce.BotConversationID = groupID;
    ELSE
		INSERT INTO BotConversationExtraInfo(GroupID, ChatGroupTypeID, Remark)
        VALUES (groupID, chatGroupTypeID, remark);
    END IF;    
    
    #Insert comma seperated list RecipientIDLst to temporary tables
    DROP TEMPORARY TABLE IF EXISTS tmp_RecipientID;
	CREATE TEMPORARY TABLE tmp_RecipientID(RecipientID INT);
    
	SET @SqlCommand = REPLACE(recipientIDs, ',', '),(');
	SET @SqlCommand = CONCAT('(', @SqlCommand, ')');
	SET @SqlCommand= CONCAT('INSERT INTO tmp_RecipientID VALUES'
														, @SqlCommand); 
	
    PREPARE Stmt FROM  @SqlCommand;
	EXECUTE Stmt;
	DEALLOCATE PREPARE Stmt;
    
    DELETE FROM BotConversationRecipient 
    WHERE BotConversationID = groupID;
    
    INSERT INTO BotConversationRecipient(BotConversationID, RecipientID)
    SELECT groupID AS BotConversationID, tri.RecipientID
    FROM tmp_RecipientID tri;
    
    DROP TEMPORARY TABLE IF EXISTS tmp_RecipientID;        
END;