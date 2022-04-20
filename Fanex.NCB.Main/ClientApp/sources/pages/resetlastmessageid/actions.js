import Site from "../lib/site";
import DataService from '../lib/dataService';

import {
    setSelected,
    setCurrentPreviousMaxMessage,
    setNewPreviousMaxMessage,
    setMissingMessageCsd,
    setMissingMessageInfo,
    setMissingProblemAccount,
    setEditNewMessagePopup
} from './resetLastMessageIdSlice';

const getCurrentPreviousMaxMessage = () => async dispatch => {
    const url = Site.resolveClientUrl('ResetLastMessageId/GetCurrentPreviousMaxMessage');
    await DataService.getDataAsync(url).then(response => {
        dispatch(setCurrentPreviousMaxMessage(response.data));
    });
};

const getNewPreviousMaxMessage = params => async dispatch => {
    const url = Site.resolveClientUrl('ResetLastMessageId/GetNewPreviousMaxMessage');
    await DataService.postDataAsync(url,params).then(response => {
        dispatch(setNewPreviousMaxMessage(response.data));
    });
};

const getMissingMessageCsd = params => async dispatch => {
    const url = Site.resolveClientUrl('ResetLastMessageId/GetMissingMessageCsd');
    await DataService.postDataAsync(url,params).then(response => {
        dispatch(setMissingMessageCsd(response.data));
    });
};

const getMissingMessageInfo = params => async dispatch => {
    const url = Site.resolveClientUrl('ResetLastMessageId/GetMissingMessageInfo');
    await DataService.postDataAsync(url,params).then(response => {
        dispatch(setMissingMessageInfo(response.data));
    });
};

const getMissingProblemAccount = params => async dispatch => {
    const url = Site.resolveClientUrl('ResetLastMessageId/GetMissingProblemAccount');
    await DataService.postDataAsync(url,params).then(response => {
        dispatch(setMissingProblemAccount(response.data));
    });
};

const createMissingMessage = params => async () => {
    const url = Site.resolveClientUrl('ResetLastMessageId/CreateMissingMessage');
    return await DataService.postDataAsync(url,params).then(response => response);
};

const editMessageInfo = params => async () => {
    const url = Site.resolveClientUrl('ResetLastMessageId/EditMessageInfo');
    return await DataService.postDataAsync(url,params).then(response => response);
};

export const ActionCreators = {
    setSelected,
    getCurrentPreviousMaxMessage,
    getNewPreviousMaxMessage,
    getMissingMessageCsd,
    getMissingMessageInfo,
    getMissingProblemAccount,
    setEditNewMessagePopup,
    createMissingMessage,
    editMessageInfo
};