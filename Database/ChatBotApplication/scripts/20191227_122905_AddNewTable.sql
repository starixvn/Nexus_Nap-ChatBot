/*
	Creator: Dominic@20191227
    Task: Chatbot For Licensee - Phase 2: Integrate Special Delete - Redmine: #122905
*/
CREATE TABLE ChatBot.NotificationSystemMsg( 
		  MessageID INT UNSIGNED NOT NULL
		, NotificationID INT UNSIGNED NULL
        , CheckMsg TEXT CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_ci'  NULL
        , CreatedDate DATETIME NOT NULL
        , CONSTRAINT PK_MessageID  PRIMARY KEY (MessageID)
        , INDEX INDEX_NotificationID USING BTREE (NotificationID)
);
    
CREATE TABLE ChatBot.SystemSetting( 
		  ID INT UNSIGNED NOT NULL
        , Description TEXT CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_ci'  NULL
		, Value TEXT CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_ci'  NULL
        , CONSTRAINT PK_ID  PRIMARY KEY (ID)
);