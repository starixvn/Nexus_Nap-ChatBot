import createReducer from '../../lib/createReducer';
import * as types from '../actions/types';

// Setting
export const settingData = createReducer(null, {
    [types.SETTING_DATA](state, action) {
        return action.data;
    }
})

// Notification Type
export const notificationTypes = createReducer(null, {
    [types.NOTIFICATION_TYPE_DATA](state, action) {
        return action.data;
    }
})

// Notification Template 
export const notificationTemplates = createReducer(null, {
    [types.TEMPLATE_TYPE_DATA](state, action) {
        return action.data;
    }
})

// Template
export const template = createReducer(null, {
    [types.TEMPLATE_DATA](state, action) {
        return action.data;
    }
})