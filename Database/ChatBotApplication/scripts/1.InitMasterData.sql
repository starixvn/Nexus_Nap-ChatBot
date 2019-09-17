INSERT INTO ChatBot.System(SystemID, SystemName) 
VALUES 
(1, 'I, Bot Site')
, (2, 'Special Delete Site');

INSERT INTO ChatBot.RecipientType(RecipientTypeID, RecipientTypeName) 
VALUES 
(1, 'Licensee')
, (2, '3rd Party Product');

INSERT INTO ChatBot.ChatGroupType(ChatGroupTypeName) 
VALUES 
('Normal Group')
, ('Extra Group');

INSERT INTO ChatBot.NotificationType(NotificationTypeName, SystemID)
VALUES 
('System UM', 1)
, ('3rd Party Product UM', 1);

INSERT INTO NotificationRecipientType(NotificationTypeID, RecipientTypeID)
VALUES
(1,1)
,(1,2)
,(2,1);

INSERT INTO NotificationChatGroupType(NotificationTypeID, ChatGroupTypeID)
VALUES
(1,1)
,(1,2)
,(2,1)
,(2,2);