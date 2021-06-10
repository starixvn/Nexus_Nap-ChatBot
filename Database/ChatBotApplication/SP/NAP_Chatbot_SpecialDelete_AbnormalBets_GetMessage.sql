/*<info serverAlias="DBACC-bodb02" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/
ALTER PROCEDURE [dbo].[NAP_Chatbot_SpecialDelete_AbnormalBets_GetMessage]
AS

/*
	Created: 20200701@Percy
	Task: ChatBot - Get Message List of Voiding Abnormal Bets from special delete site
	DB: DBACC.bodb02

	Revisions:
		- 20200701@Percy: Initialize SP [RedmineID: #135686]
		- 20200804@Percy: Add RequestId for Ticket Voiding Abnormal Bets - Phase 3 [RedmineID: #135686]
		- 20210413@Malcolm: Integrate AB (Real member) voiding tickets [RedmineID: #153112]
*/

BEGIN
	SET NOCOUNT ON

	SELECT DISTINCT vh.MessageId, vh.RequestId
	FROM dbo.TicketVoiding_VoidingHistory vh WITH (NOLOCK)
		INNER JOIN TicketVoiding_Request r WITH (NOLOCK) ON vh.RequestId = r.RequestId AND r.IssueTypeId IN (11, 15) -- abnormal bets and AB (real member)
	WHERE vh.CreatedDate >= GETDATE() - 1
		AND vh.StatusId = 2 -- tickets void successfully
	ORDER BY MessageId
END
GO