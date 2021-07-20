CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_ViewLog_NotificationRecipientList_Sel`(
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
    
	SET COUNT = (SELECT COUNT(1)
    FROM NotificationRecipientList 
	WHERE NotificationRecipientList.NotificationID = notificationID);
    
    SELECT COUNT;
    
	SELECT NRL.NotificationID
        ,CG.Name
        ,CGT.ChatGroupTypeName
        ,R.RecipientName
        ,NRL.SentTime
        ,NRL.Status
        ,NRL.RetryTime
        ,NRL.ErrMsg
    FROM (NotificationRecipientList NRL 
		INNER JOIN ChatGroupType CGT on NRL.ChatGroupTypeID = CGT.ChatGroupTypeID
        INNER JOIN ConversationGroup CG on NRL.ConversationGroupID = CG.ConversationGroupID
        INNER JOIN Recipient R on NRL.RecipientID = R.RecipientID
    )
	WHERE NRL.NotificationID = notificationID
    ORDER BY NRL.NotificationID, NRL.SentTime
    LIMIT rowStart, pageSize; 
END