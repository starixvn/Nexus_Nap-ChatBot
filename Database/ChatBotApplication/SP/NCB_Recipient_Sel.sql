CREATE PROCEDURE `NCB_Recipient_Sel`()
BEGIN
	declare LICENSEE_TYPE int default 1;
    declare PRODUCT_TYPE int default 2;
    
	select LicenseeID as ID, LicenseeName as Name, LICENSEE_TYPE as RecipientTypeID
    from Licensee
    union
	select ProductID as ID, ProductName as Name, PRODUCT_TYPE as RecipientTypeID
    from Product;
END