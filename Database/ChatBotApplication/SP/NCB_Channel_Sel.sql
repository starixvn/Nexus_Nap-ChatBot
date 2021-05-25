DROP PROCEDURE IF EXISTS NCB_Channel_Sel;
DELIMITER //

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE NCB_Channel_Sel (
	IN Id TINYINT 
    ,IN Name VARCHAR(45) 
)
SQL SECURITY DEFINER
BEGIN
	SELECT c.ChannelId, c.ChannelName
    FROM ConversationChannel c
    WHERE (`ChannelId` = Id OR Id IS NULL)
		AND (`ChannelName` = Name OR Name IS NULL);
END
//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE NCB_Channel_Sel TO 'ncb.admin'@'%';