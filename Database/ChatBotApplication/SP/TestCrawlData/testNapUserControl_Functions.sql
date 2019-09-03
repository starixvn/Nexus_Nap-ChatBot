DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.testNapUserControl_Functions;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.testNapUserControl_Functions(
	IN Functions JSON
	
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

	DROP TEMPORARY TABLE IF EXISTS tmp_NAP_Functions;
	CREATE TEMPORARY TABLE tmp_NAP_Functions(
                            FunctionId INT
                            , SiteId TINYINT UNSIGNED
                            , FunctionName VARCHAR(50) CHARACTER SET utf8mb4
                            , FunctionKey VARCHAR(50) CHARACTER SET utf8mb4
                            , Description VARCHAR(200) CHARACTER SET utf8mb4
                            , ParentId INT
                            , ParentName VARCHAR(50) CHARACTER SET utf8mb4
                            , CRUD TINYINT UNSIGNED
                            , DisplayOrder SMALLINT
                            , IsActive TINYINT
                            , CreatedBy INT
                            , CreatedDate DATETIME
                            , ModifiedBy INT
                            , ModifiedDate DATETIME
                            , Environment VARCHAR(10) CHARACTER SET utf8mb4
                        );	
    
    SET iLoop = 0;
	WHILE (JSON_EXTRACT(Functions, CONCAT('$[',iLoop, '].FunctionId'))  IS NOT NULL) DO
		INSERT INTO tmp_NAP_Functions(FunctionId 
										, SiteId 
										, FunctionName 
										, FunctionKey 
										, Description
										, ParentId 
										, ParentName
										, CRUD
										, DisplayOrder
										, IsActive
										, CreatedBy
										, CreatedDate
										, ModifiedBy
										, ModifiedDate
										, Environment
										)
		SELECT JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].FunctionId')) 
						, JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].SiteId')))
                        , JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].FunctionName')))
                        , JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].FunctionKey')))
                        , JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].Description')))
                        , CASE 
							WHEN JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].ParentId'))) = 'null' 
                            THEN null 
                            ELSE JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].ParentId'))) 
						  END
                        , JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].ParentName')))
                        , JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].CRUD')))
                        , JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].DisplayOrder')))
                        , JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].IsActive'))
                        , JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].CreatedBy')))
                        , JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].CreatedDate')))
                        , CASE 
							WHEN JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].ModifiedBy'))) = 'null' 
                            THEN NULL 
                            ELSE JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].ModifiedBy'))) 
						  END
                        , CASE 
							WHEN JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].ModifiedDate'))) = 'null' 
                            THEN NULL 
                            ELSE JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].ModifiedDate'))) 
						   END
                        , JSON_UNQUOTE(JSON_EXTRACT(Functions, CONCAT('$[', iLoop, '].Environment')));                        
		SET iLoop = iLoop +1;
	END WHILE;
	
    DELETE f FROM ChatBot.NAP_Functions f LEFT JOIN tmp_NAP_Functions tempf ON f.FunctionId = tempf.FunctionId WHERE tempf.FunctionId IS NULL;

   INSERT INTO ChatBot.NAP_Functions(FunctionId 
                                    , SiteId 
                                    , FunctionName 
                                    , FunctionKey 
                                    , Description
                                    , ParentId 
                                    , ParentName
                                    , CRUD
                                    , DisplayOrder
                                    , IsActive
                                    , CreatedBy
                                    , CreatedDate
                                    , ModifiedBy
                                    , ModifiedDate
                                    , Environment
                                )
    SELECT tmpf.FunctionId 
            , tmpf.SiteId 
            , tmpf.FunctionName 
            , tmpf.FunctionKey 
            , tmpf.Description
            , tmpf.ParentId 
            , tmpf.ParentName
            , tmpf.CRUD
            , tmpf.DisplayOrder
            , tmpf.IsActive
            , tmpf.CreatedBy
            , tmpf.CreatedDate
            , tmpf.ModifiedBy
            , tmpf.ModifiedDate
            , tmpf.Environment
    FROM tmp_NAP_Functions tmpf
    LEFT JOIN ChatBot.NAP_Functions f ON tmpf.FunctionId = f.FunctionId
    WHERE f.FunctionId IS NULL;

    UPDATE ChatBot.NAP_Functions f
    INNER JOIN tmp_NAP_Functions tmpf ON tmpf.FunctionId = f.FunctionId
    SET f.SiteId = tmpf.SiteId 
		, f.FunctionName  = tmpf.FunctionName 
		, f.FunctionKey = tmpf.FunctionKey 
		, f.Description = tmpf.Description
		, f.ParentId = tmpf.ParentId 
		, f.ParentName= tmpf.ParentName
		, f.CRUD = tmpf.CRUD
		, f.DisplayOrder= tmpf.DisplayOrder
		, f.IsActive = tmpf.IsActive
		, f.CreatedBy= tmpf.CreatedBy
		, f.CreatedDate= tmpf.CreatedDate
		, f.ModifiedBy = tmpf.ModifiedBy
		, f.ModifiedDate= tmpf.ModifiedDate
		, f.Environment = tmpf.Environment
	WHERE f.FunctionId = tmpf.FunctionId;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.testNapUserControl_Functions TO 'ncb.admin'@'%';