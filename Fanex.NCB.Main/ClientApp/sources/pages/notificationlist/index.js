import React from 'react';
import { renderLayout } from '../shared/layout/layout/index';
import NotificationList from './components/NotificationList';

// Get the application-wide store instance, prepopulating with state from the server where available.

renderLayout(
    <NotificationList />
);