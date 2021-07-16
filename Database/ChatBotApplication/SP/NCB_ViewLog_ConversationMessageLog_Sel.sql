CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_ViewLog_ConversationMessageLog_Sel`(
	IN notificationID INT
    ,IN pageIndex INT
    ,IN pageSize INT
	/*
        Revisions:
    */
)
BEGIN
	DECLARE rowStart INT;
	DECLARE COUNT INT;
    SET rowStart =	pageSize * (pageIndex - 1);   
    
	SET COUNT =( SELECT COUNT(1)
	FROM  ConversationMessageLog					  
	WHERE ConversationMessageLog.ReferenceID = notificationID);
			   
    SELECT COUNT;
    
	SELECT DISTINCT CML.ReferenceID as NotificationID
        ,CML.ToConversationID
        ,CB.Name as BotName
        ,CG.ConversationGroupID
        ,CG.Name as ConversationGroupName
        ,CML.SentTime
        ,CML.IsSuccessfull as IsSuccessful
        ,CML.ErrorMessage
        ,CML.Content
    FROM ( ConversationMessageLog CML
		INNER JOIN ConversationBot CB on CB.ConversationID = CML.FromConversationID
		INNER JOIN ConversationGroup CG on CG.ConversationID = CML.ToConversationID
    )
	WHERE CML.ReferenceID = notificationID
    ORDER BY CML.ReferenceID, CML.SentTime
    LIMIT rowStart, pageSize; 
    
    SELECT R.RecipientID, R.RecipientName, CG.ConversationID
	FROM NotificationRecipientList NRL 
	INNER JOIN Recipient R on R.RecipientID = NRL.RecipientID
	INNER JOIN ConversationGroup CG on CG.ConversationGroupID = NRL.ConversationGroupID
	WHERE NRL.NotificationID = notificationID
    LIMIT rowStart, pageSize;
END