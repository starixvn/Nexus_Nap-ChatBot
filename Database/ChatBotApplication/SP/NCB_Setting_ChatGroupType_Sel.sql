DELIMITER //
CREATE PROCEDURE `NCB_Setting_ChatGroupType_Sel`()
	/*
		Creator: Scofield@20190404        
        Revisions:
				- 20190404@Scofield: Create        
    */
BEGIN
  SELECT ChatGroupTypeID AS Id, ChatGroupTypeName AS Name
  FROM ChatGroupType;
END;