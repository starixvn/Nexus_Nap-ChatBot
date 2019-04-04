DELIMITER //

DROP PROCEDURE IF EXISTS NCB_Common_NotificationType_Sel;

CREATE PROCEDURE  NCB_Common_NotificationType_Sel(
    /*
		Creator: Dominic@20190401
        Param:

        Revisions:
				- 20190401@Dominic: First Commit
        
    */
)
BEGIN
	SELECT nt.NotificationTypeID, nt.NotificationTypeName
    FROM NotificationType nt
    ORDER BY nt.NotificationTypeName;
    
END;