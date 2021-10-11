import { createSlice } from '@reduxjs/toolkit';

const viewLogSlice = createSlice({
    name: 'ViewLog',
    initialState: {
        paramsPageSystemMessage: {
            pageIndex: 1,
            pageSize: 100
        },
        paramsPageRecipientList: {
            pageIndex: 1,
            pageSize: 100
        },
        paramsPageConversationMessage: {
            pageIndex: 1,
            pageSize: 100
        },
        systemMessages: {},
        recipients: {},
        conversationMessages: {}
    },
    reducers: {
        setParamsPageSystemMessage: (state,action) =>{
            state.paramsPageSystemMessage = action.payload;
        },
        setParamsPageRecipientList: (state,action) =>{
            state.paramsPageRecipientList = action.payload;
        },
        setParamsPageConversationMessage: (state,action) =>{
            state.paramsPageConversationMessage = action.payload;
        },
        setNotificationSystemMessage: (state,action) => {
            state.systemMessages = action.payload;
        },
        setNotificationRecipientList: (state,action) => {
            state.recipients = action.payload;
        },
        setConversationMessageLog: (state,action) => {
            state.conversationMessages = action.payload;
        },
    }
});

const { actions, reducer } = viewLogSlice;

export const{
    setParamsPageSystemMessage,
    setParamsPageRecipientList,
    setParamsPageConversationMessage,
    setNotificationSystemMessage,
    setNotificationRecipientList,
    setConversationMessageLog
} = actions;

export default reducer;

