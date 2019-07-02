import types from './types';

// Delete Popup
export const openDeletePopup = (notificationTemplateId, notificationTemplateName) => {
    const deletePopupData = {
        isShowed: true,
        notificationTemplateId: notificationTemplateId,
        content: notificationTemplateName
    };

    return {
        type: types.DELETE_POPUP_DATA,
        data: deletePopupData
    };
}

export const closeDeletePopup = () => {
    const deletePopupData = {
        isShowed: false,
        content: ''
    }

    return {
        type: types.DELETE_POPUP_DATA,
        data: deletePopupData
    };
};

// Update Popup
export const openEditPopup = item => {
    const popupData = {
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
};

export const openNewPopup = item => {
    const newEditPopupData = {
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
};

export const closeEditPopup = () => {
    const popupData = {
        isShowed: false,
        editingItem: {}
    };

    return {
        type: types.UPDATE_POPUP_DATA,
        data: popupData
    };
};

const actions = {
    openDeletePopup,
    closeDeletePopup,
    openEditPopup,
    openNewPopup,
    closeEditPopup
};

export default actions;