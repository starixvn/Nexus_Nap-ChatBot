USE ChatBot;

/*
  Created: 20200219@Percy
  DB: ChatBot
  Task:
	1. New table for Internal Department
	2. Initialize data for Abnormal Bets
*/

# Change primary key NotificationSystemMsg
UPDATE NotificationSystemMsg
  SET NotificationID = 0
  WHERE NotificationID IS NULL;

ALTER TABLE NotificationSystemMsg
  MODIFY NotificationID INT(10) UNSIGNED NOT NULL;

ALTER TABLE NotificationSystemMsg
  DROP PRIMARY KEY,
  ADD PRIMARY KEY (MessageID, NotificationID);

# New Recipient Type: Internal Department
DROP TABLE IF EXISTS InternalDepartment;
CREATE TABLE IF NOT EXISTS InternalDepartment(
      InternalDepartmentID TINYINT UNSIGNED NOT NULL,
      InternalDepartmentName VARCHAR(30),
      PRIMARY KEY (InternalDepartmentID)
    );

INSERT INTO InternalDepartment (InternalDepartmentID, InternalDepartmentName)
  VALUES (1, 'Internal CSD');

INSERT INTO RecipientType (RecipientTypeID, RecipientTypeName)
  VALUES (3, 'Internal Department');

# New Recipient: Internal Licensee, Internal Support
INSERT INTO Recipient (RecipientID, RecipientName, RecipientTypeID, SourceID)
  VALUES (546, 'Internal Licensee', 3, 1), (547, 'Internal Support', 3, 1);

# Config Group Skype
INSERT INTO ConversationGroupExtraInfo (ConversationGroupID, ChatGroupTypeID)
  VALUES (221, 1);

INSERT INTO ConversationGroupRecipient (ConversationGroupID, RecipientID)
  VALUES (221, 546), (221, 547);

# New Notification Type
INSERT INTO NotificationType (NotificationTypeID, NotificationTypeName, SystemID)
  VALUES (14, '[Internal CSD] Abnormal Bet Announcement', 2);

INSERT INTO NotificationChatGroupType (NotificationTypeID, ChatGroupTypeID)
  VALUES (14, 1);

INSERT INTO NotificationRecipientType (NotificationTypeID, RecipientTypeID)
  VALUES (14, 3);

# New Notification Template
INSERT INTO NotificationTemplate (NotificationTemplateID, NotificationTemplateName, NotificationTypeID, Title, Content, CreatorID, CreatedDate)
  VALUES (29, '[Internal CSD] Voiding Abnormal Bet Template', 14, '[Internal CSD] Voiding Abnormal Bet Template', '{General Info}', 8, UTC_TIMESTAMP());

# Job Setting
INSERT INTO SystemSetting (ID, Description, Value)
  VALUES
    (6, 'Special Delete Site - Abnormal Bets - Job Sync Status (Running/Done)', 'Done')
    ,(7, 'Special Delete Site - Abnormal Bets - Batch Size', '10')
    ,(8, 'Special Delete Site - Abnormal Bets - Previous MaxMessageID', '923517')
    ,(9, 'Special Delete Site - Abnormal Bets - Notification Template ID / MsgContentsType / NotificationType', '[
   {"MsgContentsType":103, "NotificationTypeID":14, "NotificationTemplateID":29, "IsTest":1}
 ]')
    ,(10, 'Special Delete Site - Abnormal Bets - Chat group ID which receives carbon copies of all imported messages', '222');