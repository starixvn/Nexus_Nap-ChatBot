DELIMITER //
DROP PROCEDURE `NCB_Setting_LicenseeStatus_Sel`;
CREATE PROCEDURE `NCB_Setting_LicenseeStatus_Sel`(
	/*
		Creator: Scofield@20190404
        Description: Get list of all LicenseeStatus
        Revisions:
				- 20190404@Scofield: Create        
    */
)
BEGIN
	DECLARE PRODUCT_STATUS_ID INT DEFAULT -1000;
	
	SELECT CAST(LicenseeStatusId AS SIGNED) AS Id , LicenseeStatusName AS Name
	FROM LicenseeStatus

	UNION

	SELECT PRODUCT_STATUS_ID AS Id, 'N/A' AS Name;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_Setting_LicenseeStatus_Sel TO 'ncb.admin'@'%';