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
export const createMappingPopup = createReducer({}, {
    [types.CREATE_POPUP_DATA](state, action) {
        return action.data;
    }
});

export const createSettingTypeResult = createReducer({}, {
    [types.CREATE_SETTING_TYPE_RESULT_DATA](state, action) {
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

// Recipient
export const recipients = createReducer(null, {
    [types.RECIPIENT_DATA](state, action) {
        return action.data;
    }
});

// Recipient
export const msgContentsTypes = createReducer(null, {
    [types.MSG_CONTENT_TYPE_DATA](state, action) {
        return action.data;
    }
});

const reducers = {
    settingTypes,
    jobInfo,
    gridParams,
    gridData,
    createMappingPopup,
    createSettingTypeResult,
    editSettingTypePopup,
    editSettingTypeResult,
    deleteSettingTypePopup,
    jobInfoFlag,
    notificationTypes,
    notificationTemplates,
    recipients,
    msgContentsTypes
};

export default reducers;