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
(1399358,	59478)
,(1399465,	59481)
,(1399465,	59482)
,(1399538,	59487)
,(1399538,	59488)
,(1399546,	59480)
,(1399553,	59491)
,(1399579,	59494)
,(1399604,	59495)
,(1399624,	59479)
,(1399632,	59483)
,(1399633,	59484)
,(1399652,	59501)



INSERT INTO MsgChatBot (MessageId, TargetId, JobIdentify, CheckInTime, Environment, IsFinished, RequestId)
SELECT DISTINCT m.MessageId, 0, 2, GETDATE(), 'UAT',1, m.RequestId
FROM @M m
WHERE NOT EXISTS(
	SELECT 1
	FROM MsgChatBot mb WITH(NOLOCK)
	WHERE mb.MessageId = m.MessageId AND mb.TargetId = 0 AND mb.RequestId = m.RequestId AND mb.JobIdentify = 2 AND mb.Environment = @Environment
)

