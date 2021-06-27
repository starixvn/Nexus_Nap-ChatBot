ALTER TABLE Notification
ADD COLUMN ChannelID TINYINT UNSIGNED NOT NULL;

ALTER TABLE ConversationBot
ADD COLUMN ChannelID TINYINT UNSIGNED NOT NULL;

/*CREATE TABLE ChatBot.NotificationTemplateContent
(
	NotificationTemplateID INT UNSIGNED NOT NULL,
    Content mediumtext CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_ci',
    ChannelID TINYINT UNSIGNED NOT NULL
);*/

CREATE TABLE ChatBot.ConversationChannel
(
	ChannelID TINYINT  UNSIGNED  NOT NULL,
    ChannelName VARCHAR(45)	CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_ci' NULL,
    CONSTRAINT PK_ChannelID PRIMARY KEY (ChannelID)
);

INSERT INTO ConversationChannel (ChannelID, ChannelName)
VALUES (1, 'Skype'), (2, 'Telegram');

UPDATE ConversationGroup 
SET ChannelID = 1;

ALTER TABLE ConversationGroup
MODIFY  ChannelID TINYINT UNSIGNED NOT NULL;

UPDATE Notification
SET ChannelID = 1;

UPDATE ConversationBot
SET ChannelID = 1;


