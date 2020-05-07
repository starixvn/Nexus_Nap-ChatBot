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
            toast.success('Update Job Info sucessfully');
            changeModeSaveMode();
            dispatch(setJobInfo(data));
        }
        else {
            let message = response.data && response.data.message || 'Update Job Info unsucessfully';
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
export const openDeleteMappingPopup = extra => {
    return {
        type: types.DELETE_POPUP_DATA,
        data: {
            show: true,
            ...extra
        }
    };
};

export const closeDeleteMappingPopup = () => {
    return {
        type: types.DELETE_POPUP_DATA,
        data: { show: false }
    };
};

export const deleteMapping = data => {
    return async dispatch => axios({
        url: Site.resolveClientUrl(`SettingConfiguration/DeleteMapping?templateId=${data.templateId}&msgContentsType=${data.msgContentsType}&jobId=${data.jobId}`),
        method: 'post',
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'RequestVerificationToken': Site.getXsrfToken()
        }
    }).then(response => response.data).catch(errorResponseHandler);
};

// Edit Popup

export const setEditMappingPopup = data => {
    return {
        type: types.EDIT_POPUP_DATA,
        data: data
    };
};

export const setEditMappingResult = data => {
    return {
        type: types.EDIT_RESULT_DATA,
        data: data
    };
};

export const getMapping = (msgContentsType, jobId, done) => {
    return async dispatch => axios(Site.resolveClientUrl(`SettingConfiguration/GetMapping?msgContentsType=${msgContentsType}&jobId=${jobId}`))
        .then(response => {
            done && done(response.data)
            dispatch(setEditMappingPopup({ show: true, mapping: response.data }));
        }).catch(errorResponseHandler);
};

export const editMapping = data => {
    return async dispatch => axios({
        url: Site.resolveClientUrl('SettingConfiguration/UpdateMapping'),
        method: 'post',
        data: data,
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'RequestVerificationToken': Site.getXsrfToken()
        }
    }).then(response => {
        dispatch(setEditMappingResult(response.data));
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

export const setCreateMappingResult = data => {
    return {
        type: types.CREATE_RESULT_DATA,
        data: data
    };
};

export const createMapping = data => {
    return async dispatch => axios({
        url: Site.resolveClientUrl('SettingConfiguration/CreateMapping'),
        method: 'post',
        data: data,
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'RequestVerificationToken': Site.getXsrfToken()
        }
    }).then(response => {
        dispatch(setCreateMappingResult(response.data));
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

export const getTemplates = notificationTypeId => {
    const url = Site.resolveClientUrl('NewNotification/GetTemplates');
    return (dispatch) => {
        return axios.get(url, { params: { notificationTypeId } }).then(response => {
            dispatch(setTemplates(response.data));
        }).catch(errorResponseHandler);
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

// Notification Type Ref
export const setNotificationTypeRef = data => {
    return {
        type: types.NOTIFICATION_TYPE_REF,
        data: data
    };
};

export const getNotificationTypeRef = notificationTypeId => {
    const url = Site.resolveClientUrl(`SettingConfiguration/GetNotificationTypeData?notificationTypeId=${notificationTypeId}`);
    return dispatch => {
        return axios.get(url).then(response => {
            dispatch(setNotificationTypeRef(response.data));
        }).catch(errorResponseHandler);
    };
};

const actions = {
    setSettingTypes,
    getSettingTypes,
    setGridParams,
    getGridData,
    setCreateMappingPopup,
    setCreateMappingResult,
    createMapping,
    setEditMappingPopup,
    setEditMappingResult,
    getMapping,
    editMapping,
    deleteMapping,
    openDeleteMappingPopup,
    closeDeleteMappingPopup,
    getJobInfo,
    setJobInfo,
    setJobInfoData,
    setJobInfoFlag,
    getNotificationTypes,
    setNotificationTypes,
    getTemplates,
    setTemplates,
    getMsgContentsTypes,
    setMsgContentsTypes,
    getNotificationTypeRef,
    setNotificationTypeRef
};

export default actions;