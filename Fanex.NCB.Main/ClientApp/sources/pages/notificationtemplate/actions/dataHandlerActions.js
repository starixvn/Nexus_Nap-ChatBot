import types from './types';
import axios from 'axios';
import Site from '../../lib/site';
import errorResponseHandler from '../../lib/errorResponseHandler';

// Settings
export const setNotificationTypes = data => {
    return {
        type: types.NOTIFICATIONTYPE_DATA,
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
    const url = Site.resolveClientUrl('NotificationTemplate/Get');
    return dispatch => {
        return axios.get(url, { params: params }).then(response => {
            dispatch(setGridData(response.data));
        }).catch(errorResponseHandler);
    };
};

// Delete
export const openDeleteTemplatePopup = extra => {
    return {
        type: types.DELETE_POPUP_DATA,
        data: {
            show: true,
            ...extra
        }
    };
};

export const closeDeleteTemplatePopup = () => {
    return {
        type: types.DELETE_POPUP_DATA,
        data: { show: false }
    };
};

export const deleteTemplate = notificationTemplateId => {
    return async dispatch => axios({
        url: Site.resolveClientUrl(`NotificationTemplate/Delete/${notificationTemplateId}`),
        method: 'post',
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'RequestVerificationToken': Site.getXsrfToken()
        }
    }).then(response => response.data).catch(errorResponseHandler);
};

// Edit Popup

export const setEditTemplatePopup = data => {
    return {
        type: types.EDIT_POPUP_DATA,
        data: data
    };
};

export const setEditTemplateResult = data => {
    return {
        type: types.EDIT_RESULT_DATA,
        data: data
    };
};

export const getTemplate = (templateId) => {
    return async dispatch => axios(Site.resolveClientUrl(`NotificationTemplate/GetById/${templateId}`))
        .then(response => {
            dispatch(setEditTemplatePopup({ show: true, template: response.data }));
        }).catch(errorResponseHandler);
};

export const editTemplate = (data) => {
    return async dispatch => axios({
        url: Site.resolveClientUrl('NotificationTemplate/Edit'),
        method: 'post',
        data: data,
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'RequestVerificationToken': Site.getXsrfToken()
        }
    }).then(response => {
        dispatch(setEditTemplateResult(response.data));
    }).catch(errorResponseHandler);
};

export const refreshUpdateData = () => {
    return dispatch => {
        dispatch(setUpdateData(null));
    };
};

//create template
export const setCreateTemplatePopup = data => {
    return {
        type: types.CREATE_POPUP_DATA,
        data: data
    };
};

export const setCreateTemplateResult = data => {
    return {
        type: types.CREATE_TEMPLATE_RESULT_DATA,
        data: data
    };
};

export const createTemplate = data => {
    return async dispatch => axios({
        url: Site.resolveClientUrl('NotificationTemplate/Create'),
        method: 'post',
        data: data,
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'RequestVerificationToken': Site.getXsrfToken()
        }
    }).then(response => {
        dispatch(setCreateTemplateResult(response.data));
    }).catch(errorResponseHandler);
};

// Markdown Rule
export const openMarkdownRulesPopup = () => {
    return {
        type: types.MARKDOWNRULE_POPUP_DATA,
        data: {
            show: true,
        }
    };
};

export const closeMarkdownRulesPopup = () => {
    return {
        type: types.MARKDOWNRULE_POPUP_DATA,
        data: { show: false }
    };
};

const actions = {
    setNotificationTypes,
    getNotificationTypes,
    setGridParams,
    getGridData,
    setCreateTemplatePopup,
    setCreateTemplateResult,
    createTemplate,
    setEditTemplatePopup,
    setEditTemplateResult,
    getTemplate,
    editTemplate,
    deleteTemplate,
    openDeleteTemplatePopup,
    closeDeleteTemplatePopup,
    openMarkdownRulesPopup,
    closeMarkdownRulesPopup
};

export default actions;