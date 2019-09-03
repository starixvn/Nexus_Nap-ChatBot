DELIMITER //

DROP PROCEDURE IF EXISTS ChatBot.testNapUserControl_OnUser;

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  ChatBot.testNapUserControl_OnUser(
	IN OnUser JSON
	
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

	 DROP TEMPORARY TABLE IF EXISTS tmp_NAP_OnUser;
	CREATE TEMPORARY TABLE tmp_NAP_OnUser(
                            Id INT
                            , UserId INT
                            , SiteId INT
                            , ReloadTime DATETIME
                            , IsKickedOut TINYINT
                            , IP VARCHAR(100)
                            , NetSessionId VARCHAR(200)
                        );	
    
    SET iLoop = 0;
	WHILE (JSON_EXTRACT(OnUser, CONCAT('$[',iLoop, '].Id'))  IS NOT NULL) DO
		INSERT INTO tmp_NAP_OnUser(
                              Id
                            , UserId
                            , SiteId
                            , ReloadTime
                            , IsKickedOut
                            , IP
                            , NetSessionId
                                    )
		SELECT JSON_EXTRACT(OnUser, CONCAT('$[', iLoop, '].Id')) 
						, JSON_UNQUOTE(JSON_EXTRACT(OnUser, CONCAT('$[', iLoop, '].UserId')))
                        , JSON_UNQUOTE(JSON_EXTRACT(OnUser, CONCAT('$[', iLoop, '].SiteId')))
                        , JSON_UNQUOTE(JSON_EXTRACT(OnUser, CONCAT('$[', iLoop, '].ReloadTime')))
                        , JSON_EXTRACT(OnUser, CONCAT('$[', iLoop, '].IsKickedOut'))
                        , JSON_UNQUOTE(JSON_EXTRACT(OnUser, CONCAT('$[', iLoop, '].IP')))
                        , JSON_UNQUOTE(JSON_EXTRACT(OnUser, CONCAT('$[', iLoop, '].NetSessionId')));
		SET iLoop = iLoop +1;
	END WHILE;

    DELETE ou FROM ChatBot.NAP_OnUser ou LEFT JOIN tmp_NAP_OnUser tmp ON ou.Id = tmp.Id WHERE tmp.Id IS NULL;

    INSERT INTO ChatBot.NAP_OnUser(Id
                            , UserId
                            , SiteId
                            , ReloadTime
                            , IsKickedOut
                            , IP
                            , NetSessionId
                                )
    SELECT tmp.Id 
            , tmp.UserId 
            , tmp.SiteId
            , tmp.ReloadTime
            , tmp.IsKickedOut
            , tmp.IP
            , tmp.NetSessionId
    FROM tmp_NAP_OnUser tmp
    LEFT JOIN ChatBot.NAP_OnUser ou ON tmp.Id = ou.Id
    WHERE ou.Id IS NULL;

    UPDATE ChatBot.NAP_OnUser ou
    INNER JOIN tmp_NAP_OnUser tmp ON tmp.Id = g.Id
    SET ou.Id = tmp.Id
        ,ou.UserId = tmp.UserId
        ,ou.SiteId = tmp.SiteId
        ,ou.ReloadTime = tmp.ReloadTime
        ,ou.IsKickedOut = tmp.IsKickedOut
        ,ou.IP = tmp.IP
        ,ou.NetSessionId = tmp.NetSessionId;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.testNapUserControl_OnUser TO 'ncb.admin'@'%';