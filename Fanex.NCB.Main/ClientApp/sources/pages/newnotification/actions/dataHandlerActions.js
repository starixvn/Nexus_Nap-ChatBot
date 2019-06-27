import * as types from './types';
import axios from 'axios';
import Site from '../../lib/site';
import errorResponseHandler from '../../lib/errorResponseHandler';

// Settings
export function setSettings(data) {
    return {
        type: types.SETTING_DATA,
        data: data
    }
}

export function getSettings() {
    const url = Site.resolveClientUrl('NewNotification/GetSettings');
    let data = {
        notificationTypes: [{ id: 1, name: "a" }, { id: 2, name: "b" }],
        products: [{ id: 1, name: "p1" }, { id: 2, name: "p2" }]
    }

    return (dispatch) => {        
       axios.get(url).then(response => {
           dispatch(setSettings(response.data))
       }).catch(() => { 
        errorResponseHandler()
         dispatch(setSettings(data)) //test
        })  
    }
}

// Notification Type
export function setNotificationTypes(data) {
    return {
        type: types.NOTIFICATION_TYPE_DATA,
        data: data
    };
}

export function getNotificationTypes() {
    const url = Site.resolveClientUrl('NewNotification/GetNotificationTypes')
    let data = [{ id: 1, name: "a" }, { id: 2, name: "b" }]

    return (dispatch) => {
       axios.get(url).then(response => {
           dispatch(setNotificationTypes(response.data));
       }).catch(() => { errorResponseHandler(); dispatch(setNotificationTypes(data)) }) //test
    }
}

// Notification Template
export function setTemplateTypes(data) {
    return {
        type: types.TEMPLATE_TYPE_DATA,
        data: data
    }
}

export function getTemplateTypes() {
    const url = Site.resolveClientUrl('NewNotification/GetNotificationTemplate');
    let data = [{ id: 1, name: "template 1" }, { id: 2, name: "template 2" }];

    return (dispatch) => {
       axios.get(url).then(response => {
           dispatch(setNotificationTypes(response.data));
       }).catch(() => { errorResponseHandler(); dispatch(setTemplateTypes(data))}) //test
    }
}

// Template 
export function setTemplate(data) {
    return {
        type: types.TEMPLATE_DATA,
        data: data
    };
}

export function getTemplate() {
    const url = Site.resolveClientUrl('NewNotification/GetTemplate')
    let data = {
            title: "title hh {UM date}",
            content: "good day! \n {UM time} something \n {UM time} and {End time} <br/> {UM time} and {End time} <br/> {UM time} and {End time} <br/> {a very long parameter here} {ha ha} cc"
        }

    return (dispatch) => {
       axios.get(url).then(response => {
           dispatch(setNotificationTypes(response.data))
       }).catch(() => { errorResponseHandler()
          dispatch(setTemplate(data));
       })
    }
}