import createReducer from '../../lib/createReducer';
import types from '../actions/types';

// Setting
export const settingTypes = createReducer({}, {
    [types.SETTING_TYPES_DATA](state, action) {
        return action.data;
    }
});

// Job Info
export const jobInfo = createReducer({}, {
    [types.JOB_INFO_DATA](state, action) {
        return action.data;
    }
});

export const jobInfoFlag = createReducer({}, {
    [types.JOB_INFO_FLAG](state, action) {
        return action.data
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
export const deleteMappingPopup = createReducer({}, {
    [types.DELETE_POPUP_DATA](state, action) {
        return action.data;
    }
});

// Update Popup
export const editMappingPopup = createReducer({}, {
    [types.EDIT_POPUP_DATA](state, action) {
        return action.data;
    }
});

export const editMappingResult = createReducer({}, {
    [types.EDIT_RESULT_DATA](state, action) {
        return action.data;
    }
});

// Create Popup
export const createMappingPopup = createReducer({}, {
    [types.CREATE_POPUP_DATA](state, action) {
        return action.data;
    }
});

export const createMappingResult = createReducer({}, {
    [types.CREATE_RESULT_DATA](state, action) {
        return action.data;
    }
});

// Notification Type
export const notificationTypes = createReducer(null, {
    [types.NOTIFICATION_TYPE_DATA](state, action) {
        return action.data;
    }
});

// Notification Template
export const notificationTemplates = createReducer(null, {
    [types.TEMPLATE_DATA](state, action) {
        return action.data;
    }
});

// Msg Contents Type
export const msgContentsTypes = createReducer(null, {
    [types.MSG_CONTENT_TYPE_DATA](state, action) {
        return action.data;
    }
});

export const notificationTypeRef = createReducer(null, {
    [types.NOTIFICATION_TYPE_REF](state, action) {
        return action.data;
    }
});

const reducers = {
    settingTypes,
    jobInfo,
    gridParams,
    gridData,
    createMappingPopup,
    createMappingResult,
    editMappingPopup,
    editMappingResult,
    deleteMappingPopup,
    jobInfoFlag,
    notificationTypes,
    notificationTemplates,
    msgContentsTypes,
    notificationTypeRef
};

export default reducers;