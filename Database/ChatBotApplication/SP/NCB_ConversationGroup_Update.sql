DROP PROCEDURE IF EXISTS NCB_ConversationGroup_Update;
DELIMITER //

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE NCB_ConversationGroup_Update (
	IN groupID INT,
    IN isActive BIT(1),
	OUT errCode SMALLINT,
	OUT errMsg VARCHAR(100)
/*
	Creator: Merlin.Le@20191212
	Params:
	Revisions:
		- Merlin.Le@20191212: Create
*/
)
SQL SECURITY DEFINER
BEGIN
    SET errCode = 0, errMsg = '';
	IF (
		NOT EXISTS (
			SELECT 1
			FROM ConversationGroupRecipient
			WHERE ConversationGroupID = groupID
		)
        AND EXISTS (
			SELECT 1
            FROM ConversationGroup conGroup
				LEFT JOIN ConversationGroupBot conGroupBot ON conGroup.ConversationGroupID = conGroupBot.ConversationGroupID
			WHERE conGroupBot.ConversationGroupID = groupID
        )
	)
	THEN
		SET errCode = 101, errMsg = 'Invalid status mapping.';
	ELSE
		UPDATE ConversationGroup
		SET IsActive = isActive, LastUpdate = NOW()
		WHERE ConversationGroupID = groupID;
	END IF;
END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE NCB_ConversationGroup_Update TO 'ncb.admin'@'%';