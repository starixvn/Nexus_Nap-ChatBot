import React from 'react';
import { renderLayout } from '../shared/layout/layout/index';
import { Provider } from 'react-redux';
import { configureStore } from '@reduxjs/toolkit';
import AppContainer from './components/AppContainer';
import viewLogSlice from './viewLogSlice';

const store = configureStore({
    reducer: viewLogSlice
});

renderLayout(
    <Provider store={store}>
        <AppContainer/>
    </Provider>
);