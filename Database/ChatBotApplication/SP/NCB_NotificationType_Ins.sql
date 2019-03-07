CREATE PROCEDURE `NCB_NotificationType_Ins`(
	IN notificationTypeName nvarchar(100), 
    IN recipientTypeIds nvarchar(500), 
    IN systemId INT,
    OUT errorCode INT,
    OUT errorMsg nvarchar(100)
   )
BEGIN	
	DECLARE strLen    INT DEFAULT 0;
	DECLARE SubStrLen INT DEFAULT 0;
    DECLARE notificationTypeId INT;
        
	if exists(select * from NotificationType where NotificationType.NotificationTypeName =  notificationTypeName) then
		select 1 into errorCode;
        select "Notification Type Name exists" into errorMsg;
    else
		insert into NotificationType(NotificationTypeName, SystemId)
        values (notificationTypeName, systemId);
        
        SET notificationTypeId = LAST_INSERT_ID();
        do_this:
		  LOOP			
            SET strLen = LENGTH(recipientTypeIds);            
            
            insert into NotificationTypeRecipientType(NotificationTypeID, RecipientTypeID)
            values (notificationTypeId, SUBSTRING_INDEX(recipientTypeIds, ',', 1));			

			SET SubStrLen = LENGTH(SUBSTRING_INDEX(recipientTypeIds, ',', 1));
			SET recipientTypeIds = MID(recipientTypeIds, SubStrLen + 2, strLen);

			IF recipientTypeIds = '' THEN
			  LEAVE do_this;
			END IF;
		  END LOOP do_this;
        
        select 0 into errorCode;
        select "Success" into errorMsg;
    end if;   
END