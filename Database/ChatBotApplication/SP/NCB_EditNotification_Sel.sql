DELIMITER //

DROP PROCEDURE IF EXISTS NCB_EditNotification_Sel;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  NCB_EditNotification_Sel(
     IN notificationID INT
    /*
		Creator: Dominic@20190408
        Param:
        
		Result Dataset:
				- 1st dataset: Main info of Notification (NotificationTypeID, Title, Content, ProductID)
                - 2nd dataset: List of recipientIDs
        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
SQL SECURITY DEFINER
BEGIN
	#1st dataset: Main info of Notification
	SELECT n.NotificationTypeID,  n.ProductID, n.Title, n.Content
    FROM Notification n
    WHERE n.NotificationID = notificationID;
    
    #2nd dataset: List of recipients
	SELECT   rt.RecipientTypeID, rt.RecipientTypeName, r.RecipientID, r.RecipientName
    FROM NotificationRecipientList rlst
		INNER JOIN Recipient r
			ON r.RecipientID = rlst.RecipientID
        INNER JOIN RecipientType rt
			ON rt.RecipientTypeID = r.RecipientTypeID
    WHERE rlst.NotificationID = notificationID
    ORDER BY rt.RecipientTypeID, r.RecipientID;
END;