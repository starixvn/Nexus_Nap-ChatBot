import types from './types';

// Update Popup
export const openEditPopup = item => {
    const popupData = {
        isShowed: true,
        editingItem: {
            groupID: item.groupID,
            conversationID: item.conversationID, 
            mappingStatusId: item.mappingStatusID,
            mappingStatusName: item.mappingStatusName,
            botName: item.botName,
            groupName: item.groupName,
            recipientTypeId: {
                value: item.recipientTypeID,
                isValid: item.recipientTypeID !== 0
            },
            recipientIds: {
                value: item.recipientIDs,
                isValid: item.recipientIDs !== ''
            },
            recipientStatusNames: item.recipientStatusNames,
            chatGroupTypeId: {
                value: item.chatGroupTypeID,
                isValid: item.chatGroupTypeID !== 0
            },
            remark: {
                value: item.remark,
                isValid: item.remark !== null
            }
        } 
    };

    return {
        type: types.UPDATE_POPUP_DATA,
        data: popupData
    };
}

export const closeEditPopup = () => {
    const popupData = {
        isShowed: false,
        editingItem: {}
    };

    return {
        type: types.UPDATE_POPUP_DATA,
        data: popupData
    };
}

const actions = {
    openEditPopup,
    closeEditPopup
}

export default actions
