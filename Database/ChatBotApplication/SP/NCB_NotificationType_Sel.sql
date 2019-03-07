CREATE PROCEDURE `NCB_NotificationType_Sel`(
	IN pageIndex INT, 
	IN pageSize INT)
BEGIN
	DECLARE rowStart INT;
    DECLARE total INT;
    SELECT 	pageSize * (pageIndex - 1) into rowStart;    
    
    SELECT COUNT(*) AS Total FROM NotificationType;
    
    SELECT 
		nt.NotificationTypeID AS NotificationTypeId, 
		nt.NotificationTypeName AS NotificationTypeName, 
		rt.RecipientTypeID AS RecipientTypeId, 
		rt.RecipientTypeName AS RecipientTypeName,
		s.SystemID AS SystemId,
		s.SystemName AS SystemName
	FROM (SELECT * FROM NotificationType ORDER BY NotificationTypeID LIMIT rowStart, pageSize) nt
    INNER JOIN NotificationTypeRecipientType ntrt ON nt.NotificationTypeID = ntrt.NotificationTypeID			
	INNER JOIN RecipientType rt ON rt.RecipientTypeId = ntrt.RecipientTypeID
	INNER JOIN System s ON nt.SystemId = s.SystemId
    ORDER BY nt.NotificationTypeID;    
END