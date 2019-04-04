DELIMITER //
CREATE PROCEDURE `NCB_Setting_BotName_Sel`()
 /*
		Creator: Scofield@20190404       
        Revisions:
				- 20190404@Scofield: Create
        
    */
BEGIN
	SELECT DISTINCT Name AS Id, Name AS Name
    FROM BotConversationMember
    WHERE IsActive = 1 AND Name IS NOT NULL;
END;