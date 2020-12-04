use ChatBot;

SELECT cg.Name AS GroupName, l.LicenseeName
FROM ConversationGroup cg
	INNER JOIN ConversationGroupRecipient cgr ON cg.ConversationGroupID = cgr.ConversationGroupID
    INNER JOIN Recipient r ON cgr.RecipientID = r.RecipientID
    INNER JOIN Licensee l ON r.SourceID = l.LicenseeID
WHERE cg.ConversationID LIKE '%@p2p%' -- name skype
	AND cg.IsActive = 1
    AND r.RecipientTypeID = 1 -- Licensees
    AND l.LicenseeStatusId = 3; -- Active licensees