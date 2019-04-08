DELIMITER //

DROP PROCEDURE IF EXISTS NCB_NotificationList_Sel;

CREATE PROCEDURE  NCB_NotificationList_Sel(
     IN notificationTypeIDLst VARCHAR(200)
    , IN recipientIDLst VARCHAR(200)
	, IN creatorIDLst VARCHAR(200)
    , IN notificationStatusIDLst VARCHAR(200)
	, IN configSendTimeFrom DATETIME
    , IN configSendTimeTo DATETIME
  
    /*
		Creator: Dominic@20190408
        Param:
				
        Revisions:
				- 20190401@Dominic: First Commit
        
    */
)
BEGIN
	select 1;
    
END;