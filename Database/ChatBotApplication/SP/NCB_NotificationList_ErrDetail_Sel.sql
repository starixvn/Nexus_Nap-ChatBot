DELIMITER //

DROP PROCEDURE IF EXISTS NCB_NotificationList_ErrDetail_Sel;

CREATE PROCEDURE  NCB_NotificationList_ErrDetail_Sel(
     IN notificationID INT
    /*
		Creator: Dominic@20190408
        Param:
        
		Result Dataset:
				- 1st dataset: Main Notification Info|  title, sentTime, resolvedRemark, resolvedByUsername
                - 2nd dataset: List of success Recipients
                - 3rd dataset: List of failed Recipients
        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
BEGIN
	DECLARE SENT_STATUSID_ERROR TINYINT UNSIGNED DEFAULT 2;
    DECLARE SENT_STATUSID_RESOLVED TINYINT UNSIGNED DEFAULT 5;
    
	DECLARE title VARCHAR(300) CHARACTER SET utf8mb4;
    DECLARE sentTime DATETIME;
    DECLARE resolvedRemark VARCHAR(1000) CHARACTER SET utf8mb4;
    DECLARE resolvedByUsername VARCHAR(100);
    
  SELECT n.title, n.ConfigSendTime, IFNULL(n.ResolvedRemark, ''), IFNULL(u.UserName, '')
    INTO title, sentTime, resolvedRemark, resolvedByUsername
    FROM Notification n
    LEFT JOIN User u ON u.UserID = n.ResolvedBy
    WHERE n.NotificationID = notificationID;
    
    #Insert recipient list into temporary table
    DROP TEMPORARY TABLE IF EXISTS tmp_recipientLst;
	CREATE TEMPORARY TABLE tmp_recipientLst(
															RecipientName VARCHAR(200) CHARACTER SET utf8mb4
                                                            , RecipientTypeName VARCHAR(200) CHARACTER SET utf8mb4
                                                            , ChatGroupName VARCHAR(500) CHARACTER SET utf8mb4
                                                            , Status VARCHAR(10) CHARACTER SET utf8mb4
                                                            , StatusID TINYINT UNSIGNED
                                                            , ErrMsg VARCHAR(1000) CHARACTER SET utf8mb4
                                                            , SentTime DATETIME
														);
                                                        
	INSERT INTO tmp_recipientLst(RecipientName, RecipientTypeName, ChatGroupName
														, Status, StatusID, ErrMsg, SentTime
								)
	SELECT r.RecipientName, rt.RecipientTypeName, grp.Name
					, rlst.Status, rlst.StatusID, rlst.ErrMsg, rlst.SentTime
    FROM Notification n
    INNER JOIN NotificationRecipientList rlst ON rlst.NotificationID = n.NotificationID
    INNER JOIN Recipient r ON r.RecipientID = rlst.RecipientID
    INNER JOIN RecipientType rt ON rt.RecipientTypeID = r.RecipientTypeID
    INNER JOIN BotConversation grp ON grp.ID = rlst.BotConversationID;
    
    #1st dataset: Main Notification Info
    SELECT title, sentTime, resolvedRemark, resolvedByUsername;
    
    #2nd dataset: List of success Recipients
    SELECT tmp.RecipientTypeName, tmp.RecipientName
    FROM tmp_recipientLst tmp
    WHERE tmp.StatusID NOT IN (SENT_STATUSID_ERROR, SENT_STATUSID_RESOLVED)
    ORDER BY  tmp.RecipientTypeName, tmp.RecipientName;
    
    #3rd dataset: List of failed Recipients
    SELECT tmp.RecipientTypeName, tmp.RecipientName, tmp.ChatGroupName, tmp.ErrMsg Reason
    FROM tmp_recipientLst tmp
    WHERE tmp.StatusID  IN (SENT_STATUSID_ERROR, SENT_STATUSID_RESOLVED)
    ORDER BY tmp.RecipientTypeName, tmp.RecipientName;
END;