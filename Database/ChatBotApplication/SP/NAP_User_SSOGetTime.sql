DROP PROCEDURE IF EXISTS ChatBot.NAP_User_SSOGetTime;
DELIMITER //

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.NAP_User_SSOGetTime(
    OUT time DATETIME
    /*
		Creator: Dominic@20190408
        Param:

        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
SQL SECURITY DEFINER
BEGIN
        SET time = UTC_TIMESTAMP();
END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE ChatBot.NAP_User_SSOGetTime TO 'ncb.admin'@'%';