import React from 'react';
import { renderLayout } from '../shared/layout/layout/index';
import './style.scss';

renderLayout(
    <div className="error-page">
        <p className="error-message">Access denied!</p>
        <p className="error-message">You no longer have permission to access this page. Please re-login or contact admin for support.</p>
        <p className="error-code">403</p>
    </div>
);