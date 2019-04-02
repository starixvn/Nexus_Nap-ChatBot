DELIMITER //
CREATE PROCEDURE `NCB_ChatGroupMapping_Upd`(
	IN in_GroupID INT,
    IN in_RecipientTypeID INT, 
    IN in_RecipientIDs varchar(1000), 
    IN in_ChatGroupTypeID INT,
    IN in_Remark nvarchar(1000),
    IN in_CreatorID INT,
    OUT out_ErrorCode INT, 
    OUT out_ErrorMsg nvarchar(100)
)
BEGIN
	DECLARE strLen    INT DEFAULT 0;
	DECLARE subStrLen INT DEFAULT 0;
    DECLARE recipientIDs varchar(1000);
    
	if exists(select 1 from BotConversationExtraInfo bce where bce.GroupID = in_GroupID) then
		update BotConversationExtraInfo bce
        set bce.ChatGroupTypeID = in_ChatGroupTypeID, bce.Remark = in_Remark
        where bce.GroupID = in_GroupID;
    else
		insert into BotConversationExtraInfo(GroupID, ChatGroupTypeID, Remark)
        values (in_GroupID, in_ChatGroupTypeID, in_Remark);
    end if;    
    
    delete from BotConversationRecipient
    where GroupID = in_GroupID;
    
    SET recipientIDs = in_RecipientIDs;
    do_this:
		  LOOP			
            SET strLen = LENGTH(recipientIDs);            
            
            insert into BotConversationRecipient(GroupID, RecipientID)
            values (in_GroupID, SUBSTRING_INDEX(recipientIDs, ',', 1));			

			SET subStrLen = LENGTH(SUBSTRING_INDEX(recipientIDs, ',', 1));
			SET recipientIDs = MID(recipientIDs, subStrLen + 2, strLen);

			IF recipientIDs = '' THEN
			  LEAVE do_this;
			END IF;
		  END LOOP do_this;
          
	select 0 into out_ErrorCode;
	select "" into out_ErrorMsg;
END