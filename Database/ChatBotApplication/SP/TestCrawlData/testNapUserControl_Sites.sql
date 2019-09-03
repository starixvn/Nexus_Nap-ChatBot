DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.testNapUserControl_Sites;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.testNapUserControl_Sites(
	IN Sites JSON
	
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

	DROP TEMPORARY TABLE IF EXISTS tmp_NAP_Sites;
	CREATE TEMPORARY TABLE tmp_NAP_Sites(
                            SiteId INT
                            , SiteName VARCHAR(100)
                            , Description VARCHAR(200)
                            , URL VARCHAR(200)
                            , TestURL VARCHAR(200)
                            , IsActive TINYINT
                            , Environment VARCHAR(50)
                            , SecretAccessKey VARCHAR(200)
                            , SiteKey VARCHAR(100)
                        );
    
    SET iLoop = 0;
	WHILE (JSON_EXTRACT(Sites, CONCAT('$[',iLoop, '].SiteId'))  IS NOT NULL) DO
		INSERT INTO tmp_NAP_Sites(
                              SiteId 
                            , SiteName
                            , Description
                            , URL
                            , TestURL
                            , IsActive
                            , Environment
                            , SecretAccessKey
                            , SiteKey
                                    )
		SELECT JSON_EXTRACT(Sites, CONCAT('$[', iLoop, '].SiteId')) 
						, JSON_UNQUOTE(JSON_EXTRACT(Sites, CONCAT('$[', iLoop, '].SiteName')))
                        , CASE 
                            WHEN JSON_UNQUOTE(JSON_EXTRACT(Sites, CONCAT('$[', iLoop, '].Description'))) = 'null'
                            THEN NULL
                            ELSE JSON_UNQUOTE(JSON_EXTRACT(Sites, CONCAT('$[', iLoop, '].Description')))
                          END
                        , CASE 
                            WHEN JSON_UNQUOTE(JSON_EXTRACT(Sites, CONCAT('$[', iLoop, '].URL'))) = 'null'
                            THEN NULL
                            ELSE JSON_UNQUOTE(JSON_EXTRACT(Sites, CONCAT('$[', iLoop, '].URL')))
                          END
                        , CASE 
                            WHEN JSON_UNQUOTE(JSON_EXTRACT(Sites, CONCAT('$[', iLoop, '].TestURL'))) = 'null'
                            THEN NULL
                            ELSE JSON_UNQUOTE(JSON_EXTRACT(Sites, CONCAT('$[', iLoop, '].TestURL')))
                          END
                        , JSON_EXTRACT(Sites, CONCAT('$[', iLoop, '].IsActive'))
                        , JSON_UNQUOTE(JSON_EXTRACT(Sites, CONCAT('$[', iLoop, '].Environment')))
                        , CASE 
                            WHEN JSON_UNQUOTE(JSON_EXTRACT(Sites, CONCAT('$[', iLoop, '].SecretAccessKey'))) = 'null'
                            THEN NULL
                            ELSE JSON_UNQUOTE(JSON_EXTRACT(Sites, CONCAT('$[', iLoop, '].SecretAccessKey')))
                          END                            
                        , CASE
                            WHEN JSON_UNQUOTE(JSON_EXTRACT(Sites, CONCAT('$[', iLoop, '].SiteKey'))) = 'null'
                            THEN NULL
                            ELSE JSON_UNQUOTE(JSON_EXTRACT(Sites, CONCAT('$[', iLoop, '].SiteKey')))
                          END;

		SET iLoop = iLoop +1;
	END WHILE;

    DELETE s FROM ChatBot.NAP_Sites s LEFT JOIN tmp_NAP_Sites tmp ON s.SiteId = tmp.SiteId WHERE tmp.SiteId IS NULL;

    INSERT INTO ChatBot.NAP_Sites(SiteId 
                            , SiteName
                            , Description
                            , URL
                            , TestURL
                            , IsActive
                            , Environment
                            , SecretAccessKey
                            , SiteKey)
    SELECT tmp.SiteId 
            , tmp.SiteName 
            , tmp.Description
            , tmp.URL
            , tmp.TestURL
            , tmp.IsActive
            , tmp.Environment
            , tmp.SecretAccessKey
            , tmp.SiteKey
    FROM tmp_NAP_Sites tmp
    LEFT JOIN ChatBot.NAP_Sites s ON tmp.SiteId = s.SiteId
    WHERE s.SiteId IS NULL;

    UPDATE ChatBot.NAP_Sites s
    INNER JOIN tmp_NAP_Sites tmp ON tmp.SiteId = s.SiteId
    SET s.SiteId = tmp.SiteId
        ,s.SiteName = tmp.SiteName
        ,s.Description = tmp.Description
        ,s.URL = tmp.URL
        ,s.TestURL = tmp.TestURL
        ,s.IsActive = tmp.IsActive
        ,s.Environment = tmp.Environment
        ,s.SecretAccessKey = tmp.SecretAccessKey
        ,s.SiteKey = tmp.SiteKey;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.testNapUserControl_Sites TO 'ncb.admin'@'%';