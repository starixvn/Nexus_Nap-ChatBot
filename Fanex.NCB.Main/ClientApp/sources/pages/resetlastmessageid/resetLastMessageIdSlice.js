import { createSlice } from '@reduxjs/toolkit';

const resetLastMessageIdSlice = createSlice({
    name: 'resetLastMessageId',
    initialState: {
        selected: [
            { id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0, settingName: 'PreviousMaxMessageId' },
            { id: 2, name: 'Special Delete - Internal CSD: Voiding Request', checked: true, newMaxMessageId: 0, settingName: 'LastVoidingHistoryCollectionId' },
            { id: 2, name: 'Special Delete - Internal CSD: Void by BetID', checked: true, newMaxMessageId: 0, settingName: 'LastMsgTargetId' },
            { id: 3, name: 'Site Message', checked: true, newMaxMessageId: 0, settingName: 'PreviousMaxMessageId' },
            { id: 4, name: 'Problem Player', checked: true, newMaxMessageId: 0, settingName: 'LastLogId' },
            { id: 5, name: 'Monitored Player', checked: true, newMaxMessageId: 0, settingName: 'LastLogId' },
            { id: 6, name: 'Settlement - Internal CSD', checked: true, newMaxMessageId: 0, settingName: 'LastMsgTargetId' },
            { id: 7, name: 'Settlement - Licensee', checked: true, newMaxMessageId: 0, settingName: 'LastMsgTargetId' },
        ],
        currentMessages: null,
        editNewMessagePopup: { show: false }
    },
    reducers: {
        setSelected: (state,action) =>{
            state.selected = action.payload;
        },
        setCurrentPreviousMaxMessage: (state,action) =>{
            state.currentMessages = action.payload;
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
    setEditNewMessagePopup
} = actions;

export default reducer;