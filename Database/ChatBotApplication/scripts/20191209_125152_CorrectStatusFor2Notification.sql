/*
	Task: Update Sent Status for 2 Notifications( NotificationID=383, 399 ) 
    Redmine: https://nexdev.net/issues/125152
*/

#11 affected rows
UPDATE ChatBot.NotificationRecipientList
SET Status='Sent', StatusID=1
WHERE NotificationID IN (383, 399) AND Status='Sending';

#2 affected rows
UPDATE ChatBot.Notification
SET Status='Error', StatusID=2
WHERE NotificationID IN (383, 399) AND Status='Sending';