/*<info serverAlias="DBACC-bodb02" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/
ALTER PROCEDURE [dbo].[NAP_Chatbot_SpecialDelete_AbnormalBets_Sel]
	@MessageIDLst VARCHAR(MAX)
AS

/*
	Created: 20200214@Percy
	Task: ChatBot - Get Voiding Abnormal Bets Info of special delete site
	DB: DBACC.bodb02  

	Revisions:
		- 20200214@Percy: Initialize SP [RedmineID: #127944]
*/

BEGIN
	SET NOCOUNT ON

	IF ISNULL(@MessageIDLst , '') = ''
		RETURN

	CREATE TABLE #VoidingHistory (VoidingHistoryId INT, RequestId INT, MessageId INT, ReasonId INT, VoidById INT, TransIds VARCHAR(MAX), TotalTickets INT)
	INSERT INTO #VoidingHistory (VoidingHistoryId, RequestId, MessageId, ReasonId, VoidById, TransIds, TotalTickets)
	SELECT VoidingHistoryId, RequestId, MessageId, ReasonId, UserId AS VoidById, TransIds, t.TotalTickets
	FROM dbo.TicketVoiding_VoidingHistory vh WITH (NOLOCK)
		CROSS APPLY (
			SELECT s.val FROM dbo.strSplit(@MessageIDLst, ',') s WHERE vh.MessageId = s.val
		) s
		CROSS APPLY (
			SELECT COUNT(1) AS TotalTickets FROM dbo.strSplit(vh.TransIds, ',')
		) t
	WHERE vh.StatusId = 2

	-- Request Info
	BEGIN
		DECLARE @tblRequestInfo TABLE (MessageId INT, RequestId INT, DecidedBy VARCHAR(50), RequestBy VARCHAR(50), VoidBy VARCHAR(50), Reason NVARCHAR(150), TotalTickets INT)
		
		;WITH CTE_Request AS
		(
			SELECT RequestId, ReasonId, VoidById
			FROM #VoidingHistory
			GROUP BY RequestId, ReasonId, VoidById
		)
		INSERT INTO @tblRequestInfo (MessageId, RequestId, DecidedBy, RequestBy, VoidBy, Reason, TotalTickets)
		SELECT vh.MessageId, rq.RequestId, rq.DecidedBy, rb.username AS RequestBy, vb.username AS VoidBy, rs.SubIssueTypeName AS Reason, vh.TotalTickets
		FROM #VoidingHistory vh
			INNER JOIN dbo.TicketVoiding_Request rq WITH (NOLOCK) ON rq.RequestId = vh.RequestId
			INNER JOIN dbo.TicketVoiding_SubIssueType rs WITH (NOLOCK) ON rq.SubIssueTypeId = rs.SubIssueTypeId
			LEFT JOIN dbo.Customer rb WITH (NOLOCK) ON rq.UserId = rb.custid
			LEFT JOIN dbo.Customer vb WITH (NOLOCK) ON vh.VoidById = vb.custid
		WHERE rq.IssueTypeId = 11 -- Abnormal Bets
	END

	-- Event Info
	BEGIN
		DECLARE @tblEventInfo TABLE (MessageId INT, KickOffTime VARCHAR(8), LeagueName NVARCHAR(500), HomeName NVARCHAR(100), AwayName NVARCHAR(100), HalfTimeHomeScore INT, HalfTimeAwayScore INT, FullTimeHomeScore INT, FullTimeAwayScore INT)
		
		INSERT INTO @tblEventInfo (MessageId, KickOffTime, LeagueName, HomeName, AwayName, HalfTimeHomeScore, HalfTimeAwayScore, FullTimeHomeScore, FullTimeAwayScore)
		SELECT vh.MessageId, LTRIM(RTRIM(REPLACE(REPLACE(RIGHT(CONVERT(VARCHAR(20), DATEADD(HOUR, 12, m.GlobalShowTime), 100), 7), 'AM', ' AM'), 'PM', ' PM'))) AS KickOffTime, l.LeagueName, h.TeamName AS HomeName, a.TeamName AS AwayName, m.hthomescore, m.htawayscore, m.finalhomescore, m.finalawayscore
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
		DECLARE @transIds TABLE (MessageId INT, TransId BIGINT)
		INSERT INTO @transIds (MessageId, TransId)
		SELECT vh.MessageId, t.val AS TransId
		FROM #VoidingHistory vh
			CROSS APPLY (
				SELECT val FROM dbo.strSplit(vh.TransIds, ',')
			) t

		DECLARE @tblRecommendBettype TABLE (MessageId INT, recommend INT, bettype SMALLINT)
		INSERT INTO @tblRecommendBettype (MessageId, recommend, bettype)
		SELECT t.MessageId, b.recommend, b.bettype
		FROM @transIds t
			CROSS APPLY (
				SELECT TOP 1 b.bettype, b.recommend
				FROM dbo.bettrans b WITH (NOLOCK)
				WHERE b.transid = t.TransId
			) b
	END

	-- Return Data
	BEGIN
		;WITH CTE_RequestInfo AS
		(
			SELECT
				r1.MessageId
				,r1.TotalTickets
				,STUFF((SELECT DISTINCT ', ' + CONVERT(VARCHAR(20), r2.RequestId)	FROM @tblRequestInfo r2 WHERE r1.MessageId = r2.MessageId FOR XML PATH ('')), 1, 1, '') AS RequestIdLst
				,STUFF((SELECT DISTINCT ', ' + CONVERT(VARCHAR(20), r2.DecidedBy)	FROM @tblRequestInfo r2 WHERE r1.MessageId = r2.MessageId FOR XML PATH ('')), 1, 1, '') AS DecidedByLst
				,STUFF((SELECT DISTINCT ', ' + CONVERT(VARCHAR(20), r2.RequestBy)	FROM @tblRequestInfo r2 WHERE r1.MessageId = r2.MessageId FOR XML PATH ('')), 1, 1, '') AS RequestByLst
				,STUFF((SELECT DISTINCT ', ' + CONVERT(VARCHAR(20), r2.VoidBy)		FROM @tblRequestInfo r2 WHERE r1.MessageId = r2.MessageId FOR XML PATH ('')), 1, 1, '') AS VoidByLst
				,STUFF((SELECT DISTINCT ', ' + CONVERT(VARCHAR(150), r2.Reason)		FROM @tblRequestInfo r2 WHERE r1.MessageId = r2.MessageId FOR XML PATH ('')), 1, 1, '') AS ReasonLst
			FROM @tblRequestInfo r1
			GROUP BY r1.MessageId, r1.TotalTickets
		)
		-- Bettype
		,CTE_Bettype AS
		(
			SELECT t.MessageId, b.typenamee AS TypeName
			FROM @tblRecommendBettype t
				INNER JOIN dbo.bettype b WITH (NOLOCK) ON t.bettype = b.typeid
			GROUP BY t.MessageId, b.typenamee
		)
		,CTE_MsgBettype AS
		(
			SELECT
				b1.MessageId
				,STUFF((SELECT DISTINCT ', ' + b2.TypeName FROM CTE_Bettype b2 WHERE b1.MessageId = b2.MessageId FOR XML PATH ('')), 1, 1, '') AS TypeNameLst
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
				,STUFF((SELECT DISTINCT ', ' + s2.site FROM CTE_Site s2 WHERE s1.MessageId = s2.MessageId AND isDeposit = 0 FOR XML PATH ('')), 1, 1, '') AS CreditSiteLst
				,STUFF((SELECT DISTINCT ', ' + s2.site FROM CTE_Site s2 WHERE s1.MessageId = s2.MessageId AND isDeposit = 1 FOR XML PATH ('')), 1, 1, '') AS DepositSiteLst
			FROM CTE_Site s1
			GROUP BY s1.MessageId
		)
		SELECT
			r.MessageID
			, r.TotalTickets
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
			INNER JOIN CTE_MsgBettype b ON r.MessageId = b.MessageId
			INNER JOIN CTE_MsgSite s ON r.MessageId = s.MessageId
	END
END
GO