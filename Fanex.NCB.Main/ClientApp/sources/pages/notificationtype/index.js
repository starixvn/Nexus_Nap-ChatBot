import React from 'react';
import { Provider } from 'react-redux';
import { configureStore } from '@reduxjs/toolkit';
import AppContainer from './components/AppContainer';
import { renderLayout } from '../shared/layout/layout/index';

import notificationTypeReducer from './notificationTypeSlice';
const store = configureStore({
    reducer: notificationTypeReducer
});

const isHost = document.getElementById('is-host').value === 'True';

renderLayout(
    <Provider store={store}>
        <AppContainer isHost={isHost} />
    </Provider>
);