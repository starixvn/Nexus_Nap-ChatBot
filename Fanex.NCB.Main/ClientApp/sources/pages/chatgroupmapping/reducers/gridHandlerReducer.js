import createReducer from '../../lib/createReducer';
import * as types from '../actions/types';

export const updatePopupData = createReducer({}, {
    [types.UPDATE_POPUP_DATA](state, action) {
        return action.data;
    }
});
