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