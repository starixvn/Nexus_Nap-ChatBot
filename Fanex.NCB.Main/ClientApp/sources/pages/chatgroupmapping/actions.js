import Site from '../lib/site';
import DataService from '../lib/dataService';
import {
    setSettings,
    setGridParams,
    setGridData,
    filterRecipients,
    setEditPopupData,
    setEditResult
} from './chatGroupMappingSlice';

const getSettings = params => async dispatch => {
    const url = Site.resolveClientUrl('ChatGroupMapping/GetSettings');
    await DataService.getDataAsync(url, params).then(response => {
        dispatch(setSettings(response.data));

        const submitButton = document.getElementById('submit-button');
        if (submitButton) {
            submitButton.click();
        }
    });
};

const getGridData = params => async dispatch => {
    const url = Site.resolveClientUrl('ChatGroupMapping/GetAll');
    await DataService.getDataAsync(url, params).then(response => {
        dispatch(setGridData(response.data));
    });
};

const editMapping = params => async dispatch => {
    const url = Site.resolveClientUrl('ChatGroupMapping/Edit');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setEditResult(response.data));
    });
};

const editActive = params => async () => {
    const url = Site.resolveClientUrl('ChatGroupMapping/UpdateActive');
    return await DataService.postDataAsync(url, params).then(response => response);
};

export const ActionCreators = {
    setGridParams,
    setGridData,
    filterRecipients,
    getSettings,
    getGridData,
    setEditPopupData,
    editMapping,
    setEditResult,
    editActive
};