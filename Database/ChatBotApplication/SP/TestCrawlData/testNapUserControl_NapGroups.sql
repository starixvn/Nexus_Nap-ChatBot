DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.testNapUserControl_NapGroups;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.testNapUserControl_NapGroups(
	IN NapGroups JSON
	
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

	DROP TEMPORARY TABLE IF EXISTS tmp_NAP_Groups;
	CREATE TEMPORARY TABLE tmp_NAP_Groups(
                            GroupId INT
                            , GroupName VARCHAR(100) CHARACTER SET utf8mb4
                            , Description VARCHAR(300) CHARACTER SET utf8mb4
                            , IsActive TINYINT
                            , CreatedBy INT
                            , CreatedDate DATETIME
                            , ModifiedBy INT
                            , ModifiedDate DATETIME
                        );	
    
    SET iLoop = 0;
	WHILE (JSON_EXTRACT(NapGroups, CONCAT('$[',iLoop, '].GroupId'))  IS NOT NULL) DO
		INSERT INTO tmp_NAP_Groups(
																		GroupId 
																		, GroupName 
																		, Description
                                                                        , IsActive
                                                                        , CreatedBy
                                                                        , CreatedDate
                                                                        , ModifiedBy
                                                                        , ModifiedDate
																	)
		SELECT JSON_EXTRACT(NapGroups, CONCAT('$[', iLoop, '].GroupId')) 
						, JSON_UNQUOTE(JSON_EXTRACT(NapGroups, CONCAT('$[', iLoop, '].GroupName')))
                        , JSON_UNQUOTE(JSON_EXTRACT(NapGroups, CONCAT('$[', iLoop, '].Description')))
                        , JSON_EXTRACT(NapGroups, CONCAT('$[', iLoop, '].IsActive'))
                        , JSON_UNQUOTE(JSON_EXTRACT(NapGroups, CONCAT('$[', iLoop, '].CreatedBy')))
                        , JSON_UNQUOTE(JSON_EXTRACT(NapGroups, CONCAT('$[', iLoop, '].CreatedDate')))
                        , JSON_UNQUOTE(JSON_EXTRACT(NapGroups, CONCAT('$[', iLoop, '].ModifiedBy')))
                        , JSON_UNQUOTE(JSON_EXTRACT(NapGroups, CONCAT('$[', iLoop, '].ModifiedDate')));
		SET iLoop = iLoop +1;
	END WHILE;

    DELETE g FROM ChatBot.NAP_Groups g LEFT JOIN tmp_NAP_Groups tempg ON g.GroupId = tempg.GroupId WHERE tempg.GroupId IS NULL;

    INSERT INTO ChatBot.NAP_Groups(GroupId 
                                    , GroupName 
                                    , Description
                                    , IsActive
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
    FROM tmp_NAP_Groups tmp
    LEFT JOIN ChatBot.NAP_Groups g ON tmp.GroupId = g.GroupId
    WHERE g.GroupId IS NULL;

    UPDATE ChatBot.NAP_Groups g
    INNER JOIN tmp_NAP_Groups temp ON temp.GroupId = g.GroupId
    SET g.GroupId = temp.GroupId
        ,g.GroupName = temp.GroupName
        ,g.Description = temp.Description
        ,g.IsActive = temp.IsActive
        ,g.CreatedBy = temp.CreatedBy
        ,g.CreatedDate = temp.CreatedDate
        ,g.ModifiedBy = temp.ModifiedBy
        ,g.ModifiedDate = temp.ModifiedDate;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.testNapUserControl_NapGroups TO 'ncb.admin'@'%';