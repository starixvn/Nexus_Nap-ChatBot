import React from 'react';
import { renderLayout } from '../shared/layout/layout/index';

import { Provider } from 'react-redux';
import Store from '../lib/store';
import AppContainer from './components/appContainer';
import reducer from './reducers';
// Get the application-wide store instance, prepopulating with state from the server where available.
const store = Store.createStore(reducer,
    {
        createNotificationResult: null,
        confimrationPopup: {
            isOpen: false,
            content: ''
        }
    });

renderLayout(
    <Provider store={store}>
        <AppContainer />
    </Provider>
);