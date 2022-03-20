import Site from '../lib/site';
import DataService from '../lib/dataService';
import {
    setNotificationTypes,
    setNotificationTemplates,
    setProducts,
    setRecipients,
    setPlatforms,
    setConfirmationPopup,
    setResetConfirmationPopup,
    setEditConfirmationPopup,
    setMarkdownRulesPopup,
    setCreateNotificationResult
} from './newNotificationSlice';

const getNotificationTypes = () => async dispatch => {
    const url = Site.resolveClientUrl('Common/GetNotificationTypes');
    await DataService.getDataAsync(url).then(response => {
        dispatch(setNotificationTypes(response.data));
    });
};

const getNotificationTemplates = notificationTypeId => async dispatch => {
    const url = Site.resolveClientUrl('NewNotification/GetTemplates');
    await DataService.getDataAsync(url, { notificationTypeId }).then(response => {
        dispatch(setNotificationTemplates(response.data));
    });
};

const getProducts = () => async dispatch => {
    const url = Site.resolveClientUrl('Common/GetProducts');
    await DataService.getDataAsync(url).then(response => {
        dispatch(setProducts(response.data));
    });
};

const getRecipients = (notificationTypeId, platforms, productId, channelIds) => async dispatch => {
    const url = Site.resolveClientUrl('NewNotification/GetRecipients');
    await DataService.getDataAsync(url, { notificationTypeId, platforms, productId, channelIds }).then(response => {
        dispatch(setRecipients(response.data));
    });
};

const getPlatforms = notificationTypeId => async dispatch => {
    const url = Site.resolveClientUrl('NewNotification/GetPlatforms');
    await DataService.getDataAsync(url, { notificationTypeId }).then(response => {
        dispatch(setPlatforms(response.data));
    });
};

const getResendNotification = (notificationId, isResendAll) => async () => {
    const url = Site.resolveClientUrl('NewNotification/GetResendNotification');
    return await DataService.getDataAsync(url, { notificationId, isResendAll }).then(response => response);
};

const createNewNotification = params => async dispatch => {
    const url = Site.resolveClientUrl('NewNotification/Create');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setCreateNotificationResult(response.data));
    });
};

export const ActionCreators = {
    getNotificationTypes,
    getNotificationTemplates,
    getProducts,
    getRecipients,
    getPlatforms,
    getResendNotification,
    createNewNotification,
    setNotificationTypes,
    setNotificationTemplates,
    setProducts,
    setRecipients,
    setConfirmationPopup,
    setResetConfirmationPopup,
    setEditConfirmationPopup,
    setMarkdownRulesPopup,
    setCreateNotificationResult
};