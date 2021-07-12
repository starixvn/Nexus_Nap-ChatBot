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
				 FROM (	SELECT DISTINCT A.NotificationID
							,CB.Name as BotName
							,A.ConversationGroupID
							,A.ConversationGroupName
							,A.RecipientName
							,CML.SentTime
							,CML.IsSuccessfull as IsSuccessful
							,CML.ErrorMessage
							,CML.Content
						FROM ( ConversationMessageLog CML
							INNER JOIN ConversationBot CB on CB.ConversationID = CML.FromConversationID
							INNER JOIN ( SELECT NRL.NotificationID
												,CG.ConversationGroupID
												,CG.ConversationID
												,CG.Name as ConversationGroupName
												,R.RecipientName
										 FROM ConversationGroup CG
										 INNER JOIN NotificationRecipientList NRL on CG.ConversationGroupID = NRL.ConversationGroupID
										 INNER JOIN ConversationGroupRecipient CGR on CG.ConversationGroupID = CGR.ConversationGroupID
										 INNER JOIN Recipient R on CGR.RecipientID = R.RecipientID
										) as A  on A.ConversationID = CML.ToConversationID
								)
				  WHERE A.NotificationID = notificationID) as B
			   );
    SELECT COUNT;
    
	SELECT DISTINCT A.NotificationID
        ,CB.Name as BotName
        ,A.ConversationGroupID
        ,A.ConversationGroupName
        ,A.RecipientName
        ,CML.SentTime
        ,CML.IsSuccessfull as IsSuccessful
        ,CML.ErrorMessage
        ,CML.Content
    FROM ( ConversationMessageLog CML
		INNER JOIN ConversationBot CB on CB.ConversationID = CML.FromConversationID
        INNER JOIN ( SELECT NRL.NotificationID
						,CG.ConversationGroupID
						,CG.ConversationID
						,CG.Name as ConversationGroupName
						,R.RecipientName
					 FROM ConversationGroup CG
					 INNER JOIN NotificationRecipientList NRL on CG.ConversationGroupID = NRL.ConversationGroupID
				     INNER JOIN ConversationGroupRecipient CGR on CG.ConversationGroupID = CGR.ConversationGroupID
					 INNER JOIN Recipient R on CGR.RecipientID = R.RecipientID
					) as A  on A.ConversationID = CML.ToConversationID
    )
	WHERE A.NotificationID = notificationID
    ORDER BY A.NotificationID, CML.SentTime
    LIMIT rowStart, pageSize; 
END