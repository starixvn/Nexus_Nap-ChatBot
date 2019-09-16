DROP PROCEDURE IF EXISTS ChatBot.NAP_User_Logout;
DELIMITER //

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.NAP_User_Logout(
	IN onUserID INT
    /*
		Creator: Dominic@20190408
        Param:

        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
SQL SECURITY DEFINER
BEGIN
		DECLARE userId INT;
		SET userId = 0;

		SELECT userId = ou.UserId
		FROM ChatBot.NAP_OnUser ou 
		WHERE ou.Id = onUserID AND ou.SiteId = 0;
		
		IF(userId <> 0) THEN
			UPDATE ChatBot.NAP_OnUser ou
			SET ou.IsKickedOut = 1 
			WHERE ou.UserId = userId AND ou.SiteId <> 0;
		END IF;

		DELETE FROM ChatBot.NAP_OnUser ou 
        WHERE ou.Id = onUserID;
END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE ChatBot.NAP_User_Logout TO 'ncb.admin'@'%';