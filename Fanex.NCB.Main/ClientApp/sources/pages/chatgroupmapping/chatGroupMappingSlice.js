import { createSlice } from '@reduxjs/toolkit';

const chatGroupMappingSlice = createSlice({
    name: 'ChatGroupMapping',
    initialState: {
        settings: {},
        gridParams: {},
        gridData: null,
        filteredRecipients: [],
        editPopupData: { isShowed: false },
        editResult: null
    },
    reducers: {
        setSettings: (state, action) => {
            state.settings = action.payload;
        },
        setGridParams: (state, action) => {
            state.gridParams = action.payload;
        },
        setGridData: (state, action) => {
            state.gridData = action.payload;
        },
        filterRecipients: (state, action) => {
            const { recipientTypeId, recipients } = action.payload;
            const filteredRecipients = [];
            recipientTypeId && recipients.map(item => {
                if (item.recipientTypeId === parseInt(recipientTypeId, 10)) {
                    filteredRecipients.push(item);
                }
            });

            state.filteredRecipients = filteredRecipients;
        },
        setEditPopupData: (state, action) => {
            state.editPopupData = action.payload;
        },
        setEditResult: (state, action) => {
            state.editResult = action.payload;
        }
    }
});

const { actions, reducer } = chatGroupMappingSlice;

export const {
    setSettings,
    setGridParams,
    setGridData,
    filterRecipients,
    setEditPopupData,
    setEditResult
} = actions;

export default reducer;