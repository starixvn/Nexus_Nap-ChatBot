DROP PROCEDURE IF EXISTS NCB_NotificationType_Sel;
DELIMITER //

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
    SET rowStart =	pageSize * (pageIndex - 1);  
    
    SET totalItems = (SELECT COUNT(1) FROM NotificationType nt);
    
    SELECT
		nt.NotificationTypeID AS Id, 
		nt.NotificationTypeName AS Name, 
		s.SystemID AS SystemId,
		s.SystemName AS SystemName
	FROM NotificationType nt
	INNER JOIN ChatBot.System s ON nt.SystemID = s.SystemId
    ORDER BY nt.NotificationTypeID
	LIMIT rowStart, pageSize;
    
    SELECT 
		  nrt.RecipientTypeID AS Id,
          rt.RecipientTypeName AS Name,
          nrt.NotificationTypeID AS NotificationTypeId
    FROM NotificationRecipientType nrt 
    INNER JOIN RecipientType rt ON nrt.RecipientTypeID = rt.RecipientTypeId;
	
	SELECT 
		cgt.ChatGroupTypeID AS Id,
		cgt.ChatGroupTypeName AS Name,
		nct.NotificationTypeID AS NotificationTypeId
	FROM NotificationChatGroupType nct
	INNER JOIN ChatGroupType cgt ON cgt.ChatGroupTypeID = nct.ChatGroupTypeID;
END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE ChatBot.NCB_NotificationType_Sel TO 'ncb.admin'@'%';