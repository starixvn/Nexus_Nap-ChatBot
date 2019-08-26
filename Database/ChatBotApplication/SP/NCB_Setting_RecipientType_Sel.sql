DELIMITER //
DROP PROCEDURE IF EXISTS `NCB_Setting_RecipientType_Sel`;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE `NCB_Setting_RecipientType_Sel`(
	/*
		Creator: Scofield@20190404
        Description: Get list of all recipient types
        Revisions:
				- 20190404@Scofield: Create        
				- 20190823@Oliver: removed Not Assigned
    */
)
SQL SECURITY DEFINER
BEGIN
	SELECT rt.RecipientTypeId As Id, rt.RecipientTypeName As Name
    FROM ChatBot.RecipientType rt
    ORDER BY rt.RecipientTypeId;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_Setting_RecipientType_Sel TO 'ncb.admin'@'%';