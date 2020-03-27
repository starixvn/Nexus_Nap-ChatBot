/*<info serverAlias="DBMSG-bodb_Message" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/
ALTER PROCEDURE [dbo].[NAP_Chatbot_SiteMesssage_Sel]
	@MessageID INT,
	@BatchSize INT,
	@MsgContentsTypeLst VARCHAR(2000)
AS

/*
	Created: 20200305@Percy
	Task: ChatBot - Get Message from Site Message
	DB: DBMSG.bodb_Message  

	Revisions:
		- 20200305@Percy: Initialize SP [RedmineID: #126143]
*/

BEGIN
	SET NOCOUNT ON

	CREATE TABLE #MsgContentsTypeLst(MsgContentsType TINYINT)
	CREATE UNIQUE CLUSTERED INDEX PK_MsgContentsType ON #MsgContentsTypeLst(MsgContentsType)

	DECLARE @XML XML  = NULL

	SET @XML = N'<root><r>' + REPLACE(@MsgContentsTypeLst, ',', '</r><r>') + '</r></root>'
	
	INSERT INTO #MsgContentsTypeLst(MsgContentsType)
	SELECT r.value('.', 'NVARCHAR(15)') AS MsgContentsType
	FROM @xml.nodes('//root/r') AS RECORDS(r)
	WHERE LEN(ISNULL(r.value('.', 'NVARCHAR(15)'), '')) > 0

	DECLARE @Msg TABLE (MessageId INT, MsgContentsType TINYINT, MatchId INT, Content_en NVARCHAR(4000), Content_cs NVARCHAR(4000))
	
	INSERT INTO @Msg (MessageId, MsgContentsType, MatchId, Content_en, Content_cs)
	SELECT TOP (@BatchSize) m.MessageId, m.MsgContentsType, m.MatchId, m.Content_en, m.Content_cs
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

	SELECT MessageId, MsgContentsType, MatchId, Content_en, Content_cs
	FROM @Msg
	ORDER BY MessageId

	SELECT mt.MessageId, ms.SiteName
	FROM dbo.MsgTarget mt WITH (NOLOCK)
		INNER JOIN dbo.MsgSite ms WITH (NOLOCK) ON mt.TargetId = ms.SiteId
		INNER JOIN bodb02.dbo.Site s WITH (NOLOCK) ON ms.SiteName = s.site AND s.isDeposit = 1
	WHERE EXISTS (SELECT 1 FROM @Msg m WHERE mt.MessageId = m.MessageId)
	ORDER BY mt.MessageId

	DROP TABLE #MsgContentsTypeLst
END
GO