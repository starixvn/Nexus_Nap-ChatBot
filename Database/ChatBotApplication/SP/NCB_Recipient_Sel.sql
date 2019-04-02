DELIMITER //
CREATE PROCEDURE `NCB_Recipient_Sel`()
BEGIN
	declare LICENSEE_TYPE INT = 1;
	
	select RecipientId as ID, RecipientName as Name, ls.LicenseeStatusName as Status, RecipientTypeID
    from Recipient r
	left join Licensee l on l.LicenseeId = r.SourceID and r.RecipientTypeID = LICENSEE_TYPE
	left join LicenseeStatus ls on l.LicenseeStatusId = ls.LicenseeStatusId;
END;