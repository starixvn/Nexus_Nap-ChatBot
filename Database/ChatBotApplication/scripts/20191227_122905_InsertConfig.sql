/*
	Creator: Dominic@20191227
    Task: Insert system config - Redmine: #122905
    
*/
INSERT INTO ChatBot.SystemSetting(ID, Description, Value)
VALUES (1, 'Special Delete Site - Job Sync Status (Running/Done)','Done')
, (2, 'Special Delete Site - Batch Size','10')
, (3, 'Special Delete Site - Previous MaxMessageID','910285')
, (4, 'Special Delete Site - Notification Template ID / MsgContentsType / NotificationType'
,'[
  {"MsgContentsType":101, "NotificationTypeID":9, "NotificationTemplateID":23, "IsTest":1}
  ,  {"MsgContentsType":104, "NotificationTypeID":9, "NotificationTemplateID":23, "IsTest":1}
  ,  {"MsgContentsType":105, "NotificationTypeID":9, "NotificationTemplateID":23, "IsTest":1}
  ,  {"MsgContentsType":106, "NotificationTypeID":9, "NotificationTemplateID":23, "IsTest":1}
  ,  {"MsgContentsType":199, "NotificationTypeID":9, "NotificationTemplateID":23, "IsTest":1}
  ,  {"MsgContentsType":102, "NotificationTypeID":10, "NotificationTemplateID":27, "IsTest":1}
  ,  {"MsgContentsType":103, "NotificationTypeID":11, "NotificationTemplateID":26, "IsTest":1}
  ,  {"MsgContentsType":107, "NotificationTypeID":12, "NotificationTemplateID":24, "IsTest":1}
  ,  {"MsgContentsType":108, "NotificationTypeID":13, "NotificationTemplateID":25, "IsTest":1}
]')
, (5, 'Special Delete Site - Chat group ID which receives carbon copies of all imported messages',206
)