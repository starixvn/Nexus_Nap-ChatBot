import React from 'react';
import { connect } from 'react-redux';
import './Header.scss';

const Header = props => (
  <div className="header">
      <div className="col-sm-12 col-xs-12 top-header">
          <ul className="list-unstyled pull-right b-ul__userinfo">
              <li className="datetime"><span className="icon-clock"></span>2:26:04 PM Nov 30, 2018 GMT+8</li>
              <li className="user-info">
                  <a data-toggle="dropdown"><span className="info-bar">Welcome Sa<span className="icon-arrow-user"></span></span></a>
                  <ul className="dropdown-menu menu-user-info" role="menu">
                      <li className="b-li__userinfo"><a className="userinfo-feature" href="/Auth/Index"><span className="icon-signout"></span>Sign out</a></li>
                  </ul>
              </li>
          </ul>
      </div>
  </div>
);

export default connect()(Header);
