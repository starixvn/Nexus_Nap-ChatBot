DELIMITER //
CREATE PROCEDURE `NCB_RecipientType_Sel`()
BEGIN
  SELECT RecipientTypeID AS Id, RecipientTypeName AS Name
  FROM RecipientType;
END;