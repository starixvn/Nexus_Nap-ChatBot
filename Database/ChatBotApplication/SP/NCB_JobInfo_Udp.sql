/*<info serverAlias="DBCB-DBCB_DEV" databaseType="2" executers="ncb.admin" isFunction="0" isNested="0"></info>*/
DROP PROCEDURE IF EXISTS NCB_JobInfo_Udp;
DELIMITER //
CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_JobInfo_Udp`(
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
		SET Value = DATE_ADD(UTC_TIME(), INTERVAL -4 HOUR)
	WHERE s.ID IN (16, 17);
END;
//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE NCB_JobInfo_Udp TO 'ncb.admin'@'%';