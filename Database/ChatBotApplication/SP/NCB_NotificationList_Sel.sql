DELIMITER //

DROP PROCEDURE IF EXISTS NCB_NotificationList_Sel;

CREATE PROCEDURE  NCB_NotificationList_Sel(
     IN notificationTypeIDLst VARCHAR(200)
    , IN recipientIDLst VARCHAR(5000)
	, IN creatorIDLst VARCHAR(2000)
    , IN notificationStatusIDLst VARCHAR(200)
	, IN configSendTimeFrom DATETIME
    , IN configSendTimeTo DATETIME
	, IN pageIndex INT
    , IN pageSize INT
    /*
		Creator: Dominic@20190408
        Param:
				- notificationTypeIDLst, recipientIDLst, creatorIDLst, notificationStatusIDLst: Comma separated strings for filtering
                - pageIndex: Page Index for paging, start from 1
                - pageSize: Page Size for paging
		Return Data Set:
				- 1st dataset: Number of rows
                - 2nd dataset: Returned data based on Page Index and Page Size
									, with column RecipientSentStatusJSON is in JSON format and is for notification status of a specified recipient
									Ex: RecipientSentStatusJSON = [{"IsError": 0, "RecipientName": "Happy8"}, {"IsError": 0, "RecipientName": "SALONCS"}]
        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
BEGIN
	DECLARE SENT_STATUSID_ERROR TINYINT UNSIGNED DEFAULT 2;
    DECLARE SENT_STATUSID_RESOLVED TINYINT UNSIGNED DEFAULT 5;
    
	DECLARE rowStart INT;
    SET rowStart =	pageSize * (pageIndex - 1);   
    
	#Insert comma seperated list notificationTypeIDLst to temporary tables
    DROP TEMPORARY TABLE IF EXISTS tmp_notificationTypeIDLst;
	CREATE TEMPORARY TABLE tmp_notificationTypeIDLst(NotificationTypeID SMALLINT);
    
	IF IFNULL(notificationTypeIDLst,'') <> ''THEN
		SET @sqlCommand = REPLACE(notificationTypeIDLst, ',', '),(');
		SET @sqlCommand = CONCAT('(', @sqlCommand, ')');
		SET @sqlCommand = CONCAT('INSERT INTO tmp_notificationTypeIDLst VALUES'
															, @sqlCommand); 
                                                            
		PREPARE stmt FROM  @sqlCommand;
		EXECUTE stmt;
		DEALLOCATE PREPARE stmt;
	END IF;
    
	#Insert comma seperated list recipientIDLst to temporary tables
    DROP TEMPORARY TABLE IF EXISTS tmp_recipientIDLst;
	CREATE TEMPORARY TABLE tmp_recipientIDLst(RecipientID INT);
    
	IF IFNULL(recipientIDLst,'') <> ''THEN
		SET @sqlCommand = REPLACE(recipientIDLst, ',', '),(');
		SET @sqlCommand = CONCAT('(', @sqlCommand, ')');
		SET @sqlCommand = CONCAT('INSERT INTO tmp_recipientIDLst VALUES'
															, @sqlCommand); 
                                                            
		PREPARE stmt FROM  @sqlCommand;
		EXECUTE stmt;
		DEALLOCATE PREPARE stmt;
	END IF;
    
	#Insert comma seperated list creatorIDLst to temporary tables
    DROP TEMPORARY TABLE IF EXISTS tmp_creatorIDLst;
	CREATE TEMPORARY TABLE tmp_creatorIDLst(UserID INT);
    
	IF IFNULL(creatorIDLst,'') <> ''THEN
		SET @sqlCommand = REPLACE(creatorIDLst, ',', '),(');
		SET @sqlCommand = CONCAT('(', @sqlCommand, ')');
		SET @sqlCommand = CONCAT('INSERT INTO tmp_creatorIDLst VALUES'
															, @sqlCommand); 
                                                            
		PREPARE stmt FROM  @sqlCommand;
		EXECUTE stmt;
		DEALLOCATE PREPARE stmt;
	END IF;
    
	#Insert comma seperated list notificationStatusIDLst to temporary tables
    DROP TEMPORARY TABLE IF EXISTS tmp_notificationStatusIDLst;
	CREATE TEMPORARY TABLE tmp_notificationStatusIDLst(StatusID TINYINT UNSIGNED);
    
    IF IFNULL(notificationStatusIDLst,'') <> ''THEN
		SET @sqlCommand = REPLACE(notificationStatusIDLst, ',', '),(');
		SET @sqlCommand = CONCAT('(', @sqlCommand, ')');
		SET @sqlCommand = CONCAT('INSERT INTO tmp_notificationStatusIDLst VALUES'
															, @sqlCommand); 
                                                            
		PREPARE stmt FROM  @sqlCommand;
		EXECUTE stmt;
		DEALLOCATE PREPARE stmt;
	END IF;
    
    #1st dataset: Return Total_Row for paging
    SELECT COUNT(1) Total_Row
    FROM Notification n
    WHERE n.NotificationTypeID IN (
									SELECT	tmp_nt.NotificationTypeID
                                    FROM tmp_notificationTypeIDLst tmp_nt
								)
	AND n.ConfigSendTime >= configSendTimeFrom
    AND n.ConfigSendTime <= configSendTimeTo
	AND n.CreatorID IN (
									SELECT	tmp_user.UserID
                                    FROM tmp_creatorIDLst tmp_user
								)
    AND n.StatusID IN (
									SELECT	tmp_stt.StatusID
                                    FROM tmp_notificationStatusIDLst tmp_stt
								)
    AND EXISTS (
			SELECT 1
            FROM NotificationRecipientList nrl
            WHERE nrl.NotificationID = n.NotificationID
			AND nrl.RecipientID IN (
												SELECT	tmp_rid.RecipientID
												FROM tmp_recipientIDLst tmp_rid
                                            )
    );
    
	#Get data based on Page Index and Page Size
    DROP TEMPORARY TABLE IF EXISTS tmp_Notification;
	CREATE TEMPORARY TABLE tmp_Notification(
													NotificationID INT
                                                    , CreatorID INT
                                                    , ConfigSendTime DATETIME
                                                    , Title VARCHAR(300) CHARACTER SET utf8mb4
                                                    , Content MEDIUMTEXT CHARACTER SET utf8mb4
                                                    , Status VARCHAR(10)
                                                    , StatusID TINYINT UNSIGNED
                                                    , RecipientSentStatusJSON TEXT
											);
    #Get data based on Page Index and Page Size
    INSERT INTO tmp_Notification (	
													NotificationID
                                                    , CreatorID
                                                    , ConfigSendTime
                                                    , Title
                                                    , Content
                                                    , Status
                                                    , StatusID
												)
    SELECT n.NotificationID, n.CreatorID, n.ConfigSendTime, n.Title, n.Content, n.Status, n.StatusID
    FROM Notification n
    WHERE n.NotificationTypeID IN (
									SELECT	tmp_nt.NotificationTypeID
                                    FROM tmp_notificationTypeIDLst tmp_nt
								)
	AND n.ConfigSendTime >= configSendTimeFrom
    AND n.ConfigSendTime <= configSendTimeTo
	AND n.CreatorID IN (
									SELECT	tmp_user.UserID
                                    FROM tmp_creatorIDLst tmp_user
								)
    AND n.StatusID IN (
									SELECT	tmp_stt.StatusID
                                    FROM tmp_notificationStatusIDLst tmp_stt
								)
    AND EXISTS (
			SELECT 1
            FROM NotificationRecipientList nrl
            WHERE nrl.NotificationID = n.NotificationID
			AND nrl.RecipientID IN (
												SELECT	tmp_rid.RecipientID
												FROM tmp_recipientIDLst tmp_rid
                                            )
    )
    ORDER BY n.ConfigSendTime,  n.CreatorID
    LIMIT rowStart, pageSize; 
    
    #Update RecipientSentStatus
    UPDATE tmp_Notification n
    SET n.RecipientSentStatusJSON = (
				SELECT CONCAT('[' , GROUP_CONCAT(
										json_object('RecipientName', r.RecipientName
										, 'IsError', CASE 
																	WHEN n.StatusID NOT IN (
																				SENT_STATUSID_ERROR
                                                                                , SENT_STATUSID_RESOLVED
																			) THEN 0 #Not error
																	ELSE 
																			CASE WHEN nrl.StatusID > 0 THEN 1
																						ELSE 0
																			END
														END
                                        )
							SEPARATOR ', ') ,']') 
				FROM NotificationRecipientList nrl
                INNER JOIN Recipient r ON r.RecipientID = nrl.RecipientID
                WHERE nrl.NotificationID = n.NotificationID
                ORDER BY r.RecipientName
		);
    
    #2nd dataset: Get data based on Page Index and Page Size
    SELECT *
    FROM tmp_Notification n
    ORDER BY n.ConfigSendTime,  n.CreatorID;
END;