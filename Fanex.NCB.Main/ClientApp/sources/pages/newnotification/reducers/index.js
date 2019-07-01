import { combineReducers } from 'redux';
import dataHandlerReducer  from './dataHandlerReducer';
import gridHanderReducer from './gridHandlerReducer';

export default combineReducers({
    ...dataHandlerReducer,
    ...gridHanderReducer
});