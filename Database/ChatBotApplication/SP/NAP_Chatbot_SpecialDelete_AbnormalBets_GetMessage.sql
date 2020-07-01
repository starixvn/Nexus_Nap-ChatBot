/*<info serverAlias="DBACC-bodb02" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/
ALTER PROCEDURE [dbo].[NAP_Chatbot_SpecialDelete_AbnormalBets_GetMessage]
AS

/*
	Created: 20200701@Percy
	Task: ChatBot - Get Message List of Voiding Abnormal Bets from special delete site
	DB: DBACC.bodb02

	Revisions:
		- 20200701@Percy: Initialize SP [RedmineID: #135686]
*/

BEGIN
	SET NOCOUNT ON

	SELECT DISTINCT MessageId
	FROM dbo.TicketVoiding_VoidingHistory vh WITH (NOLOCK)
		INNER JOIN TicketVoiding_Request r WITH (NOLOCK) ON vh.RequestId = r.RequestId AND r.IssueTypeId = 11 -- abnormal bets
	WHERE vh.CreatedDate >= GETDATE() - 1
		AND vh.StatusId = 2 -- tickets void successfully
	ORDER BY MessageId
END
GO