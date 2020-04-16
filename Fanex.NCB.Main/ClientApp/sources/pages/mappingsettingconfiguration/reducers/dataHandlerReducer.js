import createReducer from '../../lib/createReducer';
import types from '../actions/types';

// Setting
export const settingTypes = createReducer({}, {
    [types.SETTING_TYPES_DATA](state, action) {
        return action.data;
    }
});

// Grid
export const gridParams = createReducer({}, {
    [types.GRID_PARAMS](state, action) {
        return action.data;
    }
});

export const gridData = createReducer({}, {
    [types.GRID_DATA](state, action) {
        return action.data;
    }
});

// Delete
export const deleteSettingTypePopup = createReducer({}, {
    [types.DELETE_POPUP_DATA](state, action) {
        return action.data;
    }
});

// Update Popup
export const editSettingTypePopup = createReducer({}, {
    [types.EDIT_POPUP_DATA](state, action) {
        return action.data;
    }
});

export const editSettingTypeResult = createReducer({}, {
    [types.EDIT_RESULT_DATA](state, action) {
        return action.data;
    }
});

// Create Popup
export const createSettingTypePopup = createReducer({}, {
    [types.CREATE_POPUP_DATA](state, action) {
        return action.data;
    }
});

export const createSettingTypeResult = createReducer({}, {
    [types.CREATE_SETTING_TYPE_RESULT_DATA](state, action) {
        return action.data;
    }
});

const reducers = {
    settingTypes,
    gridParams,
    gridData,
    createSettingTypePopup,
    createSettingTypeResult,
    editSettingTypePopup,
    editSettingTypeResult,
    deleteSettingTypePopup
};

export default reducers;