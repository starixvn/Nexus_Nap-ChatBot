DROP PROCEDURE IF EXISTS NCB_Common_User_Sel;
DELIMITER //

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  NCB_Common_User_Sel(
    /*
		Creator: Dominic@20190408
        Param:

        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
SQL SECURITY DEFINER
BEGIN
	SELECT u.UserID AS ID, u.UserName AS Name
    FROM User u
    ORDER BY  u.UserName;
END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE ChatBot.NCB_Common_User_Sel TO 'ncb.admin'@'%';