import React from 'react';
import { Provider } from 'react-redux';
import { configureStore } from '@reduxjs/toolkit';
import { renderLayout } from '../shared/layout/layout/index';

import AppContainer from './components/AppContainer';
import chatGroupMappingReducer from './chatGroupMappingSlice';
const store = configureStore({
    reducer: chatGroupMappingReducer
});

renderLayout(
    <Provider store={store}>
        <AppContainer />
    </Provider>
);