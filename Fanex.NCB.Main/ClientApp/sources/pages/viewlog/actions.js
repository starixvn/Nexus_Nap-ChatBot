import Site from '../lib/site';
import DataService from '../lib/dataService';

import {
    setParamsPage,
    setSystemMessage,
    setRecipientList,
    setConversationMessage
} from './viewLogSlice';

const getParamsPage = (params) => dispatch => {
    dispatch(setParamsPage(params));
}

const getSystemMessage = (params) => async dispatch => {
    const url = Site.resolveClientUrl('ViewLog/GetSystemMessage');
    await DataService.getDataAsync(url,params).then(response => {
        dispatch(setSystemMessage(response.data));
    })
};

const getRecipientList = (params) => async dispatch => {
    const url = Site.resolveClientUrl('ViewLog/GetRecipientList');
    await DataService.getDataAsync(url,params).then(response => {
        dispatch(setRecipientList(response.data));
    })
};

const getConversationMessage = (params) => async dispatch => {
    const url = Site.resolveClientUrl('ViewLog/GetConversationMessage');
    await DataService.getDataAsync(url,params).then(response => {
        dispatch(setConversationMessage(response.data));
    })
};

export const ActionCreators = {
    getParamsPage,
    getSystemMessage,
    getRecipientList,
    getConversationMessage
};