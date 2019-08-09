import createReducer from '../../lib/createReducer';
import types from '../actions/types';

// Setting
export const settingData = createReducer(null, {
    [types.SETTING_DATA](state, action) {
        return action.data;
    }
});

// Notification Type
export const notificationTypes = createReducer(null, {
    [types.NOTIFICATION_TYPE_DATA](state, action) {
        return action.data;
    }
});

// Notification Template
export const notificationTemplates = createReducer(null, {
    [types.TEMPLATE_TYPE_DATA](state, action) {
        return action.data;
    }
});

// Template
export const template = createReducer(null, {
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

const reducers = {
    settingData,
    notificationTypes,
    notificationTemplates,
    template,
    products,
    recipients,
    createNotificationResult
};

export default reducers;