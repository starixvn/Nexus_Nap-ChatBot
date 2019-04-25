import createReducer from '../../lib/createReducer';
import * as types from '../actions/types';

// Setting
export const settingData = createReducer({}, {
    [types.SETTING_DATA](state, action) {
        return action.data;
    }
});

// Notification Type
export const notificationTypes = createReducer({}, {
    [types.NOTIFICATION_TYPE_DATA](state, action) {
        return action.data;
    }
});

// Notification Template 
export const notificationTemplates = createReducer({}, {
    [types.TEMPLATE_TYPE_DATA](state, action) {
        return action.data;
    }
});

// Template
export const template = createReducer({}, {
    [types.TEMPLATE_DATA](state, action) {
        return action.data;
    }
});