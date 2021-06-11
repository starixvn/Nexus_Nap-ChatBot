/*<info serverAlias="DBACC-bodb02" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/
USE bodb02
GO

ALTER PROCEDURE [dbo].[NAP_Chatbot_SpecialDelete_AbnormalBets_Sel]
	@MessageIDLst XML
AS

/*
	Created: 20200214@Percy
	Task: ChatBot - Get Voiding Abnormal Bets Info of special delete site
	DB: DBACC.bodb02  

	Revisions:
		- 20200214@Percy: Initialize SP [RedmineID: #127944]
		- 20200305@Percy: Extend length some fields [RedmineID: #129794]
		- 20200804@Percy: Add RequestId for Ticket Voiding Abnormal Bets - Phase 3 [RedmineID: #135686]
		- 20210413@Malcolm: Integrate AB (Real member) voiding tickets [RedmineID: #153112]
		- 20210610@Percy: Change source to get Reason (SubIssueTypeName) [RedmineID: #156724]

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


	CREATE TABLE #VoidingHistory (VoidingHistoryId INT, RequestId INT, MessageId INT, ReasonId INT, VoidById INT, TransId BIGINT)
	INSERT INTO #VoidingHistory (VoidingHistoryId, RequestId, MessageId, ReasonId, VoidById, TransId)
	SELECT vh.VoidingHistoryId, vh.RequestId, vh.MessageId, vh.ReasonId, vh.UserId AS VoidById, t.TransId
	FROM dbo.TicketVoiding_VoidingHistory vh WITH (NOLOCK)
		CROSS APPLY (
			SELECT val AS TransId FROM dbo.strSplit(vh.TransIds, ',')
		) t
	WHERE EXISTS (SELECT 1 FROM #MessageLst m WHERE vh.MessageId = m.MessageId AND vh.RequestId = m.RequestId)
		AND vh.StatusId = 2

	-- Request Info
	BEGIN
		DECLARE @tblRequestInfo TABLE (MessageId INT, RequestId INT, DecidedBy NVARCHAR(50), RequestBy NVARCHAR(200), VoidBy NVARCHAR(50), ReasonLst NVARCHAR(MAX))
		
		;WITH CTE_Request AS
		(
			SELECT MessageId, RequestId, ReasonId, VoidById
			FROM #VoidingHistory
			GROUP BY MessageId, RequestId, ReasonId, VoidById
		)
		INSERT INTO @tblRequestInfo (MessageId, RequestId, DecidedBy, RequestBy, VoidBy, ReasonLst)
		SELECT vh.MessageId, rq.RequestId, rq.DecidedBy, rb.username AS RequestBy, vb.username AS VoidBy
			,STUFF((SELECT N', ' + rs.SubIssueTypeName
					FROM dbo.TicketVoiding_RequestDetail rqd WITH (NOLOCK)
						INNER JOIN dbo.TicketVoiding_SubIssueType rs WITH (NOLOCK) ON rqd.SubIssueTypeId = rs.SubIssueTypeId
					WHERE rq.RequestId = rqd.RequestId
					FOR XML PATH('')), 1, 2, '') AS ReasonLst
		FROM CTE_Request vh
			INNER JOIN dbo.TicketVoiding_Request rq WITH (NOLOCK) ON rq.RequestId = vh.RequestId
			LEFT JOIN dbo.Customer rb WITH (NOLOCK) ON rq.UserId = rb.custid
			LEFT JOIN dbo.Customer vb WITH (NOLOCK) ON vh.VoidById = vb.custid
		WHERE rq.IssueTypeId IN (11, 15) -- Abnormal Bets, AB (real member)
	END

	-- Event Info
	BEGIN
		DECLARE @tblEventInfo TABLE (MessageId INT, KickOffTime VARCHAR(8), LeagueName NVARCHAR(500), HomeName NVARCHAR(100), AwayName NVARCHAR(100), HalfTimeHomeScore INT, HalfTimeAwayScore INT, FullTimeHomeScore INT, FullTimeAwayScore INT)
		
		INSERT INTO @tblEventInfo (MessageId, KickOffTime, LeagueName, HomeName, AwayName, HalfTimeHomeScore, HalfTimeAwayScore, FullTimeHomeScore, FullTimeAwayScore)
		SELECT DISTINCT vh.MessageId, LTRIM(RTRIM(REPLACE(REPLACE(RIGHT(CONVERT(VARCHAR(20), DATEADD(HOUR, 12, m.GlobalShowTime), 100), 7), 'AM', ' AM'), 'PM', ' PM'))) AS KickOffTime, l.LeagueName, h.TeamName AS HomeName, a.TeamName AS AwayName, m.hthomescore, m.htawayscore, m.finalhomescore, m.finalawayscore
		FROM #VoidingHistory vh
			INNER JOIN dbo.TicketVoiding_RequestDetail rd WITH (NOLOCK) ON rd.RequestId = vh.RequestId
			CROSS APPLY (
				SELECT m.hthomescore, m.htawayscore, m.finalhomescore, m.finalawayscore, m.GlobalShowTime
				FROM dbo.match m WITH(NOLOCK)
				WHERE m.matchid = rd.matchid
				UNION
				SELECT m.hthomescore, m.htawayscore, m.finalhomescore, m.finalawayscore, m.GlobalShowTime
				FROM dbo.match14 m WITH(NOLOCK)
				WHERE m.matchid = rd.matchid
			) m
			INNER JOIN dbo.League l WITH (NOLOCK) ON rd.LeagueId = l.LeagueID
			INNER JOIN dbo.Team h WITH (NOLOCK) ON rd.HomeId = h.TeamId
			INNER JOIN dbo.Team a WITH (NOLOCK) ON rd.AwayId = a.TeamId
	END

	-- Ticket Info
	BEGIN
		DECLARE @tblRecommendBettype TABLE (MessageId INT, recommend INT, bettype SMALLINT, liveindicator BIT)
		INSERT INTO @tblRecommendBettype (MessageId, recommend, bettype, liveindicator)
		SELECT vh.MessageId, b.recommend, b.bettype, ISNULL(b.liveindicator, 0)
		FROM #VoidingHistory vh
			CROSS APPLY (
				SELECT TOP 1 b.bettype, b.liveindicator, b.recommend
				FROM dbo.bettrans b WITH (NOLOCK)
				WHERE b.transid = vh.TransId
			) b
		GROUP BY vh.MessageId, b.recommend, b.bettype, ISNULL(b.liveindicator, 0)
	END

	-- Return Data
	BEGIN
		-- Request Info
		;WITH CTE_RequestInfo AS
		(
			SELECT
				r1.MessageId
				,STUFF((SELECT DISTINCT N', ' + CONVERT(NVARCHAR(20), r2.RequestId)		FROM @tblRequestInfo r2 WHERE r1.MessageId = r2.MessageId FOR XML PATH ('')), 1, 2, '') AS RequestIdLst
				,STUFF((SELECT DISTINCT N', ' + CONVERT(NVARCHAR(200), r2.DecidedBy)	FROM @tblRequestInfo r2 WHERE r1.MessageId = r2.MessageId FOR XML PATH ('')), 1, 2, '') AS DecidedByLst
				,STUFF((SELECT DISTINCT N', ' + CONVERT(NVARCHAR(50), r2.RequestBy)		FROM @tblRequestInfo r2 WHERE r1.MessageId = r2.MessageId FOR XML PATH ('')), 1, 2, '') AS RequestByLst
				,STUFF((SELECT DISTINCT N', ' + CONVERT(NVARCHAR(50), r2.VoidBy)		FROM @tblRequestInfo r2 WHERE r1.MessageId = r2.MessageId FOR XML PATH ('')), 1, 2, '') AS VoidByLst
				,STUFF((SELECT DISTINCT N', ' + CONVERT(NVARCHAR(150), r2.ReasonLst)	FROM @tblRequestInfo r2 WHERE r1.MessageId = r2.MessageId FOR XML PATH ('')), 1, 2, '') AS ReasonLst
			FROM @tblRequestInfo r1
			GROUP BY r1.MessageId
		)
		-- Total tickets
		,CTE_TotalTickets AS
		(
			SELECT MessageId, COUNT(DISTINCT TransId) AS TotalTickets
			FROM #VoidingHistory
			GROUP BY MessageId
		)
		-- Bettype
		,CTE_Bettype AS
		(
			SELECT t.MessageId, CASE WHEN t.liveindicator = 0 THEN 'Non-Live ' ELSE 'Live ' END + ISNULL(d.typenamee, b.typenamee) AS TypeName
			FROM @tblRecommendBettype t
				INNER JOIN dbo.bettype b WITH (NOLOCK) ON t.bettype = b.typeid
				LEFT JOIN dbo.bettype d WITH (NOLOCK) ON b.DisplayedTypeID = d.typeid
			GROUP BY t.MessageId, CASE WHEN t.liveindicator = 0 THEN 'Non-Live ' ELSE 'Live ' END + ISNULL(d.typenamee, b.typenamee)
		)
		,CTE_MsgBettype AS
		(
			SELECT
				b1.MessageId
				,STUFF((SELECT DISTINCT ', ' + b2.TypeName FROM CTE_Bettype b2 WHERE b1.MessageId = b2.MessageId FOR XML PATH ('')), 1, 2, '') AS TypeNameLst
			FROM CTE_Bettype b1
			GROUP BY b1.MessageId
		)
		-- Site
		,CTE_Site AS
		(
			SELECT DISTINCT t.MessageId, CASE WHEN s.custid IS NULL AND c.site NOT IN ('CN88', '789y') THEN 'NV88' ELSE c.site END AS site, CASE WHEN s.custid IS NULL THEN 0 ELSE 1 END AS isDeposit
			FROM @tblRecommendBettype t
				INNER JOIN dbo.Customer c WITH (NOLOCK) ON t.recommend = c.custid
				LEFT JOIN dbo.Dep_CustSuper s WITH (NOLOCK) ON c.srecommend = s.custid
		)
		,CTE_MsgSite AS
		(
			SELECT
				s1.MessageId
				,STUFF((SELECT DISTINCT ', ' + s2.site FROM CTE_Site s2 WHERE s1.MessageId = s2.MessageId AND isDeposit = 0 FOR XML PATH ('')), 1, 2, '') AS CreditSiteLst
				,STUFF((SELECT DISTINCT ', ' + s2.site FROM CTE_Site s2 WHERE s1.MessageId = s2.MessageId AND isDeposit = 1 FOR XML PATH ('')), 1, 2, '') AS DepositSiteLst
			FROM CTE_Site s1
			GROUP BY s1.MessageId
		)
		,CTE_IsTest AS
		(
			SELECT DISTINCT t.MessageId, 0 AS IsTest
			FROM @tblRecommendBettype t
				INNER JOIN dbo.Customer c WITH (NOLOCK) ON t.recommend = c.custid
			WHERE c.currency <> 20
		)
		SELECT
			r.MessageID
			, CONVERT(BIT, ISNULL(i.IsTest, 1)) AS IsTest
			, t.TotalTickets
			, ISNULL(r.RequestIDLst, '') RequestIDLst
			, ISNULL(r.DecidedByLst, '') DecidedByLst
			, ISNULL(r.RequestByLst, '') RequestByLst
			, ISNULL(r.VoidByLst, '') VoidByLst
			, ISNULL(r.ReasonLst, '') ReasonLst
			, ISNULL(e.KickOffTime, '') KickOffTime
			, ISNULL(e.LeagueName, '') LeagueName
			, ISNULL(e.HomeName, '') HomeName
			, ISNULL(e.AwayName, '') AwayName
			, e.HalfTimeHomeScore
			, e.HalfTimeAwayScore
			, e.FullTimeHomeScore
			, e.FullTimeAwayScore
			, ISNULL(b.TypeNameLst, '') TypeNameLst
			, ISNULL(s.CreditSiteLst, '') CreditSiteLst
			, ISNULL(s.DepositSiteLst, '') DepositSiteLst
		FROM CTE_RequestInfo r
			INNER JOIN @tblEventInfo e ON r.MessageId = e.MessageId
			INNER JOIN CTE_TotalTickets t ON r.MessageId = t.MessageId
			INNER JOIN CTE_MsgBettype b ON r.MessageId = b.MessageId
			INNER JOIN CTE_MsgSite s ON r.MessageId = s.MessageId
			LEFT JOIN CTE_IsTest i ON r.MessageId = i.MessageId
	END

	DROP TABLE #MessageLst
	DROP TABLE #VoidingHistory
END
GO