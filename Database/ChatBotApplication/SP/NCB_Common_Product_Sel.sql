DELIMITER //

DROP PROCEDURE IF EXISTS NCB_Common_Product_Sel;

CREATE PROCEDURE  NCB_Common_Product_Sel(
    /*
		Creator: Dominic@20190401
        Param:

        Revisions:
				- 20190401@Dominic: First Commit
        
    */
)
BEGIN

	SELECT p.ProductId, p.ProductName
    FROM Product p
    ORDER BY p.ProductName;
    
END;