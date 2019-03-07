CREATE PROCEDURE `NCB_NotificationType_Del`(
	IN notificationTypeId INT,    
    OUT errorCode INT,
    OUT errorMsg nvarchar(100)
   )
BEGIN	     
	delete from NotificationType
    where NotificationType.notificationTypeId = notificationTypeId;
    
	select 0 into errorCode;
	select "Success" into errorMsg;    
END