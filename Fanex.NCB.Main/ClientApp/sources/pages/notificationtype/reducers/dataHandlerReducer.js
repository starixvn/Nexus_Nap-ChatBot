import createReducer from '../../lib/createReducer';
import types from '../actions/types';

// Grid
export const gridParams = createReducer({}, {
    [types.GRID_PARAMS](state, action){
        return action.data;
    }
});

export const gridData = createReducer({}, {
    [types.GRID_DATA](state, action){
        return action.data;
    }
});

// Setting
export const settingData = createReducer({}, {
    [types.SETTING_DATA](state, action){
        return action.data;
    }
});

// Delete
export const deleteData = createReducer({}, {
    [types.DELETE_DATA](state, action){
        return action.data;
    }
});


// Update
export const updateParams = createReducer({}, {
    [types.UPDATE_PARAMS](state, action){
        return action.data;
    }
});

export const updateData = createReducer({}, {
    [types.UPDATE_DATA](state, action){
        return action.data;
    }
});

const reducers = {
    gridParams,
    gridData,
    settingData,
    deleteData,
    updateParams,
    updateData
};

export default reducers;