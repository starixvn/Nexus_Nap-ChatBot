CREATE DATABASE `ChatBot` /*!40100 DEFAULT CHARACTER SET utf8mb4 */;

Use ChatBot;

CREATE TABLE `BotConversation` (
  `GroupID` int(11) NOT NULL AUTO_INCREMENT,
  `ConversationID` varchar(70) NOT NULL,
  `Name` varchar(500) CHARACTER SET utf8 NOT NULL,
  `IsBotActive` bit(1) NOT NULL,
  `ChannelID` varchar(45) DEFAULT NULL,
  `BotName` varchar(50) NOT NULL,
  `LastUpdate` datetime NOT NULL,
  PRIMARY KEY (`GroupID`,`ConversationID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COMMENT='this table is used for bot service';
