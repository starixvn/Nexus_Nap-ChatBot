DELIMITER //
CREATE PROCEDURE `NCB_Recipient_Sel`()
BEGIN
	DECLARE LICENSEE_TYPE INT DEFAULT 1;
	
	SELECT RecipientId AS ID, RecipientName AS Name, ls.LicenseeStatusName AS Status, RecipientTypeID
    FROM Recipient r
	LEFT JOIN Licensee l ON l.LicenseeId = r.SourceID AND r.RecipientTypeID = LICENSEE_TYPE
	LEFT JOIN LicenseeStatus ls ON l.LicenseeStatusId = ls.LicenseeStatusId;
END;