import { createSlice } from '@reduxjs/toolkit';

const viewLogSlice = createSlice({
    name: 'ViewLog',
    initialState: {
        paramsPage: {
            pageIndex: 1,
            pageSize: 100
        },
        systemMessage: {},
        recipientList: {},
        conversationMessage: {}
    },
    reducers: {
        setParamsPage: (state,action) =>{
            state.paramsPage = action.payload;
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
    setParamsPage,
    setSystemMessage,
    setRecipientList,
    setConversationMessage
} = actions;

export default reducer;

