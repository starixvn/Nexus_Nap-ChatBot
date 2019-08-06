DELIMITER //
DROP PROCEDURE IF EXISTS `NCB_Setting_SearchRecipient_Sel`;
CREATE PROCEDURE `NCB_Setting_SearchRecipient_Sel`(
	IN query VARCHAR(200)
	/*
		Creator: Scofield@20190404       
        Revisions:
				- 20190404@Scofield: Create
        
    */
) 
BEGIN	
	SELECT RecipientID AS ID, RecipientName AS Name
	FROM Recipient
	WHERE RecipientName LIKE CONCAT('%', query, '%')
    ORDER BY RecipientName;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_Setting_SearchRecipient_Sel TO 'ncb.admin'@'%';