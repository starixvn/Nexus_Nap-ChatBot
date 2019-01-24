import * as types from './types';
import axios from 'axios';
//import Site from '../../lib/site';

// Grid
export function setGridParams(value){  
    return {
        type: types.GRID_PARAMS,
        data: value
    }
}

export function getGridData(params){   
    const url = '/NotificationType/GetAll';    
    
    return (dispatch) => {
        axios.get(url, {params: params}).then(response => {            
            dispatch(setGridData(response.data))
        });
    };
}

export function setGridData(data){
    return {
        type: types.GRID_DATA,
        data: data
    };
}

// Settings
export function getSettings(){
    const url = '/NotificationType/GetSettings';
    
    return (dispatch) => {
        axios.get(url).then(response => {
            dispatch(setSettings(response.data))
        });
    };
}

export function setSettings(data){
    return {
        type: types.SETTING_DATA,
        data: data
    };
}

// Delete
export function deleteItem(id){ 
    const url = '/NotificationType/Delete';   
    var params = JSON.stringify({id: id});

    return async(dispatch) => {
        const request = await axios.post(url, params);
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
export function setUpdateParams(value){
    return {
        type: types.UPDATE_PARAMS,
        data: value
    }
}

export function update(params, isNew){
    const url = isNew ? '/NotificationType/Add' : '/NotificationType/Edit';

    return async(dispatch) => {
        const request = await axios.post(url, JSON.stringify(params));
        const data = await request.data; 
        dispatch(setUpdateData({addedresult: data}));        
    };
} 

export function setUpdateData(data){
    return {
        type: types.UPDATE_DATA,
        data: data
    };
}

export function refreshUpdateData(){
    return (dispatch) => {
        dispatch(setUpdateData({addedresult: false}));
    };    
}

