/*
	Creator: Dominic@20200109
    Task: Update config , set IsTest = 0  - Redmine: #126864
    
*/

UPDATE SystemSetting
SET Value = 
'[
  {"MsgContentsType":101, "NotificationTypeID":9, "NotificationTemplateID":23, "IsTest":0}
  ,  {"MsgContentsType":104, "NotificationTypeID":9, "NotificationTemplateID":23, "IsTest":0}
  ,  {"MsgContentsType":105, "NotificationTypeID":9, "NotificationTemplateID":23, "IsTest":0}
  ,  {"MsgContentsType":106, "NotificationTypeID":9, "NotificationTemplateID":23, "IsTest":0}
  ,  {"MsgContentsType":199, "NotificationTypeID":9, "NotificationTemplateID":23, "IsTest":0}
  ,  {"MsgContentsType":102, "NotificationTypeID":10, "NotificationTemplateID":27, "IsTest":0}
  ,  {"MsgContentsType":103, "NotificationTypeID":11, "NotificationTemplateID":26, "IsTest":0}
  ,  {"MsgContentsType":107, "NotificationTypeID":12, "NotificationTemplateID":24, "IsTest":0}
  ,  {"MsgContentsType":108, "NotificationTypeID":13, "NotificationTemplateID":25, "IsTest":0}
]'
WHERE ID = 4;