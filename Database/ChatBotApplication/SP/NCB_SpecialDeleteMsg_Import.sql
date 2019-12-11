DROP PROCEDURE IF EXISTS NCB_SpecialDeleteMsg_Import;
DELIMITER //

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE  NCB_SpecialDeleteMsg_Import(
	IN messageHeader JSON,
	IN messageDetail JSON,
    OUT errCode INT,
    OUT errMsg VARCHAR(200)

    /*
		Creator: Dominic@20191211
        Task: Chatbot For Licensee - Phase 2: Integrate Special Delete - Redmine:#122905
        Param:

        Revisions:
				- 20191211@Dominic: First Commit
        
    */
)
SQL SECURITY DEFINER
BEGIN
    # Variables
	DECLARE iLoop INT;

    #Constants

    DECLARE RECIPIENT_TYPE_LICENSEE INT DEFAULT 1;
    DECLARE RECIPIENT_TYPE_PRODUCT INT DEFAULT 2;
    DECLARE LICENSEE_SOURCE_DESCRIPTION VARCHAR(50) DEFAULT 'bodb_OIC.dbo.Licensee';
    DECLARE PRODUCT_SOURCE_DESCRIPTION VARCHAR(50) DEFAULT 'bodb_OIC.dbo.Product';



############################ Licensee ###########################################

    DROP TEMPORARY TABLE IF EXISTS tmp_Licensee;
    CREATE TEMPORARY TABLE IF NOT EXISTS tmp_Licensee(
            LicenseeId INT, 
            LicenseeName VARCHAR(500), 
            RegisteredLicenseeName VARCHAR(500), 
            LicenseeStatusId INT, 
            LicenseeTypeId INT
            );
    
    SET iLoop = 0;
    WHILE (JSON_EXTRACT(Licensees, CONCAT('$[',iLoop, '].LicenseeId'))  IS NOT NULL) DO 
		INSERT INTO tmp_Licensee(LicenseeId, LicenseeName, RegisteredLicenseeName, LicenseeStatusId, LicenseeTypeId)
        VALUES (
			JSON_UNQUOTE(JSON_EXTRACT(Licensees, CONCAT('$[', iLoop, '].LicenseeId'))), 
            JSON_UNQUOTE(JSON_EXTRACT(Licensees, CONCAT('$[', iLoop, '].LicenseeName'))),
            JSON_UNQUOTE(JSON_EXTRACT(Licensees, CONCAT('$[', iLoop, '].RegisteredLicenseeName'))),
            JSON_UNQUOTE(JSON_EXTRACT(Licensees, CONCAT('$[', iLoop, '].LicenseeStatusId'))),
            JSON_UNQUOTE(JSON_EXTRACT(Licensees, CONCAT('$[', iLoop, '].LicenseeTypeId')))
		);
        
        SET iLoop := iLoop + 1;
    END WHILE;    
    
    DELETE l FROM Licensee l LEFT JOIN tmp_Licensee tmp ON l.LicenseeId = tmp.LicenseeId WHERE tmp.LicenseeId IS NULL;

    INSERT INTO  Licensee(LicenseeId, LicenseeName, RegisteredLicenseeName, LicenseeStatusId, LicenseeTypeId)    
    SELECT LicenseeId, LicenseeName, RegisteredLicenseeName, LicenseeStatusId, LicenseeTypeId
    FROM tmp_Licensee
    ON DUPLICATE KEY UPDATE 
		LicenseeId = values(LicenseeId), 
        LicenseeName = values(LicenseeName), 
        RegisteredLicenseeName = values(RegisteredLicenseeName), 
        LicenseeStatusId = values(LicenseeStatusId), 
        LicenseeTypeId = values(LicenseeTypeId);


END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE NCB_SpecialDeleteMsg_Import TO 'ncb.admin'@'%';