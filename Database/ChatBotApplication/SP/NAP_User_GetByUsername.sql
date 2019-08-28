DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.NAP_User_GetByUsername;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.NAP_User_GetByUsername(
	IN userName VARCHAR(50)
    /*
		Creator: Dominic@20190408
        Param:

        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
SQL SECURITY DEFINER
BEGIN
		SELECT 
			u.UserId
			,c.firstname
			,c.lastname
			,c.email
			,ci.username
			,c.userpwd
			,u.LastLoginDate
			,u.CreatedDate
			,u.ModifiedDate
			,g.GroupId
			,g.GroupName
			,ci1.custid		AS CreatorId
			,ci1.username	AS CreatorUsername
			,ci2.custid		AS ModifierId
			,ci2.username	AS ModifierUsername
		FROM  ChatBot.NAP_Users AS u
			INNER JOIN ChatBot.Customer AS c  ON u.UserId = c.custid
			INNER JOIN ChatBot.custInfo AS ci ON u.UserId = ci.custid
			LEFT JOIN ChatBot.NAP_Groups AS g  ON g.GroupId = u.GroupId
			LEFT JOIN ChatBot.custInfo AS ci1  ON u.CreatedBy = ci1.custid
			LEFT JOIN ChatBot.custInfo AS ci2  ON u.ModifiedBy = ci2.custid
		WHERE c.username = userName and ci.closed = 0;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NAP_User_GetByUsername TO 'ncb.admin'@'%';