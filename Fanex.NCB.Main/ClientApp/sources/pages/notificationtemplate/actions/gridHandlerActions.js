import * as types from './types';
//import Site from '../../lib/site';

// Delete Popup
export function openDeletePopup(notificationTemplateId, notificationTemplateName) {
    let deletePopupData = {
        isShowed: true,
        notificationTemplateId: notificationTemplateId,
        content: notificationTemplateName
    };

    return {
        type: types.DELETE_POPUP_DATA,
        data: deletePopupData
    };
}

export function closeDeletePopup() {
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
    let popupData = {
        isShowed: true,
        editingItem: {
            templateId: item.templateId,
            notificationTypeID: {
                value: item.notificationTypeId,
                isValid: true
            },            
            templateName: {
                value: item.templateName,
                isValid: true
            },
            title: {
                value: item.title,
                isValid: true
            },
            content: {
                value: item.content,
                isValid: true
            } 
        },
        isNew: false
    };

    return {
        type: types.UPDATE_POPUP_DATA,
        data: popupData
    };
}

export function openNewPopup() {
    let newEditPopupData = {
        isShowed: true,
        editingItem: {
            templateId: item.templateId,
            notificationTypeID: {
                value: -1,
                isValid: false
            },
            templateName: {
                value: '',
                isValid: false
            },
            title: {
                value: '',
                isValid: false
            },
            content: {
                value: '',
                isValid: false
            }
        },
        isNew: true
    };

    return {
        type: types.UPDATE_POPUP_DATA,
        data: newEditPopupData
    };
}

export function closeEditPopup() {
    let popupData = {
        isShowed: false,
        editingItem: {}       
    };

    return {
        type: types.UPDATE_POPUP_DATA,
        data: popupData
    };
}