import createReducer from '../../lib/createReducer';
import types from '../actions/types';

// Setting
export const notificationTypes = createReducer({}, {
    [types.NOTIFICATIONTYPE_DATA](state, action) {
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
export const deleteResponseData = createReducer({}, {
    [types.DELETE_RESPONSE_DATA](state, action) {
        return action.data;
    }
});

// Update Popup
export const updateResponseData = createReducer({}, {
    [types.UPDATE_RESPONSE_DATA](state, action) {
        return action.data;
    }
});

const reducers = {
    notificationTypes,
    gridParams,
    gridData,
    deleteResponseData,
    updateResponseData
};

export default reducers;