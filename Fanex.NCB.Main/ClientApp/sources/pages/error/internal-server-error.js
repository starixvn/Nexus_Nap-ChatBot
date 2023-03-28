import React from 'react';
import ReactDOM from 'react-dom';
import './style.scss';

const rootElement = document.getElementById('error-content');

ReactDOM.render(
    <div className="error-page">
        <p className="error-message">Oops, something went wrong.</p>
        <p className="error-message">Try to refresh this page of feel free to contact us if the problem persists</p>
        <p className="error-code">500</p>
        <p className="error-message">Server Error</p>
    </div>,
    rootElement
);