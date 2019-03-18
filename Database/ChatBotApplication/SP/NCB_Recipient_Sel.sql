CREATE PROCEDURE `NCB_Recipient_Sel`()
BEGIN
	select RecipientId as ID, SourceName as Name, SourceStatusName as Status, RecipientTypeID
    from Recipient;
END