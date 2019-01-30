
CREATE DATABASE `ChatBot` /*!40100 DEFAULT CHARACTER SET latin1 */;

Use ChatBot;

CREATE TABLE `BotConversation` (
  `ID` varchar(70) NOT NULL,
  `ConversationID` smallint(5) NOT NULL,
  `Name` varchar(2048) CHARACTER SET utf8 NOT NULL,
  `IsBotActive` bit(1) NOT NULL,
  `ChannelID` varchar(45) NOT NULL,
  `BotName` varchar(50) CHARACTER SET utf8 NOT NULL,
  `LastUpdate` datetime NOT NULL,
  PRIMARY KEY (`ID`,`ConversationID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COMMENT='this table is used for bot service';
