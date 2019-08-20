DELIMITER //

DROP PROCEDURE IF EXISTS NCB_NotificationType_Sel;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE NCB_NotificationType_Sel(
	IN pageIndex INT, 
	IN pageSize INT,
    OUT totalItems INT
	/*
		Creator: Oliver@20190820
				Params:
						- pageIndex: Page Index for paging
						- pageSize: Page Size for paging
				Revisions:
						- 20190729@Oliver: Revised total items to output    		
    */
	)
SQL SECURITY DEFINER
BEGIN
	DECLARE rowStart INT;
    DECLARE total INT;
    SELECT 	pageSize * (pageIndex - 1) into rowStart;    
    
    SET totalItems = (SELECT COUNT(1) FROM NotificationType);
    
    SELECT distinct
		nt.NotificationTypeID AS Id, 
		nt.NotificationTypeName AS Name, 
		s.SystemID AS SystemId,
		s.SystemName AS SystemName
	FROM (SELECT * FROM NotificationType ORDER BY NotificationTypeID LIMIT rowStart, pageSize) nt
	INNER JOIN System s ON nt.SystemId = s.SystemId
    ORDER BY nt.NotificationTypeID;    
    
    SELECT 
		  nrt.RecipientTypeID AS Id,
          rt.RecipientTypeName AS Name,
          nrt.NotificationTypeID AS NotificationTypeId
    FROM NotificationRecipientType nrt 
    INNER JOIN RecipientType rt ON nrt.RecipientTypeID = rt.RecipientTypeId;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_NotificationType_Sel TO 'ncb.admin'@'%';