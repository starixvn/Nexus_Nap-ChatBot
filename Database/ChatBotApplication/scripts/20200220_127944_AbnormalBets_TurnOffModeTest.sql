/*
  Created: 20200220@Percy
  DB: ChatBot
  Task: Turn off mode test for new job
*/
UPDATE SystemSetting
  SET Value = '[
   {"MsgContentsType":103, "NotificationTypeID":14, "NotificationTemplateID":29, "IsTest":0}
 ]'
  WHERE ID = 9;