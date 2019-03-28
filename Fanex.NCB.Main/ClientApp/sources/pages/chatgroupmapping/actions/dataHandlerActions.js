import * as types from './types';
import axios from 'axios';
//import Site from '../../lib/site';

// Settings
export function setSettings(data) {
    return {
        type: types.SETTING_DATA,
        data: data
    };
}

export function getSettings() {
    const url = '/ChatGroupMapping/GetSettingsAsync';

    return (dispatch) => {        
        axios.get(url).then(response => {
            dispatch(setSettings(response.data));
        });
    };
}


// Grid
export function setGridParams(value) {
    return {
        type: types.GRID_PARAMS,
        data: value
    };
}

export function setGridData(data) {
    return {
        type: types.GRID_DATA,
        data: data
    };
}

export function getGridData(params) {
    const url = '/ChatGroupMapping/GetAllAsync';

    return (dispatch) => {
        axios.get(url, { params: params }).then(response => {
            dispatch(setGridData(response.data));
        });
    };
}

// Edit Popup
export function setFilterRecipients(data) {
    return {
        type: types.FILTERED_RECIPIENT,
        data: data
    };
}

export function filterRecipients(recipients, recipientTypeId) {
    let filteredRecipients = [];
    recipientTypeId && recipients.map((item, index) => {
        if (item.recipientTypeId === recipientTypeId) {
            filteredRecipients.push(item);
        }
    });  

    return (dispatch) => {       
        dispatch(setFilterRecipients(filteredRecipients));
    };
}

export function setUpdateData(data) {
    return {
        type: types.UPDATE_DATA,
        data: data
    };
}

export function update(params) {
    const url = '/ChatGroupMapping/EditAsync';

    let headers = {
        'Content-Type': 'application/json'
    };
    return async (dispatch) => {
        const request = await axios.post(url, JSON.stringify(params), { headers: headers });
        const data = await request.data;
        console.log(data);
        dispatch(setUpdateData(data));
    };
}

export function refreshUpdateData() {
    return (dispatch) => {
        dispatch(setUpdateData(null));
    };
}
