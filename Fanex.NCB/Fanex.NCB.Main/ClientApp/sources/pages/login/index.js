import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import configureStore from './store/configureStore';
import App from './App';

// Get the application-wide store instance, prepopulating with state from the server where available.
const initialState = window.initialReduxState;
const store = configureStore(history, initialState);
const rootElement = document.getElementById('login');

ReactDOM.render(
  <Provider store={store}>
      <App />
  </Provider>,
  rootElement); 


