import { createSlice } from '@reduxjs/toolkit';

const newNotificationSlice = createSlice({
    name: 'NewNotification',
    initialState: {
        notificationTypes: [],
        notificationTemplates: [],
        products: [],
        recipients: {
            recipientOptions: [],
            selectedRecipients: []
        },
        confirmationPopup: {
            isOpen: false,
            content: ''
        },
        resetConfirmationPopup: {
            isOpen: false
        },
        editConfirmationPopup: {
            isOpen: false
        },
        markdownRulesPopup: {
            isOpen: false
        },
        createNotificationResult: null
    },
    reducers: {
        setNotificationTypes: (state, action) => {
            state.notificationTypes = action.payload;
        },
        setNotificationTemplates: (state, action) => {
            state.notificationTemplates = action.payload;
        },
        setProducts: (state, action) => {
            state.products = action.payload;
        },
        setRecipients: (state, action) => {
            state.recipients.recipientOptions = action.payload.recipients;
            state.recipients.selectedRecipients = action.payload.selectedRecipients;
        },
        setConfirmationPopup: (state, action) => {
            state.confirmationPopup = action.payload;
        },
        setResetConfirmationPopup: (state, action) => {
            state.resetConfirmationPopup = action.payload;
        },
        setEditConfirmationPopup: (state, action) => {
            state.editConfirmationPopup = action.payload;
        },
        setMarkdownRulesPopup: (state, action) => {
            state.markdownRulesPopup = action.payload;
        },
        setCreateNotificationResult: (state, action) => {
            state.createNotificationResult = action.payload;
        }
    }
});

const { actions, reducer } = newNotificationSlice;

export const {
    setNotificationTypes,
    setNotificationTemplates,
    setProducts,
    setRecipients,
    setConfirmationPopup,
    setResetConfirmationPopup,
    setEditConfirmationPopup,
    setMarkdownRulesPopup,
    setCreateNotificationResult
} = actions;

export default reducer;