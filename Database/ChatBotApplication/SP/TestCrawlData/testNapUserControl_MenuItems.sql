DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.testNapUserControl_MenuItems;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.testNapUserControl_MenuItems(
	IN MenuItems JSON
	
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
################# NAP_MenuItems ###########################

	DROP TEMPORARY TABLE IF EXISTS tmp_NAP_MenuItems;
	CREATE TEMPORARY TABLE tmp_NAP_MenuItems(
                              MenuItemId INT
                            , FunctionId INT
                            , SiteId INT
                            , MenuItemName VARCHAR(200) CHARACTER SET utf8mb4
                            , Description VARCHAR(300) CHARACTER SET utf8mb4
                            , ParentId INT
                            , ParentName VARCHAR(200) CHARACTER SET utf8mb4
                            , Policy INT
                            , URL VARCHAR(200)
                            , DisplayOrder INT
                            , IsActive TINYINT
                            , CreatedBy INT
                            , CreatedDate DATETIME
                            , ModifiedBy INT
                            , ModifiedDate DATETIME
                        );	
    
    SET iLoop = 0;
	WHILE (JSON_EXTRACT(MenuItems, CONCAT('$[',iLoop, '].MenuItemId'))  IS NOT NULL) DO
		INSERT INTO tmp_NAP_MenuItems(
                                                                          MenuItemId
																		, FunctionId 
																		, SiteId 
																		, MenuItemName 
																		, Description
																		, ParentId 
                                                                        , ParentName
																		, Policy
                                                                        , URL
                                                                        , DisplayOrder
                                                                        , IsActive
                                                                        , CreatedBy
                                                                        , CreatedDate
                                                                        , ModifiedBy
                                                                        , ModifiedDate
																	)
		SELECT JSON_EXTRACT(MenuItems, CONCAT('$[', iLoop, '].MenuItemId')) 
						, JSON_UNQUOTE(JSON_EXTRACT(MenuItems, CONCAT('$[', iLoop, '].FunctionId')))
						, JSON_UNQUOTE(JSON_EXTRACT(MenuItems, CONCAT('$[', iLoop, '].SiteId')))
                        , JSON_UNQUOTE(JSON_EXTRACT(MenuItems, CONCAT('$[', iLoop, '].MenuItemName')))
                        , JSON_UNQUOTE(JSON_EXTRACT(MenuItems, CONCAT('$[', iLoop, '].Description')))
                        , JSON_UNQUOTE(JSON_EXTRACT(MenuItems, CONCAT('$[', iLoop, '].ParentId')))
                        , JSON_UNQUOTE(JSON_EXTRACT(MenuItems, CONCAT('$[', iLoop, '].ParentName')))
                        , JSON_UNQUOTE(JSON_EXTRACT(MenuItems, CONCAT('$[', iLoop, '].Policy')))
                        , JSON_UNQUOTE(JSON_EXTRACT(MenuItems, CONCAT('$[', iLoop, '].URL')))
                        , JSON_UNQUOTE(JSON_EXTRACT(MenuItems, CONCAT('$[', iLoop, '].DisplayOrder')))
                        , JSON_EXTRACT(MenuItems, CONCAT('$[', iLoop, '].IsActive'))
                        , JSON_UNQUOTE(JSON_EXTRACT(MenuItems, CONCAT('$[', iLoop, '].CreatedBy')))
                        , JSON_UNQUOTE(JSON_EXTRACT(MenuItems, CONCAT('$[', iLoop, '].CreatedDate')))
                        , JSON_UNQUOTE(JSON_EXTRACT(MenuItems, CONCAT('$[', iLoop, '].ModifiedBy')))
                        , JSON_UNQUOTE(JSON_EXTRACT(MenuItems, CONCAT('$[', iLoop, '].ModifiedDate')));
		SET iLoop = iLoop +1;
	END WHILE;

    DELETE f FROM ChatBot.NAP_MenuItems f LEFT JOIN tmp_NAP_MenuItems tmp ON f.MenuItemId = tmp.MenuItemId WHERE tmp.MenuItemId IS NULL;

   INSERT INTO ChatBot.NAP_MenuItems(MenuItemId
                                    , FunctionId 
                                    , SiteId 
                                    , MenuItemName 
                                    , Description
                                    , ParentId 
                                    , ParentName
                                    , Policy
                                    , URL
                                    , DisplayOrder
                                    , IsActive
                                    , CreatedBy
                                    , CreatedDate
                                    , ModifiedBy
                                    , ModifiedDate
                                )
    SELECT tmpf.MenuItemId 
            , tmpf.FunctionId 
            , tmpf.SiteId 
            , tmpf.MenuItemName 
            , tmpf.Description
            , tmpf.ParentId 
            , tmpf.ParentName
            , tmpf.Policy
            , tempf.URL
            , tmpf.DisplayOrder
            , tmpf.IsActive
            , tmpf.CreatedBy
            , tmpf.CreatedDate
            , tmpf.ModifiedBy
            , tmpf.ModifiedDate
    FROM tmp_NAP_MenuItems tmpf
    LEFT JOIN ChatBot.NAP_MenuItems f ON tmpf.MenuItemId = f.MenuItemId
    WHERE f.MenuItemId IS NULL;

    UPDATE ChatBot.NAP_MenuItems f
    INNER JOIN tmp_NAP_MenuItems tmpf ON tmpf.MenuItemId = f.MenuItemId
    SET f.MenuItemId = tempf.MenuItemId
            , f.FunctionId = tmpf.FunctionId
            , f.SiteId = tmpf.SiteId 
            , f.MenuItemName  = tmpf.MenuItemName 
            , f.Description = tmpf.Description
            , f.ParentId = tmpf.ParentId 
            , f.ParentName= tmpf.ParentName
            , f.Policy = tmpf.Policy
            , f.URL = tmpf.URL
            , f.DisplayOrder= tmpf.DisplayOrder
            , f.IsActive = tmpf.IsActive
            , f.CreatedBy= tmpf.CreatedBy
            , f.CreatedDate= tmpf.CreatedDate
            , f.ModifiedBy = tmpf.ModifiedBy
            , f.ModifiedDate = tmpf.ModifiedDate;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.testNapUserControl_MenuItems TO 'ncb.admin'@'%';