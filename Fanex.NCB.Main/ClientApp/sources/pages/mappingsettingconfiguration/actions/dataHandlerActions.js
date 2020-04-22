import types from './types';
import axios from 'axios';
import Site from '../../lib/site';
import errorResponseHandler from '../../lib/errorResponseHandler';
import { toast } from 'react-toastify';

// Settings
export const setSettingTypes = data => {
    return {
        type: types.SETTING_TYPES_DATA,
        data: data
    };
};

export const getSettingTypes = () => {
    const url = Site.resolveClientUrl('SettingConfiguration/GetSettingTypes');
    return dispatch => {
        return axios.get(url).then(response => {
            dispatch(setSettingTypes(response.data));
        }).catch(errorResponseHandler);
    };
};

// Job Info
export const setJobInfo = data => {
    return {
        type: types.JOB_INFO_DATA,
        data: data
    };
};

export const setJobInfoData = (data, changeModeSaveMode) => {
    return async dispatch => axios({
        url: Site.resolveClientUrl('SettingConfiguration/UpdateJobInfo'),
        method: 'post',
        data: data,
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'RequestVerificationToken': Site.getXsrfToken()
        }
    }).then(response => {
        if (response.data && response.data.isSuccessful) {
            toast.success('Successfully');
            changeModeSaveMode();
            dispatch(setJobInfo(data));
        }
        else {
            let message = response.data && response.data.message || 'Unexpected error';
            toast.error(message);
        }
    }).catch(errorResponseHandler);
};

export const getJobInfo = jobId => {
    const url = Site.resolveClientUrl(`SettingConfiguration/GetJobInfo?jobId=${jobId}`);
    return dispatch => {
        return axios.get(url).then(response => {
            dispatch(setJobInfo(response.data));
        }).catch(errorResponseHandler);
    };
};

export const setJobInfoFlag = data => {
    return {
        type: types.JOB_INFO_FLAG,
        data: data
    };
};

// Grid
export const setGridParams = value => {
    return {
        type: types.GRID_PARAMS,
        data: value
    };
};

export const setGridData = data => {
    return {
        type: types.GRID_DATA,
        data: data
    };
};

export const getGridData = params => {
    const url = Site.resolveClientUrl('SettingConfiguration/GetMappings');
    return dispatch => {
        return axios.get(url, { params: params }).then(response => {
            dispatch(setGridData(response.data));
        }).catch(errorResponseHandler);
    };
};

// Delete
export const openDeleteSettingTypePopup = extra => {
    return {
        type: types.DELETE_POPUP_DATA,
        data: {
            show: true,
            ...extra
        }
    };
};

export const closeDeleteSettingTypePopup = () => {
    return {
        type: types.DELETE_POPUP_DATA,
        data: { show: false }
    };
};

export const deleteSettingType = type => {
    return async dispatch => axios({
        url: Site.resolveClientUrl(`SettingConfiguration/Delete?type=${type}`),
        method: 'post',
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'RequestVerificationToken': Site.getXsrfToken()
        }
    }).then(response => response.data).catch(errorResponseHandler);
};

// Edit Popup

export const setEditSettingTypePopup = data => {
    return {
        type: types.EDIT_POPUP_DATA,
        data: data
    };
};

export const setEditSettingTypeResult = data => {
    return {
        type: types.EDIT_RESULT_DATA,
        data: data
    };
};

export const getSettingType = (type) => {
    return async dispatch => axios(Site.resolveClientUrl(`SettingConfiguration/GetByType?type=${type}`))
        .then(response => {
            dispatch(setEditSettingTypePopup({ show: true, settingType: response.data }));
        }).catch(errorResponseHandler);
};

export const editSettingType = (data) => {
    return async dispatch => axios({
        url: Site.resolveClientUrl('SettingConfiguration/Update'),
        method: 'post',
        data: data,
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'RequestVerificationToken': Site.getXsrfToken()
        }
    }).then(response => {
        dispatch(setEditSettingTypeResult(response.data));
    }).catch(errorResponseHandler);
};

export const refreshUpdateData = () => {
    return dispatch => {
        dispatch(setUpdateData(null));
    };
};

//create mapping
export const setCreateMappingPopup = data => {
    return {
        type: types.CREATE_POPUP_DATA,
        data: data
    };
};

export const setCreateSettingTypeResult = data => {
    return {
        type: types.CREATE_SETTING_TYPE_RESULT_DATA,
        data: data
    };
};

export const createSettingType = data => {
    return async dispatch => axios({
        url: Site.resolveClientUrl('SettingConfiguration/Create'),
        method: 'post',
        data: data,
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'RequestVerificationToken': Site.getXsrfToken()
        }
    }).then(response => {
        dispatch(setCreateSettingTypeResult(response.data));
    }).catch(errorResponseHandler);
};

// Notification Type
export const setNotificationTypes = data => {
    return {
        type: types.NOTIFICATION_TYPE_DATA,
        data: data
    };
};

export const getNotificationTypes = () => {
    const url = Site.resolveClientUrl('Common/GetNotificationTypes');
    return dispatch => {
        return axios.get(url).then(response => {
            dispatch(setNotificationTypes(response.data));
        }).catch(errorResponseHandler);
    };
};

// Notification Template
export const setTemplates = data => {
    return {
        type: types.TEMPLATE_DATA,
        data: data
    };
};

export const getTemplates = (notificationTypeId) => {
    const url = Site.resolveClientUrl('NewNotification/GetTemplates');
    return (dispatch) => {
        return axios.get(url, { params: { notificationTypeId } }).then(response => {
            dispatch(setTemplates(response.data));
        }).catch(errorResponseHandler);
    };
};

// Recipient
export const setRecipients = data => {
    return {
        type: types.RECIPIENT_DATA,
        data: data
    };
};

export const getRecipients = (notificationTypeId, productId) => {
    const url = Site.resolveClientUrl('NewNotification/GetRecipients');
    return dispatch => {
        axios.get(url, {
            params: { notificationTypeId, productId }
        }).then(response => {
            dispatch(setRecipients(response.data));
        }).catch(() => {
            errorResponseHandler();
        });
    };
};

// Msg Contents Type
export const setMsgContentsTypes = data => {
    return {
        type: types.MSG_CONTENT_TYPE_DATA,
        data: data
    };
};

export const getMsgContentsTypes = params => {
    const url = Site.resolveClientUrl('SettingConfiguration/GetMsgContentsTypes');
    return dispatch => {
        return axios.get(url, { params: params }).then(response => {
            dispatch(setMsgContentsTypes(response.data));
        }).catch(errorResponseHandler);
    };
};

const actions = {
    setSettingTypes,
    getSettingTypes,
    setGridParams,
    getGridData,
    setCreateMappingPopup,
    setCreateSettingTypeResult,
    createSettingType,
    setEditSettingTypePopup,
    setEditSettingTypeResult,
    getSettingType,
    editSettingType,
    deleteSettingType,
    openDeleteSettingTypePopup,
    closeDeleteSettingTypePopup,
    getJobInfo,
    setJobInfo,
    setJobInfoData,
    setJobInfoFlag,
    getNotificationTypes,
    setNotificationTypes,
    getTemplates,
    setTemplates,
    getRecipients,
    setRecipients,
    getMsgContentsTypes,
    setMsgContentsTypes
};

export default actions;