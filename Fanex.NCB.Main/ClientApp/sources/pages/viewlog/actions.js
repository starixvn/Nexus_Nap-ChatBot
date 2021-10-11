import Site from '../lib/site';
import DataService from '../lib/dataService';

import {
    setParamsPageSystemMessage,
    setParamsPageRecipientList,
    setParamsPageConversationMessage,
    setNotificationSystemMessage,
    setNotificationRecipientList,
    setConversationMessageLog
} from './viewLogSlice';

const getParamsPageSystemMessage = (params) => dispatch => {
    dispatch(setParamsPageSystemMessage(params));
}

const getParamsPageRecipientList = (params) => dispatch => {
    dispatch(setParamsPageRecipientList(params));
}

const getParamsPageConversationMessage = (params) => dispatch => {
    dispatch(setParamsPageConversationMessage(params));
}

const getNotificationSystemMessage = (params) => async dispatch => {
    const url = Site.resolveClientUrl('ViewLog/GetNotificationSystemMessage');
    await DataService.getDataAsync(url,params).then(response => {
        dispatch(setNotificationSystemMessage(response.data));
    });
};

const getNotificationRecipientList = (params) => async dispatch => {
    const url = Site.resolveClientUrl('ViewLog/GetNotificationRecipientList');
    await DataService.getDataAsync(url,params).then(response => {
        dispatch(setNotificationRecipientList(response.data));
    });
};

const getConversationMessageLog = (params) => async dispatch => {
    const url = Site.resolveClientUrl('ViewLog/GetConversationMessageLog');
    await DataService.getDataAsync(url,params).then(response => {
        dispatch(setConversationMessageLog(response.data));
    });
};

export const ActionCreators = {
    getParamsPageSystemMessage,
    getParamsPageRecipientList,
    getParamsPageConversationMessage,
    getNotificationSystemMessage,
    getNotificationRecipientList,
    getConversationMessageLog
};