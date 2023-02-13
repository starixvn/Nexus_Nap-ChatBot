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