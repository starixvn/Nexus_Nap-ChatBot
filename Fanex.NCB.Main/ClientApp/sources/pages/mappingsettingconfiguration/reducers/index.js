import { combineReducers } from 'redux';
import dataHandlerReducer from './dataHandlerReducer';

export default combineReducers({
    ...dataHandlerReducer
});