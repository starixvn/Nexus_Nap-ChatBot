import * as types from './types';
import axios from 'axios';
import Site from '../../lib/site';
import errorResponseHandler from '../../lib/errorResponseHandler';

// Settings
export function setSettings(data) {
    return {
        type: types.SETTING_DATA,
        data: data
    };
}

export function getSettings() {
    const url = Site.resolveClientUrl('NewNotification/GetSettings');

    return (dispatch) => {        
        axios.get(url).then(response => {
            dispatch(setSettings(response.data));
        }).catch(() => { errorResponseHandler(); });
    };
}


