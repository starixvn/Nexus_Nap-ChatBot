DROP PROCEDURE IF EXISTS NCB_SettingConfiguration_JobInfo_Sel;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_SettingConfiguration_JobInfo_Sel`(
    IN      jobId       INT
    /*
		Creator: 20200417@Merlin

        Revisions:
            - 20200417@Merlin: Get job info - RedmineID: #132039
			- 20210513@Malcolm: Integrate Telegram [RedmineID: #151807]
    */
)
SQL SECURITY DEFINER
BEGIN
    DECLARE jobStatus       VARCHAR (10);
    DECLARE jobBatchSize    INT;
    DECLARE maxMessageId    INT;
    DECLARE testingGroup    INT;
	DECLARE testingGrouplst TEXT;

    SELECT ss.Value INTO jobStatus
    FROM SystemSetting ss
    WHERE ss.JobIdentify = jobId
        AND ss.Description LIKE '%Job Sync Status%';

    SELECT ss.Value INTO jobBatchSize
    FROM SystemSetting ss
    WHERE ss.JobIdentify = jobId
        AND ss.Description LIKE '%Batch Size%';

    SELECT ss.Value INTO maxMessageId
    FROM SystemSetting ss
    WHERE ss.JobIdentify = jobId
        AND ss.Description LIKE '%Previous MaxMessageID%';

    SELECT ss.Value INTO testingGrouplst
    FROM SystemSetting ss
    WHERE ss.JobIdentify = jobId
        AND ss.Description LIKE '%Chat group ID%';
		
		
	DROP TEMPORARY TABLE IF EXISTS tmpGroupId;
	CREATE TEMPORARY TABLE tmpGroupId(ConversationGroupID INT UNSIGNED);
    
    IF IFNULL(testingGrouplst, '') <> '' THEN
		SET @SqlCommand = REPLACE(testingGrouplst, ',', '''),(''');
		SET @SqlCommand = CONCAT('(''', @SqlCommand, ''')');
		SET @SqlCommand= CONCAT('INSERT INTO tmpGroupId VALUES', @SqlCommand); 
		
		PREPARE Stmt FROM  @SqlCommand;
		EXECUTE Stmt;
		DEALLOCATE PREPARE Stmt;
	END IF;
		
	SELECT t.ConversationGroupID INTO testingGroup
	FROM tmpGroupId t
		INNER JOIN ConversationGroup g ON g.ConversationGroupID = t.ConversationGroupID
		INNER JOIN ConversationChannel cc ON cc.ChannelId = g.ChannelId
	WHERE cc.ChannelName = 'Skype';

    SELECT
        jobId           AS  Id
        ,jobStatus      AS  Status
        ,jobBatchSize   AS  BatchSize
        ,maxMessageId   AS  PreviousMaxMessageId
        ,testingGroup   AS  TestingGroup;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_SettingConfiguration_JobInfo_Sel TO 'ncb.admin'@'%';