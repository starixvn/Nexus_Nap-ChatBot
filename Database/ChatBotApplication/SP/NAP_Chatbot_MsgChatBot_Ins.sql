/*<info serverAlias="DBMSG-bodb_Message" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/
ALTER PROCEDURE [dbo].[NAP_Chatbot_MsgChatBot_Ins]
	@MessageIDLst	XML
	,@Environment	VARCHAR(50)
	,@JobIdentify	TINYINT = 0
	,@ErrCode		INT = 0 OUTPUT
	,@ErrMsg		NVARCHAR(2000) = '' OUTPUT
AS

/*
	Created: 20200630@Percy
	Task: ChatBot - Insert MsgId into MsgChatBot
	DB: DBMSG.bodb_Message  

	Revisions:
		- 20200630@Percy: Initialize SP [RedmineID: #136492]

	Param Explanation:
		@MessageIDLst:
			<root>
				<r MessageID=1 TargetID=1 />
				<r MessageID=1 TargetID=2 />
				<r MessageID=2 TargetID=2 />
			</root>
*/

BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		DECLARE @MsgChatBot TABLE (MessageId INT, TargetId INT, IsInsert BIT)
		INSERT INTO @MsgChatBot (MessageId, TargetId, IsInsert)
		SELECT
			t.c.value('@MessageID', 'INT') AS MessageID
			,t.c.value('@TargetID', 'INT') AS TargetID
			,1 AS IsInsert
		FROM @MessageIDLst.nodes('root/r') AS t(c)
		OPTION (OPTIMIZE FOR (@MessageIDLst = NULL))

		INSERT INTO dbo.MsgChatBot (MessageId, TargetId, JobIdentify, Environment, IsFinished)
		SELECT m.MessageId, m.TargetId, @JobIdentify, @Environment, CASE WHEN @JobIdentify = 3 THEN 1 ELSE 0 END
		FROM @MsgChatBot m
			LEFT JOIN dbo.MsgChatBot mc WITH (NOLOCK) ON m.MessageId = mc.MessageId AND m.TargetId = mc.TargetId AND mc.Environment = @Environment
		WHERE mc.MessageId IS NULL
		
		-- delete old msg
		DELETE dbo.MsgChatBot WITH (ROWLOCK, UPDLOCK)
		WHERE CheckInTime < DATEADD(MONTH, -1, GETDATE())
			AND IsFinished = 1
	END TRY
	BEGIN CATCH
		SELECT @ErrCode = ERROR_NUMBER(), @ErrMsg = ERROR_MESSAGE()
		RETURN
	END CATCH
END
GO