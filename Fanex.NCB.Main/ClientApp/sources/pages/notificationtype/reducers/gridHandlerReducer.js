import createReducer from '../../lib/createReducer';
import * as types from '../actions/types';

export const activeRowIndex = createReducer({}, {
    [types.ACTIVE_ROW_INDEX](state, action){
        return action.data;
    }
});

export const updatePopupData = createReducer({}, {
    [types.UPDATE_POPUP_DATA](state, action){
        return action.data;
    }
});

export const deletePopupData = createReducer({}, {
    [types.DELETE_POPUP_DATA](state, action){
        return action.data;
    }
});