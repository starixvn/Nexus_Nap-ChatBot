import types from './types';
import axios from 'axios';
import Site from '../../lib/site';
import errorResponseHandler from '../../lib/errorResponseHandler';

// Settings
export const setSettings = data => {
    return {
        type: types.SETTING_DATA,
        data: data
    };
}

export const getSettings = () => {
    const url = Site.resolveClientUrl('NotificationTemplate/GetSettings');
    const fakeData = {
        notificationTypes: [
            { id: 1, name: "UM" },
            { id: 2, name: "Something" }
        ]
    };

    return dispatch => {
        return axios.get(url).then(response => {
            dispatch(setSettings(response.data));
        }).catch(() => {
            errorResponseHandler();
            dispatch(setSettings(fakeData));
        });
    };
}

// Grid
export const setGridParams = value => {
    return {
        type: types.GRID_PARAMS,
        data: value
    };
}

export const setGridData = data => {
    return {
        type: types.GRID_DATA,
        data: data
    };
}

export const getGridData = params => {
    const url = Site.resolveClientUrl('NotificationTemplate/GetAll');
    const fakeData = [
        {
            notificationTemplateId: 1,
            notificationTypeId: 1,
            notificationTypeName: "UM",
            templateName: "Weekly UM1",
            title: "WEEKLY MAINTENANCEMENT - UM DATE",
            content: "Good day!\nThis is Shane, kindly take note of the below message"
        },
        {
            notificationTemplateId: 2,
            notificationTypeId: 1,
            notificationTypeName: "UM",
            templateName: "Weekly UM2",
            title: "WEEKLY MAINTENANCEMENT - UM DATE",
            content: "Good day!\nThis is Shane, kindly take note of the below message"
        }
    ];

    return dispatch => {
        return axios.get(url, { params: params }).then(response => {
            dispatch(setGridData(response.data));
        }).catch(() => {
            errorResponseHandler();
            dispatch(setGridData({
                data: fakeData,
                count: 3
            }));
        });
    };
}

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
}

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
}

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
       };
    };
}

export const refreshUpdateData = () => {
    return dispatch => {
        dispatch(setUpdateData(null));
    };
}

const actions = {
    setSettings,
    getSettings,
    setGridParams,
    getGridData,
    deleteItem,
    setUpdateData,
    update,
    refreshUpdateData
};

export default actions;