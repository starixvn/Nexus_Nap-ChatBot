DELIMITER //
DROP PROCEDURE IF EXISTS `NCB_Common_RecipientType_Sel`;

CREATE PROCEDURE `NCB_Common_RecipientType_Sel`(
	/*
		Creator: Dominic@20190423
        Description: Get list of all recipient types
        Revisions:
				- 20190423@Dominic: Create        
    */
)
BEGIN
  SELECT RecipientTypeID AS Id, RecipientTypeName AS Name
  FROM RecipientType
  ORDER BY RecipientTypeID;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_Common_RecipientType_Sel TO 'ncb.admin'@'%';