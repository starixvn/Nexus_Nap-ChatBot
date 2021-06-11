import Site from '../lib/site';
import DataService from '../lib/dataService';
import {
    setSettings,
    setGridParams,
    setGridData,
    setActiveRowIndex,
    setCreateEditPopupData,
    setCreateEditResult,
    setDeletePopupData,
    setDeleteResult
} from './notificationTypeSlice';

const getSettings = () => async dispatch => {
    const url = Site.resolveClientUrl('NotificationType/GetSettings');
    await DataService.getDataAsync(url).then(response => {
        dispatch(setSettings(response.data));
    });
};

const getGridData = params => async dispatch => {
    const url = Site.resolveClientUrl('NotificationType/GetAll');
    await DataService.getDataAsync(url, params).then(response => {
        dispatch(setGridData(response.data));
    });
};

const createEditNotificationType = (params, isNew) => async dispatch => {
    const url = isNew ? Site.resolveClientUrl('NotificationType/Add') : Site.resolveClientUrl('NotificationType/Edit');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setCreateEditResult(response.data));
    });
};

const deleteNotificationType = notificationTypeId => async dispatch => {
    const url = Site.resolveClientUrl('NotificationType/Delete/' + notificationTypeId);
    await DataService.postDataAsync(url).then(response => {
        dispatch(setDeleteResult(response.data));
    });
};

export const ActionCreators = {
    setSettings,
    setGridParams,
    setGridData,
    setActiveRowIndex,
    setCreateEditPopupData,
    setCreateEditResult,
    setDeletePopupData,
    setDeleteResult,
    getSettings,
    getGridData,
    createEditNotificationType,
    deleteNotificationType
};