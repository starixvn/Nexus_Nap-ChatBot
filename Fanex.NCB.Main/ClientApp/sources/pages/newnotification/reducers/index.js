import { combineReducers } from 'redux';
import * as dataHandlerReducer  from './dataHandlerReducer';
import * as gridHanderReducer from './gridHandlerReducer';

export default combineReducers({
    ...dataHandlerReducer,
    ...gridHanderReducer
})
