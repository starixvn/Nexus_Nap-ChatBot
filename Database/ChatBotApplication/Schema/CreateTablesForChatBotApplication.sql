Use ChatBot;

CREATE TABLE `System` (
  `SystemID` int NOT NULL AUTO_INCREMENT,  
  `SystemName` varchar(100) CHARACTER SET utf8 NOT NULL,  
  PRIMARY KEY (`SystemID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `NotificationType` (
  `NotificationTypeID` int NOT NULL AUTO_INCREMENT,  
  `NotificationTypeName` varchar(100) CHARACTER SET utf8 NOT NULL,  
  `SystemID` int NOT NULL,  
  PRIMARY KEY (`MessageTypeID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `NotificationTemplate` (
  `NotificationTemplateID` int NOT NULL AUTO_INCREMENT,  
  `NotificationTemplateName` varchar(500) CHARACTER SET utf8 NOT NULL,  
  `NotificationTypeID` int NOT NULL,  
  `Title` varchar(500) CHARACTER SET utf8 NOT NULL,  
  `Content` text(20000) CHARACTER SET utf8 NOT NULL,  
  `CreatorID` int NOT NULL,  
  `CreatedDate` datetime NOT NULL,  
  `LastModifiedDate` datetime NOT NULL,  
  `LastModifiedBy` int NOT NULL,  
  PRIMARY KEY (`TemplateID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Notification` (
  `NotificationID` int NOT NULL AUTO_INCREMENT,  
  `NotificationTemplateID` int NOT NULL,  
  `NotificationTypeID` int NOT NULL,  
  `Title` varchar(500) CHARACTER SET utf8 NOT NULL,  
  `Content` text(20000) CHARACTER SET utf8 NOT NULL,  
  `IsImmediate` bit NOT NULL,  
  `ConfigTime` datetime NOT NULL,  
  `CreatorID` int NOT NULL,  
  `CreatedDate` datetime NOT NULL,  
  `Remark` varchar(1000) CHARACTER SET utf8 NOT NULL,  
  `ResolvedBy` int NOT NULL,  
  `ResolvedDate` datetime NOT NULL,
  PRIMARY KEY (`MessageID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `RecipientType` (
  `RecipientTypeID` int NOT NULL AUTO_INCREMENT,  
  `RecipientTypeName` varchar(500) CHARACTER SET utf8 NOT NULL,  
  PRIMARY KEY (`RecipientTypeID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `NotificationTypeRecipientType` (
  `NotificationTypeID` int NOT NULL,  
  `RecipientTypeID`  int NOT NULL ,  
  PRIMARY KEY (`MessageTypeID`, `RecipientTypeID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Conversation` (
  `ConversationID` int NOT NULL,  
  `BotName` varchar(500) CHARACTER SET utf8 NOT NULL,  
  `RecipientTypeID` int NOT NULL,  
  `Remark` varchar(1000) CHARACTER SET utf8 NOT NULL,  
  PRIMARY KEY (`ConversationID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `ConversationRecipient` (
  `ConversationID` int NOT NULL,  
  `RecipientID` int NOT NULL,  
  `CreatedDate` datetime NOT NULL,
  `CreatorID` int NOT NULL,  
  PRIMARY KEY (`ConversationID`, `RecipientID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `NotificationRecipient` (
  `RecipientID` int NOT NULL,  
  `RecipientTypeID` int NOT NULL,  
  `NotificationID` int NOT NULL,  
  `SentTime` datetime NOT NULL,
  `Status` varchar(50) CHARACTER SET utf8 NOT NULL,  
  `StatusID` tinyint NOT NULL,  
  `Remark` varchar(1000) CHARACTER SET utf8 NOT NULL,  
  PRIMARY KEY (`RecipientID`, `RecipientTypeID`, `MessageID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `LogAction` (
  `ActionID` int NOT NULL AUTO_INCREMENT,  
  `ActionName` varchar(500) CHARACTER SET utf8 NOT NULL,  
  `SPName` varchar(200) CHARACTER SET utf8 NOT NULL,  
  PRIMARY KEY (`ActionID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `UserActionLog` (
  `LogID` bigint NOT NULL AUTO_INCREMENT,  
  `LogDate` datetime NOT NULL,
  `ActionID` int NOT NULL,  
  `UserID` int NOT NULL,  
  `TrackingValue` text(20000) CHARACTER SET utf8 NOT NULL,  
  `TrackingValue2` text(20000) CHARACTER SET utf8 NOT NULL,  
  `Duration` int NOT NULL,    
  PRIMARY KEY (`LogID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Licensee` (
  `LicenseeID` int NOT NULL,  
  `LicenseeName` varchar(100) CHARACTER SET utf8 NOT NULL,  
  `RegisteredLicenseeName` varchar(100) CHARACTER SET utf8 NOT NULL,  
  `LicenseeStatusId` tinyint NOT NULL,  
  `LicenseeTypeId` tinyint NOT NULL,  
  PRIMARY KEY (`LicenseeID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `LicenseeType` (
  `LicenseeTypeID` int NOT NULL,  
  `LicenseeTypeName` varchar(100) CHARACTER SET utf8 NOT NULL,    
  PRIMARY KEY (`LicenseeTypeID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Product` (
  `ProductID` int NOT NULL,  
  `ProductName` varchar(100) CHARACTER SET utf8 NOT NULL,    
  PRIMARY KEY (`ProductID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `LicenseeProduct` (
  `LicenseeID` int NOT NULL,  
  `ProductID` int NOT NULL,  
  `LaunchDate` datetime NOT NULL,
  `TerminationDate` datetime NOT NULL,
  PRIMARY KEY (`LicenseeID`, `ProductID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `LicenseeNotificationType` (
  `LicenseeID` int NOT NULL,  
  `NotificationTypeID` int NOT NULL,  
  `IsSkipped` bit NOT NULL,  
  PRIMARY KEY (`LicenseeID`, `MessageTypeID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `SystemSetting` (
  `SettingId` INT NOT NULL,
  `SettingValue` varchar(500) CHARACTER SET utf8 NOT NULL,
  `SettingType` varchar(100) CHARACTER SET utf8 NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8,  
  PRIMARY KEY (`SettingId`, `SettingType`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Users` (
  `UserID` int NOT NULL,  
  `UserName` varchar(100) CHARACTER SET utf8 NOT NULL,
  `AuthKey` varchar(100) CHARACTER SET utf8 NOT NULL,
  `Claims` varchar(4000) CHARACTER SET utf8 NOT NULL,  
  `LastUpdate` datetime NOT NULL,
  `LoginProvider` varchar(100) CHARACTER SET utf8 NOT NULL,
  `ProviderKey` varchar(100) CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `ExternalSetting` (
  `Provider` varchar(100) CHARACTER SET utf8 NOT NULL,
  `ProviderKey` varchar(100) CHARACTER SET utf8 NOT NULL,
  `SettingName` varchar(100) CHARACTER SET utf8 NOT NULL,
  `SettingValue` varchar(4000) CHARACTER SET utf8 NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8 NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `SystemLogs` (
  `LogID` int NOT NULL AUTO_INCREMENT,  
  `LogType` tinyint NOT NULL,  
  `Message` text(20000) CHARACTER SET utf8 NOT NULL,
  `LogDate` datetime NOT NULL,
  `IssueBy` varchar(100) CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`LogID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

