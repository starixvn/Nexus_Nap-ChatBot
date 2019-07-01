import types from './types';
import axios from 'axios';
import Site from '../../lib/site';

// Grid
export const setGridParams = data => {  
    return {
        type: types.GRID_PARAMS,
        data: data
    };
}

export const getGridData = params => {   
    const url = Site.resolveClientUrl('NotificationType/GetAll');
    return dispatch => {
        axios.get(url, {params: params}).then(response => {        
            dispatch(setGridData(response.data));
        });
    };
}

export const setGridData = data => {
    return {
        type: types.GRID_DATA,
        data: data
    };
}

// Settings
export const getSettings = () => {
    const url = Site.resolveClientUrl('NotificationType/GetSettings');
    return dispatch => {
        axios.get(url).then(response => {
            dispatch(setSettings(response.data));
        });
    };
}

export const setSettings = data => {
    return {
        type: types.SETTING_DATA,
        data: data
    };
}

// Delete
export const deleteItem = notificationTypeId => { 
    const url = Site.resolveClientUrl('NotificationType/Delete');
    const headers = {
        'Content-Type': 'application/json'
    };

    return async dispatch => {
        const request = await axios.post(url, notificationTypeId, { headers: headers });
        const data = await request.data;
        dispatch(setDeleteData(data));
    };
}

export function setDeleteData(data){    
    return {
        type: types.DELETE_DATA,
        data: data
    };
}

// Update
export const setUpdateParams = value => {
    return {
        type: types.UPDATE_PARAMS,
        data: value
    };
}

export const update = (params, isNew) => {
    const url = isNew ? '/NotificationType/Add' : '/NotificationType/Edit';
    const headers = {
        'Content-Type': 'application/json'
    };

    return async dispatch => {
        const request = await axios.post(url, JSON.stringify(params), { headers: headers });
        const data = await request.data;
        dispatch(setUpdateData({addedresult: data}));
    };
}

export const setUpdateData = data => {
    return {
        type: types.UPDATE_DATA,
        data: data
    };
}

export const refreshUpdateData = () => {
    return dispatch => {
        dispatch(setUpdateData({ addedresult: false }));
    };
}

const actions = {
    setGridParams,
    getGridData,
    setGridData,
    getSettings,
    setSettings,
    deleteItem,
    setDeleteData,
    setUpdateParams,
    update,
    setUpdateData,
    refreshUpdateData
};

export default actions;


