DROP PROCEDURE IF EXISTS NCB_MsgSetting_SpecialDelete_AbnormalAccount_Sel;
DELIMITER //

CREATE DEFINER = 'ChatBotSystem'@'%' PROCEDURE NCB_MsgSetting_SpecialDelete_AbnormalAccount_Sel(
  /*
    Creator   : 20210423@Malcolm
    Task      : NAP ChatBot - Get Setting for Abnormal Account
    Params    :

    Revisions :
      - 20210423@Percy: Initiliaze SP [RedmineID: #152175]
  */
)
SQL SECURITY DEFINER
BEGIN
  # Variables
  DECLARE iLoop INT;
  DECLARE batchSize INT;
  DECLARE previousMaxMsgID INT;
  DECLARE templateContentType JSON;
  DECLARE msgContentTypeLst TEXT;
  
  # Get setting  
  SELECT Value INTO batchSize
    FROM SystemSetting
    WHERE ID = 19;

  SELECT Value INTO previousMaxMsgID
    FROM SystemSetting
    WHERE ID = 22;

	SELECT ss.Value INTO templateContentType
    FROM SystemSetting ss
    WHERE ss.ID = 20;

  DROP TEMPORARY TABLE IF EXISTS tmp_MsgContentType;
  CREATE TEMPORARY TABLE IF NOT EXISTS tmp_MsgContentType(
    MsgContentType INT
    );

  SET iLoop = 0;
  WHILE (JSON_EXTRACT(templateContentType, CONCAT('$[',iLoop, '].MsgContentsType'))  IS NOT NULL) DO
    INSERT INTO tmp_MsgContentType(MsgContentType)
      VALUES (
      JSON_UNQUOTE(JSON_EXTRACT(templateContentType, CONCAT('$[', iLoop, '].MsgContentsType')))
      );

    SET iLoop := iLoop + 1;
  END WHILE;

  SELECT GROUP_CONCAT(MsgContentType ORDER BY MsgContentType SEPARATOR ',') INTO msgContentTypeLst
    FROM tmp_MsgContentType;

  SELECT previousMaxMsgID AS MaxMessageID, batchSize AS BatchSize, msgContentTypeLst AS MsgContentsTypeLst;
END;

//
DELIMITER ;
GRANT EXECUTE ON PROCEDURE NCB_MsgSetting_SpecialDelete_AbnormalAccount_Sel TO 'ncb.admin'@'%';