import {combineReducers} from 'redux';
import * as dataHandlerReducer  from './dataHandlerReducer';

export default combineReducers(Object.assign(
    dataHandlerReducer
));