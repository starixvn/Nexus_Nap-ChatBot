/*
	Created: 20200603@Percy
	DB: CHATBOT
	Task: Manually update MaxMessageId for Abnormal Bets
*/

USE ChatBot;

UPDATE SystemSetting
SET Value = '947251'
WHERE ID = 8;