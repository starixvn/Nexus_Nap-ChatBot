DELIMITER //

DROP PROCEDURE IF EXISTS NCB_UserLog_RunSP_Ins;

CREATE PROCEDURE  NCB_UserLog_RunSP_Ins(
	IN in_ActionID SMALLINT
	,IN in_UserID INT
    ,IN in_LogContent TEXT
    ,IN in_Duration INT
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
NCB_UserLog_RunSP_Ins:BEGIN
	INSERT INTO UserLog_RunSP(LogTime, ActionID, UserID, LogContent, Duration)
    SELECT NOW() LogTime, in_ActionID, in_UserID, in_LogContent, in_Duration;
END;

