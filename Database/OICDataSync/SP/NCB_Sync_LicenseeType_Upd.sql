DELIMITER //

DROP PROCEDURE IF EXISTS NCB_Sync_LicenseeType_Upd;

CREATE DEFINER=`root`@`%` PROCEDURE `NCB_Sync_LicenseeType_Upd`(
	IN licenseeTypes JSON
	/*
		Creator: Scofield@20190905
        Params:
				- licenseeTypes: LicenseeTypes                
        Revisions:
				- 20190905@Scofield: Create        
    */
)
NCB_Sync_LicenseeType_Upd:BEGIN
	DECLARE jsonLength BIGINT UNSIGNED DEFAULT JSON_LENGTH(licenseeTypes);
    DECLARE jsonIndex BIGINT UNSIGNED DEFAULT 0;
    
    DROP TEMPORARY TABLE IF EXISTS jsonTemp;
    CREATE TEMPORARY TABLE IF NOT EXISTS jsonTemp(LicenseeTypeId INT, LicenseeTypeName VARCHAR(500));
    
    WHILE jsonIndex < jsonLength DO 
		INSERT INTO jsonTemp(LicenseeTypeId, LicenseeTypeName)
        VALUES (
			JSON_EXTRACT(licenseeTypes, CONCAT('$[', jsonIndex, '].LicenseeTypeId')), 
            JSON_EXTRACT(licenseeTypes, CONCAT('$[', jsonIndex, '].LicenseeTypeName'))
		);
        
        SET jsonIndex := jsonIndex + 1;
    END WHILE;    
    
    INSERT INTO  Licensee(LicenseeTypeId, LicenseeTypeName)    
    SELECT LicenseeTypeId, LicenseeTypeName
    FROM jsonTemp
    ON DUPLICATE KEY UPDATE 
		LicenseeTypeId = values(LicenseeTypeId), 
        LicenseeTypeName = values(LicenseeTypeName);
    
    DROP TEMPORARY TABLE IF EXISTS jsonTemp;
END