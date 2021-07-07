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
        FROM (	SELECT DISTINCT NRL.NotificationID
						,CB.Name as BotName
						,CG.ConversationGroupID
						,R.RecipientName
						,CML.SentTime
						,CML.IsSuccessfull
						,CML.ErrorMessage
						,CML.Content
				FROM ( ConversationGroup CG 
					INNER JOIN NotificationRecipientList NRL on CG.ConversationGroupID = NRL.ConversationGroupID					
					INNER JOIN ConversationGroupBot CGB on CG.ConversationGroupID = CGB.ConversationGroupID
					INNER JOIN ConversationBot CB on CGB.ConversationBotID = CB.ConversationBotID					
					INNER JOIN ConversationGroupRecipient CGR on CG.ConversationGroupID = CGR.ConversationGroupID
					INNER JOIN Recipient R on CGR.RecipientID = R.RecipientID					
					INNER JOIN ConversationMessageLog CML on CG.ConversationID = CML.ToConversationID
					)
				WHERE NRL.NotificationID = notificationID) as A
        );
    SELECT COUNT;
    
	SELECT DISTINCT NRL.NotificationID
        ,CB.Name as BotName
        ,CG.ConversationGroupID
        ,CG.Name as ConversationGroupName
        ,R.RecipientName
        ,CML.SentTime
        ,CML.IsSuccessfull as IsSuccessful
        ,CML.ErrorMessage
        ,CML.Content
    FROM ( ConversationGroup CG 
		INNER JOIN NotificationRecipientList NRL on CG.ConversationGroupID = NRL.ConversationGroupID        
		INNER JOIN ConversationGroupBot CGB on CG.ConversationGroupID = CGB.ConversationGroupID
        INNER JOIN ConversationBot CB on CGB.ConversationBotID = CB.ConversationBotID
        INNER JOIN ConversationGroupRecipient CGR on CG.ConversationGroupID = CGR.ConversationGroupID
        INNER JOIN Recipient R on CGR.RecipientID = R.RecipientID
        INNER JOIN ConversationMessageLog CML on CG.ConversationID = CML.ToConversationID
    )
	WHERE NRL.NotificationID = notificationID
    ORDER BY NRL.NotificationID, CML.SentTime
    LIMIT rowStart, pageSize; 
END