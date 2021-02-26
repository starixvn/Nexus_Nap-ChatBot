/*
	Created: 20210226@Percy
    DB: ChatBot
    Task: Manually update data notification due to issue from service
    RedmineID: #150241
*/

USE ChatBot;

#NotificationRecipientList
CREATE TEMPORARY TABLE tmp_NotificationRecipientList (NotificationId INT, ConversationGroupId INT, SentTime DATETIME);
INSERT INTO tmp_NotificationRecipientList (NotificationId, ConversationGroupId, SentTime)
SELECT NotificationId, ConversationGroupId, SentTime
FROM (
	SELECT nrl.NotificationId, nrl.ConversationGroupId, nrl.SentTime
		, ROW_NUMBER() OVER(PARTITION BY nrl.NotificationId, nrl.ConversationGroupId ORDER BY nrl.SentTime) AS RN
	FROM NotificationRecipientList nrl
	WHERE nrl.NotificationId IN (50542,50543,50545,50547,50549,50552,50555,50557,50558,50559,50561,50562,50568,50570,50573)
		AND nrl.StatusId = 1
) t
WHERE t.RN = 1;

SELECT *
FROM Notification n
WHERE n.NotificationId IN (SELECT t.NotificationId FROM tmp_NotificationRecipientList t)
	AND n.StatusId = 3; #Sending
    
UPDATE Notification n
SET n.Status = 'Sent', n.StatusId = 1
WHERE n.NotificationId IN (SELECT t.NotificationId FROM tmp_NotificationRecipientList t)
	AND n.StatusId = 3;
    
UPDATE NotificationRecipientList nrl
	INNER JOIN tmp_NotificationRecipientList t ON nrl.NotificationId = t.NotificationId
		AND nrl.ConversationGroupId = t.ConversationGroupId
SET nrl.Status = 'Sent', nrl.StatusId = 1, nrl.SentTime = t.SentTime
WHERE nrl.StatusId = 3;

DROP TEMPORARY TABLE tmp_NotificationRecipientList;