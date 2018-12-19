import React from 'react';
import { connect } from 'react-redux';
import './Login.scss';

const Login = props => (
  <div className="login-container">
      <form method="post" action="/auth/SignIn" className="form-login">
          {/* <div className="logo"><img src={Logo} alt="Nap Chatbot" /></div> */}

          <div className="logo"></div>

          <h1 className="header">Welcome to CHATBOT</h1>
          <div className="input-group">
              <div className="input-group-prepend">
                <span className="icon-username"></span>
              </div>
              <input name="userName" type="text" className="form-control"/>
          </div>
          <div className="input-group">
              <div className="input-group-prepend">
                <span className="icon-username"></span>
              </div>
              <input name="password" type="password" className="form-control"/>
              <input type="hidden" name="returnUrl" value="/Home/Index" />
          </div>
          <input type="submit" value="SIGN IN" className="button"/>
      </form>
      <div className="form-footer">
            <span className="bold">Best view in:</span>
            <ul className="list-inline">
                <li><span className="icon-firefox"></span> Firefox 64+</li>
                <li><span className="icon-chrome"></span> Chrome 71+</li>
            </ul>
      </div>
  </div>
);

export default connect()(Login);
