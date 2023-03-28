import React from 'react';
import ReactDOM from 'react-dom';
import './style.scss';

const rootElement = document.getElementById('error-content');

ReactDOM.render(
    <div className="error-page">
        <p className="error-message">The page you are looking for may have been moved, deleted or possibly never existed.</p>
        <p className="error-code">404</p>
    </div>,
    rootElement
);