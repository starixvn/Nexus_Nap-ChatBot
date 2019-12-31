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
  {"MsgContentsType":101, "NotificationTypeID":15, "NotificationTemplateID":14, "IsTest":1}
  ,  {"MsgContentsType":104, "NotificationTypeID":15, "NotificationTemplateID":14, "IsTest":1}
  ,  {"MsgContentsType":105, "NotificationTypeID":15, "NotificationTemplateID":14, "IsTest":1}
  ,  {"MsgContentsType":106, "NotificationTypeID":15, "NotificationTemplateID":14, "IsTest":1}
  ,  {"MsgContentsType":199, "NotificationTypeID":15, "NotificationTemplateID":14, "IsTest":1}
  ,  {"MsgContentsType":102, "NotificationTypeID":16, "NotificationTemplateID":15, "IsTest":1}
  ,  {"MsgContentsType":103, "NotificationTypeID":17, "NotificationTemplateID":16, "IsTest":1}
  ,  {"MsgContentsType":107, "NotificationTypeID":18, "NotificationTemplateID":17, "IsTest":1}
  ,  {"MsgContentsType":108, "NotificationTypeID":19, "NotificationTemplateID":18, "IsTest":1}
]')
, (5, 'Special Delete Site - Chat group ID which receives carbon copies of all imported messages',206
)