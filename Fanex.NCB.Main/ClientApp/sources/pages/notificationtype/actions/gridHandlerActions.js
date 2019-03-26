import * as types from './types';
//import Site from '../../lib/site';

// Grid
export function updateActiveRowIndex(value){
    return {
        type: types.ACTIVE_ROW_INDEX,
        data: value
    };
}

// Delete Popup
export function openDeletePopup(notificationTypeId, notificationTypeName){
    let deletePopupData = {
        isShowed: true,        
        notificationTypeId: notificationTypeId,
        content: notificationTypeName
    };

    return {
        type: types.DELETE_POPUP_DATA,
        data: deletePopupData
    };
}

export function closeDeletePopup(){
    let deletePopupData = {
        isShowed: false,        
        content: ""
    };

    return {
        type: types.DELETE_POPUP_DATA,
        data: deletePopupData
    };
}

// Update Popup
export function openEditPopup(item) {
    let newEditPopupData = {
        isShowed: true,
        editingItem: {
            notificationTypeId: item.notificationTypeId,
            notificationTypeName: {
                value: item.notificationTypeName,
                isValid: true
            },
            recipientTypeIds: {
                value: item.recipientTypes.map((v, i) => { return v.id;}).join(","),
                isValid: true
            },
            systemId: {
                value: item.systemId,
                isValid: true
            }
        },
        popupTitle: "Edit Notification Type",
        successMsg: "Edit Notification Type Successfully",
        isNew: false
    };

    return {
        type: types.UPDATE_POPUP_DATA,
        data: newEditPopupData
    };
}

export function openNewPopup(){
    let newEditPopupData = {
        isShowed: true,
        editingItem: {
            notificationTypeName: {
                value: '',
                isValid: false
            },
            recipientTypeIds: {
                value: "",
                isValid: false
            },
            systemId: {
                value: -1,
                isValid: false
            }
        },
        popupTitle: "Create New Notification Type",
        successMsg: "Create New Notification Type Successfully",
        isNew: true
    };

    return {
        type: types.UPDATE_POPUP_DATA,
        data: newEditPopupData
    };
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
    };
}
