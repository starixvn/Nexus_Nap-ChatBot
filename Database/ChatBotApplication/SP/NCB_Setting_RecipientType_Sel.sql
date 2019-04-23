DELIMITER //
DROP PROCEDURE IF EXISTS `NCB_Setting_RecipientType_Sel`;

CREATE PROCEDURE `NCB_Setting_RecipientType_Sel`(
	/*
		Creator: Scofield@20190404
        Description: Get list of all recipient types
        Revisions:
				- 20190404@Scofield: Create        
    */
)
BEGIN
  SELECT RecipientTypeID AS Id, RecipientTypeName AS Name
  FROM RecipientType
  UNION ALL
  SELECT -1000 AS Id, 'Not Assigned' AS Name;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_Setting_RecipientType_Sel TO 'ncb.admin'@'%';