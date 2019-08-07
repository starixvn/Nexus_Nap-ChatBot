import React from 'react';
import { renderLayout } from '../shared/layout/layout/index';

import { Provider } from 'react-redux';
import configureStore from './store/configureStore';
import App from './App';

// Get the application-wide store instance, prepopulating with state from the server where available.
const initialState = window.initialReduxState;
const store = configureStore(initialState);

renderLayout(
    <Provider store={store}>
        <App />
    </Provider>
);