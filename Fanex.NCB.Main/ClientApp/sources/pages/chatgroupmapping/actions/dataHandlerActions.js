import * as types from './types';
import axios from 'axios';
//import Site from '../../lib/site';

// Settings
export function getSettings() {
    const url = '/ChatGroupMapping/GetSettings';

    return (dispatch) => {        
        axios.get(url).then(response => {
            dispatch(setSettings(response.data))
        });
    };
}

export function setSettings(data) {
    return {
        type: types.SETTING_DATA,
        data: data
    };
}

// Grid
export function setGridParams(value) {
    return {
        type: types.GRID_PARAMS,
        data: value
    }
}

export function getGridData(params) {
    const url = '/ChatGroupMapping/GetAll';

    return (dispatch) => {
        axios.get(url, { params: params }).then(response => {
            dispatch(setGridData(response.data))
        });
    };
}

export function setGridData(data) {
    return {
        type: types.GRID_DATA,
        data: data
    };
}