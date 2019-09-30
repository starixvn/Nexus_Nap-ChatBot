import types from './types';

// Grid
export const updateActiveRowIndex = value => {
    return {
        type: types.ACTIVE_ROW_INDEX,
        data: value
    };
};

// Delete Popup
export const openDeletePopup = (notificationTypeId, notificationTypeName) => {
    const deletePopupData = {
        isShowed: true,
        notificationTypeId: notificationTypeId,
        content: notificationTypeName,
        successMsg: 'Delete Notification Type Successfully'
    };

    return {
        type: types.DELETE_POPUP_DATA,
        data: deletePopupData
    };
};

export const closeDeletePopup = () => {
    const deletePopupData = {
        isShowed: false,
        content: ''
    };

    return {
        type: types.DELETE_POPUP_DATA,
        data: deletePopupData
    };
};

// Update Popup
export const openEditPopup = item => {
    const newEditPopupData = {
        isShowed: true,
        editingItem: {
            notificationTypeId: item.id,
            notificationTypeName: {
                value: item.name,
                isValid: true
            },
            recipientTypeIds: {
                value: item.recipientTypes.map(v => v.id).join(','),
                isValid: true
            },
            chatGroupTypeIds: {
              value: item.chatGroupTypes.map(v => v.id).join(','),
              isValid: true
            },
            systemId: {
                value: item.systemId,
                isValid: true
            }
        },
        popupTitle: 'Edit Notification Type',
        successMsg: 'Edit Notification Type Successfully',
        isNew: false
    };

    return {
        type: types.UPDATE_POPUP_DATA,
        data: newEditPopupData
    };
};

export const openNewPopup = () => {
    const newEditPopupData = {
        isShowed: true,
        editingItem: {
            notificationTypeName: {
                value: '',
                isValid: false
            },
            recipientTypeIds: {
                value: '',
                isValid: false
            },
            chatGroupTypeIds: {
                value: '',
                isValid: false
            },
            systemId: {
                value: 0,
                isValid: false
            }
        },
        popupTitle: 'Create New Notification Type',
        successMsg: 'Create New Notification Type Successfully',
        isNew: true
    };

    return {
        type: types.UPDATE_POPUP_DATA,
        data: newEditPopupData
    };
};

export const closeNewEditPopup = () => {
    const newEditPopupData = {
        isShowed: false,
        editingItem: {},
        popupTitle: '',
        successMsg: ''
    };

    return {
        type: types.UPDATE_POPUP_DATA,
        data: newEditPopupData
    };
};

const actions = {
    updateActiveRowIndex,
    openDeletePopup,
    closeDeletePopup,
    openEditPopup,
    openNewPopup,
    closeNewEditPopup
};

export default actions;