DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.testNapUserControl_Users;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.testNapUserControl_Users(
	IN Users JSON
	
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
################# NAP_ss ###########################

	 DROP TEMPORARY TABLE IF EXISTS tmp_NAP_Users;
	CREATE TEMPORARY TABLE tmp_NAP_Users(
                            UserId INT
                            , GroupId INT
                            , LastLoginDate DATETIME
                            , CreatedBy INT
                            , CreatedDate DATETIME
                            , ModifiedBy INT
                            , ModifiedDate DATETIME
                        );
    
    SET iLoop = 0;
	WHILE (JSON_EXTRACT(Users, CONCAT('$[',iLoop, '].UserId'))  IS NOT NULL) DO
		INSERT INTO tmp_NAP_Users(
                              UserId
                            , GroupId
                            , LastLoginDate
                            , CreatedBy
                            , CreatedDate
                            , ModifiedBy
                            , ModifiedDate
                                    )
		SELECT JSON_EXTRACT(Users, CONCAT('$[', iLoop, '].UserId')) 
						, JSON_UNQUOTE(JSON_EXTRACT(Users, CONCAT('$[', iLoop, '].GroupId')))
						, CASE 
                            WHEN JSON_UNQUOTE(JSON_EXTRACT(Users, CONCAT('$[', iLoop, '].LastLoginDate'))) = 'null'
                            THEN NULL
                            ELSE JSON_UNQUOTE(JSON_EXTRACT(Users, CONCAT('$[', iLoop, '].LastLoginDate')))
                          END
                        , JSON_UNQUOTE(JSON_EXTRACT(Users, CONCAT('$[', iLoop, '].CreatedBy')))
                        , JSON_UNQUOTE(JSON_EXTRACT(Users, CONCAT('$[', iLoop, '].CreatedDate')))
                        , CASE
                            WHEN JSON_UNQUOTE(JSON_EXTRACT(Users, CONCAT('$[', iLoop, '].ModifiedBy'))) = 'null'
                            THEN NULL
                            ELSE JSON_UNQUOTE(JSON_EXTRACT(Users, CONCAT('$[', iLoop, '].ModifiedBy')))
                          END
                        , CASE
                            WHEN JSON_UNQUOTE(JSON_EXTRACT(Users, CONCAT('$[', iLoop, '].ModifiedDate'))) = 'null'
                            THEN NULL
                            ELSE JSON_UNQUOTE(JSON_EXTRACT(Users, CONCAT('$[', iLoop, '].ModifiedDate')))
                          END;

		SET iLoop = iLoop +1;
	END WHILE;

    DELETE u FROM ChatBot.NAP_Users u
            LEFT JOIN tmp_NAP_Users tmp ON u.UserId = tmp.UserId 
            WHERE tmp.UserId IS NULL;

    INSERT INTO ChatBot.NAP_Users(UserId
                            , GroupId
                            , LastLoginDate
                            , CreatedBy
                            , CreatedDate
                            , ModifiedBy
                            , ModifiedDate)
    SELECT tmp.UserId 
            , tmp.GroupId 
            , tmp.LastLoginDate
            , tmp.CreatedBy
            , tmp.CreatedDate
            , tmp.ModifiedBy
            , tmp.ModifiedDate
    FROM tmp_NAP_Users tmp
    LEFT JOIN ChatBot.NAP_Users m ON m.UserId = tmp.UserId 
    WHERE m.UserId IS NULL;

    UPDATE ChatBot.NAP_Users m
    INNER JOIN tmp_NAP_Users tmp ON m.UserId = tmp.UserId 
    SET m.GroupId = tmp.GroupId
        ,m.LastLoginDate = tmp.LastLoginDate
        ,m.CreatedBy = tmp.CreatedBy
        ,m.CreatedDate = tmp.CreatedDate
        ,m.ModifiedBy = tmp.ModifiedBy
        ,m.ModifiedDate = tmp.ModifiedDate;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.testNapUserControl_Users TO 'ncb.admin'@'%';