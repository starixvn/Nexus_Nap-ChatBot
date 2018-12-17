import React, { Component } from 'react';
import { Route } from 'react-router';
//import Layout from './components/Layout';
import NotificationList from './components/NotificationList';
//import FetchData from './components/FetchData';
import { connect } from 'react-redux';
//import Login from './components/Login';

class App extends Component {
  constructor(props) {
    super(props);      

  }
 
  render() { 
    return (      
       <NotificationList />
    );
  }
}

export default connect(
  state => state.login
)(App);