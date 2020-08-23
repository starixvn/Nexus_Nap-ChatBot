/*
	Created: 20200823@Percy
    DB: ChatBot
    Task: Manually update MaxMsgID setting due to issue from Special Delete Site
*/

USE ChatBot;

UPDATE SystemSetting
SET Value = '989101'
WHERE ID = 3;