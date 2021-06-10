/*<info serverAlias="DBMSG-bodb_Message"></info>*/
/*
	Created: 20210610@Percy
	DB: DBMSG-bodb_Message
	Task: Manually update data for Chatbot
*/
use bodb_Message
go

-- 5 rows
update dbo.MsgChatBot with (rowlock)
set IsFinished = 1
where Environment = 'Production' and IsFinished = 0 and JobIdentify = 2
	and CheckInTime >= '2021-06-10'
