DELIMITER //

DROP PROCEDURE IF EXISTS NCB_NewNotification_Recipient_Sel;

CREATE PROCEDURE  NCB_NewNotification_Recipient_Sel(
	IN NotificationTypeID SMALLINT
	,IN ProductID INT
    /*
		Creator: Dominic@20190401
        Param:
				- ProductID: Applied to filter licensees who are offered a specific product, NULL if not specified
        Revisions:
				- 20190401@Dominic: First Commit
        
    */
)
BEGIN
	DROP TEMPORARY TABLE IF EXISTS tmp_Recipient;
	CREATE TEMPORARY TABLE tmp_Recipient(
																	RecipientTypeID SMALLINT
																	, RecipientID INT
																	, RecipientName VARCHAR(200)
																);
    
    #Get a list of Recipients by NotificationTypeID
    INSERT INTO tmp_Recipient(RecipientTypeID
														, RecipientID
                                                        , RecipientName
													)
    SELECT r.RecipientTypeID
					, r.RecipientID
                    , r.RecipientName
    FROM Recipient r
    WHERE r.RecipientTypeID IN (
						SELECT nrt.RecipientTypeID 
                        FROM NotificationRecipientType nrt 
                        WHERE nrt.NotificationTypeID = NotificationTypeID
	);
                        
	#Filter Recipient list by an offered product
    IF ProductID IS NOT NULL THEN
		DELETE main
        FROM tmp_Recipient main
        INNER JOIN Recipient re 
				ON re.RecipientID = main.RecipientID 
					AND re.RecipientTypeID = 1 #RecipientType is Licensees
        LEFT JOIN LicenseeProduct lp 
				ON lp.LicenseeID = re.SourceID 
					AND lp.ProductID = ProductID
        WHERE lp.LaunchDate IS NULL
			OR (
						 lp.LaunchDate IS NOT NULL 
                        AND NOW() <  lp.LaunchDate
					)
			OR (
						lp.TerminationDate IS NOT NULL 
                        AND NOW()  > lp.TerminationDate
					);
    END IF;
    
    SELECT r.RecipientTypeID
					,CASE 
							WHEN r.RecipientTypeID =1 
									THEN 'All Licensees' 
							ELSE 'All 3rd Parties' 
					END  RecipientTypeName
					, r.RecipientID, r.RecipientName
    FROM tmp_Recipient r
    ORDER BY RecipientTypeID, RecipientName;
    
    #Drop Unused temporary tables
     DROP TEMPORARY TABLE IF EXISTS tmp_Recipient;
    
END;