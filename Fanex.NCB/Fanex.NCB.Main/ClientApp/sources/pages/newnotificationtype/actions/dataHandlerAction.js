import * as types from './types';
import axios from 'axios';
//import Site from '../../lib/site';

const fetchData = (params) => {
    const url = '/NewNotificationType/GetAll';
    return axios.get(url, params);
}

export function getNotificationTypes(){
    return (dispatch) => {
        fetchData().then(response => {
            dispatch(setNotificationTypes(response.data))
        });
    };
}
   
export function setNotificationTypes(data){
    return {
        type: types.RECEIVE_NOTIFICATION_TYPES,
        data: data
    };
}