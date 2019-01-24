import * as types from './types';
import axios from 'axios';
//import Site from '../../lib/site';

// Grid
export function updateActiveRowIndex(value){
    return {
        type: types.ACTIVE_ROW_INDEX,
        data: value
    }
}

// Delete Popup
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
    console.log("close deelte popup");
    let deletePopupData = {
        isShowed: false,        
        content: ""
    };

    return {
        type: types.DELETE_POPUP_DATA,
        data: deletePopupData
    }
}

// Update Popup
export function openEditPopup(item){
    let newEditPopupData = {
        isShowed: true,
        editingItem: item,
        popupTitle: "Edit Notification Type",
        successMsg: "Edit Notification Type Successfully",
        isNew: false
    };

    return {
        type: types.UPDATE_POPUP_DATA,
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
        type: types.UPDATE_POPUP_DATA,
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
        type: types.UPDATE_POPUP_DATA,
        data: newEditPopupData
    }
}
