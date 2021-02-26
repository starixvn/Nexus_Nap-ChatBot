/*
	Created: 20210226@Percy
    DB: ChatBot
    Task: Manually update data notification due to issue from service
    RedmineID: #150241
*/

USE ChatBot;

# Conversation Group has more than 1 Recipient
CREATE TEMPORARY TABLE tmp_ConversationGroup (ConversationGroupId INT);
INSERT INTO tmp_ConversationGroup (ConversationGroupId)
SELECT ConversationGroupId
FROM ConversationGroupRecipient
GROUP BY ConversationGroupId
HAVING COUNT(1) > 1;

#NotificationRecipientList
CREATE TEMPORARY TABLE tmp_NotificationRecipientList (NotificationId INT, ConversationGroupId INT, RecipientId INT);
INSERT INTO tmp_NotificationRecipientList (NotificationId, ConversationGroupId, RecipientId)
SELECT nrl.NotificationId, nrl.ConversationGroupId, nrl.RecipientId
FROM NotificationRecipientList nrl
	INNER JOIN tmp_ConversationGroup cg ON nrl.ConversationGroupId = cg.ConversationGroupId
WHERE nrl.StatusID = 3; #Sending
    
#Update sending to sent
UPDATE NotificationRecipientList nrl
	INNER JOIN tmp_NotificationRecipientList t ON nrl.NotificationId = t.NotificationId AND nrl.ConversationGroupId = t.ConversationGroupId AND nrl.RecipientId = t.RecipientId
SET nrl.Status = 'Sent', nrl.StatusId = 1;

UPDATE Notification n
SET n.Status = 'Sent', n.StatusId = 1
WHERE n.NotificationId IN (SELECT t.NotificationId FROM tmp_NotificationRecipientList t)
	AND n.StatusId = 3; #Sending

DROP TEMPORARY TABLE tmp_ConversationGroup;
DROP TEMPORARY TABLE tmp_NotificationRecipientList;