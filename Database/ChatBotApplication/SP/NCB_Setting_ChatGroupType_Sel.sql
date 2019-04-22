DELIMITER //
DROP PROCEDURE `NCB_Setting_ChatGroupType_Sel`;
CREATE PROCEDURE `NCB_Setting_ChatGroupType_Sel`(
	/*
		Creator: Scofield@20190404        
        Revisions:
				- 20190404@Scofield: Create        
    */
)	
BEGIN
  SELECT ChatGroupTypeID AS Id, ChatGroupTypeName AS Name
  FROM ChatGroupType;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_Setting_ChatGroupType_Sel TO 'ncb.admin'@'%';