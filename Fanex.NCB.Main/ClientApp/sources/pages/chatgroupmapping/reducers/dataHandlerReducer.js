import createReducer from '../../lib/createReducer';
import types from '../actions/types';

// Setting
export const settingData = createReducer({}, {
    [types.SETTING_DATA](state, action) {
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

// Update Popup
export const filteredRecipients = createReducer({}, {
    [types.FILTERED_RECIPIENT](state, action) {
        return action.data;
    }
});

export const updateData = createReducer({}, {
    [types.UPDATE_DATA](state, action) {
        return action.data;
    }
});

const reducers = {
    settingData,
    gridParams,
    gridData,
    filteredRecipients,
    updateData
};

export default reducers
