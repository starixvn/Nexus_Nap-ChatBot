import createReducer from '../../lib/createReducer';
import * as types from '../actions/types';

export const notificationTypeList = createReducer({}, {
    [types.RECEIVE_NOTIFICATION_TYPES](state, action){
        return action.data;
    }
})

export const deleteNotificationType = createReducer({}, {
    [types.DELETED_NOTIFICATION_TYPE](state, action){
        return action.data;
    }
})

export const notificationTypeSettings = createReducer({}, {
    [types.RECEIVE_SETTINGS](state, action){
        return action.data;
    }
})

export const addNotificationType = createReducer({}, {
    [types.ADD_NOTIFICATION_TYPE](state, action){
        return action.data;
    }
})