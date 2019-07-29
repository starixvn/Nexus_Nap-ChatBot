DELIMITER //
DROP PROCEDURE IF EXISTS `NCB_Setting_BotName_Sel`;
CREATE PROCEDURE `NCB_Setting_BotName_Sel`(
	/*
		Creator: Scofield@20190404       
        Revisions:
				- 20190404@Scofield: Create
        
    */
) 
BEGIN
	SELECT DISTINCT b.ConversationBotID AS Id, b.Name AS Name
    FROM ConversationBot b
    WHERE Name IS NOT NULL AND Name <> ''
    ORDER BY b.Name;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_Setting_BotName_Sel TO 'ncb.admin'@'%';


