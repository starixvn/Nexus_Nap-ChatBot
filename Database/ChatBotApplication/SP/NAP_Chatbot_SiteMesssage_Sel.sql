/*<info serverAlias="DBMSG-bodb_Message" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/
ALTER PROCEDURE [dbo].[NAP_Chatbot_SiteMesssage_Sel]
	@MessageID				INT
	,@LastPostTime			DATETIME
	,@BatchSize				INT
	,@MsgContentsTypeLst	VARCHAR(2000)
	,@Environment			VARCHAR(20) = ''
AS

/*
	Created: 20200305@Percy
	Task: ChatBot - Get Message from Site Message
	DB: DBMSG.bodb_Message  

	Revisions:
		- 20200305@Percy: Initialize SP [RedmineID: #126143]
		- 20200630@Percy: Add LastPostTime for rescan message [RedmineID: #136492]
*/

BEGIN
	SET NOCOUNT ON

	SELECT @LastPostTime = DATEADD(MINUTE, -30, @LastPostTime)

	CREATE TABLE #MsgContentsTypeLst(MsgContentsType TINYINT)
	CREATE UNIQUE CLUSTERED INDEX PK_MsgContentsType ON #MsgContentsTypeLst(MsgContentsType)

	DECLARE @XML XML  = NULL

	SET @XML = N'<root><r>' + REPLACE(@MsgContentsTypeLst, ',', '</r><r>') + '</r></root>'
	
	INSERT INTO #MsgContentsTypeLst(MsgContentsType)
	SELECT r.value('.', 'NVARCHAR(15)') AS MsgContentsType
	FROM @xml.nodes('//root/r') AS RECORDS(r)
	WHERE LEN(ISNULL(r.value('.', 'NVARCHAR(15)'), '')) > 0

	DECLARE @Msg TABLE (MessageId INT, MsgContentsType TINYINT, MatchId INT, Content_en NVARCHAR(4000), Content_cs NVARCHAR(4000), PostTime DATETIME, IsNew TINYINT)
	
	-- New messages from @MessageID
	INSERT INTO @Msg (MessageId, MsgContentsType, MatchId, Content_en, Content_cs, PostTime, IsNew)
	SELECT TOP (@BatchSize) m.MessageId, m.MsgContentsType, m.MatchId, m.Content_en, m.Content_cs, m.PostTime, 1
	FROM dbo.Msg m WITH (NOLOCK)
	WHERE m.MessageId > @MessageID
		AND m.msgtype = 3
		AND EXISTS (SELECT 1 FROM #MsgContentsTypeLst mct WHERE m.MsgContentsType = mct.MsgContentsType)
		AND m.isbatch = 0
		AND EXISTS (SELECT 1
					FROM dbo.MsgTarget mt WITH (NOLOCK)
						INNER JOIN dbo.MsgSite ms WITH (NOLOCK) ON mt.TargetId = ms.SiteId
						INNER JOIN bodb02.dbo.Site s WITH (NOLOCK) ON ms.SiteName = s.site AND s.isDeposit = 1
					WHERE mt.MessageId = m.MessageId)
	ORDER BY m.MessageId
	OPTION(RECOMPILE)

	-- Rescan messages from @LastPostTime
	INSERT INTO @Msg (MessageId, MsgContentsType, MatchId, Content_en, Content_cs, PostTime, IsNew)
	SELECT m.MessageId, m.MsgContentsType, m.MatchId, m.Content_en, m.Content_cs, m.PostTime, 0
	FROM dbo.Msg m WITH (NOLOCK)
	WHERE m.PostTime >= @LastPostTime
		AND m.msgtype = 3
		AND EXISTS (SELECT 1 FROM #MsgContentsTypeLst mct WHERE m.MsgContentsType = mct.MsgContentsType)
		AND m.isbatch = 0
		AND EXISTS (
			SELECT 1
			FROM dbo.MsgChatBot mc WITH (NOLOCK)
				INNER JOIN dbo.MsgTarget mt WITH (NOLOCK) ON mc.MessageId = mt.MessageId AND mc.TargetId = mt.TargetId
			WHERE mt.MessageId = m.MessageId
				AND mc.JobIdentify = 3
				AND 1 = CASE
						WHEN @Environment = 'Local' AND mc.IsFinished = 0 THEN 1
						WHEN @Environment = 'UAT' AND mc.IsFinished_UAT = 0 THEN 1
						WHEN @Environment = 'Production' AND mc.IsFinished = 0 THEN 1
						ELSE 0 END
		)
	ORDER BY m.MessageId

	DELETE m
	FROM (
		SELECT MessageId, IsNew, ROW_NUMBER() OVER(PARTITION BY MessageId ORDER BY IsNew) AS RN
		FROM @Msg
	) m
	WHERE m.RN > 1

	SELECT MessageId, MsgContentsType, MatchId, Content_en, Content_cs, PostTime, IsNew
	FROM @Msg
	ORDER BY MessageId

	SELECT mt.MessageId, mt.TargetId, ms.SiteName
	FROM dbo.MsgTarget mt WITH (NOLOCK)
		INNER JOIN @Msg m ON mt.MessageId = m.MessageId
		INNER JOIN dbo.MsgSite ms WITH (NOLOCK) ON mt.TargetId = ms.SiteId
		INNER JOIN bodb02.dbo.Site s WITH (NOLOCK) ON ms.SiteName = s.site AND s.isDeposit = 1
	WHERE
		m.IsNew = 1
		OR EXISTS (
			SELECT 1
			FROM dbo.MsgChatBot mc WITH (NOLOCK)
			WHERE mc.MessageId = mt.MessageId
				AND mc.TargetId = mt.TargetId
				AND mc.JobIdentify = 3
				AND 1 = CASE
						WHEN @Environment = 'Local' AND mc.IsFinished = 0 THEN 1
						WHEN @Environment = 'UAT' AND mc.IsFinished_UAT = 0 THEN 1
						WHEN @Environment = 'Production' AND mc.IsFinished = 0 THEN 1
						ELSE 0 END
		)
	ORDER BY mt.MessageId

	DROP TABLE #MsgContentsTypeLst
END
GO