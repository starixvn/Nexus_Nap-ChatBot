import { createSlice } from '@reduxjs/toolkit';

const resetLastMessageIdSlice = createSlice({
    name: 'resetLastMessageId',
    initialState: {
        selected: [
            { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0 },
            { id: 2, name: 'Special Delete - Internal CSD', checked: true, newMaxMessageId: 0 },
            { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0 },
            { id: 4, name: 'Problem Player', checked: true, newMaxMessageId: 0 },
            { id: 5, name: 'Monitored Player', checked: true, newMaxMessageId: 0 }
        ],
        currentMessages: null,
        newMessages: null,
        missingMessageCsd: null,
        missingMessages: null,
        missingProblemAccount: null,
        missingMonitoredPlayer: null,
        editNewMessagePopup: { show: false }
    },
    reducers: {
        setSelected: (state,action) =>{
            state.selected = action.payload;
        },
        setCurrentPreviousMaxMessage: (state,action) =>{
            state.currentMessages = action.payload;
        },
        setNewPreviousMaxMessage: (state,action) =>{
            state.newMessages = action.payload;
        },
        setMissingMessageCsd: (state,action) =>{
            state.missingMessageCsd = action.payload;
        },
        setMissingMessageInfo: (state,action) =>{
            state.missingMessages = action.payload;
        },
        setMissingProblemAccount: (state,action) =>{
            state.missingProblemAccount = action.payload;
        },
        setMissingMonitoredPlayer: (state,action) =>{
            state.missingMonitoredPlayer = action.payload;
        },
        setEditNewMessagePopup: (state,action) =>{
            state.editNewMessagePopup = action.payload;
        }
    }
});

const { actions, reducer } = resetLastMessageIdSlice;

export const{
    setSelected,
    setCurrentPreviousMaxMessage,
    setNewPreviousMaxMessage,
    setMissingMessageCsd,
    setMissingMessageInfo,
    setMissingProblemAccount,
    setMissingMonitoredPlayer,
    setEditNewMessagePopup
} = actions;

export default reducer;