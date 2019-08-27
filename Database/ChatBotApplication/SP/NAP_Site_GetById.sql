DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.NAP_Site_GetById;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.NAP_Site_GetById(
    IN siteID INT
    /*
		Creator: Dominic@20190408
        Param:

        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
SQL SECURITY DEFINER
BEGIN
		SELECT s.SiteId,
			SiteName,
			SiteKey,
			SecretAccessKey,
			Description,
			URL,
			TestURL,
			IsActive,
			Environment
		FROM ChatBot.NAP_Sites s
		WHERE s.SiteId = siteID;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NAP_Site_GetById TO 'ncb.admin'@'%';