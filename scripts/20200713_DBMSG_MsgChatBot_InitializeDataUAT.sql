/*<info serverAlias="DBMSG-bodb_Message"></info>*/
/*
	Created: 20200713@Percy
	DB: DBMSG-bodb_Message
	Task: Initialize data for UAT testing
*/

USE bodb_Message
GO

INSERT INTO dbo.MsgChatBot (MessageId,TargetId,JobIdentify,CheckInTime,Environment,IsFinished)
SELECT DISTINCT MessageId,0,2,GETDATE(),'UAT',1
FROM DBACC.bodb02.dbo.TicketVoiding_VoidingHistory vh WITH (NOLOCK)
WHERE vh.CreatedDate >= GETDATE() - 1
	AND vh.StatusId = 2 -- tickets void successfully
ORDER BY MessageId