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




