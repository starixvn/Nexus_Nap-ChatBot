DROP PROCEDURE IF EXISTS NCB_NotificationList_ErrorNotifications_Sel;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_NotificationList_ErrorNotifications_Sel`(
    /*
		Creator: 20200204@Oliver

        Revisions:
            - 20200204@Oliver: init SP -- Redmine ID: #126213
    */
)
BEGIN
	DECLARE SENT_STATUSID_ERROR TINYINT UNSIGNED DEFAULT 2;
    DECLARE PAGE_SIZE_DEFAULT TINYINT UNSIGNED DEFAULT 20;
    
    CREATE TEMPORARY TABLE IF NOT EXISTS tmp_ErrNotification(NotificationTypeID INT,
            ConfigSendTime DATE,
            CreatorID INT,
            Count INT);
	INSERT INTO tmp_ErrNotification(NotificationTypeID, ConfigSendTime, CreatorId, Count)
    SELECT  n.NotificationTypeID
        ,DATE(n.ConfigSendTime) ConfigSendTime
        ,n.CreatorID
        ,COUNT(*) As Count
    FROM Notification n
    INNER JOIN NotificationType nt ON n.NotificationTypeID = nt.NotificationTypeID
    WHERE n.StatusID = SENT_STATUSID_ERROR
    AND EXISTS (
			SELECT 1
            FROM NotificationRecipientList nrl
            WHERE nrl.NotificationID = n.NotificationID
    )
	GROUP BY n.NotificationTypeID
			,DATE(n.ConfigSendTime)
			,n.CreatorID;
    
	SELECT DISTINCT
        n.NotificationID
        ,n.NotificationTypeID
        ,n.ConfigSendTime
        ,n.Title
        ,n.Content
        ,n.CreatedDate
        ,n.CreatorID
        ,CEILING(tmp.Count/PAGE_SIZE_DEFAULT) As PageIndex
    FROM Notification n
    INNER JOIN NotificationType nt ON n.NotificationTypeID = nt.NotificationTypeID
    INNER JOIN Customer c on n.CreatorID = c.custid    
    LEFT JOIN tmp_ErrNotification tmp ON DATE(n.ConfigSendTime) = DATE(tmp.ConfigSendTime) AND n.CreatorID = tmp.CreatorID AND n.NotificationTypeID = tmp.NotificationTypeID
    WHERE n.StatusID = SENT_STATUSID_ERROR
    AND EXISTS (
			SELECT 1
            FROM NotificationRecipientList nrl
            WHERE nrl.NotificationID = n.NotificationID
    )
    ORDER BY n.ConfigSendTime DESC, n.NotificationID DESC;
    
    DROP TEMPORARY TABLE IF EXISTS tmp_ErrNotification;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_NotificationList_ErrorNotifications_Sel TO 'ncb.admin'@'%';