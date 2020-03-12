USE ChatBot;

/*
  Created: 20200305@Percy
  DB: ChatBot
  Task: Initialize data for Site Message
*/

# New table
DROP TABLE IF EXISTS MatchInfo;
CREATE TABLE IF NOT EXISTS MatchInfo(
  MatchID INT NOT NULL,
  MsgContentsType INT NOT NULL,
  PRIMARY KEY (MatchID, MsgContentsType)
  );

# New System
INSERT INTO ChatBot.System (SystemID, SystemName)
  VALUES (3, 'Admin Site - Site Message');

# New Notification Type
INSERT INTO NotificationType (NotificationTypeID, NotificationTypeName, SystemID)
  VALUES (34, 'Removed Live Announcement', 3)
        ,(35, 'Rejected Match Announcement', 3)
        ,(36, 'Valid Bets Announcement', 3)
        ,(37, 'Refunded Match Announcement', 3)
        ,(38, 'Re-settled Match Announcement', 3)
        ,(39, 'Move Statement Announcement', 3);

INSERT INTO NotificationChatGroupType (NotificationTypeID, ChatGroupTypeID)
  VALUES (34, 1), (35, 1), (36, 1), (37, 1), (38, 1), (39, 1);

INSERT INTO NotificationRecipientType (NotificationTypeID, RecipientTypeID)
  VALUES (34, 1), (35, 1), (36, 1), (37, 1), (38, 1), (39, 1);

# New Notification Template
INSERT INTO NotificationTemplate (NotificationTemplateName, NotificationTypeID, Title, Content, CreatorID, CreatedDate)
  VALUES ('Remove Live Template', 34, 'Remove Live Template', 'Good Day!
This is 1on1 Customer Service. **Kindly take note of the below message [请注意以下公告]:

【赛事延迟派彩通知】
【Match Delay Payout Notification】**

{EN Announcement}

{CN Announcement}

Sport: {Sport EN CN} {Kick Off Lable}: {KO Time}
League: {League Name EN CN}
**Match: {Team Name EN CN}

>> Above match we have 【CANCEL LIVE】 due to related department confirming match details and will【LATE PAYOUT】. Should there be any update, will notify again. Thank you!
>> 以上赛事已【取消走地】并 【延迟结算】由于有关部门确认赛事细节。若有任何进展将再另行通知 。谢谢! **

Thank you for your kind attention ~', 8, UTC_TIMESTAMP())
        ,('Ticket Pending Rejected Template', 35, 'Ticket Pending Rejected Template', 'Good Day!
This is 1on1 Customer Service. **Kindly take note of the below message [请注意以下公告]: **

{EN Announcement}

{CN Announcement}

Thank you for your kind attention ~', 8, UTC_TIMESTAMP())
        ,('Rejected & Removed Live Template', 35, 'Rejected & Removed Live Template', 'Good Day!
This is 1on1 Customer Service. **Kindly take note of the below message [请注意以下公告]:

【赛事延迟派彩通知】
【Match Delay Payout Notification】**

{EN Announcement}

{CN Announcement}

Sport: {Sport EN CN} {Kick Off Lable}: {KO Time}
League: {League Name EN CN}
**Match: {Team Name EN CN}

>> Above match we have 【CANCEL LIVE】 due to related department confirming match details and will【LATE PAYOUT】. Should there be any update, will notify again. Thank you!
>> 以上赛事已【取消走地】并 【延迟结算】由于有关部门确认赛事细节。若有任何进展将再另行通知 。谢谢! **

Thank you for your kind attention ~', 8, UTC_TIMESTAMP())
        ,('Neutral Ground Template', 36, 'Neutral Ground Template', 'Good Day!
This is 1on1 Customer Service. **Kindly take note of the below message [请注意以下公告]: **

{EN Announcement}

{CN Announcement}

Thank you for your kind attention ~', 8, UTC_TIMESTAMP())
        ,('Refunded Match Template', 37, 'Refunded Match Template', 'Good Day!
This is 1on1 Customer Service. **Kindly take note of the below message [请注意以下公告]: **

{EN Announcement}

{CN Announcement}

Thank you for your kind attention ~', 8, UTC_TIMESTAMP())
        ,('Refunded Match - Done Payout Template', 37, 'Refunded Match - Done Payout Template', 'Good Day! 
This is 1on1 Customer Service. **Kindly take note of the below message [请注意以下公告]: **

{EN Announcement}

{CN Announcement}
**
>> The above match payout already. Thank you. 
>> 以上赛事已经结算了。谢谢! **

Thank you for your kind attention ~', 8, UTC_TIMESTAMP())
        ,('Special Bet Type Wrong Result', 38, 'Special Bet Type Wrong Result', '	Good Day!
This is 1on1 Customer Service. **Kindly take note of the below message [请注意以下公告]: **

{EN Announcement}

{CN Announcement}
**
>> Above match result has been changed. This might caused some customer bets will have negative balance shown in the account.
>> 以上赛事赛果已被更正。这将导致某些有下注于该盘口的客户帐号会出现负数。

>> Please be informed that we will not be sending any email notification with attached affected list from now onwards if there is any "Result Amended/Recalculation". You may access to the provided Internal Admin, click onto "Notification" - "Resettlement" on the left panel for the checking of matches'' affected list.
>> 请知悉，目前所有“重新结算”事项之受影响客户名单将不以邮件方式提供。贵司可透过后台系统登录并点击 “通知” - “重新结算” 查询赛事受影响客户名单。 **

Thank you for your kind attention ~', 8, UTC_TIMESTAMP())
        ,('Move Statement Template', 39, 'Move Statement Template', 'Good Day!
This is 1on1 Customer Service. **Kindly take note of the below message [请注意以下公告]:

【赛事延迟派彩通知】
【Match Delay Payout Notification】 **

{EN Announcement}

{CN Announcement}

Sport: {Sport EN CN} {Kick Off Lable}: {KO Time}
League: {League Name EN CN}
**Match: {Team Name EN CN}

>> Above match will [DELAY PAYOUT] due to related department confirming match details. Should there be any update, will notify again. Thank you!
>> 以上赛事将 【延迟结算】 由于有关部门确认赛事细节。若有任何进展将再另行通知 。谢谢! **

Thank you for your kind attention ~', 8, UTC_TIMESTAMP());

# Job Setting
INSERT INTO SystemSetting (ID, Description, Value)
  VALUES
    (11, 'Admin Site - Site Message - Job Sync Status (Running/Done)', 'Done')
    ,(12, 'Admin Site - Site Message - Batch Size', '10')
    ,(13, 'Admin Site - Site Message - Previous MaxMessageID', '923517')
    ,(14, 'Admin Site - Site Message - Notification Template ID / MsgContentsType / NotificationType', '[
{"MsgContentsType":1, "NotificationTypeID":34, "NotificationTemplateID":27, "IsTest":0}
, {"MsgContentsType":2, "NotificationTypeID":35, "NotificationTemplateID":28, "IsTest":0}
, {"MsgContentsType":3, "NotificationTypeID":35, "NotificationTemplateID":29, "IsTest":0}
, {"MsgContentsType":4, "NotificationTypeID":36, "NotificationTemplateID":30, "IsTest":0}
, {"MsgContentsType":5, "NotificationTypeID":37, "NotificationTemplateID":31, "IsTest":0}
, {"MsgContentsType":6, "NotificationTypeID":38, "NotificationTemplateID":33, "IsTest":0}
, {"MsgContentsType":7, "NotificationTypeID":39, "NotificationTemplateID":34, "IsTest":0}
, {"MsgContentsType":8, "NotificationTypeID":37, "NotificationTemplateID":32, "IsTest":0}
 ]')
    ,(15, 'Admin Site - Site Message - Chat group ID which receives carbon copies of all imported messages', '427');