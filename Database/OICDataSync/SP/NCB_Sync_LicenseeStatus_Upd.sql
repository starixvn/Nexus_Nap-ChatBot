DELIMITER //

DROP PROCEDURE IF EXISTS NCB_Sync_LicenseeStatus_Upd;

CREATE DEFINER=`root`@`%` PROCEDURE `NCB_Sync_LicenseeStatus_Upd`(
	IN licenseeStatuses JSON
	/*
		Creator: Scofield@20190905
        Params:
				- licenseeStatuses: LicenseeStatuses                
        Revisions:
				- 20190905@Scofield: Create        
    */
)
NCB_Sync_LicenseeStatus_Upd:BEGIN
	DECLARE jsonLength BIGINT UNSIGNED DEFAULT JSON_LENGTH(licenseeStatuses);
    DECLARE jsonIndex BIGINT UNSIGNED DEFAULT 0;
    
    DROP TEMPORARY TABLE IF EXISTS jsonTemp;
    CREATE TEMPORARY TABLE IF NOT EXISTS jsonTemp(LicenseeStatusId INT, LicenseeStatusName VARCHAR(500));
    
    WHILE jsonIndex < jsonLength DO 
		INSERT INTO jsonTemp(LicenseeStatusId, LicenseeStatusName)
        VALUES (
			JSON_EXTRACT(licenseeStatuses, CONCAT('$[', jsonIndex, '].LicenseeStatusId')), 
            JSON_EXTRACT(licenseeStatuses, CONCAT('$[', jsonIndex, '].LicenseeStatusName'))
		);
        
        SET jsonIndex := jsonIndex + 1;
    END WHILE;    
    
    INSERT INTO  Licensee(LicenseeStatusId, LicenseeStatusName)    
    SELECT LicenseeStatusId, LicenseeStatusName
    FROM jsonTemp
    ON DUPLICATE KEY UPDATE 
		LicenseeStatusId = values(LicenseeStatusId), 
        LicenseeStatusName = values(LicenseeStatusName);
    
    DROP TEMPORARY TABLE IF EXISTS jsonTemp;
END