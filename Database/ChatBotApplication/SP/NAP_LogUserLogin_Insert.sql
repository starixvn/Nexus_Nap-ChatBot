DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.NAP_LogUserLogin_Insert;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.NAP_LogUserLogin_Insert(
	IN userID INT,
    IN siteID INT, 
    IN iP VARCHAR(50), 
    IN logDate DATETIME,
    IN doing VARCHAR(300), 
    IN domainName VARCHAR(255), 
    IN browserName VARCHAR(255), 
    IN platform VARCHAR(255), 
    IN countryCode VARCHAR(64), 
    IN duration INT
    /*
		Creator: Dominic@20190408
        Param:

        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
SQL SECURITY DEFINER
BEGIN
		INSERT INTO ChatBot.NAPLog_UserLogin(
			UserId,
			SiteId,
			IP,
			LogDate,
			Doing,
			DomainName,
			BrowserName,
			Platform,
			CountryCode,
			Duration)
		VALUES (
			userID,
			siteID,
			iP,
			logDate,
			doing,
			domainName,
			browserName,
			platform,
			countryCode,
			duration);
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NAP_LogUserLogin_Insert TO 'ncb.admin'@'%';