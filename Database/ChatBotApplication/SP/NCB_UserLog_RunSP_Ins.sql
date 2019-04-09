DELIMITER //

DROP PROCEDURE IF EXISTS NCB_UserLog_RunSP_Ins;

CREATE PROCEDURE  NCB_UserLog_RunSP_Ins(
	IN actionID SMALLINT
	, IN userID INT
    , IN logContent TEXT
    , IN logTime DATETIME(3)
    , IN duration INT
    /*
		Creator: Dominic@20190401
        Param:
				- ActionID: ActionID for SP
				- Duration: SP execution duration in miliseconds
                - UserID: Who run SP
        Revisions:
				- 20190401@Dominic: First Commit
		
    */
)
BEGIN
	INSERT INTO UserLog_RunSP(LogTime, ActionID, UserID, LogContent, Duration)
    SELECT logTime, actionID, userID, logContent, duration;
END;