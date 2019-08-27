DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.NAP_User_CheckIn;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.NAP_User_CheckIn(
	IN userID INT,
    IN siteID INT, 
    IN netSessionId VARCHAR(30),
	IN iP VARCHAR(50), 
    IN loginDate DATETIME,
    OUT onUserId INT
    /*
		Creator: Dominic@20190408
        Param:

        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
SQL SECURITY DEFINER
BEGIN
		UPDATE ChatBot.NAP_OnUser ou
		SET ou.IsKickedOut = 1 
		WHERE ou.UserId = userID 	
		AND ou.SiteId = siteID;

		INSERT INTO ChatBot.NAP_OnUser(UserId, SiteId, IsKickedOut, IP, NetSessionId)
		VALUES (userID, siteID, 0, iP, netSessionId);
		
		SET  onUserId = LAST_INSERT_ID();

		IF (siteID = 0) THEN
				UPDATE ChatBot.NAP_Users u
				SET u.LastLoginDate = loginDate
				WHERE u.UserId = userID;
		ELSE
				UPDATE ChatBot.NAP_UserSite s
				SET s.LastLoginDate = loginDate
				WHERE s.SiteId = siteID	
				AND s.UserId = userID;
		END IF;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NAP_User_CheckIn TO 'ncb.admin'@'%';