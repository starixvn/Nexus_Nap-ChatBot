import * as types from './types';
//import Site from '../../lib/site';

// Update Popup
export function openEditPopup(item) {
    console.log(item);
    let newEditPopupData = {
        isShowed: true,
        editingItem: {
            conversationId: item.conversationId,
            mappingStatusId: item.mappingStatusId,
            mappingStatusName: item.mappingStatusName,
            botName: item.botName,
            groupName: item.groupName,
            recipientTypeId: item.recipientTypeId,
            recipientIds: item.recipientIds,
            licenseeStatus: item.licenseeStatus,
            remark: item.remark
        },
        popupTitle: "Edit Mapping Status",
        successMsg: "Edit Mapping Status Successfully"
    };

    return {
        type: types.UPDATE_POPUP_DATA,
        data: newEditPopupData
    }
}

export function closeEditPopup() {
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
