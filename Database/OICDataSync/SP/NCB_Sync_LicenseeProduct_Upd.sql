DELIMITER //

DROP PROCEDURE IF EXISTS NCB_Sync_LicenseeProduct_Upd;

CREATE DEFINER=`root`@`%` PROCEDURE `NCB_Sync_LicenseeProduct_Upd`(
	IN licenseeProducts JSON
	/*
		Creator: Scofield@20190905
        Params:
				- licenseeProducts: LicenseeProducts                
        Revisions:
				- 20190905@Scofield: Create        
    */
)
NCB_Sync_LicenseeProduct_Upd:BEGIN
	DECLARE jsonLength BIGINT UNSIGNED DEFAULT JSON_LENGTH(licenseeProducts);
    DECLARE jsonIndex BIGINT UNSIGNED DEFAULT 0;
    
    DROP TEMPORARY TABLE IF EXISTS jsonTemp;
    CREATE TEMPORARY TABLE IF NOT EXISTS jsonTemp(LicenseeId INT, ProductId INT, LaunchDate DATE, TerminationDate DATE);
    
    WHILE jsonIndex < jsonLength DO 
		INSERT INTO jsonTemp(LicenseeId, ProductId, LaunchDate, TerminationDate)
        VALUES (
			JSON_EXTRACT(licenseeProducts, CONCAT('$[', jsonIndex, '].LicenseeId')), 
            JSON_EXTRACT(licenseeProducts, CONCAT('$[', jsonIndex, '].ProductId')),
            JSON_EXTRACT(licenseeProducts, CONCAT('$[', jsonIndex, '].LaunchDate')),
            JSON_EXTRACT(licenseeProducts, CONCAT('$[', jsonIndex, '].TerminationDate'))
		);
        
        SET jsonIndex := jsonIndex + 1;
    END WHILE;    
    
    INSERT INTO  Licensee(LicenseeId, ProductId, LaunchDate, TerminationDate)    
    SELECT LicenseeId, ProductId, LaunchDate, TerminationDate
    FROM jsonTemp
    ON DUPLICATE KEY UPDATE 
		LicenseeId = values(LicenseeId), 
        ProductId = values(ProductId), 
        LaunchDate = values(LaunchDate), 
        TerminationDate = values(TerminationDate);
    
    DROP TEMPORARY TABLE IF EXISTS jsonTemp;
END