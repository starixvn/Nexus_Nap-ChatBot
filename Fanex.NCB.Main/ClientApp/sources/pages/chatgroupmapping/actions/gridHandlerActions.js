import * as types from './types';
//import Site from '../../lib/site';

// Update Popup
export function openEditPopup(item) {
    let popupData = {
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
                isValid: item.recipientIDs !== ""
            }, 
            recipientStatusNames: item.recipientStatusNames,
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
