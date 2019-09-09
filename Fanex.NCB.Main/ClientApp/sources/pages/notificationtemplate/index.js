import React from 'react';
import { renderLayout } from '../shared/layout/layout/index';

import { Provider } from 'react-redux';
import Store from '../lib/store';
import AppContainer from './components/AppContainer';
import reducer from './reducers/index';

// Get the application-wide store instance, prepopulating with state from the server where available.
const initialState = window.initialReduxState;
const store = Store.createStore(reducer, {
    notificationTypes: []
});

renderLayout(
    <Provider store={store}>
        <AppContainer />
    </Provider>
);