DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.testNapUserControl_UserFunction;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.testNapUserControl_UserFunction(
	IN UserFunction JSON
	
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
################# NAP_Functions ###########################

	DROP TEMPORARY TABLE IF EXISTS tmp_NAP_UserFunction;
	CREATE TEMPORARY TABLE tmp_NAP_UserFunction(
                            UserId INT
                            , FunctionId INT
                            , SiteId INT
                            , Policy INT
                            , CreatedBy INT
                            , CreatedDate DATETIME
                            , ModifiedBy INT
                            , ModifiedDate DATETIME
                        );
    
    SET iLoop = 0;
	WHILE (JSON_EXTRACT(UserFunction, CONCAT('$[',iLoop, '].UserId'))  IS NOT NULL) DO
		INSERT INTO tmp_NAP_UserFunction(
                              UserId
                            , FunctionId
                            , SiteId
                            , Policy
                            , CreatedBy
                            , CreatedDate
                            , ModifiedBy
                            , ModifiedDate
                                    )
		SELECT JSON_EXTRACT(UserFunction, CONCAT('$[', iLoop, '].UserId')) 
						, JSON_UNQUOTE(JSON_EXTRACT(UserFunction, CONCAT('$[', iLoop, '].FunctionId')))
                        , JSON_UNQUOTE(JSON_EXTRACT(UserFunction, CONCAT('$[', iLoop, '].SiteId')))
                        , JSON_UNQUOTE(JSON_EXTRACT(UserFunction, CONCAT('$[', iLoop, '].Policy')))
                        , JSON_UNQUOTE(JSON_EXTRACT(UserFunction, CONCAT('$[', iLoop, '].CreatedBy')))
                        , JSON_UNQUOTE(JSON_EXTRACT(UserFunction, CONCAT('$[', iLoop, '].CreatedDate')))
                        , JSON_UNQUOTE(JSON_EXTRACT(UserFunction, CONCAT('$[', iLoop, '].ModifiedBy')))
                        , JSON_UNQUOTE(JSON_EXTRACT(UserFunction, CONCAT('$[', iLoop, '].ModifiedDate')));

		SET iLoop = iLoop +1;
	END WHILE;

    DELETE uf FROM ChatBot.NAP_UserFunction uf 
            LEFT JOIN tmp_NAP_UserFunction tmp ON uf.UserId = tmp.UserId 
                                        AND uf.FunctionId = tmp.FunctionId
                                        AND uf.Policy = tmp.Policy
                                        AND uf.SiteId = tmp.SiteId
            WHERE tmp.UserId IS NULL;

    INSERT INTO ChatBot.NAP_UserFunction(UserId
                            , FunctionId
                            , SiteId
                            , Policy
                            , CreatedBy
                            , CreatedDate
                            , ModifiedBy
                            , ModifiedDate)
    SELECT tmp.UserId 
            , tmp.FunctionId 
            , tmp.SiteId
            , tmp.Policy
            , tmp.CreatedBy
            , tmp.CreatedDate
            , tmp.ModifiedBy
            , tmp.ModifiedDate
    FROM tmp_NAP_UserFunction tmp
    LEFT JOIN ChatBot.NAP_UserFunction uf ON uf.UserId = tmp.UserId 
                                        AND uf.FunctionId = tmp.FunctionId
                                        AND uf.Policy = tmp.Policy
                                        AND uf.SiteId = tmp.SiteId
    WHERE uf.UserId IS NULL;

    UPDATE ChatBot.NAP_UserFunction uf
    INNER JOIN tmp_NAP_UserFunction tmp ON uf.UserId = tmp.UserId 
                                        AND uf.FunctionId = tmp.FunctionId
                                        AND uf.Policy = tmp.Policy
                                        AND uf.SiteId = tmp.SiteId
    SET uf.UserId = tmp.UserId
        ,uf.FunctionId = tmp.FunctionId
        ,uf.SiteId = tmp.SiteId
        ,uf.Policy = tmp.Policy
        ,uf.CreatedBy = tmp.CreatedBy
        ,uf.CreatedDate = tmp.CreatedDate
        ,uf.ModifiedBy = tmp.ModifiedBy
        ,uf.ModifiedDate = tmp.ModifiedDate;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.testNapUserControl_UserFunction TO 'ncb.admin'@'%';