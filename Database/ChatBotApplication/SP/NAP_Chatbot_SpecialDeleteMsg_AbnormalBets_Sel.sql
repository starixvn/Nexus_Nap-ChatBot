/*<info serverAlias="DBMSG-bodb_Message" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/
ALTER PROCEDURE [dbo].[NAP_Chatbot_SpecialDeleteMsg_AbnormalBets_Sel]
	@MessageIDLst		VARCHAR(MAX),
	@BatchSize			INT,
	@MsgContentsTypeLst VARCHAR(2000),
	@Environment		VARCHAR(50) = 'UAT'
AS

/*
	Created: 20200214@Percy
	Task: ChatBot - Get Voiding Abnormal Bets Message of special delete site
	DB: DBMSG.bodb_Message  

	Revisions:
		- 20200214@Percy: Initialize SP [RedmineID: #127944]
		- 20200610@Percy: Exclude message void by BetId [RedmineID: #135686]
		- 20200701@Percy: Enhance mechanism to get new MessageID - Phase 2 [RedmineID: #135686]
*/

BEGIN
	SET NOCOUNT ON

	DECLARE @XML XML

	-- Insert messages
	CREATE TABLE #Message (MessageId INT, TargetId INT, JobIdentify TINYINT)

	SET @XML = N'<root><r>' + REPLACE(@MessageIDLst, ',', '</r><r>') + '</r></root>'
	
	INSERT INTO #Message (MessageId, TargetId, JobIdentify)
	SELECT r.value('.', 'NVARCHAR(20)') AS MessageId, 0 AS TargetId, 2 AS JobIdentify
	FROM @xml.nodes('//root/r') AS RECORDS(r)
	WHERE LEN(ISNULL(r.value('.', 'NVARCHAR(20)'), '')) > 0

	INSERT INTO dbo.MsgChatBot (MessageId, TargetId, JobIdentify, Environment, IsFinished)
	SELECT m.MessageId, m.TargetId, m.JobIdentify, @Environment, 0
	FROM #Message m
		LEFT JOIN dbo.MsgChatBot mc WITH (NOLOCK)
			ON m.MessageId = mc.MessageId
				AND m.TargetId = mc.TargetId
				AND m.JobIdentify = mc.JobIdentify
				AND mc.Environment = @Environment
	WHERE mc.MessageId IS NULL

	DROP TABLE #Message
	
	-- MsgContentsType
	CREATE TABLE #MsgContentsTypeLst (MsgContentsType INT)
	CREATE UNIQUE CLUSTERED INDEX PK_MsgContentsType ON #MsgContentsTypeLst (MsgContentsType)

	SET @XML = N'<root><r>' + REPLACE(@MsgContentsTypeLst, ',', '</r><r>') + '</r></root>'
	
	INSERT INTO #MsgContentsTypeLst(MsgContentsType)
	SELECT r.value('.', 'NVARCHAR(15)') AS MsgContentsType
	FROM @xml.nodes('//root/r') AS RECORDS(r)
	WHERE LEN(ISNULL(r.value('.', 'NVARCHAR(15)'), '')) > 0

	-- Get top new messages
	SELECT TOP (@BatchSize) m.MessageId, m.MsgContentsType, m.Content_en ,m.Content_cs
	FROM dbo.Msg m WITH (NOLOCK)
		INNER JOIN dbo.MsgChatBot mc WITH (NOLOCK) ON m.MessageId = mc.MessageId
	WHERE mc.TargetId = 0
		AND mc.JobIdentify = 2
		AND mc.Environment = @Environment
		AND mc.IsFinished = 0
		AND m.msgtype = 1
		AND EXISTS (SELECT 1 FROM #MsgContentsTypeLst mct WHERE m.MsgContentsType = mct.MsgContentsType)
		AND m.isbatch = 1
		AND m.Content_en LIKE '%@@%'
	ORDER BY m.MessageId
	OPTION(RECOMPILE)

	DROP TABLE #MsgContentsTypeLst
END
GO