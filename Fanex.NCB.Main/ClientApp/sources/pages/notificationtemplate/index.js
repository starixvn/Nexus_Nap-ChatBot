import React from 'react';
import { renderLayout } from '../shared/layout/layout/index';

import { Provider } from 'react-redux';
import Store from '../lib/store';
import AppContainer from './components/AppContainer';
import reducer from './reducers/index';

// Get the application-wide store instance, prepopulating with state from the server where available.
const store = Store.createStore(reducer, {
    notificationTypes: [],
    createTemplatePopup: {
        show: false
    },
    editTemplatePopup: {
        show: false
    },
    deleteTemplatePopup: {
        show: false
    },
    createTemplateResult: null,
    editTemplateResult: null,
    deleteTemplateResult: null,
    markdownRulesPopup: {
        show: false
    }
});

renderLayout(
    <Provider store={store}>
        <AppContainer />
    </Provider>
);