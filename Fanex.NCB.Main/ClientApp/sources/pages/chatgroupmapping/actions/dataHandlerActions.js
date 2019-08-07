import types from './types';
import axios from 'axios';
import Site from '../../lib/site';
import errorResponseHandler from '../../lib/errorResponseHandler';

// Settings
export const setSettings = data => {
    return {
        type: types.SETTING_DATA,
        data: data
    };
};

export const getSettings = () => {
    const url = Site.resolveClientUrl('ChatGroupMapping/GetSettings');
    return dispatch => {
        axios.get(url).then(response => {
            dispatch(setSettings(response.data));
            const submitButton = document.getElementById('submit-button');
            if (submitButton) {
                submitButton.click();
            };
        }).catch(() => { errorResponseHandler(); });
    };
};

// Grid
export const setGridParams = value => {
    return {
        type: types.GRID_PARAMS,
        data: value
    };
};

export const setGridData = data => {
    return {
        type: types.GRID_DATA,
        data: data
    };
};

export const getGridData = params => {
    const url = Site.resolveClientUrl('ChatGroupMapping/GetAll');
    return dispatch => {
        axios.get(url, { params: params }).then(response => {
            dispatch(setGridData(response.data));
        }).catch(() => { errorResponseHandler(); });
    };
};

// Edit Popup
export const setFilterRecipients = data => {
    return {
        type: types.FILTERED_RECIPIENT,
        data: data
    };
};

export const filterRecipients = (recipients, recipientTypeId) => {
    const filteredRecipients = [];
    recipientTypeId && recipients.map(item => {
        if (item.recipientTypeId === parseInt(recipientTypeId, 10)) {
            filteredRecipients.push(item);
        }
    });

    return dispatch => {
        dispatch(setFilterRecipients(filteredRecipients));
    };
};

export const setUpdateData = data => {
    return {
        type: types.UPDATE_DATA,
        data: data
    };
};

export const update = params => {
    const url = Site.resolveClientUrl('ChatGroupMapping/Edit');
    const headers = {
        'Content-Type': 'application/json',
        'RequestVerificationToken': Site.getXsrfToken()
    };

    return async dispatch => {
        try {
            const request = await axios.post(url, JSON.stringify(params), { headers: headers });
            const data = await request.data;
            dispatch(setUpdateData(data));
        } catch (err) {
            errorResponseHandler();
        }
    };
};

export const refreshUpdateData = () => {
    return dispatch => {
        dispatch(setUpdateData(null));
    };
};

const actions = {
    setSettings,
    getSettings,
    setGridParams,
    setGridData,
    getGridData,
    setFilterRecipients,
    filterRecipients,
    setUpdateData,
    update,
    refreshUpdateData
};

export default actions;