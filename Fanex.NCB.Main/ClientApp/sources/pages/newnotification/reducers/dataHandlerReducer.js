import createReducer from '../../lib/createReducer';
import types from '../actions/types';

// Notification Type
export const notificationTypes = createReducer(null, {
    [types.NOTIFICATION_TYPE_DATA](state, action) {
        return action.data;
    }
});

// Notification Template
export const notificationTemplates = createReducer(null, {
    [types.TEMPLATE_DATA](state, action) {
        return action.data;
    }
});

// Product
export const products = createReducer(null, {
    [types.PRODUCT_DATA](state, action) {
        return action.data;
    }
});

// Product
export const recipients = createReducer(null, {
    [types.RECIPIENT_DATA](state, action) {
        return action.data;
    }
});

export const createNotificationResult = createReducer({}, {
    [types.CREATENOTIFICATIONRESULT_DATA](state, action) {
        return action.data;
    }
});

export const confimrationPopup = createReducer({}, {
    [types.CONFIRMATIONPOPUP_DATA](state, action) {
        return action.data;
    }
});

export const resetConfimrationPopup = createReducer({}, {
    [types.RESET_CONFIRMATIONPOPUP_DATA](state, action) {
        return action.data;
    }
});

export const editConfimrationPopup = createReducer({}, {
    [types.EDIT_CONFIRMATIONPOPUP_DATA](state, action) {
        return action.data;
    }
});

const reducers = {
    notificationTypes,
    notificationTemplates,
    products,
    recipients,
    createNotificationResult,
    confimrationPopup,
    resetConfimrationPopup,
    editConfimrationPopup
};

export default reducers;