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
	DROP TEMPORARY TABLE IF EXISTS tmp_recipientLst;
	CREATE TEMPORARY TABLE tmp_recipientLst(
																	RecipientTypeID SMALLINT
																	, RecipientID INT
																	, RecipientName VARCHAR(200)
                                                                    , SourceID INT
																);
    
    #Get a list of mapped-status Recipients by NotificationTypeID
    INSERT INTO tmp_recipientLst(RecipientTypeID
														, RecipientID
                                                        , RecipientName
                                                        , SourceID
													)
    SELECT DISTINCT r.RecipientTypeID
					, r.RecipientID
                    , r.RecipientName
                    , r.SourceID
    FROM Recipient r
		INNER JOIN BotConversationRecipient bcr
				ON bcr.RecipientID = r.RecipientID
		INNER JOIN BotConversationExtraInfo bcri
				ON bcri.BotConversationID = bcr.BotConversationID
		INNER JOIN BotConversation con
				ON con.ID = bcr.BotConversationID
		INNER JOIN BotConversationMember bot
				ON bot.ConversationID = con.ConversationID
					AND bot.IsBot = 1
                    AND bot.IsActive = 1
    WHERE bcri.ChatGroupTypeID IS NOT NULL
		AND r.RecipientTypeID IN (
							SELECT nrt.RecipientTypeID 
							FROM NotificationRecipientType nrt 
							WHERE nrt.NotificationTypeID = NotificationTypeID
		);
    
    #Delete InActive Licensee
    DELETE main
	FROM tmp_recipientLst main
	INNER JOIN Licensee l
			ON l.LicenseeId = main.SourceID
				AND main.RecipientTypeID = 1 #RecipientType is Licensees
	WHERE  l.LicenseeStatusId <> 3;
                        
	#Filter Recipient list by an offered product
    IF ProductID IS NOT NULL THEN
		DELETE main
        FROM tmp_recipientLst main
        LEFT JOIN LicenseeProduct lp 
				ON lp.LicenseeID = main.SourceID 
					AND lp.ProductID = ProductID
                    AND main.RecipientTypeID = 1 #RecipientType is Licensees
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
    FROM tmp_recipientLst r
    ORDER BY RecipientTypeID, RecipientName;
    
    #Drop Unused temporary tables
     DROP TEMPORARY TABLE IF EXISTS tmp_recipientLst;
    
END;