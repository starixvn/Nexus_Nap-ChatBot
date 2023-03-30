import React from 'react';
import ReactDOM from 'react-dom';
import './style.scss';

const rootElement = document.getElementById('error-content');

ReactDOM.render(
    <div className="error-container">
        <div className="main-content-error">
            <div className="content-page-error page-error">
                <h1>Internal Server Error - 500</h1>
                <div className="describe">
                    <h2 className="content-error">Oops, something went wrong.<br /> Try to refresh this page or feel free to contact us if the problem persists.</h2>
                </div>
                <div className="btn">
                    <a href="Auth/Index" className="btn-login">Login Page</a>
                </div>
            </div>
            <img src="images/error/500.jpg" alt="500 page" />
        </div>
    </div>,
    rootElement
);