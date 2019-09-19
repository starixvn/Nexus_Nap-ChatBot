import types from './types';
import axios from 'axios';
import Site from '../../lib/site';

// IsHost
export const setIsHost = data => {
  return {
    type: types.IS_HOST,
    data: data
  };
};

export const getIsHost = () => {
  return dispatch => {
    dispatch(setIsHost(document.getElementById('is-host').value === "True"));
  };
};

// Grid
export const setGridParams = data => {
    return {
        type: types.GRID_PARAMS,
        data: data
    };
};

export const setGridData = data => {
    return {
        type: types.GRID_DATA,
        data: data
    };
};

export const getGridData = params => {
    const url = Site.resolveClientUrl('NotificationType/GetAll');
    return dispatch => {
        axios.get(url, { params: params }).then(response => {
            dispatch(setGridData(response.data));
        });
    };
};

// Settings
export const setSettings = data => {
    return {
        type: types.SETTING_DATA,
        data: data
    };
};

export const getSettings = () => {
    const url = Site.resolveClientUrl('NotificationType/GetSettings');
    return dispatch => {
        axios.get(url).then(response => {
            dispatch(setSettings(response.data));
        });
    };
};

// Delete
export const setDeleteData = data => {
    return {
        type: types.DELETE_DATA,
        data: data
    };
};

export const deleteItem = notificationTypeId => {
    const url = Site.resolveClientUrl('NotificationType/Delete');
    const headers = {
        'Content-Type': 'application/json',
        'RequestVerificationToken': Site.getXsrfToken()
    };

    return async dispatch => {
        const response = await axios.post(url, notificationTypeId, { headers: headers });
        const data = await response.data;
        dispatch(setDeleteData({ deleteResult: data }));
    };
};


export const refreshDeleteData = () => {
  return dispatch => {
    dispatch(setDeleteData({ deleteResult: null }));
  };
};

// Update
export const setUpdateParams = value => {
    return {
        type: types.UPDATE_PARAMS,
        data: value
    };
};

export const setUpdateData = data => {
    return {
        type: types.UPDATE_DATA,
        data: data
    };
};

export const update = (params, isNew) => {
    const url = isNew ? 'NotificationType/Add' : 'NotificationType/Edit';
    const headers = {
        'Content-Type': 'application/json',
        'RequestVerificationToken': Site.getXsrfToken()
    };

    return async dispatch => {
        const response = await axios.post(Site.resolveClientUrl(url), JSON.stringify(params), { headers: headers });
        const data = await response.data;
        dispatch(setUpdateData({ addedresult: data }));
    };
};

export const refreshUpdateData = () => {
    return dispatch => {
        dispatch(setUpdateData({ addedresult: null }));
    };
};

const actions = {
  setIsHost,
  getIsHost,
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
  refreshUpdateData,
  refreshDeleteData
};

export default actions;