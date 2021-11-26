DROP PROCEDURE IF EXISTS NCB_JobInfo_Upd;
DELIMITER //
CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_JobInfo_Upd`(
	IN JobInfo JSON
    /*
		Creator: 20211010@Malcolm
        Revisions:
			- 20211010@Malcolm: Get information of Job [RedmineID: #162825]
		Param:
				- @JobInfo
							[  
								{ "JobIdentify":1, "MessageId":"12" }
							]
    */
)
SQL SECURITY DEFINER
BEGIN
    DROP TEMPORARY TABLE IF EXISTS tmp_JobInfo;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmp_JobInfo
    (
		JobIdentify TINYINT, 
        MessageId   INT UNSIGNED
    );
    
    INSERT INTO tmp_JobInfo (JobIdentify, MessageId)
    SELECT t.JobIdentify, t.MessageId
    FROM 
		JSON_TABLE
        (
			JobInfo,
            "$[*]"
            COLUMNS
            (
				JobIdentify TINYINT PATH "$.JobIdentify", 
				MessageId INT UNSIGNED PATH "$.MessageId"
            )
        ) AS t;
        
    UPDATE SystemSetting AS s
	INNER JOIN tmp_JobInfo AS j ON j.JobIdentify = s.JobIdentify
		SET s.Value = j.MessageId
    WHERE s.ID IN (3, 8, 13);

    UPDATE SystemSetting AS s
	INNER JOIN tmp_JobInfo AS j ON j.JobIdentify = s.JobIdentify
		SET Value = DATE_ADD(UTC_TIMESTAMP(), INTERVAL -4 HOUR)
	WHERE s.ID IN (16, 17);
END;
//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE NCB_JobInfo_Upd TO 'ncb.admin'@'%';