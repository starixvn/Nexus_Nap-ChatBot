/*<info serverAlias="DBMSG-bodb_Message" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/
ALTER PROCEDURE [dbo].[NAP_Chatbot_SpecialDeleteMsg_AbnormalAccount_Sel]
	@MessageIDLst		XML,
	@BatchSize			INT,
	@MsgContentsTypeLst VARCHAR(2000),
	@Environment		VARCHAR(50) = 'UAT'
AS

/*
	Created: 20210419@Malcolm
	Task: ChatBot - Get Message List of Voiding Abnormal Account from special delete site
	DB: DBACC.bodb02

	Revisions:
		- 20210419@Malcolm: Initialize SP [RedmineID: #152175]
	Param's Explanation:
		@MessageIDLst:
			<root>
				<r MessageID=1 RequestID=1 />
				<r MessageID=1 RequestID=2 />
				<r MessageID=3 RequestID=2 />
			</root>
		
*/

BEGIN
	SET NOCOUNT ON

	DECLARE @XML XML

	CREATE TABLE #Message (MessageId INT, TargetId INT, JobIdentify TINYINT, RequestId INT)

	INSERT INTO #Message (MessageId, TargetId, JobIdentify, RequestId)
	SELECT 
		t.c.value('@MessageID', 'INT') AS MessageId
		,0 AS TargetId
		,4 AS JobIdentify
		,t.c.value('@RequestID', 'INT') AS RequestID
	FROM @MessageIDLst.nodes('root/r') AS t(c)
	OPTION(OPTIMIZER FOR (@MessageIDLst = NULL))

	INSERT INTO dbo.MsgChatBot (MessageId, TargetId, JobIdentify, RequestId, IsFinished, Environment)
	SELECT MessageId, TargetId, JobIdentify, RequestId, 0, @Environment
	FROM #Message

	CREATE TABLE #MsgContentsTypeLst (MsgContentsType INT)

	SET @XML = N'<root><r>' + REPLACE(@MsgContentsTypeLst,',', '</r><r>') + '</r></root>'

	INSERT INTO #MsgContentsTypeLst (MsgContentsType)
	SELECT 
		t.c.value('.', 'NVARCHAR(15)') AS MsgContentsType
	FROM @XML.nodes('root/r') t(c)
	WHERE LEN(ISNULL(t.c.value('.', 'NVARCHAR(15)'), '')) > 0

	DECLARE @tblMessage TABLE (MessageId INT, MsgContentsType TINYINT, Content_en NVARCHAR(4000), Content_cs NVARCHAR(4000))

	INSERT INTO @tblMessage (MessageId, MsgContentsType, Content_en, Content_cs)
	SELECT TOP (@BatchSize) m.MessageId, m.MsgContentsType, m.Content_en, m.Content_cs
	FROM dbo.Msg m WITH(NOLOCK)
	WHERE EXISTS(
		SELECT 1
		FROM dbo.MsgChatBot c WITH(NOLOCK)
		WHERE c.MessageId = m.MessageId
			AND c.Environment = @Environment
			AND c.IsFinished =0
			AND c.JobIdentify = 4
			AND c.TargetId = 0
	) AND EXISTS (SELECT 1 FROM #MsgContentsTypeLst l WHERE l.MsgContentsType = m.MsgContentsType)
	
	SELECT t.MessageId, mcb.RequestId, t.MsgContentsType, t.Content_en, t.Content_cs
	FROM @tblMessage t
		INNER JOIN dbo.MsgChatBot mcb WITH(NOLOCK) ON mcb.MessageId = t.MessageId
	WHERE mcb.Environment = @Environment
		AND mcb.IsFinished = 0
		AND mcb.JobIdentify = 4
		AND mcb.TargetId = 0

	DROP TABLE #Message
	DROP TABLE #MsgContentsTypeLst
END
GO