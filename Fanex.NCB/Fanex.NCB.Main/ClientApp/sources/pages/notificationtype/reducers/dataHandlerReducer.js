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

export const gridParams = createReducer({}, {
    [types.SET_GRID_PARAMS](state, action){
        return action.data;
    }
})


export const newEditParams = createReducer({}, {
    [types.SET_NEW_EDIT_PARAMS](state, action){
        return action.data;
    }
})

export const isShowNewEditPoup = createReducer({}, {
    [types.SHOW_NEW_EDIT_POPUP](state, action){
        return action.data;
    }
})

export const isShowDeletePoup = createReducer({}, {
    [types.SHOW_DELETE_POPUP](state, action){
        return action.data;
    }
})

export const newEditPopupData = createReducer({}, {
    [types.NEW_EDIT_POPUP_DATA](state, action){
        return action.data;
    }
})

export const activeRowIndex = createReducer({}, {
    [types.ACTIVE_ROW_INDEX](state, action){
        return action.data;
    }
})

export const deletePopupData = createReducer({}, {
    [types.DELETE_POPUP_DATA](state, action){
        return action.data;
    }
})