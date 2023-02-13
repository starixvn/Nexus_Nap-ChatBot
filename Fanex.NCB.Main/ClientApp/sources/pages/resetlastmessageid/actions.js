import Site from "../lib/site";
import DataService from '../lib/dataService';

import {
    setSelected,
    setCurrentPreviousMaxMessage,
    setEditNewMessagePopup
} from './resetLastMessageIdSlice';

const getCurrentPreviousMaxMessage = () => async dispatch => {
    const url = Site.resolveClientUrl('ResetLastMessageId/GetCurrentPreviousMaxMessage');
    await DataService.getDataAsync(url).then(response => {
        dispatch(setCurrentPreviousMaxMessage(response.data));
    });
};

const createMissingMessage = params => async () => {
    const url = Site.resolveClientUrl('ResetLastMessageId/CreateMissingMessage');
    return await DataService.postDataAsync(url,params).then(response => response);
};

export const ActionCreators = {
    setSelected,
    getCurrentPreviousMaxMessage,
    setEditNewMessagePopup,
    createMissingMessage
};