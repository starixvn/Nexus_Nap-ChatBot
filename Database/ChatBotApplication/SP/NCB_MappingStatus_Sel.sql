CREATE PROCEDURE `NCB_MappingStatus_Sel`()
BEGIN
	SELECT SettingID as Id, SettingValue as Name
    FROM SystemSetting
    WHERE SettingType = 'MappingStatus'
    ORDER BY SettingID;
END