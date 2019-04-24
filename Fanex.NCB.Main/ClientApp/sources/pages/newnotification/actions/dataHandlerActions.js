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
    const url = Site.resolveClientUrl('NewNotification/GetSettings');

    return (dispatch) => {        
        axios.get(url).then(response => {
            dispatch(setSettings(response.data));
        }).catch(() => { errorResponseHandler(); });
    };
}

// Notification Type
export function setNotificationTypes(data) {
    return {
        type: types.NOTIFICATION_TYPE_DATA,
        data: data
    };
}

export function getNotificationTypes() {
    const url = Site.resolveClientUrl('NewNotification/GetNotificationTypes');

    let data = [{ id: 1, name: "a" }, { id: 2, name: "b" }];
    return (dispatch) => dispatch(setNotificationTypes(data));

    //return (dispatch) => {
    //    axios.get(url).then(response => {
    //        dispatch(setNotificationTypes(response.data));
    //    }).catch(() => { errorResponseHandler(); });
    //};
}