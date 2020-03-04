/*
	Creator: Dominic@20191227
    Task: Chatbot For Licensee - Phase 2: Integrate Special Delete - Redmine: #122905
*/
#Step 1: Add a new column to ConversationGroup
ALTER TABLE ConversationGroup ADD COLUMN IsActive BIT  DEFAULT 1;

#Step 2: Add a new column to ConversationMessageLog
ALTER TABLE ConversationMessageLog ADD COLUMN ReferenceID INT NULL;

#Step 3: Add a new index to ConversationMessageLog
ALTER TABLE ConversationMessageLog ADD INDEX INDEX_ReferenceID USING BTREE (ReferenceID);

#Step 4: Change data type of column UserLog_RunSP.LogContent
ALTER TABLE UserLog_RunSP CHANGE COLUMN LogContent LogContent LONGTEXT CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_unicode_ci'  NOT NULL;