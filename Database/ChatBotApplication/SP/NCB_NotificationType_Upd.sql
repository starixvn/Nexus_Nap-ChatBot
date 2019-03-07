CREATE PROCEDURE `NCB_NotificationType_Upd`(
	IN id INT,
	IN notificationTypeName varchar(100), 
    IN recipientTypeIds varchar(500), 
    IN systemId INT,
    OUT errorCode INT, 
    OUT errorMsg nvarchar(100)
   )
BEGIN	 
	DECLARE strLen    INT DEFAULT 0;
	DECLARE SubStrLen INT DEFAULT 0;
    
	if exists(select * from NotificationType nt where nt.NotificationTypeName =  notificationTypeName and nt.NotificationTypeId <> id) then
		select 1 into errorCode;
        select "Notification Type Name exists" into errorMsg;
    else
		update NotificationType nt
        set nt.NotificationTypeName = notificationTypeName, nt.SystemId = systemId
        where nt.NotificationTypeId = id;
        
        delete from NotificationTypeRecipientType
        where NotificationTypeID = id;
        
        do_this:
		  LOOP			
            SET strLen = LENGTH(recipientTypeIds);            
            
            insert into NotificationTypeRecipientType(NotificationTypeID, RecipientTypeID)
            values (id, SUBSTRING_INDEX(recipientTypeIds, ',', 1));			

			SET SubStrLen = LENGTH(SUBSTRING_INDEX(recipientTypeIds, ',', 1));
			SET recipientTypeIds = MID(recipientTypeIds, SubStrLen + 2, strLen);

			IF recipientTypeIds = '' THEN
			  LEAVE do_this;
			END IF;
		  END LOOP do_this;
        
        select 0 into errorCode;
        select "" into errorMsg;
    end if;    
END