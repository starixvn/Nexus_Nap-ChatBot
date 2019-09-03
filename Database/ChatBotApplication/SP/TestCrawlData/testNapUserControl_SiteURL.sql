DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.testNapUserControl_SiteURL;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.testNapUserControl_SiteURL(
	IN SiteURL JSON
	
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

	DROP TEMPORARY TABLE IF EXISTS tmp_NAP_SiteURL;
	CREATE TEMPORARY TABLE tmp_NAP_SiteURL(
                            SiteURLId INT
                            , SiteId INT
                            , URL VARCHAR(200)
                            , Environment VARCHAR(50)
                            , IsDefault TINYINT
                            , IsActive TINYINT
                        );
    
    SET iLoop = 0;
	WHILE (JSON_EXTRACT(SiteURL, CONCAT('$[',iLoop, '].SiteURLId'))  IS NOT NULL) DO
		INSERT INTO tmp_NAP_SiteURL(
                              SiteURLId
                            , SiteId
                            , URL
                            , Environment
                            , IsDefault
                            , IsActive
                                    )
		SELECT JSON_EXTRACT(SiteURL, CONCAT('$[', iLoop, '].SiteURLId')) 
						, JSON_UNQUOTE(JSON_EXTRACT(SiteURL, CONCAT('$[', iLoop, '].SiteId')))
                        , JSON_UNQUOTE(JSON_EXTRACT(SiteURL, CONCAT('$[', iLoop, '].URL')))
                        , JSON_UNQUOTE(JSON_EXTRACT(SiteURL, CONCAT('$[', iLoop, '].Environment')))
                        , JSON_EXTRACT(SiteURL, CONCAT('$[', iLoop, '].IsDefault'))
                        , JSON_EXTRACT(SiteURL, CONCAT('$[', iLoop, '].IsActive'));

		SET iLoop = iLoop +1;
	END WHILE;

    DELETE s FROM ChatBot.NAP_SiteURL s LEFT JOIN tmp_NAP_SiteURL tmp ON s.SiteURLId = tmp.SiteURLId WHERE tmp.SiteURLId IS NULL;

    INSERT INTO ChatBot.NAP_SiteURL(SiteURLId
                            , SiteId
                            , URL
                            , Environment
                            , IsDefault
                            , IsActive)
    SELECT tmp.SiteURLId 
            , tmp.SiteId 
            , tmp.URL
            , tmp.Environment
            , tmp.IsDefault
            , tmp.IsActive
    FROM tmp_NAP_SiteURL tmp
    LEFT JOIN ChatBot.NAP_SiteURL s ON tmp.SiteURLId = s.SiteURLId
    WHERE s.SiteURLId IS NULL;

    UPDATE ChatBot.NAP_SiteURL s
    INNER JOIN tmp_NAP_SiteURL tmp ON tmp.SiteURLId = s.SiteURLId
    SET s.SiteURLId = tmp.SiteURLId
        ,s.SiteId = tmp.SiteId
        ,s.URL = tmp.URL
        ,s.Environment = tmp.Environment
        ,s.IsDefault = tmp.IsDefault
        ,s.IsActive = tmp.IsActive;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.testNapUserControl_SiteURL TO 'ncb.admin'@'%';