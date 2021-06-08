/*<info serverAlias="DBTXN-bodb02" executers="bodbAdminNet" isFunction="0" isNested="0"></info>*/
ALTER PROCEDURE [dbo].[NAP_Chatbot_SpecialDelete_AbnormalBets_GetUsernameLst]
	@MessageIDLst XML
AS

/*
	Created: 20210608@Malcolm
	Task: ChatBot - Get Usernames
	DB: DBTXN.bodb02

	Revisions:
		- 20210607@Malcolm: Initialize SP [RedmineID: #135686]

	@MessageIDLst:
			<root>
				<r MessageID="1" VoidingHistoryId="1" Usernames="xxx,u1"/>
				<r MessageID="1" VoidingHistoryId="2" Usernames="zzz,l"/>
				<r MessageID="3" VoidingHistoryId="2" Usernames="nn,m"/>
			</root>
		
*/

BEGIN
	SET NOCOUNT ON

	CREATE TABLE #MessageLst (MessageId INT, VoidingHistoryId INT, Username VARCHAR(30), CustId INT, IsValid BIT DEFAULT(0))

	INSERT INTO  #MessageLst (MessageId, VoidingHistoryId, Username)
	SELECT
		t.c.value('@MessageID', 'INT') AS MessageId
		,t.c.value('@VoidingHistoryId', 'INT') AS VoidingHistoryId
		,u.Username 
	FROM @MessageIDLst.nodes('root/r') AS t(c)
	CROSS APPLY
	(
		SELECT f.val AS Username
		FROM f_SplitString(t.c.value('@Usernames', 'NVARCHAR(MAX)'),',') f
	) AS u(Username)
	OPTION (OPTIMIZE FOR (@MessageIDLst = NULL))

	CREATE CLUSTERED INDEX Ix_VoidingHistory ON #MessageLst (VoidingHistoryId, MessageId)

	UPDATE m
		SET m.CustId = l.custid
	FROM #MessageLst m
	CROSS APPLY
	(
		SELECT TOP 1 c.roleid, c.custid
		FROM CustDB.dbo.Customer c WITH(NOLOCK)
		WHERE c.username = m.Username
	) l

	CREATE TABLE #Resutl (VoidingHistoryId INT,TransId BIGINT)
	CREATE CLUSTERED INDEX Ix_Result ON #Resutl (VoidingHistoryId)

	INSERT INTO #Resutl (VoidingHistoryId, TransId)
	SELECT VoidingHistoryId, transid
	FROM dbo.zzzbettrans b WITH(NOLOCK)
	WHERE EXISTS(SELECT 1 FROM #MessageLst m WHERE b.VoidingHistoryId = m.VoidingHistoryId)

	UPDATE m
		SET m.IsValid = 1
	FROM #MessageLst m
	WHERE EXISTS (
		SELECT 1 FROM #Resutl r 
		CROSS APPLY
		(
			SELECT TOP 1 b.custid, b.recommend, b.mrecommend, b.srecommend
			FROM dbo.bettrans b WITH(NOLOCK)
			WHERE b.transid = r.TransId
		) AS l
		WHERE m.VoidingHistoryId = r.VoidingHistoryId	
			AND m.CustId IN (l.custid, l.recommend, l.mrecommend, l.srecommend)
	)

	DELETE #MessageLst WHERE IsValid = 0

	;WITH CTE_Message AS
	(
		SELECT m.MessageId, m.VoidingHistoryId
		FROM #MessageLst m
		GROUP BY m.MessageId, m.VoidingHistoryId
	)
	SELECT 
		m.MessageId
		,m.VoidingHistoryId
		,STUFF((SELECT ',' + Username FROM #MessageLst l WHERE l.MessageId = m.MessageId AND l.VoidingHistoryId = m.VoidingHistoryId ORDER BY Username FOR XML PATH ('')), 1, 1, '') AS UsernameLst
	FROM CTE_Message m

	DROP TABLE #MessageLst, #Resutl

END
GO