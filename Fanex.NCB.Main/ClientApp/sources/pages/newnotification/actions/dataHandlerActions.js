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
    console.log("get settings");
    const url = Site.resolveClientUrl('NewNotification/GetSettings');

    let data = {
        notificationTypes: [{ id: 1, name: "a" }, { id: 2, name: "b" }],
        products: [{ id: 1, name: "p1" }, { id: 2, name: "p2" }]
    };
    return (dispatch) => dispatch(setSettings(data));

    //return (dispatch) => {        
    //    axios.get(url).then(response => {
    //        dispatch(setSettings(response.data));
    //    }).catch(() => { errorResponseHandler(); });
    //};
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

// Notification Template
export function setTemplateTypes(data) {
    return {
        type: types.TEMPLATE_TYPE_DATA,
        data: data
    };
}

export function getTemplateTypes() {
    const url = Site.resolveClientUrl('NewNotification/GetNotificationTemplate');

    let data = [{ id: 1, name: "template 1" }, { id: 2, name: "template 2" }];
    return (dispatch) => dispatch(setTemplateTypes(data));

    //return (dispatch) => {
    //    axios.get(url).then(response => {
    //        dispatch(setNotificationTypes(response.data));
    //    }).catch(() => { errorResponseHandler(); });
    //};
}

// Template 
export function setTemplate(data) {
    return {
        type: types.TEMPLATE_DATA,
        data: data
    };
}

export function getTemplate() {
    const url = Site.resolveClientUrl('NewNotification/GetTemplate');



    return async (dispatch) => {
        let data1 = await {
            title: "title {UM date}",
            content: "good day! \n {UM time} something \n {UM time} and {End time} <br/> {UM time} and {End time} <br/> {UM time} and {End time}"
        };
        dispatch(setTemplate(data1));
    };

    //return (dispatch) => {
    //    axios.get(url).then(response => {
    //        dispatch(setNotificationTypes(response.data));
    //    }).catch(() => { errorResponseHandler(); });
    //};
}