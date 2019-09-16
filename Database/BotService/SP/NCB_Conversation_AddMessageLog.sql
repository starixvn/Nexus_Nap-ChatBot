DROP PROCEDURE IF EXISTS `NCB_Conversation_AddMessageLog`;
DELIMITER //

CREATE DEFINER=`ChatBotSystem` PROCEDURE `NCB_Conversation_AddMessageLog`(
	IN fromConversationId VARCHAR(200)  
    , IN toConversationId VARCHAR(200)  
    , IN content TEXT CHARACTER SET utf8mb4
	, IN isSuccessfull BIT
    , IN errorMessage VARCHAR(1000) CHARACTER SET utf8mb4
	, OUT errCode SMALLINT
	, OUT errMsg VARCHAR(100)
    /*
		Creator: Quinn@20190814
        Param:
				- fromConversationId: Conversation Id of bot
                - toConversationId: Conversation Id of personal/ group which received msg.
                - content: content of message
                - isSuccessfull: status of sending msg
                - errorMessage: error msg
        Revisions:
				- 20190814@Quinn: First Commit
        
    */
)
SQL SECURITY DEFINER
BEGIN
	DECLARE currentTime DATETIME(3);
	SET currentTime = UTC_TIMESTAMP(3);
    
	INSERT INTO ConversationMessageLog(
		FromConversationID,
        ToConversationID,
        Content,
        SentTime,
        IsSuccessfull,
        ErrorMessage
    )
    VALUES(
		fromConversationId,
        toConversationId,
        content,
        currentTime,
        isSuccessfull,
        errorMessage
    );
END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE ChatBot.NCB_Conversation_AddMessageLog TO 'ncb.bot'@'%';