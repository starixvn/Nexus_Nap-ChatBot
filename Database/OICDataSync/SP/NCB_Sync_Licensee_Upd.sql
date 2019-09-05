DELIMITER //

DROP PROCEDURE IF EXISTS NCB_Sync_Licensee_Upd;

CREATE DEFINER=`root`@`%` PROCEDURE `NCB_Sync_Licensee_Upd`(
	IN licensees JSON
	/*
		Creator: Scofield@20190905
        Params:
				- licensees: Licensees                
        Revisions:
				- 20190905@Scofield: Create        
    */
)
NCB_Sync_Licensee_Upd:BEGIN
	DECLARE jsonLength BIGINT UNSIGNED DEFAULT JSON_LENGTH(licensees);
    DECLARE jsonIndex BIGINT UNSIGNED DEFAULT 0;
    
    DROP TEMPORARY TABLE IF EXISTS jsonTemp;
    CREATE TEMPORARY TABLE IF NOT EXISTS jsonTemp(LicenseeId INT, LicenseeName VARCHAR(500), RegisteredLicenseeName VARCHAR(500), LicenseeStatusId INT, LicenseeTypeId INT);
    
    WHILE jsonIndex < jsonLength DO 
		INSERT INTO jsonTemp(LicenseeId, LicenseeName, RegisteredLicenseeName, LicenseeStatusId, LicenseeTypeId)
        VALUES (
			JSON_EXTRACT(licensees, CONCAT('$[', jsonIndex, '].LicenseeId')), 
            JSON_EXTRACT(licensees, CONCAT('$[', jsonIndex, '].LicenseeName')),
            JSON_EXTRACT(licensees, CONCAT('$[', jsonIndex, '].RegisteredLicenseeName')),
            JSON_EXTRACT(licensees, CONCAT('$[', jsonIndex, '].LicenseeStatusId')),
            JSON_EXTRACT(licensees, CONCAT('$[', jsonIndex, '].LicenseeTypeId'))
		);
        
        SET jsonIndex := jsonIndex + 1;
    END WHILE;    
    
    INSERT INTO  Licensee(LicenseeId, LicenseeName, RegisteredLicenseeName, LicenseeStatusId, LicenseeTypeId)    
    SELECT LicenseeId, LicenseeName, RegisteredLicenseeName, LicenseeStatusId, LicenseeTypeId
    FROM jsonTemp
    ON DUPLICATE KEY UPDATE 
		LicenseeId = values(LicenseeId), 
        LicenseeName = values(LicenseeName), 
        RegisteredLicenseeName = values(RegisteredLicenseeName), 
        LicenseeStatusId = values(LicenseeStatusId), 
        LicenseeTypeId = values(LicenseeTypeId);
    
    DROP TEMPORARY TABLE IF EXISTS jsonTemp;
END