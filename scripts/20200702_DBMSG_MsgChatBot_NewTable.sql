/*<info serverAlias="DBMSG-bodb_Message"></info>*/

/*
	Created: 20200702@Percy
	DB: DBMSG-bodb_Message
	Task: New table for ChatBot - History processed messages
*/

USE bodb_Message
GO

CREATE TABLE dbo.MsgChatBot
(
	MessageId			INT NOT NULL
	,TargetId			INT NOT NULL
	,JobIdentify		TINYINT NOT NULL
	,CheckInTime		DATETIME CONSTRAINT DF_MsgChatBot_CheckInTime DEFAULT (GETDATE())
	,Environment		VARCHAR(50) NOT NULL
	,IsFinished 		BIT CONSTRAINT DF_MsgChatBot_IsFinished DEFAULT (0)
	CONSTRAINT PK_MsgChatBot PRIMARY KEY
	(
		MessageId ASC
		,TargetId ASC
		,JobIdentify ASC
	) ON [DATA]
) ON [DATA]
GO