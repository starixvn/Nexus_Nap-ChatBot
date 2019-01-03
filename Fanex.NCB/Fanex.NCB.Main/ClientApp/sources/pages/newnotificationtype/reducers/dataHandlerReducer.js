import createReducer from '../../lib/createReducer';
import * as types from '../actions/types';

export const notificationTypeList = createReducer({}, {
    [types.RECEIVE_NOTIFICATION_TYPES](state, action){
        return action.data;
    }
})