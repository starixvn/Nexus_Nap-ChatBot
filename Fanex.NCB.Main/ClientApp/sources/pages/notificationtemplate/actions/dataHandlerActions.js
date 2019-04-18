import * as types from './types';
import axios from 'axios';
import Site from '../../lib/site';
import errorResponseHandler from '../../lib/errorResponseHandler';

// Settings
export function setSettings(data) {
    return {
        type: types.SETTING_DATA,
        data: data
    };
}

export function getSettings() {
    const url = Site.resolveClientUrl('NotificationTemplate/GetSettings');

    //return (dispatch) => {        
    //    axios.get(url).then(response => {
    //        dispatch(setSettings(response.data));
    //    }).catch(() => { errorResponseHandler(); });
    //};

    var fakeData = {
        notificationTypes: [
            { id: 1, name: "UM" },
            { id: 2, name: "Something" }
        ]
    };
    return (dispatch) => dispatch(setSettings(fakeData));
}


// Grid
export function setGridParams(value) {
    return {
        type: types.GRID_PARAMS,
        data: value
    };
}

export function setGridData(data) {
    return {
        type: types.GRID_DATA,
        data: data
    };
}

export function getGridData(params) {
    const url = Site.resolveClientUrl('NotificationTemplate/GetAll');

    //return (dispatch) => {
    //    axios.get(url, { params: params }).then(response => {
    //        dispatch(setGridData(response.data));
    //    }).catch(() => { errorResponseHandler(); });
    //};
    var fakeData = [
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

    return (dispatch) => dispatch(setGridData({ data: fakeData, count: 3 }));
}

// Delete
export function deleteItem(notificationTypeId) {
    const url = Site.resolveClientUrl('NotificationTemplate/Delete');
    let headers = {
        'Content-Type': 'application/json'
    };

    //return async (dispatch) => {
    //    const request = await axios.post(url, notificationTypeId, { headers: headers });
    //    const data = await request.data;
    //    dispatch(setDeleteData(data));
    //};

    var fakeData = { errorCode: 0 };
    return (dispatch) => dispatch(setDeleteData(fakeData));
}

export function setDeleteData(data) {
    return {
        type: types.DELETE_RESPONSE_DATA,
        data: data
    };
}


// Edit Popup
export function setUpdateData(data) {
    return {
        type: types.UPDATE_RESPONSE_DATA,
        data: data
    };
}

export function update(params, isNew) {
    const url = isNew ? '/NotificationType/Add' : '/NotificationType/Edit';
    const resolvedUrl = Site.resolveClientUrl(url);

    let headers = {
        'Content-Type': 'application/json',
        'RequestVerificationToken': Site.getXsrfToken()
    };

    //return async (dispatch) => {
    //    try {
    //        const request = await axios.post(resolvedUrl, JSON.stringify(params), { headers: headers });
    //        const data = await request.data;
    //    } catch (err) {
    //        errorResponseHandler();
    //    }
    //
    //    dispatch(setUpdateData(data));
    //};
    var fakeData = { errorCode: 0 };
    return (dispatch) => dispatch(setUpdateData(fakeData));
}

export function refreshUpdateData() {
    return (dispatch) => {
        dispatch(setUpdateData(null));
    };
}
