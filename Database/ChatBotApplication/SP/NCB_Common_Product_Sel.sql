DROP PROCEDURE IF EXISTS NCB_Common_Product_Sel;
DELIMITER //

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  NCB_Common_Product_Sel(
    /*
		Creator: Dominic@20190401
        Param:

        Revisions:
				- 20190401@Dominic: First Commit
        
    */
)
SQL SECURITY DEFINER
BEGIN

	SELECT p.ProductId AS ID, p.ProductName AS Name
    FROM Product p
    ORDER BY p.ProductName;
    
END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE NCB_Common_Product_Sel TO 'ncb.admin'@'%';