DELIMITER //

DROP PROCEDURE IF EXISTS NCB_UserLog_RunSP_Ins;

CREATE PROCEDURE  NCB_UserLog_RunSP_Ins(
	IN ActionID SMALLINT
	, IN UserID INT
    , IN LogContent TEXT
    , IN Duration INT
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
    SELECT NOW() LogTime, ActionID, UserID, LogContent, Duration;
END;

