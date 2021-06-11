import { createSlice } from '@reduxjs/toolkit';

const notificationTypeSlice = createSlice({
    name: 'NotificationType',
    initialState: {
        settings: {},
        gridParams: {},
        gridData: null,
        activeRowIndex: -1,
        createEditPopupData: { isShowed: false },
        createEditResult: null,
        deletePopupData: { isShowed: false },
        deleteResult: null
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
        setActiveRowIndex: (state, action) => {
            state.activeRowIndex = action.payload;
        },
        setCreateEditPopupData: (state, action) => {
            state.createEditPopupData = action.payload;
        },
        setCreateEditResult: (state, action) => {
            state.createEditResult = action.payload;
        },
        setDeletePopupData: (state, action) => {
            state.deletePopupData = action.payload;
        },
        setDeleteResult: (state, action) => {
            state.deleteResult = action.payload;
        }
    }
});

const { actions, reducer } = notificationTypeSlice;

export const {
    setSettings,
    setGridParams,
    setGridData,
    setActiveRowIndex,
    setCreateEditPopupData,
    setCreateEditResult,
    setDeletePopupData,
    setDeleteResult
} = actions;

export default reducer;