/*<info serverAlias="DBMSG-bodb_Message" executers="bodbAdminNet" viewers="" isFunction="0" isNested="0"></info>*/
CREATE PROCEDURE [dbo].[NAP_Chatbot_SpecialDeleteMsg_Sel]
	@MessageID INT
	, @BatchSize INT
	, @MsgContentsTypeLst VARCHAR(2000)
AS
/*
	Created: Dominic
	Date: 20191127
	Task: Get messages of special delete site[RedmineID #122905]
	DB: DBMSG/bodb_Message  

	Revisions:
		- 20191127@Dominic: init
*/
BEGIN
	SET NOCOUNT ON

	CREATE TABLE #TmpMsg(MessageId INT , MsgContentsType INT, Content_en NVARCHAR(4000), Content_cs NVARCHAR(4000))
	CREATE TABLE #MsgContentsTypeLst(Val INT)
	CREATE UNIQUE CLUSTERED INDEX PK_MsgContentsType ON #MsgContentsTypeLst(Val)

	DECLARE @XML XML  = NULL

	SET @XML = N'<root><r>' + REPLACE(@MsgContentsTypeLst, ',', '</r><r>') + '</r></root>'
	
	INSERT INTO #MsgContentsTypeLst(val)
	SELECT r.value('.', 'NVARCHAR(15)') Val
	FROM @xml.nodes('//root/r') AS RECORDS(r)
	WHERE LEN(ISNULL(r.value('.', 'NVARCHAR(15)'), '')) > 0

	

	INSERT INTO #TmpMsg(MessageId,  MsgContentsType, Content_en, Content_cs)
	SELECT TOP (@BatchSize) m.MessageId, m.MsgContentsType, m.Content_en ,m.Content_cs
	from MsgTarget mt (NOLOCK)
	INNER JOIN msg m (NOLOCK) on mt.MessageId = m.MessageId	
	INNER JOIN bodb02.dbo.Dep_CustSuper dc (NOLOCK) on mt. TargetId = dc. custid	
	where  m.MessageId > @MessageID
	AND m.msgtype = 1	
	AND m.MsgContentsType IN (SELECT Val FROM #MsgContentsTypeLst)
	AND m.isbatch = 1	
	AND mt.isDeleted = 0
	AND m.Content_en LIKE '%@@%'
	AND mt.BetTransId IS NOT NULL
	AND (site IN ('spondemo' ,'Athena000')
				OR currency NOT IN (20,27,28)
			)
	GROUP BY m.MessageId,  m.MsgContentsType, Content_en, Content_cs
	ORDER BY m.MessageId
	OPTION(RECOMPILE)

	CREATE UNIQUE CLUSTERED INDEX PK_MessageID ON #TmpMsg(MessageId)
	
	--1st dataset: message header
	SELECT MessageId, MsgContentsType, Content_en, Content_cs 
	FROM #TmpMsg
	ORDER BY MessageId

	--2nd dataset: message detail
	SELECT DISTINCT m.MessageId, dc.site
	FROM MsgTarget mt (NOLOCK)	
		INNER JOIN #TmpMsg m (NOLOCK) ON mt.MessageId = m.MessageId	
		INNER JOIN bodb02. dbo. Dep_CustSuper dc (NOLOCK) ON mt. TargetId = dc. custid	
	WHERE mt.isDeleted = 0 
		AND mt.BetTransId IS NOT NULL
		AND (
			site IN ('spondemo' ,'Athena000')
			OR currency NOT IN (20,27,28)
		)
	ORDER BY m.MessageId, dc.site

	DROP TABLE #TmpMsg
END