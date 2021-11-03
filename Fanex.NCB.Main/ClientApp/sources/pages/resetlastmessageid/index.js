import React from 'react';
import { Provider } from 'react-redux';
import { renderLayout } from '../shared/layout/layout/index';
import { configureStore } from '@reduxjs/toolkit';
import AppContainer from './components/AppContainer';
import resetLastMessageIdSlice from './resetLastMessageIdSlice';

const store = configureStore({
    reducer: resetLastMessageIdSlice
});

renderLayout(
    <Provider store={store}>
        <AppContainer />
    </Provider>
);