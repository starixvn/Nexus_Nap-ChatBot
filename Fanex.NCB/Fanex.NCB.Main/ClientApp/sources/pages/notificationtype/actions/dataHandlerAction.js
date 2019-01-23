import * as types from './types';
import axios from 'axios';
//import Site from '../../lib/site';

export function getNotificationTypes(pageIndex, pageSize){
    console.log("getNotificationTypes");
    console.log(pageIndex);
    const url = '/NotificationType/GetAll';    
    var params = {pageIndex: pageIndex, pageSize: pageSize};
    
    return (dispatch) => {
        axios.get(url, {params: params}).then(response => {            
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

export function updateNotificationType(params, isNew){
    const url = isNew ? '/NotificationType/Add' : '/NotificationType/Edit';

    return async(dispatch) => {
        const request = await axios.post(url, JSON.stringify(params));
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

export function setGridParams(value){
    console.log("set grid params");
    console.log(value);
    return {
        type: types.SET_GRID_PARAMS,
        data: value
    }
}

export function setNewEditParams(value){
    return {
        type: types.SET_NEW_EDIT_PARAMS,
        data: value
    }
}

export function openDeletePopup(content){
    let deletePopupData = {
        isShowed: true,        
        content: content
    };

    return {
        type: types.DELETE_POPUP_DATA,
        data: deletePopupData
    }
}

export function closeDeletePopup(){
    let deletePopupData = {
        isShowed: false,        
        content: ""
    };

    return {
        type: types.DELETE_POPUP_DATA,
        data: deletePopupData
    }
}

export function openEditPopup(item){
    let newEditPopupData = {
        isShowed: true,
        editingItem: item,
        popupTitle: "Edit Notification Type",
        successMsg: "Edit Notification Type Successfully",
        isNew: false
    };

    return {
        type: types.NEW_EDIT_POPUP_DATA,
        data: newEditPopupData
    }
}

export function openNewPopup(){
    let newEditPopupData = {
        isShowed: true,
        editingItem: {},
        popupTitle: "Create New Notification Type",
        successMsg: "Create New Notification Type Successfully",
        isNew: true
    };

    return {
        type: types.NEW_EDIT_POPUP_DATA,
        data: newEditPopupData
    }
}

export function closeNewEditPopup(){
    let newEditPopupData = {
        isShowed: false,
        editingItem: {},
        popupTitle: "",
        successMsg: ""
    };

    return {
        type: types.NEW_EDIT_POPUP_DATA,
        data: newEditPopupData
    }
}

export function updateActiveRowIndex(value){
    return {
        type: types.ACTIVE_ROW_INDEX,
        data: value
    }
}