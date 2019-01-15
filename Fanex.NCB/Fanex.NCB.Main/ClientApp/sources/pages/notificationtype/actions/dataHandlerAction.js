import * as types from './types';
import axios from 'axios';
//import Site from '../../lib/site';

export function getNotificationTypes(){
    const url = '/NotificationType/GetAll';
    
    return (dispatch) => {
        axios.get(url).then(response => {
            dispatch(setNotificationTypes(response.data))
        });
    };
}
   
export function setNotificationTypes(data){
    return {
        type: types.RECEIVE_NOTIFICATION_TYPES,
        data: data
    };
}

 export function deleteNotificationType(id){ 
    const url = '/NotificationType/Delete';   
    var params = JSON.stringify({id: id});

    return async(dispatch) => {
        const request = await axios.post(url, params);
        const data = await request.data; 
        dispatch(setDeleteNotificationType(data));        
    };
} 

export function setDeleteNotificationType(data){    
    return {
        type: types.DELETED_NOTIFICATION_TYPE,
        data: data
    };
}

export function getSettings(){
    const url = '/NotificationType/GetSettings';
    
    return (dispatch) => {
        axios.get(url).then(response => {
            dispatch(setSettings(response.data))
        });
    };
}

export function setSettings(data){
    return {
        type: types.RECEIVE_SETTINGS,
        data: data
    };
}

 export function addNewNotificationType(params){
    const url = '/NotificationType/Add';

    var params = JSON.stringify(params);

    return async(dispatch) => {
        const request = await axios.post(url, params);
        const data = await request.data; 
        dispatch(setAddNewNotificationType({addedresult: data}));        
    };
} 

 export function editNewNotificationType(params){
    const url = '/NotificationType/Edit';

    var params = JSON.stringify(params);

    return async(dispatch) => {
        const request = await axios.post(url, params);
        const data = await request.data; 
        dispatch(setAddNewNotificationType({addedresult: data}));        
    };
} 

export function setAddNewNotificationType(data){
    return {
        type: types.ADD_NOTIFICATION_TYPE,
        data: data
    };
}

export function refreshAddNotificationType(){
    return (dispatch) => {
        dispatch(setAddNewNotificationType({addedresult: false}));
    };    
}