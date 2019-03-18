CREATE PROCEDURE `NCB_ChatGroupMapping_Upd`(
	IN groupID INT,
	IN conversationID varchar(100), 
    IN recipientTypeID INT, 
    IN recipientIDs varchar(1000), 
    IN remark nvarchar(1000),
    IN creatorID INT,
    OUT errorCode INT, 
    OUT errorMsg nvarchar(100)
)
BEGIN
	DECLARE strLen    INT DEFAULT 0;
	DECLARE SubStrLen INT DEFAULT 0;
    
	if exists(select * from Conversation c where c.GroupID = groupID) then
		update Conversation c
        set c.RecipientTypeID = recipientTypeID, c.Remark = remark
        where c.GroupID = groupID;
    else
		insert into Conversation(GroupID, RecipientTypeID, Remark)
        values (groupID, recipientTypeID, remark);
    end if;
    
    delete from ConversationRecipient
    where ConversationRecipient.GroupID = groupID;
    
    do_this:
		  LOOP			
            SET strLen = LENGTH(recipientIDs);            
            
            insert into ConversationRecipient(GroupID, RecipientID, CreatedDate, CreatorID)
            values (groupID, SUBSTRING_INDEX(recipientIDs, ',', 1), Now(), creatorID);			

			SET SubStrLen = LENGTH(SUBSTRING_INDEX(recipientIDs, ',', 1));
			SET recipientIDs = MID(recipientIDs, SubStrLen + 2, strLen);

			IF recipientIDs = '' THEN
			  LEAVE do_this;
			END IF;
		  END LOOP do_this;
          
	select 0 into errorCode;
	select "" into errorMsg;
END