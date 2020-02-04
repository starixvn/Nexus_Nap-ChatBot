DROP PROCEDURE IF EXISTS NCB_ResendNotification_Sel;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_ResendNotification_Sel`(
     IN notificationID INT
     , IN isSendAll BIT
    /*
		Creator: 20200116@Percy
        Task: Chatbot For Licensee - Resend Notification
        Redmine: https://nexdev.net/issues/127217
        Param:
				- isSendAll = 0: Resend Error
		Return Data Set:
				- #1st dataset: Main Notification Info
                - #2nd dataset: List of Recipients
        Revisions:
				- 20200116@Percy: Initialize SP [RedmineID: #127217]
				- 20200122@Oliver: Get more info of recipients [RedmineID: #127451]
    */
)
BEGIN
	DECLARE SENT_STATUSID_ERROR TINYINT UNSIGNED DEFAULT 2;
    DECLARE SENT_STATUSID_RESOLVED TINYINT UNSIGNED DEFAULT 5;
    
	#1st dataset: Main Notification Info
	SELECT n.NotificationID 
		, nt.NotificationTypeID
		, nt.NotificationTypeName
		, n.ProductID
		, p.ProductName
		, n.Title
        , n.Content
		, 1 AS SendImmediately
		, n.ConfigSendTime
    FROM Notification n
		INNER JOIN NotificationType nt ON n.NotificationTypeID = nt.NotificationTypeID
		LEFT JOIN Product p ON n.ProductID = p.ProductID
    WHERE n.NotificationID = notificationID;
    
    #2nd dataset: List of Recipients
    SELECT DISTINCT nrl.RecipientID
				, r.RecipientName
				, r.RecipientTypeID
				, l.LicenseeTypeId
    FROM NotificationRecipientList nrl
	INNER JOIN Recipient r ON nrl.RecipientID = r.RecipientID
	LEFT JOIN Licensee l ON r.SourceID = l.LicenseeId
	WHERE nrl.NotificationID = notificationID
		AND (isSendAll = 1
			OR nrl.StatusID IN (SENT_STATUSID_ERROR, SENT_STATUSID_RESOLVED)
        );
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_ResendNotification_Sel TO 'ncb.admin'@'%';