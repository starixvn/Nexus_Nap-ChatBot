DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.NAP_User_GetUserSiteFunctionKeyPolicy;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.NAP_User_GetUserSiteFunctionKeyPolicy(
	IN userID INT
    , IN siteID INT
    , IN functionKey VARCHAR(50)
    /*
		Creator: Dominic@20190408
        Param:

        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
SQL SECURITY DEFINER
BEGIN
		DROP TEMPORARY TABLE IF EXISTS tmp_UserFunctionPolicy;
		CREATE TEMPORARY TABLE tmp_UserFunctionPolicy
        SELECT 
					uf.FunctionId, 
					uf.Policy
				FROM ChatBot.NAP_UserFunction AS uf 
					INNER JOIN ChatBot.NAP_Functions AS f  ON uf.FunctionId = f.FunctionId
				WHERE 
					uf.UserId = userID
					AND f.SiteId = siteID
					AND f.FunctionKey = functionKey
					AND f.IsActive = 1;
                    
		DROP TEMPORARY TABLE IF EXISTS tmp_GroupFunctionPolicy;
		CREATE TEMPORARY TABLE tmp_GroupFunctionPolicy
        SELECT 
					gf.FunctionId, 
					gf.Policy
				FROM  ChatBot.NAP_GroupFunction AS gf 
					INNER JOIN ChatBot.NAP_Users AS u  ON u.GroupId = gf.GroupId
					INNER JOIN ChatBot.NAP_Functions AS f  ON gf.FunctionId = f.FunctionId
				WHERE 
					u.UserId = userID
					AND f.SiteId = siteID
					AND f.FunctionKey = functionKey
					AND f.IsActive = 1;
    
    
		SELECT  CASE 
							WHEN IFNULL(t.APolicy, 0) < IFNULL(t.BPolicy, 0) THEN IFNULL(t.BPolicy, 0) 
							ELSE IFNULL(t.APolicy, 0) 
						END AS Policy
		FROM (
			SELECT t1.AFunctionId, t1.APolicy, t2.BFunctionId, t2.BPolicy
            FROM tmp_UserFunctionPolicy t1
            LEFT JOIN tmp_GroupFunctionPolicy t2 
					ON t1.FunctionId = t2.FunctionId
			UNION ALL
            SELECT t1.AFunctionId, t1.APolicy, t2.BFunctionId, t2.BPolicy
            FROM tmp_UserFunctionPolicy t1
            RIGHT JOIN tmp_GroupFunctionPolicy t2 
					ON t1.FunctionId = t2.FunctionId
			WHERE t1.FunctionId IS NULL
		) t;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NAP_User_GetUserSiteFunctionKeyPolicy TO 'ncb.admin'@'%';