/*
	Created: 20200214@Percy
	DB: DBCHATBOT.ChatBot
	Task: Change primary key of NotificationSystemMsg table: from (MessageID) to (MessageID, NotificationID)
*/
USE ChatBot;

UPDATE NotificationSystemMsg
  SET NotificationID = 0
  WHERE NotificationID IS NULL;

ALTER TABLE NotificationSystemMsg
  MODIFY NotificationID INT(10) UNSIGNED NOT NULL;

ALTER TABLE NotificationSystemMsg
  DROP PRIMARY KEY,
  ADD PRIMARY KEY (MessageID, NotificationID);