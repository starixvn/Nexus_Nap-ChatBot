DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.testNapUserControl_UserSite;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.testNapUserControl_UserSite(
	IN UserSite JSON
	
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

	DROP TEMPORARY TABLE IF EXISTS tmp_NAP_UserSite;
	CREATE TEMPORARY TABLE tmp_NAP_UserSite(
                            UserId INT
                            , SiteId INT
                            , Level INT
                            , IsActive TINYINT
                            , LastLoginDate INT
                            , CreatedBy INT
                            , CreatedDate DATETIME
                            , ModifiedBy INT
                            , ModifiedDate DATETIME
                            , ExpiredDate DATETIME
                            , IsNeverExpired TINYINT
                        );
    
    SET iLoop = 0;
	WHILE (JSON_EXTRACT(UserSite, CONCAT('$[',iLoop, '].UserId'))  IS NOT NULL) DO
		INSERT INTO tmp_NAP_UserSite(
                              UserId
                            , SiteId
                            , Level
                            , IsActive
                            , LastLoginDate
                            , CreatedBy
                            , CreatedDate
                            , ModifiedBy
                            , ModifiedDate
                            , ExpiredDate
                            , IsNeverExpired
                                    )
		SELECT JSON_EXTRACT(UserSite, CONCAT('$[', iLoop, '].UserId')) 
						, JSON_UNQUOTE(JSON_EXTRACT(UserSite, CONCAT('$[', iLoop, '].SiteId')))
						, JSON_UNQUOTE(JSON_EXTRACT(UserSite, CONCAT('$[', iLoop, '].Level')))
                        , JSON_EXTRACT(UserSite, CONCAT('$[', iLoop, '].IsActive'))
						, JSON_UNQUOTE(JSON_EXTRACT(UserSite, CONCAT('$[', iLoop, '].LastLoginDate')))
                        , JSON_UNQUOTE(JSON_EXTRACT(UserSite, CONCAT('$[', iLoop, '].CreatedBy')))
                        , JSON_UNQUOTE(JSON_EXTRACT(UserSite, CONCAT('$[', iLoop, '].CreatedDate')))
                        , JSON_UNQUOTE(JSON_EXTRACT(UserSite, CONCAT('$[', iLoop, '].ModifiedBy')))
                        , JSON_UNQUOTE(JSON_EXTRACT(UserSite, CONCAT('$[', iLoop, '].ModifiedDate')))
                        , JSON_UNQUOTE(JSON_EXTRACT(UserSite, CONCAT('$[', iLoop, '].ExpiredDate')))
                        , JSON_EXTRACT(UserSite, CONCAT('$[', iLoop, '].IsNeverExpired'));

		SET iLoop = iLoop +1;
	END WHILE;

    DELETE us FROM ChatBot.NAP_UserSite us
            LEFT JOIN tmp_NAP_UserSite tmp ON us.UserId = tmp.UserId 
            WHERE tmp.UserId IS NULL;

    INSERT INTO ChatBot.NAP_UserSite(UserId
                            , SiteId
                            , Level
                            , IsActive
                            , LastLoginDate
                            , CreatedBy
                            , CreatedDate
                            , ModifiedBy
                            , ModifiedDate
                            , ExpiredDate
                            , IsNeverExpired)
    SELECT tmp.UserId 
            , tmp.SiteId 
            , tmp.Level
            , tmp.IsActive
            , tmp.LastLoginDate
            , tmp.CreatedBy
            , tmp.CreatedDate
            , tmp.ModifiedBy
            , tmp.ModifiedDate
            , tmp.ExpiredDate
            , tmp.IsNeverExpired
    FROM tmp_NAP_UserSite tmp
    LEFT JOIN ChatBot.NAP_UserSite m ON m.UserId = tmp.UserId 
    WHERE m.UserId IS NULL;

    UPDATE ChatBot.NAP_UserSite m
    INNER JOIN tmp_NAP_UserSite tmp ON m.UserId = tmp.UserId 
    SET m.UserId = tmp.UserId
        ,m.SiteId = tmp.SiteId
        ,m.IsActive = tmp.IsActive
        ,m.LastLoginDate = tmp.LastLoginDate
        ,m.CreatedBy = tmp.CreatedBy
        ,m.CreatedDate = tmp.CreatedDate
        ,m.ModifiedBy = tmp.ModifiedBy
        ,m.ModifiedDate = tmp.ModifiedDate
        ,m.ExpiredDate = tmp.ExpiredDate
        ,m.IsNeverExpired = tmp.IsNeverExpired;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.testNapUserControl_UserSite TO 'ncb.admin'@'%';