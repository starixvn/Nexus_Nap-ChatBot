DELIMITER //
CREATE PROCEDURE `NCB_ChatGroupType_Sel`()
BEGIN
  SELECT ChatGroupTypeID AS Id, ChatGroupTypeName AS Name
  FROM ChatGroupType;
END;