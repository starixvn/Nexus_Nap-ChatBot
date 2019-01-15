import createReducer from '../../lib/createReducer';
import * as types from '../actions/types';

export const notificationTypeListData = createReducer({}, {
    [types.RECEIVE_NOTIFICATION_TYPES](state, action){
        return action.data;
    }
})

export const deleteNotificationTypeData = createReducer({}, {
    [types.DELETED_NOTIFICATION_TYPE](state, action){
        return action.data;
    }
})

export const notificationTypeSettingsData = createReducer({}, {
    [types.RECEIVE_SETTINGS](state, action){
        return action.data;
    }
})

export const addNotificationTypeData = createReducer({}, {
    [types.ADD_NOTIFICATION_TYPE](state, action){
        return action.data;
    }
})