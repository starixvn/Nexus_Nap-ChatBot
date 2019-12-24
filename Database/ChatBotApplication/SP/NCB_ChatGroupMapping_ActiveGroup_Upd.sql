DROP PROCEDURE IF EXISTS NCB_ChatGroupMapping_ActiveGroup_Upd;
DELIMITER //

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE NCB_ChatGroupMapping_ActiveGroup_Upd (
	IN groupID INT,
    IN isActive BIT(1),
	OUT errCode SMALLINT,
	OUT errMsg VARCHAR(100)
/*
	Creator: Merlin.Le@20191212
	Params:
	Revisions:
		- Merlin.Le@20191212: Create -- Redmine ID: 125345
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
				INNER JOIN ConversationGroupBot conGroupBot ON conGroup.ConversationGroupID = conGroupBot.ConversationGroupID
			WHERE conGroupBot.ConversationGroupID = groupID
        )
	)
	THEN
		SET errCode = 101, errMsg = 'Invalid status mapping.';
	ELSE
		UPDATE ConversationGroup conGroup
		SET conGroup.IsActive = isActive, conGroup.LastUpdate = UTC_TIMESTAMP(3)
		WHERE conGroup.ConversationGroupID = groupID;
	END IF;
END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE NCB_ChatGroupMapping_ActiveGroup_Upd TO 'ncb.admin'@'%';