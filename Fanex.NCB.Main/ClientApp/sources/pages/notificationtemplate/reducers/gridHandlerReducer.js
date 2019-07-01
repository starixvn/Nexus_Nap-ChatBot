import createReducer from '../../lib/createReducer';
import types from '../actions/types';

export const updatePopupData = createReducer({}, {
    [types.UPDATE_POPUP_DATA](state, action) {
        return action.data;
    }
});

export const deletePopupData = createReducer({}, {
    [types.DELETE_POPUP_DATA](state, action) {
        return action.data;
    }
});

const reducers = {
    updatePopupData,
    deletePopupData
};

export default reducers;