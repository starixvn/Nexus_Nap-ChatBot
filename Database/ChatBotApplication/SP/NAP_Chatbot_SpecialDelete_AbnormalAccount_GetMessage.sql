/*<info serverAlias="DBACC-bodb02" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/
CREATE PROCEDURE [dbo].[NAP_Chatbot_SpecialDelete_AbnormalAccount_GetMessage]
AS

/*
	Created: 20210419@Malcolm
	Task: ChatBot - Get Message List of Voiding Abnormal Account from special delete site
	DB: DBACC.bodb02

	Revisions:
		- 20210419@Malcolm: Initialize SP [RedmineID: #152175]
		
*/

BEGIN
	SET NOCOUNT ON

	SELECT DISTINCT vh.MessageId, vh.RequestId
	FROM dbo.TicketVoiding_VoidingHistory vh WITH (NOLOCK)
		INNER JOIN TicketVoiding_Request r WITH (NOLOCK) ON vh.RequestId = r.RequestId AND r.IssueTypeId = 13 -- Abnormal Account
	WHERE vh.CreatedDate >= GETDATE() - 1
		AND vh.StatusId = 2 -- tickets void successfully
	ORDER BY MessageId
END
GO