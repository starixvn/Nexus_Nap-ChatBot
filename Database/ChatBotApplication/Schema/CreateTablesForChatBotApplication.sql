CREATE DATABASE ChatBot CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_520_ci ;


CREATE TABLE ChatBot.BotConversation(
		ID INT AUTO_INCREMENT NOT NULL
		, ConversationID VARCHAR(70) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NOT NULL
		, Name VARCHAR(500) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NULL
		, ChannelID VARCHAR(45) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'  NULL
		, UpdatedBy VARCHAR(50) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NOT NULL
		, LastUpdate DATETIME NOT NULL
		, ServiceUrl VARCHAR(500) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'  NULL
		, CONSTRAINT PK_ID  PRIMARY KEY (ID)
		, UNIQUE INDEX UNIQUE_ConversationID USING BTREE (ConversationID)
  );

CREATE TABLE ChatBot.BotConversationMember(
		 ID INT AUTO_INCREMENT NOT NULL
		, MemberID VARCHAR(70) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NOT NULL
		, ConversationID VARCHAR(70) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NOT NULL
		, Name VARCHAR(70) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NULL
		, LastUpdate DATETIME NOT NULL
		, IsBot BIT NOT NULL
		, IsActive BIT NOT NULL
		, UpdatedBy VARCHAR(50) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'  NOT NULL
        , CONSTRAINT PK_ID  PRIMARY KEY (ID)
        , INDEX INDEX_ConversationID USING BTREE (ConversationID)
        , UNIQUE INDEX UNIQUE_MemberID_ConversationID USING BTREE (ConversationID, MemberID)
  );
  
CREATE TABLE ChatBot.System(
		SystemID SMALLINT  AUTO_INCREMENT NOT NULL
        , SystemName VARCHAR(100) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NOT NULL
        , CONSTRAINT PK_SystemID PRIMARY KEY (SystemID)
        , UNIQUE INDEX UNIQUE_SystemName USING BTREE (SystemName)
);

CREATE TABLE ChatBot.RecipientType(
		RecipientTypeID SMALLINT  AUTO_INCREMENT NOT NULL
        , RecipientTypeName VARCHAR(200) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NOT NULL
        , CONSTRAINT PK_RecipientTypeID PRIMARY KEY (RecipientTypeID)
        , UNIQUE INDEX UNIQUE_RecipientTypeName USING BTREE (RecipientTypeName)
);

CREATE TABLE ChatBot.Recipient(
		RecipientID INT  AUTO_INCREMENT NOT NULL
        , RecipientName VARCHAR(200) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NOT NULL
        , RecipientTypeID SMALLINT NOT NULL
        , SourceID INT NULL
        , SourceDescription VARCHAR(200) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'  NULL
        , CONSTRAINT PK_RecipientID PRIMARY KEY (RecipientID)
        , UNIQUE INDEX UNIQUE_RecipientName USING BTREE (RecipientName)
        , INDEX INDEX_RecipientTypeID USING BTREE (RecipientTypeID)
);

CREATE TABLE ChatBot.BotConversationRecipient(
		BotConversationID INT  NOT NULL
        , RecipientID INT  NOT NULL
        , CONSTRAINT PK_BotConversationID_RecipientID PRIMARY KEY (BotConversationID,RecipientID)
);

CREATE TABLE ChatBot.ChatGroupType(
		ChatGroupTypeID SMALLINT  AUTO_INCREMENT NOT NULL
        , ChatGroupTypeName VARCHAR(200) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NOT NULL
        , CONSTRAINT PK_ChatGroupTypeID PRIMARY KEY (ChatGroupTypeID)
        , UNIQUE INDEX UNIQUE_ChatGroupTypeName USING BTREE (ChatGroupTypeName)
);

CREATE TABLE ChatBot.BotConversationExtraInfo(
		BotConversationID INT  NOT NULL
        , ChatGroupTypeID SMALLINT  NULL
         , Remark VARCHAR(200) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NULL
        , CONSTRAINT PK_BotConversationID PRIMARY KEY (BotConversationID)
);

CREATE TABLE ChatBot.NotificationType(
		NotificationTypeID SMALLINT  AUTO_INCREMENT NOT NULL
        , NotificationTypeName VARCHAR(200) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NOT NULL
        , SystemID SMALLINT  NOT NULL
        , CONSTRAINT PK_NotificationTypeID PRIMARY KEY (NotificationTypeID)
        , UNIQUE INDEX UNIQUE_NotificationTypeName USING BTREE (NotificationTypeName)
        , INDEX INDEX_SystemID USING BTREE (SystemID)
);

CREATE TABLE ChatBot.NotificationChatGroupType(
		NotificationTypeID SMALLINT  NOT NULL
        , ChatGroupTypeID SMALLINT  NOT NULL
        , CONSTRAINT PK_NotificationTypeID_ChatGroupTypeID PRIMARY KEY (NotificationTypeID,ChatGroupTypeID)
);

CREATE TABLE ChatBot.NotificationRecipientType(
		NotificationTypeID SMALLINT  NOT NULL
        , RecipientTypeID SMALLINT  NOT NULL
        , CONSTRAINT PK_NotificationTypeID_RecipientTypeID PRIMARY KEY (NotificationTypeID,RecipientTypeID)
);

CREATE TABLE ChatBot.NotificationTypeRecipientException(
		NotificationTypeID SMALLINT  NOT NULL
        , RecipientID INT  NOT NULL
        , CONSTRAINT PK_NotificationTypeID_RecipientID PRIMARY KEY (NotificationTypeID,RecipientID)
);

CREATE TABLE ChatBot.NotificationTemplate(
		 NotificationTemplateID INT AUTO_INCREMENT NOT NULL
		, NotificationTemplateName  VARCHAR(200) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NOT NULL
		, NotificationTypeID SMALLINT NOT NULL
		, Title  VARCHAR(300) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NOT NULL
		, Content MEDIUMTEXT CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NOT NULL
		, CreatorID INT NOT NULL
		, CreatedDate DATETIME NOT NULL
		, LastModifiedDate DATETIME NULL
		, LastModifiedBy DATETIME NULL
        , CONSTRAINT PK_NotificationTemplateID  PRIMARY KEY (NotificationTemplateID)
        , INDEX INDEX_NotificationTypeID USING BTREE (NotificationTypeID)
        , UNIQUE INDEX UNIQUE_NotificationTypeID_NotificationTemplateName USING BTREE (NotificationTypeID, NotificationTemplateName)
  );

  CREATE TABLE ChatBot.Notification(
		 NotificationID INT AUTO_INCREMENT NOT NULL
		, NotificationTypeID SMALLINT NOT NULL
		, Title VARCHAR(300) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NOT NULL
		, Content MEDIUMTEXT CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci' NOT NULL
		, ConfigSendTime DATETIME NOT NULL
		, CreatorID INT NOT NULL
		, CreatedDate DATETIME NOT NULL
        , StatusID TINYINT UNSIGNED NOT NULL
		, ResolvedRemark VARCHAR(1000) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'  NULL
		, ResolvedBy INT NULL
		, ResolvedDate DATETIME NULL
        , CONSTRAINT PK_NotificationID  PRIMARY KEY (NotificationID)
  );

CREATE TABLE ChatBot.NotificationRecipientList(
		NotificationID INT  NOT NULL
        , BotConversationID  INT  NOT NULL
        , ChatGroupTypeID SMALLINT  NOT NULL
        , RecipientID INT  NOT NULL
		, SentTime DATETIME NULL
		, Status VARCHAR(10) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'  NOT NULL
		, StatusID TINYINT UNSIGNED NOT NULL
		, ErrMsg VARCHAR(1000) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'  NULL
        , CONSTRAINT PK_NotificationID_BotConversationID_RecipientID_ChatGroupTypeID PRIMARY KEY (NotificationID, BotConversationID, RecipientID, ChatGroupTypeID)
);

CREATE TABLE ChatBot.Licensee(
		LicenseeId INT  NOT NULL
		, LicenseeName VARCHAR(50) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'   NULL
		, RegisteredLicenseeName VARCHAR(50) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'   NULL
		, LicenseeStatusId TINYINT UNSIGNED NULL
		, LicenseeTypeId TINYINT UNSIGNED NULL
        , CONSTRAINT PK_LicenseeId PRIMARY KEY (LicenseeId)
);

CREATE TABLE ChatBot.LicenseeType(
		LicenseeTypeId TINYINT UNSIGNED  NOT NULL
		, LicenseeTypeName VARCHAR(30) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'   NULL
        , CONSTRAINT PK_LicenseeTypeId PRIMARY KEY (LicenseeTypeId)
);

CREATE TABLE ChatBot.Product(
		ProductId TINYINT UNSIGNED NOT NULL
		, ProductName VARCHAR(50) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'   NULL
        , CONSTRAINT PK_ProductId PRIMARY KEY (ProductId)
);

CREATE TABLE ChatBot.LicenseeProduct(
		LicenseeId INT  NOT NULL
		, ProductId TINYINT UNSIGNED NOT NULL
		, LaunchDate DATETIME NULL
        , TerminationDate DATETIME NULL
        , CONSTRAINT PK_LicenseeId_ProductId PRIMARY KEY (LicenseeId, ProductId)
);

CREATE TABLE ChatBot.LicenseeStatus(
		LicenseeStatusId TINYINT UNSIGNED NOT NULL
		, LicenseeStatusName VARCHAR(30) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'   NULL
        , CONSTRAINT PK_LicenseeStatusId PRIMARY KEY (LicenseeStatusId)
);

CREATE TABLE ChatBot.LogAction(
		ActionID SMALLINT  NOT NULL
		, ActionName VARCHAR(500) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'   NOT NULL
        , SPName VARCHAR(50) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'   NOT NULL
        , CONSTRAINT PK_ActionID PRIMARY KEY (ActionID)
);

CREATE TABLE ChatBot.UserLog_RunSP(
		 LogID BIGINT AUTO_INCREMENT NOT NULL
		, LogTime DATETIME NOT NULL
		, ActionID SMALLINT  NOT NULL
		, UserID INT NOT NULL
        , LogContent TEXT CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'  NOT NULL
        , Duration INT NOT NULL
		, CONSTRAINT PK_LogID PRIMARY KEY (LogID)
        , INDEX INDEX_LogTime USING BTREE (LogTime)
 );        
 
CREATE TABLE ChatBot.SystemLog(
		LogID BIGINT AUTO_INCREMENT NOT NULL
        , MachineName VARCHAR(50) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'   NOT NULL
        , LogTime DATETIME NOT NULL
		, Level VARCHAR(50) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'   NOT NULL
        , LogContent TEXT CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'  NOT NULL
        , Logger VARCHAR(50) CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'   NOT NULL
		, Callsite TEXT CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'   NULL
        , Exception TEXT CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_520_ci'   NULL
        , CONSTRAINT PK_LogID PRIMARY KEY (LogID)
        , INDEX INDEX_LogTime USING BTREE (LogTime)
);        

CREATE TABLE ChatBot.User(
		UserID INT NOT NULL
        , UserName VARCHAR(100) NOT NULL
		, AuthKey 	VARCHAR(40) NULL
        , Claims 	TEXT NULL
		, LastUpdate 	DATETIME NULL
		, LoginProvider 	VARCHAR(25) NULL
		, ProviderKey 	TEXT NULL
        , CONSTRAINT PK_UserID PRIMARY KEY (UserID)
        , UNIQUE INDEX UNIQUE_UserName USING BTREE (UserName)
);       
