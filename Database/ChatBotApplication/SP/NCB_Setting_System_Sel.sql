DELIMITER //
DROP PROCEDURE IF EXISTS `NCB_Setting_System_Sel`;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE `NCB_Setting_System_Sel`(
	/*
		Creator: Scofield@20190911
        Description: Get list of all system
        Revisions:
				- 20190911@Scofield: Create        				
    */
)
SQL SECURITY DEFINER
BEGIN
	SELECT SystemID As Id, SystemName As Name
    FROM ChatBot.System
    ORDER BY SystemID;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_Setting_System_Sel TO 'ncb.admin'@'%';