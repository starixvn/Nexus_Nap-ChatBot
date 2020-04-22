DROP PROCEDURE IF EXISTS NCB_SettingConfiguration_NotificationTypeData_Sel;

DELIMITER //

CREATE DEFINER=`ChatBotSystem`@`%` PROCEDURE `NCB_SettingConfiguration_NotificationTypeData_Sel`(
	IN notificationTypeId INT
    /*
		Creator: 20200423@Merlin

        Revisions:
            - 20200423@Merlin: Get setting type - RedmineID: #132039
    */
)
SQL SECURITY DEFINER
BEGIN
	DECLARE systemId			INT;
    DECLARE systemName			VARCHAR (250);
    DECLARE recipientTypeId		INT;
    DECLARE recipientTypeName	VARCHAR (250);
    
    SELECT cs.SystemID INTO systemId
    FROM NotificationType nt
    INNER JOIN ChatBot.System cs ON cs.SystemID = nt.SystemID
	WHERE nt.NotificationTypeID = notificationTypeId;
    
    SELECT cs.SystemName INTO systemName
    FROM NotificationType nt
    INNER JOIN ChatBot.System cs ON cs.SystemID = nt.SystemID
	WHERE nt.NotificationTypeID = notificationTypeId;
        
	SELECT rt.RecipientTypeID INTO recipientTypeId
    FROM NotificationRecipientType nrt
    INNER JOIN RecipientType rt ON rt.RecipientTypeID = nrt.RecipientTypeID
    WHERE nrt.NotificationTypeID = notificationTypeId;
        
	SELECT rt.RecipientTypeName INTO recipientTypeName
    FROM NotificationRecipientType nrt
    INNER JOIN RecipientType rt ON rt.RecipientTypeID = nrt.RecipientTypeID
    WHERE nrt.NotificationTypeID = notificationTypeId;
    
    SELECT 
	    systemId AS SystemId
        ,systemName AS SystemName
        ,recipientTypeId AS RecipientTypeId
        ,recipientTypeName AS RecipientTypeName;
END;

//
DELIMITER ;

GRANT EXECUTE ON PROCEDURE NCB_SettingConfiguration_NotificationTypeData_Sel TO 'ncb.admin'@'%';