/*<info serverAlias="DBMSG-bodb_Message" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/

/*
	Created: 20211010@malcolm
	
*/

DECLARE @Environment VARCHAR(50) = 'UAT'

INSERT INTO MsgChatBot (MessageId, TargetId, JobIdentify, CheckInTime, Environment, IsFinished, RequestId)
SELECT DISTINCT mt.MessageId, mt.TargetId, 3, GETDATE(), 'UAT', 1, 0
FROM Msg m WITH(NOLOCK)
	INNER JOIN MsgTarget mt WITH(NOLOCK) ON mt.MessageId = m.MessageId
WHERE m.PostTime >= GETDATE() - 1 AND m.MsgType = 3 AND m.IsBatch = 0
	AND NOT EXISTS(
		SELECT 1
		FROM MsgChatBot mb WITH(NOLOCK)
		WHERE mb.MessageId = mt.MessageId AND mb.TargetId = mt.MsgTargetId AND RequestId = 0 AND JobIdentify = 3 AND mb.Environment = @Environment
	)


INSERT INTO MsgChatBot (MessageId, TargetId, JobIdentify, CheckInTime, Environment, IsFinished, RequestId)
SELECT DISTINCT mt.MessageId, mt.TargetId, 1, GETDATE(), 'UAT', 1, 0
FROM Msg m WITH(NOLOCK)
	INNER JOIN MsgTarget mt WITH(NOLOCK) ON mt.MessageId = m.MessageId
WHERE m.PostTime >= GETDATE() - 1 AND m.MsgType = 1 AND m.IsBatch = 1 AND m.isDeleted = 0
AND NOT EXISTS(
		SELECT 1
		FROM MsgChatBot mb WITH(NOLOCK)
		WHERE mb.MessageId = mt.MessageId AND mb.TargetId = mt.MsgTargetId AND RequestId = 0 AND JobIdentify = 1 AND mb.Environment = @Environment
	)

DECLARE @M TABLE (MessageId INT, RequestId INT)
insert into @M
VALUES
(1380214,	59103)
,(1380223,	59104)
,(1380224,	59105)
,(1380325,	59106)
,(1380507,	59108)
,(1380655,	59109)
,(1380655,	59110)
,(1380765,	59113)
,(1380787,	59116)
,(1381063,	59122)
,(1381068,	59115)
,(1381162,	59128)
,(1381164,	59129)



INSERT INTO MsgChatBot (MessageId, TargetId, JobIdentify, CheckInTime, Environment, IsFinished, RequestId)
SELECT DISTINCT m.MessageId, 0, 2, GETDATE(), 'UAT',1, m.RequestId
FROM @M m
WHERE NOT EXISTS(
	SELECT 1
	FROM MsgChatBot mb WITH(NOLOCK)
	WHERE mb.MessageId = m.MessageId AND mb.TargetId = 0 AND mb.RequestId = m.RequestId AND mb.JobIdentify = 2 AND mb.Environment = @Environment
)

