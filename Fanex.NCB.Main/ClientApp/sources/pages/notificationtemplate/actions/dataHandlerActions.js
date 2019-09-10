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
export const deleteItem = notificationTypeId => {
    const url = Site.resolveClientUrl('NotificationTemplate/Delete');
    const headers = {
        'Content-Type': 'application/json'
    };
    const fakeData = { errorCode: 0 };

    return async dispatch => {
        try {
            const request = await axios.post(url, notificationTypeId, { headers: headers });
            const data = await request.data;
            dispatch(setDeleteData(data));
            return data;
        } catch {
            errorResponseHandler();
            dispatch(setDeleteData(fakeData));
        };
    };
};

export const setDeleteData = data => {
    return {
        type: types.DELETE_RESPONSE_DATA,
        data: data
    };
}

// Edit Popup
export const setUpdateData = data => {
    return {
        type: types.UPDATE_RESPONSE_DATA,
        data: data
    };
};

export const update = (params, isNew) => {
    const url = isNew ? '/NotificationType/Add' : '/NotificationType/Edit';
    const resolvedUrl = Site.resolveClientUrl(url);
    const headers = {
        'Content-Type': 'application/json',
        'RequestVerificationToken': Site.getXsrfToken()
    };
    const fakeData = { errorCode: 0 };

    return async dispatch => {
        try {
            const request = await axios.post(resolvedUrl, JSON.stringify(params), { headers: headers });
            const data = await request.data;
            dispatch(setUpdateData(data));
        } catch (err) {
            errorResponseHandler();
            dispatch(setUpdateData(fakeData));
        }
    };
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

const actions = {
    setNotificationTypes,
    getNotificationTypes,
    setGridParams,
    getGridData,
    deleteItem,
    setUpdateData,
    update,
    refreshUpdateData,
    setCreateTemplatePopup,
    setCreateTemplateResult
};

export default actions;