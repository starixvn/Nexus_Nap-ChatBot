/*<info serverAlias="DBACC-bodb02" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/
ALTER PROCEDURE [dbo].[NAP_Chatbot_SpecialDelete_AbnormalAccount_Sel]
	@MessageIDLst XML
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
				<r MessageID=2 RequestID=2 />
				<r MessageID=3 RequestID=3 />
			</root>
*/

BEGIN
	SET NOCOUNT ON

	CREATE TABLE #MessageLst (MessageId INT, RequestId INT)
	INSERT INTO  #MessageLst (MessageId, RequestId)
	SELECT
		t.c.value('@MessageID', 'INT') AS MessageId
		,t.c.value('@RequestID', 'INT') AS RequestId
	FROM @MessageIDLst.nodes('root/r') AS t(c)
	OPTION (OPTIMIZE FOR (@MessageIDLst = NULL))

	CREATE TABLE #VoidingHistory 
	(
		VoidingHistoryId	INT, 
		RequestId			INT, 
		MessageId			INT, 
		VoidById			INT, 
		VoidBy				VARCHAR(30),
		RequestBy	        VARCHAR(30),
		MinWinlostdate		DATETIME, 
		MaxWinlostdate		DATETIME,
		Usernames 			NVARCHAR(MAX), 
		CreditSite 			NVARCHAR(MAX),
		DepositSite 		NVARCHAR(MAX)
	)
	INSERT INTO #VoidingHistory (VoidingHistoryId, RequestId, MessageId, VoidById, MinWinlostdate, MaxWinlostdate)
	SELECT vh.VoidingHistoryId, vh.RequestId, vh.MessageId, vh.UserId AS VoidById, vh.MinWinlostdate, vh.MaxWinlostdate
	FROM dbo.TicketVoiding_VoidingHistory vh WITH (NOLOCK)
	WHERE EXISTS (SELECT 1 FROM #MessageLst m WHERE vh.MessageId = m.MessageId AND vh.RequestId = m.RequestId)
		AND vh.StatusId = 2

	UPDATE vh
		SET vh.RequestBy = rb.username
			,vh.VoidBy = vb.username
	FROM #VoidingHistory vh
		INNER JOIN dbo.TicketVoiding_Request rq WITH (NOLOCK) ON rq.RequestId = vh.RequestId
		LEFT JOIN dbo.Customer rb WITH (NOLOCK) ON rq.UserId = rb.custid
		LEFT JOIN dbo.Customer vb WITH (NOLOCK) ON vh.VoidById = vb.custid
	WHERE rq.IssueTypeId = 13 -- Abnormal Account

	UPDATE vh
		SET vh.Usernames = UsernameLst
	FROM #VoidingHistory vh
	CROSS APPLY
	(
		SELECT TOP 1 Usernames
		FROM dbo.TicketVoiding_RequestDetail d WITH(NOLOCK)
		WHERE d.RequestId = vh.RequestId
	)_(UsernameLst)

	;WITH CTE AS
	(
		SELECT vh.MessageId, c.site, CASE WHEN s.custid IS NULL THEN 1 ELSE 0 END AS IsCredit
		FROM #VoidingHistory vh
		CROSS APPLY strSplit(vh.Usernames, ',') f
			INNER JOIN dbo.Customer c WITH(NOLOCK) ON c.username = CONVERT(VARCHAR(30), f.val)
			LEFT JOIN dbo.Dep_CustSuper s WITH(NOLOCK) ON s.custid = c.srecommend
		
	),CTE_Site AS
	(
		SELECT 
			e.MessageId 
			,STUFF((SELECT DISTINCT ',' + c.site FROM CTE c WHERE c.MessageId = e.MessageId AND c.IsCredit = 1 FOR XML PATH('')),1,1,'') AS CreditSiteLst
			,STUFF((SELECT DISTINCT ',' + d.site FROM CTE d WHERE d.MessageId = e.MessageId AND d.IsCredit = 0 FOR XML PATH('')),1,1,'') AS DepositSiteLst	 
		FROM CTE e
		GROUP BY e.MessageId
	)
	SELECT vh.MessageId, vh.RequestId, vh.RequestBy, vh.VoidBy, vh.Usernames, s.CreditSiteLst, s.DepositSiteLst
	FROM #VoidingHistory vh
		INNER JOIN CTE_Site s ON vh.MessageId = s.MessageId

	DROP TABLE #MessageLst
	DROP TABLE #VoidingHistory
END
GO