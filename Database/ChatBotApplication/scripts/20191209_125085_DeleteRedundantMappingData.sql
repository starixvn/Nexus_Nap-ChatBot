/*
	Task: Delete mapping data for ConversationGroupID = 2
    Redmine: https://nexdev.net/issues/125085
    Server: 10.99.99.100:3306 (PRO)
    Database: ChatBot
*/

DELETE cgr FROM ChatBot.ConversationGroupRecipient cgr WHERE cgr.ConversationGroupID = 2;

DELETE extra FROM ChatBot.ConversationGroupExtraInfo extra WHERE extra.ConversationGroupID = 2;