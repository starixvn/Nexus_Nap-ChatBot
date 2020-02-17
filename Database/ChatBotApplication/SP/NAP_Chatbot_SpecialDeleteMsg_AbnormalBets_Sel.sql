/*<info serverAlias="DBMSG-bodb_Message" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/
ALTER PROCEDURE [dbo].[NAP_Chatbot_SpecialDeleteMsg_AbnormalBets_Sel]
	@MessageID INT,
	@BatchSize INT,
	@MsgContentsTypeLst VARCHAR(2000)
AS

/*
	Created: 20200214@Percy
	Task: ChatBot - Get Voiding Abnormal Bets Message of special delete site
	DB: DBMSG.bodb_Message  

	Revisions:
		- 20200214@Percy: Initialize SP [RedmineID: #127944]
*/

BEGIN
	SET NOCOUNT ON

	CREATE TABLE #MsgContentsTypeLst(MsgContentsType INT)
	CREATE UNIQUE CLUSTERED INDEX PK_MsgContentsType ON #MsgContentsTypeLst(MsgContentsType)

	DECLARE @XML XML  = NULL

	SET @XML = N'<root><r>' + REPLACE(@MsgContentsTypeLst, ',', '</r><r>') + '</r></root>'
	
	INSERT INTO #MsgContentsTypeLst(MsgContentsType)
	SELECT r.value('.', 'NVARCHAR(15)') AS MsgContentsType
	FROM @xml.nodes('//root/r') AS RECORDS(r)
	WHERE LEN(ISNULL(r.value('.', 'NVARCHAR(15)'), '')) > 0

	SELECT TOP (@BatchSize) m.MessageId, m.MsgContentsType, m.Content_en ,m.Content_cs
	FROM dbo.Msg m WITH (NOLOCK)
	WHERE m.MessageId > @MessageID
		AND m.msgtype = 1
		AND EXISTS (SELECT 1 FROM #MsgContentsTypeLst mct WHERE m.MsgContentsType = mct.MsgContentsType)
		AND m.isbatch = 1
		AND m.Content_en LIKE '%@@%'
	ORDER BY m.MessageId
	OPTION(RECOMPILE)

	DROP TABLE #MsgContentsTypeLst
END
GO