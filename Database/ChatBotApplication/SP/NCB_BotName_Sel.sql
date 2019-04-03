DELIMITER //
CREATE PROCEDURE `NCB_BotName_Sel`()
BEGIN
	SELECT DISTINCT Name AS Id, Name AS Name
    FROM BotConversationMember
    WHERE IsActive = 1 AND Name IS NOT NULL;
END;