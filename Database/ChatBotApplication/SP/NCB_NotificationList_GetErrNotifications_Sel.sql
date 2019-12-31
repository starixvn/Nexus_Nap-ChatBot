DROP PROCEDURE IF EXISTS NCB_NotificationList_GetErrNotifications_Sel;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_NotificationList_GetErrNotifications_Sel`(
    /*
		Creator: Merlin.Le@20191230

        Revisions:
            - 20191230@Merlin.Le: First Commit -- Redmine ID: #126213
    */
)
BEGIN
	DECLARE SENT_STATUSID_ERROR TINYINT UNSIGNED DEFAULT 2;
    
	SELECT
        n.NotificationID
        ,nt.NotificationTypeName
        ,n.CreatorID
        ,c.username CreatorName
        ,n.ConfigSendTime
        ,n.Title
        ,n.Content
        ,n.CreatedDate
        ,n.StatusID
        ,n.Status
    FROM Notification n
    INNER JOIN NotificationType nt ON n.NotificationTypeID = nt.NotificationTypeID
    INNER JOIN Customer c on n.CreatorID = c.custid
    WHERE StatusID = SENT_STATUSID_ERROR;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_NotificationList_GetErrNotifications_Sel TO 'ncb.admin'@'%';