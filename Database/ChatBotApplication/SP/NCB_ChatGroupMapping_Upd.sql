DELIMITER //
CREATE PROCEDURE `NCB_ChatGroupMapping_Upd`(
	IN in_GroupID INT,
    IN in_RecipientTypeID INT, 
    IN in_RecipientIDs VARCHAR(1000), 
    IN in_ChatGroupTypeID INT,
    IN in_Remark VARCHAR(1000),
    IN in_CreatorID INT,
    OUT out_ErrorCode INT, 
    OUT out_ErrorMsg VARCHAR(100)
)
BEGIN
	DECLARE strLen    INT DEFAULT 0;
	DECLARE subStrLen INT DEFAULT 0;
    DECLARE recipientIDs VARCHAR(1000);
    
	IF EXISTS(SELECT 1 FROM BotConversationExtraInfo bce WHERE bce.GroupID = in_GroupID) THEN
		UPDATE BotConversationExtraInfo bce
        SET bce.ChatGroupTypeID = in_ChatGroupTypeID, bce.Remark = in_Remark
        WHERE bce.GroupID = in_GroupID;
    ELSE
		INSERT INTO BotConversationExtraInfo(GroupID, ChatGroupTypeID, Remark)
        VALUES (in_GroupID, in_ChatGroupTypeID, in_Remark);
    END IF;    
    
    DELETE FROM BotConversationRecipient
    WHERE GroupID = in_GroupID;
    
    SET recipientIDs = in_RecipientIDs;
    do_this:
		  LOOP			
            SET strLen = LENGTH(recipientIDs);            
            
            INSERT INTO BotConversationRecipient(GroupID, RecipientID)
            VALUES (in_GroupID, SUBSTRING_INDEX(recipientIDs, ',', 1));			

			SET subStrLen = LENGTH(SUBSTRING_INDEX(recipientIDs, ',', 1));
			SET recipientIDs = MID(recipientIDs, subStrLen + 2, strLen);

			IF recipientIDs = '' THEN
			  LEAVE do_this;
			END IF;
		  END LOOP do_this;
          
	SELECT 0 INTO out_ErrorCode;
	SELECT "" INTO out_ErrorMsg;
END;