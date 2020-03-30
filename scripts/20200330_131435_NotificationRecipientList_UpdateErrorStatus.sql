USE ChatBot;

/*
  Created: 20200330@Percy
  DB: ChatBot
  Task: Manually update status from Sending to Error - NotificationID = 10224
*/

#NotificationRecipientList
UPDATE NotificationRecipientList
  SET Status = 'Error'
      ,StatusID = 2
  WHERE NotificationID = 10224
	AND StatusID = 3;

#Notification
UPDATE Notification
  SET Status = 'Error'
      ,StatusID = 2
  WHERE NotificationID = 10224
	AND StatusID = 3;