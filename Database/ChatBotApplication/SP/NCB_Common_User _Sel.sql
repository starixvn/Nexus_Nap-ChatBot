DELIMITER //

DROP PROCEDURE IF EXISTS NCB_Common_User_Sel;

CREATE PROCEDURE  NCB_Common_User_Sel(
    /*
		Creator: Dominic@20190408
        Param:

        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
BEGIN
	SELECT u.UserID, u.UserName
    FROM User u
    ORDER BY  u.UserName;
END;