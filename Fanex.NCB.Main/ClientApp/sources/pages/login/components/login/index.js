import React, { Component } from 'react';
import CryptoJS from '../../../lib/cryptoJS';
import AppData from '../../../lib/AppData';
import Site from '../../../lib/site';

import '../../../shared/components/font.css';
import '../../../shared/components/messages.scss';
import './Login.scss';

class Login extends Component {
    constructor(props) {
        super(props);
        this.state = {
            password: '',
            invalidLogin: AppData.invalidLogin
        };
        this.token = Site.getXsrfToken();
    }

    handleChangePassword = event => {
        this.setState({
            password: event.target.value
        });
    }

    handleSubmit = event => {
        event.preventDefault();
        this.setState({
            password: CryptoJS.cfsEnCode(this.refs.password.value)
        }, this.submitForm);
    }

    submitForm = () => {
        this.refs.form.submit();
    }

    render() {
        return (
            <div className="login-container">
                <form ref="form" method="post" action={Site.resolveClientUrl(`auth/SignIn${location.search}`)} className="form-login" onSubmit={this.handleSubmit}>
                    <input name="__RequestVerificationToken" type="hidden" value={this.token} />
                    <a className="logo" href="#"><img src={Site.resolveClientUrl('images/Logo.svg')} alt="Nap Chatbot" /></a>
                    <h1 className="header">Welcome to CHATBOT</h1>
                    {
                        this.state.invalidLogin
                            ? <div className="msg-error">Invalid Username or Password</div>
                            : null
                    }
                    <div className="input-group">
                        <div className="input-group-prepend">
                            <span className="icon-username" />
                        </div>
                        <input name="userName" type="text" className="form-control" required />
                    </div>
                    <div className="input-group">
                        <div className="input-group-prepend">
                            <span className="icon-password" />
                        </div>
                        <input ref="password" name="password" type="password" className="form-control"
                            required />
                    </div>
                    <input type="submit" value="SIGN IN" className="button" />
                </form>
                <div className="form-footer">
                    <span className="bold">Best view in:</span>
                    <ul className="list-inline">
                        <li><span className="icon-firefox" /> Firefox 64+</li>
                        <li><span className="icon-chrome" /> Chrome 71+</li>
                    </ul>
                </div>
            </div>
        );
    }
}

export default Login;