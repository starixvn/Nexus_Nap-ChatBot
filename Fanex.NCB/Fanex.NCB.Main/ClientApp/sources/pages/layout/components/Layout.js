import React from 'react';
import { connect } from 'react-redux';
import './Layout.scss';
import Header from './Header';
import Content from './Content';

const Layout = props => (
  <div>
    <Header></Header>
    <Content></Content>
  </div>
);

export default connect()(Layout);
