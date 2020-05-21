/*
  Created: 20200521@Percy
  DB: CHATBOT (PRODUCTION)
  Task: Change BOT name
*/

USE ChatBot;

UPDATE ConversationBot
SET `Name` = 'SABASports'
WHERE ConversationBotID = 1;

UPDATE ConversationBot
SET `Name` = 'SABA'
WHERE ConversationBotID = 2;