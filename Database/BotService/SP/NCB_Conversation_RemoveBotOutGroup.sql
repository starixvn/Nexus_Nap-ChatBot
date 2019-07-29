DELIMITER //

DROP PROCEDURE IF EXISTS `NCB_Conversation_RemoveBotOutGroup`;

CREATE DEFINER=`ncb.admin`@`%` PROCEDURE `NCB_Conversation_RemoveBotOutGroup`(
	IN botConversationId VARCHAR(200) CHARACTER SET 'utf8mb4' 
    , IN conversationId VARCHAR(200) CHARACTER SET 'utf8mb4' 
	, IN updatedBy VARCHAR(50)
	, OUT errCode SMALLINT
	, OUT errMsg VARCHAR(100)
    /*
		Creator: Quinn@20190726
        Param:
				- botConversationId: Conversation Id of Bot
                - onversationId: Conversation Id of Group
                - updatedBy: Conversation of user which add BOT into group.
        Revisions:
				- 20190726@Quinn: First Commit
        
    */
)
BEGIN

	#Variables used to write User Logs
	DECLARE startTime DATETIME(3);
    DECLARE finishTime 		DATETIME(3); 
    DECLARE duration 			INT;							
    DECLARE logContent 		TEXT CHARACTER SET utf8mb4;
    DECLARE ACTION_ID 		INT DEFAULT 1;
    
	DECLARE conversationBotId INT;
    DECLARE conversationGroupId INT;
    DECLARE currentTime DATETIME(3);
    
    #Get Start Time to calcualte execution duration
    SET  startTime = UTC_TIMESTAMP(3);
    SET currentTime = startTime;
    SET errCode=0, errMsg='';
    
    
    
	IF (EXISTS (SELECT 1 FROM ConversationBot AS cb WHERE cb.ConversationId = botConversationId)
		AND EXISTS (SELECT 1 FROM ConversationGroup cg WHERE cg.ConversationId = conversationId)) 
    THEN
		SELECT cb.ConversationBotID INTO conversationBotId 
        FROM ConversationBot AS cb 
        WHERE cb.ConversationId = botConversationId 
        LIMIT 1;
        
		SELECT cg.ConversationGroupID INTO conversationGroupId 
        FROM ConversationGroup AS cg 
        WHERE cg.ConversationId = conversationId 
        LIMIT 1;
        
        DELETE cgb
		FROM ConversationGroupBot AS cgb
		WHERE cgb.ConversationGroupID = conversationGroupId
		AND cgb.ConversationBotID = conversationBotId;
		
    END IF;
    
    
     #Get Finish Time to calcualte execution duration
    SET finishTime = UTC_TIMESTAMP(3);
    SET duration =  TIMESTAMPDIFF(MICROSECOND
																		, startTime
                                                                        , finishTime)  / 1000; 	#miliseconds
    
    #Get Log Content, JSON format

END

