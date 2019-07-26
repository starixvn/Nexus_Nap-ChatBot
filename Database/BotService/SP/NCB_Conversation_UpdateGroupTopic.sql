DELIMITER //

DROP PROCEDURE IF EXISTS `NCB_Conversation_UpdateGroupTopic`;

CREATE DEFINER=`ncb.admin`@`%` PROCEDURE `NCB_Conversation_UpdateGroupTopic`(
    IN conversationId VARCHAR(70)
    , IN groupName VARCHAR(500)
	, IN updatedBy VARCHAR(50)
	, OUT errCode SMALLINT
	, OUT errMsg VARCHAR(100)
    /*
		Creator: Quinn@20190726
        Param:
                - conversationId: Conversation Id of Group
                - groupName: topic of group chat
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
    
    DECLARE currentTime DATETIME(3);
    
    #Get Start Time to calcualte execution duration
    SET  startTime = UTC_TIMESTAMP(3);
    SET currentTime = startTime;
    SET errCode=0, errMsg='';
    
    
    UPDATE ConversationGroup cg
    SET cg.Name = groupName
		,cg.UpdatedBy = updatedBy
        ,cg.LastUpdate = currentTime
    WHERE cg.ConversationID = conversationId;
    

     #Get Finish Time to calcualte execution duration
    SET finishTime = UTC_TIMESTAMP(3);
    SET duration =  TIMESTAMPDIFF(MICROSECOND
																		, startTime
                                                                        , finishTime)  / 1000; 	#miliseconds
    
    #Get Log Content, JSON format

END