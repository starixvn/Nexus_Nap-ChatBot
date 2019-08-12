import React from 'react';
import { renderLayout } from '../shared/layout/layout/index';
import App from './App';

// Get the application-wide store instance, prepopulating with state from the server where available.

renderLayout(
    <App />
);