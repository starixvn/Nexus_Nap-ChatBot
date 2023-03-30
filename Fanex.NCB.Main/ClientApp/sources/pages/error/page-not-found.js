import React from 'react';
import ReactDOM from 'react-dom';
import './style.scss';

const rootElement = document.getElementById('error-content');

ReactDOM.render(
    <div className="error-container">
        <div className="main-content-error">
            <img src="images/error/404.png" alt="404 page" />
            <div className="content-page-error">
                <h1>Page not found - 404</h1>
                <div className="describe">
                    <h2 className="content-error">The page you are looking for may have been moved, deleted, or possibly never existed.</h2>
                </div>
                <div className="btn">
                    <a href="Auth/Index" className="btn-login">Login Page</a>
                </div>
            </div>
        </div>
    </div>
    ,rootElement
);