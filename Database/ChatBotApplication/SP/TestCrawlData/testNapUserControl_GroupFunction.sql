DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.testNapUserControl_GroupFunction;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.testNapUserControl_GroupFunction(
	IN GroupFunction JSON
	
    /*
		Creator: Oliver@20190829
        Param:

        Revisions:
				- 20190829@Oliver: First Commit
        
    */
)
SQL SECURITY DEFINER
BEGIN
	DECLARE iLoop INT;
################# NAP_GroupFunctions ###########################

	 DROP TEMPORARY TABLE IF EXISTS tmp_NAP_GroupFunction;
	CREATE TEMPORARY TABLE tmp_NAP_GroupFunction(
                            GroupId INT
                            , FunctionId INT
                            , SiteId INT
                            , Policy INT
                            , CreatedBy INT
                            , CreatedDate DATETIME
                            , ModifiedBy INT
                            , ModifiedDate DATETIME
                        );	
    
    SET iLoop = 0;
	WHILE (JSON_EXTRACT(GroupFunction, CONCAT('$[',iLoop, '].GroupId'))  IS NOT NULL) DO
		INSERT INTO tmp_NAP_GroupFunction(
																		GroupId 
																		, FunctionId 
																		, SiteId
                                                                        , Policy
                                                                        , CreatedBy
                                                                        , CreatedDate
                                                                        , ModifiedBy
                                                                        , ModifiedDate
																	)
		SELECT JSON_EXTRACT(GroupFunction, CONCAT('$[', iLoop, '].GroupId')) 
						, JSON_UNQUOTE(JSON_EXTRACT(GroupFunction, CONCAT('$[', iLoop, '].FunctionId')))
                        , JSON_UNQUOTE(JSON_EXTRACT(GroupFunction, CONCAT('$[', iLoop, '].SiteId')))
                        , JSON_UNQUOTE(JSON_EXTRACT(GroupFunction, CONCAT('$[', iLoop, '].Policy')))
                        , JSON_UNQUOTE(JSON_EXTRACT(GroupFunction, CONCAT('$[', iLoop, '].CreatedBy')))
                        , JSON_UNQUOTE(JSON_EXTRACT(GroupFunction, CONCAT('$[', iLoop, '].CreatedDate')))
                        , JSON_UNQUOTE(JSON_EXTRACT(GroupFunction, CONCAT('$[', iLoop, '].ModifiedBy')))
                        , JSON_UNQUOTE(JSON_EXTRACT(GroupFunction, CONCAT('$[', iLoop, '].ModifiedDate')));
		SET iLoop = iLoop +1;
	END WHILE;

    DELETE g FROM ChatBot.NAP_GroupFunction g LEFT JOIN tmp_NAP_GroupFunction tempg ON g.GroupId = tempg.GroupId 
                            AND g.FunctionId = tempg.FuntcionId 
                            AND g.Policy = tempg.Policy 
                            AND g.SiteId = tempg.SiteId
    WHERE tempg.GroupId IS NULL;

    INSERT INTO ChatBot.NAP_GroupFunction(GroupId 
                                    , FunctionId 
                                    , SiteId
                                    , Policy
                                    , CreatedBy
                                    , CreatedDate
                                    , ModifiedBy
                                    , ModifiedDate
                                )
    SELECT tmp.GroupId 
            , tmp.GroupName 
            , tmp.Description
            , tmp.IsActive
            , tmp.CreatedBy
            , tmp.CreatedDate
            , tmp.ModifiedBy
            , tmp.ModifiedDate
    FROM tmp_NAP_GroupFunction tmp
    LEFT JOIN ChatBot.NAP_GroupFunction g ON tmp.GroupId = g.GroupId 
                AND tmp.FunctionId = g.FunctionId 
                AND tmp.SiteId = g.SiteId
                AND tmp.Policy = g.Policy
    WHERE g.GroupId IS NULL;

    UPDATE ChatBot.NAP_GroupFunction g
    INNER JOIN tmp_NAP_GroupFunction temp ON temp.GroupId = g.GroupId 
                AND temp.FunctionId = g.FunctionId 
                AND temp.SiteId = g.SiteId
                AND temp.Policy = g.Policy
    SET g.GroupId = temp.GroupId
        ,g.GroupName = temp.FunctionId
        ,g.Description = temp.SiteId
        ,g.IsActive = temp.Policy
        ,g.CreatedBy = temp.CreatedBy
        ,g.CreatedDate = temp.CreatedDate
        ,g.ModifiedBy = temp.ModifiedBy
        ,g.ModifiedDate = temp.ModifiedDate;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.testNapUserControl_GroupFunction TO 'ncb.admin'@'%';