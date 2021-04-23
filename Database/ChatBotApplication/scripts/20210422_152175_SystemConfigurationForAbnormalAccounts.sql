USE ChatBot;

/*
  Created: 20210422@Malcolm
  DB: ChatBot
  Task: Integrate Abnormal Account
*/

SELECT SystemID INTO @SystemId FROM ChatBot.System WHERE SystemName = 'Special Delete Site';

INSERT INTO NotificationType (NotificationTypeName, SystemID)
VALUES ('[Internal CSD] Abnormal Account Announcement', @SystemId);

SELECT NotificationTypeId INTO @NotificationTypeId FROM NotificationType WHERE NotificationTypeName = '[Internal CSD] Abnormal Account Announcement';

INSERT INTO NotificationTemplate (NotificationTemplateName, NotificationTypeID, Title, Content, CreatorID, CreatedDate)
VALUES ('[Internal CSD] Abnormal Account Template', @NotificationTypeId, '[Internal CSD] Voiding Abnormal Account Template', '{General Info}', 8, UTC_TIMESTAMP());

SELECT NotificationTemplateId INTO @NotificationTemplateId FROM NotificationTemplate WHERE NotificationTemplateName = '[Internal CSD] Abnormal Account Template';

SET @Description = CONCAT('[{"IsTest": 0, "MsgContentsType": 101, "NotificationTypeID": ', @NotificationTypeId,', "NotificationTemplateID": ', @NotificationTemplateID ,', "TemplateRequiredParams": "{General Info}"}',
					       ',{"IsTest": 0, "MsgContentsType": 102, "NotificationTypeID": ',@NotificationTypeId,', "NotificationTemplateID": ', @NotificationTemplateID,', "TemplateRequiredParams": "{General Info}"}]');

INSERT INTO SystemSetting (ID, Description, Value, JobIdentify, JobDescription)
VALUES (18, 'Special Delete Site - Abnormal Bets - Job Sync Status (Running/Done)', 'Done', 4 ,'Special Delete - Abnormal Account- Internal CSD')
,(19, 'Special Delete Site - Abnormal Account - Batch Size', 10, 4 ,'Special Delete - Abnormal Account- Internal CSD')
,(20, 'Special Delete Site - Abnormal Account - Notification Template ID / MsgContentsType / NotificationType', @Description, 4 ,'Special Delete - Abnormal Account- Internal CSD')
,(21, 'Special Delete Site - Abnormal Account - Chat group ID which receives carbon copies of all imported messages', 426, 4 ,'Special Delete - Abnormal Account- Internal CSD')
,(22, 'Special Delete Site - Abnormal Account - Previous MaxMessageID', 0, 4 ,'Special Delete - Abnormal Account- Internal CSD');

SELECT RecipientTypeID INTO @RecipientTypeID FROM RecipientType WHERE RecipientTypeName = 'Internal Department';

INSERT INTO NotificationRecipientType (NotificationTypeID, RecipientTypeID)
VALUES (@NotificationTypeId, @RecipientTypeID)






