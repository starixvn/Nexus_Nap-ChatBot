DELIMITER //

DROP PROCEDURE IF EXISTS NCB_Common_NotificationStatus_Sel;

CREATE PROCEDURE  NCB_Common_NotificationStatus_Sel(
    /*
		Creator: Dominic@20190408
        Param:

        Revisions:
				- 20190408@Dominic: First Commit
        
    */
)
BEGIN
	#Hard coding and not create any physical tables to store data because this kind of data
    # is rarely changed, we gonna consider later
	SELECT StatusID AS ID, StatusName AS Name
    FROM (
		SELECT 0 StatusID,  'Planned' StatusName
		UNION ALL
		SELECT 1 StatusID,  'Sent' StatusName
		UNION ALL
		SELECT 2 StatusID,  'Error' StatusName
		UNION ALL
		SELECT 3 StatusID,  'Sending' StatusName
		UNION ALL
		SELECT 4 StatusID,  'Cancelled' StatusName
		UNION ALL
		SELECT 5 StatusID,  'Resolved' StatusName
    ) AS tmp
    ORDER BY StatusID;
END;

GRANT EXECUTE ON PROCEDURE ChatBot.NCB_Common_NotificationStatus_Sel TO 'ncb.admin'@'%';