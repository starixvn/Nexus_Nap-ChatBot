DELIMITER //

DROP PROCEDURE IF EXISTS `NCB_Conversation_AddBotToGroup`;

CREATE DEFINER=`ncb.admin`@`%` PROCEDURE `NCB_Conversation_AddBotToGroup`(
	IN botConversationId VARCHAR(70)
	, IN botName VARCHAR(70)
    , IN conversationId VARCHAR(70)
	, IN groupName VARCHAR(500)
	, IN channelId VARCHAR(45)
    , IN serviceUrl VARCHAR(500)
	, IN updatedBy VARCHAR(50)
	, OUT errCode SMALLINT
	, OUT errMsg VARCHAR(100)
    /*
		Creator: Quinn@20190726
        Param:
				- botConversationId: Conversation Id of Bot
                - botName: Bot Name
                - conversationId: Conversation Id of Group
                - groupName: Topic of Group
                - channelId: Channel Id (skype, telegram, facebook, ..)
                - serviceUrl: 
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
    
	DECLARE conversationBotID INT;
    DECLARE conversationGroupId INT;
    DECLARE currentTime DATETIME(3);
    
    #Get Start Time to calcualte execution duration
    SET  startTime = UTC_TIMESTAMP(3);
    SET currentTime = startTime;
    SET errCode=0, errMsg='';
    
	IF (NOT EXISTS (SELECT 1 FROM ConversationBot AS cb WHERE cb.ConversationId = botConversationId)) 
    THEN
		INSERT INTO ConversationBot(`ConversationId`, `Name`)
        VALUES (botConversationId, botName);
        
        SET conversationBotID = LAST_INSERT_ID();
	ELSE 
        SELECT cb.ConversationBotID INTO conversationBotID 
        FROM ConversationBot AS cb 
        WHERE cb.ConversationId = botConversationId 
        LIMIT 1;
    END IF;
    
    IF (NOT EXISTS (SELECT 1 FROM ConversationGroup cg WHERE cg.ConversationId = conversationId))
    THEN
		INSERT INTO ConversationGroup(
            `ConversationID`
            ,`Name`
            ,`ChannelID`
            ,`ServiceUrl`
            ,`UpdatedBy`
            ,`LastUpdate`)
        VALUES (
			conversationId
			,botName
            ,channelId
            ,serviceUrl
            ,updatedBy
            ,currentTime
            );
        
        SET conversationGroupId = LAST_INSERT_ID();
    ELSE
        SELECT cg.ConversationGroupID INTO conversationGroupId 
        FROM ConversationGroup AS cg 
        WHERE cg.ConversationId = conversationId 
        LIMIT 1;
    END IF;
    
    IF (NOT EXISTS (SELECT 1 
		FROM ConversationGroupBot AS cgb 
        WHERE cgb.ConversationGroupID = conversationGroupId 
			AND cgb.ConversationBotID = conversationBotID))
    THEN
		INSERT INTO ConversationGroupBot(
            `ConversationGroupID`
            ,`ConversationBotID`
            ,`CreatedTime`)
        VALUES (
			conversationGroupId
			,conversationBotID
            ,currentTime
            );
    END IF;

    
     #Get Finish Time to calcualte execution duration
    SET finishTime = UTC_TIMESTAMP(3);
    SET duration =  TIMESTAMPDIFF(MICROSECOND
																		, startTime
                                                                        , finishTime)  / 1000; 	#miliseconds
    
    #Get Log Content, JSON format

END