CREATE DEFINER=`elton.ton`@`%` PROCEDURE `NCB_Common_Recipient_Sel`()
BEGIN
	DECLARE RECIPIENT_TYPE_LICENSEES 	SMALLINT DEFAULT 1;
	SELECT rt.RecipientTypeID
		,rt.RecipientTypeName
		, lt.LicenseeTypeId
        ,lt.LicenseeTypeName
		, r.RecipientID
        ,r.RecipientName
    FROM RecipientType rt
		INNER JOIN Recipient r ON rt.RecipientTypeID = r.RecipientTypeID
		LEFT JOIN Licensee lic ON r.RecipientTypeID = RECIPIENT_TYPE_LICENSEES AND lic.LicenseeId = r.SourceID
		LEFT JOIN LicenseeType lt ON lt.LicenseeTypeId = lic.LicenseeTypeId
    ORDER BY RecipientTypeID,  lt.LicenseeTypeName, RecipientName;
END