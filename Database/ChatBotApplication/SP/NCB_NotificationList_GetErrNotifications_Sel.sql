DROP PROCEDURE IF EXISTS NCB_NotificationList_GetErrNotifications_Sel;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_NotificationList_GetErrNotifications_Sel`(
     IN notificationTypeIDLst   VARCHAR(1000)
    , IN recipientIDLst         TEXT
    /*
		Creator: Merlin.Le@2020-01-08

        Revisions:
            - 2020-01-08@Merlin.Le: First Commit -- Redmine ID: #126213
    */
)
BEGIN
	DECLARE SENT_STATUSID_ERROR TINYINT UNSIGNED DEFAULT 2;
    DECLARE DEFAULT_CONFIG_FROMDATE DATETIME DEFAULT '2019-08-01 00:00:00';
    
	#Insert comma seperated list notificationTypeIDLst to temporary tables
    DROP TEMPORARY TABLE IF EXISTS tmp_notificationTypeIDLst;
	CREATE TEMPORARY TABLE tmp_notificationTypeIDLst(NotificationTypeID SMALLINT);
    
	IF IFNULL(notificationTypeIDLst,'') <> ''THEN
		SET @sqlCommand = REPLACE(notificationTypeIDLst, ',', '),(');
		SET @sqlCommand = CONCAT('(', @sqlCommand, ')');
		SET @sqlCommand = CONCAT('INSERT INTO tmp_notificationTypeIDLst VALUES', @sqlCommand); 
                                                            
		PREPARE stmt FROM  @sqlCommand;
		EXECUTE stmt;
		DEALLOCATE PREPARE stmt;
	END IF;
    
	#Insert comma seperated list recipientIDLst to temporary tables
    DROP TEMPORARY TABLE IF EXISTS tmp_recipientIDLst;
	CREATE TEMPORARY TABLE tmp_recipientIDLst(RecipientID INT, UNIQUE INDEX Index_RecipientID(RecipientID));
    
	IF IFNULL(recipientIDLst,'') <> ''THEN
		SET @sqlCommand = REPLACE(recipientIDLst, ',', '),(');
		SET @sqlCommand = CONCAT('(', @sqlCommand, ')');
		SET @sqlCommand = CONCAT('INSERT INTO tmp_recipientIDLst VALUES', @sqlCommand); 
                                                            
		PREPARE stmt FROM  @sqlCommand;
		EXECUTE stmt;
		DEALLOCATE PREPARE stmt;
	END IF;
    
	#Insert comma seperated list notificationStatusIDLst to temporary tables
    DROP TEMPORARY TABLE IF EXISTS tmp_notificationStatusIDLst;
	CREATE TEMPORARY TABLE tmp_notificationStatusIDLst(StatusID TINYINT UNSIGNED);
    
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
    WHERE n.NotificationTypeID IN (
        SELECT	tmp_nt.NotificationTypeID
        FROM tmp_notificationTypeIDLst tmp_nt
    )
	AND (creatorID IS NULL OR n.CreatorID = creatorID)
    AND n.StatusID = SENT_STATUSID_ERROR
    AND n.ConfigSendTime >= DEFAULT_CONFIG_FROMDATE
    AND EXISTS (
			SELECT 1
            FROM NotificationRecipientList nrl
            WHERE nrl.NotificationID = n.NotificationID
			AND nrl.RecipientID IN (
                SELECT	tmp_rid.RecipientID
                FROM tmp_recipientIDLst tmp_rid
            )
    )
    ORDER BY n.ConfigSendTime DESC, n.NotificationID DESC;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_NotificationList_GetErrNotifications_Sel TO 'ncb.admin'@'%';