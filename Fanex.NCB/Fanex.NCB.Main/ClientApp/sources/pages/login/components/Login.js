import React from 'react';
import { connect } from 'react-redux';
import './Login.scss';

const Login = props => (
  <div className="login-container">
      <form method="post" action="/auth/SignIn" className="form-login">
          <div className="header">ChatBot System</div>
          <div className="field">
              <input name="userName" type="text" className="txtinput username"/>
          </div>
          <div className="field">
              <input name="password" type="password" className="txtinput password"/>
              <input type="hidden" name="returnUrl" value="/Home/Index" />
          </div>
          <div className="field">
              <input type="submit" value="SIGN IN" className="btnSubmit"/>
          </div>
      </form>
  </div>
);

export default connect()(Login);
