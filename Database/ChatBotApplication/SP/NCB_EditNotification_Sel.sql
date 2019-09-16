DROP PROCEDURE IF EXISTS NCB_EditNotification_Sel;
DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_EditNotification_Sel`(
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
BEGIN
	#1st dataset: Main info of Notification
	SELECT n.NotificationID, n.NotificationTypeID, nt.NotificationTypeName,  n.ProductID, p.ProductName, n.Title, n.Content, n.SendImmediately, n.ConfigSendTime
    FROM Notification n
		INNER JOIN NotificationType nt ON n.NotificationTypeID = nt.NotificationTypeID
        LEFT JOIN Product p ON n.ProductID = p.ProductID
    WHERE n.NotificationID = notificationID AND n.StatusID = 4;
    
    #2nd dataset: List of recipients
	SELECT DISTINCT rlst.RecipientID
    FROM NotificationRecipientList rlst
	WHERE rlst.NotificationID = notificationID;
END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE ChatBot.NCB_EditNotification_Sel TO 'ncb.admin'@'%';