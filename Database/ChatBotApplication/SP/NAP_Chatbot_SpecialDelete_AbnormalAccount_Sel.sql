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
				<r MessageID=1 RequestID=2 />
				<r MessageID=3 RequestID=2 />
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

	CREATE TABLE #VoidingHistory (VoidingHistoryId INT, RequestId INT, MessageId INT, ReasonId INT, VoidById INT)
	INSERT INTO #VoidingHistory (VoidingHistoryId, RequestId, MessageId, ReasonId, VoidById)
	SELECT vh.VoidingHistoryId, vh.RequestId, vh.MessageId, vh.ReasonId, vh.UserId AS VoidById
	FROM dbo.TicketVoiding_VoidingHistory vh WITH (NOLOCK)
	WHERE EXISTS (SELECT 1 FROM #MessageLst m WHERE vh.MessageId = m.MessageId AND vh.RequestId = m.RequestId)
		AND vh.StatusId = 2

	-- Request Info
	BEGIN
		DECLARE @tblRequestInfo TABLE 
		(
			MessageId INT, 
			RequestId INT, 
			RequestBy NVARCHAR(200), 
			VoidBy NVARCHAR(50), 
			Usernames NVARCHAR(MAX), 
			MaxWinlostDate DATETIME,
			MinWinlostDate DATETIME,
			CreditSite NVARCHAR(MAX),
			DepositSite NVARCHAR(MAX)
		)
		
		;WITH CTE_Request AS
		(
			SELECT MessageId, RequestId, ReasonId, VoidById
			FROM #VoidingHistory
			GROUP BY MessageId, RequestId, ReasonId, VoidById
		)
		INSERT INTO @tblRequestInfo (MessageId, RequestId, RequestBy, VoidBy)
		SELECT vh.MessageId, rq.RequestId, rb.username AS RequestBy, vb.username AS VoidBy
		FROM CTE_Request vh
			INNER JOIN dbo.TicketVoiding_Request rq WITH (NOLOCK) ON rq.RequestId = vh.RequestId
			LEFT JOIN dbo.Customer rb WITH (NOLOCK) ON rq.UserId = rb.custid
			LEFT JOIN dbo.Customer vb WITH (NOLOCK) ON vh.VoidById = vb.custid
		WHERE rq.IssueTypeId = 13 -- Abnormal Account
	END

	UPDATE t
		SET t.Usernames = _Usernames
	FROM @tblRequestInfo t
	CROSS APPLY
	(
		SELECT TOP 1 Usernames
		FROM dbo.TicketVoiding_RequestDetail d WITH(NOLOCK)
		WHERE d.RequestId = t.RequestId
	)_(_Usernames)

	;WITH CTE AS
	(
		SELECT t.MessageId, c.site, CASE WHEN s.custid IS NULL THEN 1 ELSE 0 END AS IsCredit
		FROM @tblRequestInfo t
		CROSS APPLY strSplit(t.Usernames, ',') f
			INNER JOIN dbo.Customer c WITH(NOLOCK) ON c.username = f.val
			LEFT JOIN dbo.Dep_CustSuper s WITH(NOLOCK) ON s.custid = c.srecommend
		
	), CTE_Site AS
	(
		SELECT 
			e.MessageId 
			,STUFF((SELECT ',' + c.site FROM CTE c WHERE c.MessageId = e.MessageId AND c.IsCredit = 1 FOR XML PATH('')),1,1,'') AS CreditSite
			,STUFF((SELECT ',' + d.site FROM CTE d WHERE d.MessageId = e.MessageId AND d.IsCredit = 0 FOR XML PATH('')),1,1,'') AS DepositSite		 
		FROM CTE e
	)
	SELECT t.MessageId, t.RequestId, t.RequestBy, t.VoidBy, t.Usernames, s.CreditSite, s.DepositSite
	FROM @tblRequestInfo t
		INNER JOIN CTE_Site s ON t.MessageId = s.MessageId

	DROP TABLE #MessageLst
	DROP TABLE #VoidingHistory
END
GO