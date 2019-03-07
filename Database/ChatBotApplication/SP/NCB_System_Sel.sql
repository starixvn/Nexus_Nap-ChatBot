CREATE PROCEDURE `NCB_System_Sel`()
BEGIN
  SELECT SystemID AS Id, SystemName AS Name
  FROM System;
END