/*
	Created: 20210226@Percy
    DB: ChatBot
    Task: Manually update data notification due to issue from service
    RedmineID: #150241
	
    Revisions:
		- 20210308@Percy: Update NotificationId list
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
	WHERE nrl.NotificationId IN (51216,51217,51218,51219,51220)
) t
WHERE t.RN = 1;

UPDATE tmp_NotificationRecipientList nrl
	INNER JOIN Notification n ON nrl.NotificationId = n.NotificationId
SET nrl.SentTime = n.CreatedDate
WHERE nrl.SentTime IS NULL;
    
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