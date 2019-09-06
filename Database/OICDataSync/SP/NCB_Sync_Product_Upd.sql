DELIMITER //

DROP PROCEDURE IF EXISTS NCB_Sync_Product_Upd;

CREATE DEFINER=`root`@`%` PROCEDURE `NCB_Sync_Product_Upd`(
	IN products JSON
	/*
		Creator: Scofield@20190905
        Params:
				- products: Products
        Revisions:
				- 20190905@Scofield: Create        
    */
)
NCB_Sync_Product_Upd:BEGIN
	DECLARE jsonLength BIGINT UNSIGNED DEFAULT JSON_LENGTH(products);
    DECLARE jsonIndex BIGINT UNSIGNED DEFAULT 0;
    
    DROP TEMPORARY TABLE IF EXISTS jsonTemp;
    CREATE TEMPORARY TABLE IF NOT EXISTS jsonTemp(ProductId INT, ProductName VARCHAR(500));
    
    WHILE jsonIndex < jsonLength DO 
		INSERT INTO jsonTemp(ProductId, ProductName)
        VALUES (
			JSON_EXTRACT(products, CONCAT('$[', jsonIndex, '].ProductId')), 
            JSON_EXTRACT(products, CONCAT('$[', jsonIndex, '].ProductName'))            
		);
        
        SET jsonIndex := jsonIndex + 1;
    END WHILE;    
    
    INSERT INTO Product(ProductId, ProductName)    
    SELECT ProductId, ProductName
    FROM jsonTemp
    ON DUPLICATE KEY UPDATE 
		ProductId = values(ProductId), 
        ProductName = values(ProductName);
    
    DROP TEMPORARY TABLE IF EXISTS jsonTemp;
END