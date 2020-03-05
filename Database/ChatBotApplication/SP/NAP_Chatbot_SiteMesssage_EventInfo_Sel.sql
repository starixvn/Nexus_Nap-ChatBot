/*<info serverAlias="DBACC-bodb02" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/
ALTER PROCEDURE [dbo].[NAP_Chatbot_SiteMesssage_EventInfo_Sel]
	@MatchIDLst VARCHAR(MAX)
AS

/*
	Created: 20200305@Percy
	Task: ChatBot - Get Event Info from Site Message
	DB: DBACC.bodb02  

	Revisions:
		- 20200305@Percy: Initialize SP [RedmineID: #126143]
*/

BEGIN
	SET NOCOUNT ON

	IF ISNULL(@MatchIDLst , '') = ''
		RETURN

	CREATE TABLE #MatchLst (MatchId INT)

	DECLARE @XML XML  = NULL
	SET @XML = N'<root><r>' + REPLACE(@MatchIDLst, ',', '</r><r>') + '</r></root>'
	
	INSERT INTO #MatchLst (MatchId)
	SELECT r.value('.', 'VARCHAR(30)') AS MatchId
	FROM @xml.nodes('//root/r') AS RECORDS(r)
	WHERE LEN(ISNULL(r.value('.', 'VARCHAR(30)'), '')) > 0

	SELECT
		m.MatchId, m.SportType, m.KickOffTime
		,s.SportName AS SportNameEN, s.SportNameCS
		,l.IsTest, l.leaguename AS LeagueNameEN, l.leaguenamecn AS LeagueNameCS
		,h.teamname AS HomeNameEN, h.teamnamecn AS HomeNameCS
		,a.teamname AS AwayNameEN, a.teamnamecn AS AwayNameCS
	FROM dbo.match m WITH (NOLOCK)
		INNER JOIN dbo.League l WITH (NOLOCK) ON l.leagueid = m.leagueid
		INNER JOIN dbo.Sports s WITH (NOLOCK) ON s.SportType = m.sporttype
		INNER JOIN dbo.Team h WITH (NOLOCK) ON h.teamid = m.homeid
		INNER JOIN dbo.Team a WITH (NOLOCK) ON a.teamid = m.awayid
	WHERE EXISTS (SELECT 1 FROM #MatchLst ml WHERE m.matchid = ml.MatchId)
	
	DROP TABLE #MatchLst
END
GO