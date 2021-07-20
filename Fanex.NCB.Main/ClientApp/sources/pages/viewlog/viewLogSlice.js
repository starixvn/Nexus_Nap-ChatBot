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
        systemMessage: {},
        recipientList: {},
        conversationMessage: {}
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
        setSystemMessage: (state,action) => {
            state.systemMessage = action.payload;
        },
        setRecipientList: (state,action) => {
            state.recipientList = action.payload;
        },
        setConversationMessage: (state,action) => {
            state.conversationMessage = action.payload;
        },
    }
});

const { actions, reducer } = viewLogSlice;

export const{
    setParamsPageSystemMessage,
    setParamsPageRecipientList,
    setParamsPageConversationMessage,
    setSystemMessage,
    setRecipientList,
    setConversationMessage
} = actions;

export default reducer;

