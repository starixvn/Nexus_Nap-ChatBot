DELIMITER //
DROP PROCEDURE `NCB_Setting_Recipient_Sel`;
CREATE PROCEDURE `NCB_Setting_Recipient_Sel`(
	/*
		Creator: Scofield@20190404
        Description: Get list of all recipients with their status
        Revisions:
				- 20190404@Scofield: Create        
    */
)
BEGIN
	DECLARE LICENSEE_TYPE INT DEFAULT 1;
	
	SELECT RecipientId AS ID, RecipientName AS Name, ls.LicenseeStatusName AS Status, RecipientTypeID
    FROM Recipient r
	LEFT JOIN Licensee l ON l.LicenseeId = r.SourceID AND r.RecipientTypeID = LICENSEE_TYPE
	LEFT JOIN LicenseeStatus ls ON l.LicenseeStatusId = ls.LicenseeStatusId;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_Setting_Recipient_Sel TO 'ncb.admin'@'%';