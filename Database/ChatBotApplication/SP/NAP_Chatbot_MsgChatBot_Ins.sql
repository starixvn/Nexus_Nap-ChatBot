/*<info serverAlias="DBMSG-bodb_Message" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/
CREATE PROCEDURE [dbo].[NAP_Chatbot_MsgChatBot_Ins]
	@MessageIDLst	XML
	,@Environment	VARCHAR(50)
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
			,0 AS IsInsert
		FROM @MessageIDLst.nodes('root/r') AS t(c)
		OPTION (OPTIMIZE FOR (@MessageIDLst = NULL))
		
		UPDATE m
		SET m.IsInsert = 0
		FROM @MsgChatBot m
			INNER JOIN dbo.MsgChatBot mc WITH (NOLOCK) ON m.MessageId = mc.MessageId AND m.TargetId = mc.TargetId

		-- update exists msg
		IF @Environment = 'UAT'
			UPDATE mc WITH (ROWLOCK)
			SET mc.IsFinished = 1, mc.FinishedTime = GETDATE()
			FROM dbo.MsgChatBot mc
				INNER JOIN @MsgChatBot m ON mc.MessageId = m.MessageId AND mc.TargetId = m.TargetId
			WHERE mc.IsFinished = 0
		ELSE IF @Environment IN ('Local', 'Production')
			UPDATE mc WITH (ROWLOCK)
			SET mc.IsFinished_UAT = 1, mc.FinishedTime_UAT = GETDATE()
			FROM dbo.MsgChatBot mc
				INNER JOIN @MsgChatBot m ON mc.MessageId = m.MessageId AND mc.TargetId = m.TargetId
			WHERE mc.IsFinished_UAT = 0

		-- insert new msg
		INSERT INTO dbo.MsgChatBot (MessageId, TargetId)
		SELECT MessageId, TargetId
		FROM @MsgChatBot
		WHERE IsInsert = 1
	END TRY
	BEGIN CATCH
		SELECT @ErrCode = ERROR_NUMBER(), @ErrMsg = ERROR_MESSAGE()
		RETURN
	END CATCH
END
GO