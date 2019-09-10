DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.NCB_OIC_SyncData;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.NCB_OIC_SyncData(
	IN Licensees JSON,
	IN LicenseeProducts JSON, 
	IN LicenseeStatuses JSON, 
	IN LicenseeTypes JSON, 
	IN Products JSON, 
    OUT errCode INT,
    OUT errMsg VARCHAR(200)

    /*
		Creator: Oliver@20190909
        Param:

        Revisions:
				- 20190909@Oliver: Merge 5 SP into 1
        
    */
)
SQL SECURITY DEFINER
BEGIN
	DECLARE iLoop INT;

############################ Licensee ###########################################

    DROP TEMPORARY TABLE IF EXISTS tmp_Licensee;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmp_Licensee(
            LicenseeId INT, 
            LicenseeName VARCHAR(500), 
            RegisteredLicenseeName VARCHAR(500), 
            LicenseeStatusId INT, 
            LicenseeTypeId INT
            );
    
    SET iLoop = 0;
    WHILE (JSON_EXTRACT(Licensees, CONCAT('$[',iLoop, '].LicenseeId'))  IS NOT NULL) DO 
		INSERT INTO tmp_Licensee(LicenseeId, LicenseeName, RegisteredLicenseeName, LicenseeStatusId, LicenseeTypeId)
        VALUES (
			JSON_UNQUOTE(JSON_EXTRACT(Licensees, CONCAT('$[', iLoop, '].LicenseeId'))), 
            JSON_UNQUOTE(JSON_EXTRACT(Licensees, CONCAT('$[', iLoop, '].LicenseeName'))),
            JSON_UNQUOTE(JSON_EXTRACT(Licensees, CONCAT('$[', iLoop, '].RegisteredLicenseeName'))),
            JSON_UNQUOTE(JSON_EXTRACT(Licensees, CONCAT('$[', iLoop, '].LicenseeStatusId'))),
            JSON_UNQUOTE(JSON_EXTRACT(Licensees, CONCAT('$[', iLoop, '].LicenseeTypeId')))
		);
        
        SET iLoop := iLoop + 1;
    END WHILE;    
    
    DELETE l FROM ChatBot.Licensee l LEFT JOIN tmp_Licensee tmp ON l.LicenseeId = tmp.LicenseeId WHERE tmp.LicenseeId IS NULL;

    INSERT INTO  ChatBot.Licensee(LicenseeId, LicenseeName, RegisteredLicenseeName, LicenseeStatusId, LicenseeTypeId)    
    SELECT LicenseeId, LicenseeName, RegisteredLicenseeName, LicenseeStatusId, LicenseeTypeId
    FROM tmp_Licensee
    ON DUPLICATE KEY UPDATE 
		LicenseeId = values(LicenseeId), 
        LicenseeName = values(LicenseeName), 
        RegisteredLicenseeName = values(RegisteredLicenseeName), 
        LicenseeStatusId = values(LicenseeStatusId), 
        LicenseeTypeId = values(LicenseeTypeId);

############################ Licensee Product ###########################################

    DROP TEMPORARY TABLE IF EXISTS tmp_LicenseeProduct;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmp_LicenseeProduct(LicenseeId INT, ProductId INT, LaunchDate DATETIME, TerminationDate DATETIME);
    
    SET iLoop = 0;
    WHILE (JSON_EXTRACT(LicenseeProducts, CONCAT('$[',iLoop, '].LicenseeId'))) DO 
		INSERT INTO tmp_LicenseeProduct(LicenseeId, ProductId, LaunchDate, TerminationDate)
        VALUES (
			JSON_UNQUOTE(JSON_EXTRACT(LicenseeProducts, CONCAT('$[', iLoop, '].LicenseeId'))), 
            JSON_UNQUOTE(JSON_EXTRACT(LicenseeProducts, CONCAT('$[', iLoop, '].ProductId'))),
            JSON_UNQUOTE(JSON_EXTRACT(LicenseeProducts, CONCAT('$[', iLoop, '].LaunchDate'))),
            CASE
                WHEN JSON_UNQUOTE(JSON_EXTRACT(LicenseeProducts, CONCAT('$[', iLoop, '].TerminationDate'))) = 'null'
                THEN NULL
                ELSE JSON_UNQUOTE(JSON_EXTRACT(LicenseeProducts, CONCAT('$[', iLoop, '].TerminationDate')))
            END
		);
        
        SET iLoop := iLoop + 1;
    END WHILE;

    DELETE lp FROM ChatBot.LicenseeProduct lp 
    LEFT JOIN tmp_LicenseeProduct tmp ON lp.LicenseeId = tmp.LicenseeId AND lp.ProductId = tmp.ProductId 
    WHERE tmp.LicenseeId IS NULL;
    
    INSERT INTO  LicenseeProduct(LicenseeId, ProductId, LaunchDate, TerminationDate)    
    SELECT LicenseeId, ProductId, LaunchDate, TerminationDate
    FROM tmp_LicenseeProduct
    ON DUPLICATE KEY UPDATE 
		LicenseeId = values(LicenseeId), 
        ProductId = values(ProductId), 
        LaunchDate = values(LaunchDate), 
        TerminationDate = values(TerminationDate);

############################ Licensee Status ###########################################

    DROP TEMPORARY TABLE IF EXISTS tmp_LicenseeStatus;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmp_LicenseeStatus(LicenseeStatusId INT, LicenseeStatusName VARCHAR(500));
    
    SET iLoop = 0;
    WHILE (JSON_EXTRACT(LicenseeStatuses, CONCAT('$[',iLoop, '].LicenseeStatusId'))) DO 
        INSERT INTO tmp_LicenseeStatus(LicenseeStatusId, LicenseeStatusName)
        VALUES (
            JSON_UNQUOTE(JSON_EXTRACT(LicenseeStatuses, CONCAT('$[', iLoop, '].LicenseeStatusId'))), 
            JSON_UNQUOTE(JSON_EXTRACT(LicenseeStatuses, CONCAT('$[', iLoop, '].LicenseeStatusName')))
        );
        
        SET iLoop := iLoop + 1;
    END WHILE;
    
    DELETE ls FROM ChatBot.LicenseeStatus ls LEFT JOIN tmp_LicenseeStatus tmp ON ls.LicenseeStatusId = tmp.LicenseeStatusId WHERE tmp.LicenseeStatusId IS NULL;

    INSERT INTO  LicenseeStatus(LicenseeStatusId, LicenseeStatusName)    
    SELECT LicenseeStatusId, LicenseeStatusName
    FROM tmp_LicenseeStatus
    ON DUPLICATE KEY UPDATE 
        LicenseeStatusId = values(LicenseeStatusId), 
        LicenseeStatusName = values(LicenseeStatusName);

############################ Licensee Types ###########################################

    DROP TEMPORARY TABLE IF EXISTS tmp_LicenseeType;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmp_LicenseeType(LicenseeTypeId INT, LicenseeTypeName VARCHAR(500));
    
    SET iLoop = 0;
    WHILE (JSON_EXTRACT(LicenseeTypes, CONCAT('$[',iLoop, '].LicenseeTypeId'))) DO 
		INSERT INTO tmp_LicenseeType(LicenseeTypeId, LicenseeTypeName)
        VALUES (
			JSON_UNQUOTE(JSON_EXTRACT(LicenseeTypes, CONCAT('$[', iLoop, '].LicenseeTypeId'))), 
            JSON_UNQUOTE(JSON_EXTRACT(LicenseeTypes, CONCAT('$[', iLoop, '].LicenseeTypeName')))
		);
        
        SET iLoop := iLoop + 1;
    END WHILE;    
    
    DELETE lt FROM ChatBot.LicenseeType lt LEFT JOIN tmp_LicenseeType tmp ON lt.LicenseeTypeId = tmp.LicenseeTypeId WHERE tmp.LicenseeTypeId IS NULL;

    INSERT INTO  LicenseeType(LicenseeTypeId, LicenseeTypeName)    
    SELECT LicenseeTypeId, LicenseeTypeName
    FROM tmp_LicenseeType
    ON DUPLICATE KEY UPDATE 
		LicenseeTypeId = values(LicenseeTypeId), 
        LicenseeTypeName = values(LicenseeTypeName);

############################ Products ###########################################

    DROP TEMPORARY TABLE IF EXISTS tmp_Product;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmp_Product(ProductId INT, ProductName VARCHAR(500));
    
    SET iLoop = 0;
    WHILE (JSON_EXTRACT(Products, CONCAT('$[',iLoop, '].ProductId'))) DO 
		INSERT INTO tmp_Product(ProductId, ProductName)
        VALUES (
			JSON_UNQUOTE(JSON_EXTRACT(Products, CONCAT('$[', iLoop, '].ProductId'))), 
            JSON_UNQUOTE(JSON_EXTRACT(Products, CONCAT('$[', iLoop, '].ProductName')))            
		);
        
        SET iLoop := iLoop + 1;
    END WHILE;    
    
    DELETE p FROM ChatBot.Product p LEFT JOIN tmp_Product tmp ON p.ProductId = tmp.ProductId WHERE tmp.ProductId IS NULL;

    INSERT INTO Product(ProductId, ProductName)    
    SELECT ProductId, ProductName
    FROM tmp_Product
    ON DUPLICATE KEY UPDATE 
		ProductId = values(ProductId), 
        ProductName = values(ProductName);
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_OIC_SyncData TO 'ncb.admin'@'%';