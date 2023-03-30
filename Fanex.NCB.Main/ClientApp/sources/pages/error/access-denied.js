import React from 'react';
import { renderLayout } from '../shared/layout/layout/index';
import './style.scss';

renderLayout(
    <div className="error-container">
        <div className="main-content-error">
            <img src="images/error/403.jpg" alt="403 page" />
            <div className="content-page-error">
                <h1>Unauthorized user - 403</h1>
                <div className="describe">
                    <h2 className="title-sub">Access denied!</h2>
                    <h2 className="content-error">You no longer have permission to access this page. Please re-login or contact admin for support.</h2>
                </div>
                <div className="btn">
                    <a href="Auth/Index" className="btn-login">Login Page</a>
                </div>
            </div>
        </div>
    </div>
);