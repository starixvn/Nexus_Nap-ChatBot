DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.NAP_MenuItem_GetByUserAndSiteIncludeGroup;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.NAP_MenuItem_GetByUserAndSiteIncludeGroup(
	IN userID INT
    , IN siteID INT
    , IN environment VARCHAR(10)
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
        SELECT t.FunctionId, t.Policy
		FROM ChatBot.NAP_UserFunction t
		WHERE t.UserId = userID AND t.SiteId = siteID;
                    
		DROP TEMPORARY TABLE IF EXISTS tmp_GroupFunctionPolicy;
		CREATE TEMPORARY TABLE tmp_GroupFunctionPolicy
       SELECT gf.FunctionId, gf.Policy
		FROM dbo.NAP_GroupFunction AS gf
			INNER JOIN dbo.NAP_Users AS u ON u.GroupId = gf.GroupId
		WHERE u.UserId = userID AND gf.SiteId = siteID;
    
		SELECT 
				m.MenuItemId,
				m.FunctionId,
				f.FunctionName,
				m.SiteId,
				s.SiteName,
				m.MenuItemName,
				m.Description,
				m.ParentId,
				m.ParentName,
				uf.Policy,
				m.URL,
				m.DisplayOrder,
				m.IsActive,
				m.CreatedBy		AS CreatorId,
				cb.username		AS CreatorUsername,
				m.CreatedDate,
				m.ModifiedBy	AS ModifierId,
				mb.username		AS ModifierUsername,
				m.ModifiedDate,
				m.Environment
		FROM 
			(
					SELECT CASE 
										WHEN IFNULL(t.AFunctionID, 0) = 0 THEN t.BFunctionID 
										ELSE t.AFunctionID 
									END AS FunctionID 
									, CASE 
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
					) t
        ) AS uf
		INNER JOIN ChatBot.NAP_MenuItems AS m ON uf.FunctionId = m.FunctionId AND uf.Policy >= m.Policy
		INNER JOIN ChatBot.NAP_Functions AS f ON uf.FunctionId = f.FunctionId
		INNER JOIN ChatBot.NAP_Sites AS s ON m.SiteId = s.SiteId
		LEFT JOIN ChatBot.Customer AS cb ON m.CreatedBy = cb.custid
		LEFT JOIN ChatBot.Customer AS mb ON m.ModifiedBy = mb.custid
        WHERE (environment = 'UAT' OR (environment != 'UAT' AND m.Environment = environment AND f.Environment = environment))
		AND m.IsActive = 1
		AND f.IsActive = 1;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NAP_MenuItem_GetByUserAndSiteIncludeGroup TO 'ncb.admin'@'%';