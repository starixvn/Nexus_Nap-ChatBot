CREATE PROCEDURE `NCB_BotName_Sel`()
BEGIN
	SELECT DISTINCT Name as Id, Name as Name
    FROM BotCoversationMember
    WHERE IsActive = 1 AND Name IS NOT NULL;
END