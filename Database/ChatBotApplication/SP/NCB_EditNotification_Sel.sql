DELIMITER //

DROP PROCEDURE IF EXISTS NCB_EditNotification_Sel;

CREATE PROCEDURE  NCB_EditNotification_Sel(
     IN notificationID INT
    /*
		Creator: Dominic@20190408
        Param:
        
		Result Dataset:
				- 1st dataset: Main info of Notification (NotificationTypeID, Title, Content)
                - 2nd dataset: List of recipientIDs
        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
BEGIN
	#1st dataset: Main info of Notification
	SELECT n.NotificationTypeID,  n.Title, n.Content
    FROM Notification n
    WHERE n.NotificationID = notificationID;
    
    #2nd dataset: List of recipients
	SELECT rlst.RecipientID
    FROM NotificationRecipientList rlst
    WHERE rlst.NotificationID = notificationID;
END;