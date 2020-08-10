/*<info serverAlias="DBMSG-bodb_Message"></info>*/

/*
	Created: 20200804@Percy
	DB: DBMSG.bodb_Message
	Task: Add RequestId into MsgChatBot for keep track Ticket Voiding Request
*/

USE bodb_Message
GO

ALTER TABLE dbo.MsgChatBot
	ADD RequestId INT NULL CONSTRAINT DF_MsgChatBot_RequestId DEFAULT ((0))
GO

WHILE 1 = 1
BEGIN
	UPDATE TOP (5000) dbo.MsgChatBot WITH (ROWLOCK)
	SET RequestId = 0
	WHERE RequestId IS NULL

	IF @@ROWCOUNT < 5000
		BREAK

	WAITFOR DELAY '00:00:01.280'
END
GO

ALTER TABLE dbo.MsgChatBot
	ALTER COLUMN RequestId INT NOT NULL
GO

ALTER TABLE dbo.MsgChatBot
	DROP CONSTRAINT PK_MsgChatBot
GO

ALTER TABLE dbo.MsgChatBot
	ADD CONSTRAINT PK_MsgChatBot PRIMARY KEY
	(
		MessageId ASC
		,TargetId ASC
		,RequestId ASC
		,JobIdentify ASC
	)
GO