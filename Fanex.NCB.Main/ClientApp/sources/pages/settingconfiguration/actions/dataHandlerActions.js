import types from './types';
import axios from 'axios';
import Site from '../../lib/site';
import errorResponseHandler from '../../lib/errorResponseHandler';

// Settings
export const setSettingTypes = data => {
    return {
        type: types.SETTING_TYPES_DATA,
        data: data
    };
};

export const getSettingTypes = () => {
    const url = Site.resolveClientUrl('Common/GetSettingTypes');
    return dispatch => {
        return axios.get(url).then(response => {
            dispatch(setSettingTypes(response.data));
        }).catch(errorResponseHandler);
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
    const url = Site.resolveClientUrl('SettingConfiguration/GetSettingTypes');
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

//create template
export const setCreateSettingPopup = data => {
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

const actions = {
    setSettingTypes,
    getSettingTypes,
    setGridParams,
    getGridData,
    setCreateSettingPopup,
    setCreateSettingTypeResult,
    createSettingType,
    setEditSettingTypePopup,
    setEditSettingTypeResult,
    getSettingType,
    editSettingType,
    deleteSettingType,
    openDeleteSettingTypePopup,
    closeDeleteSettingTypePopup
};

export default actions;