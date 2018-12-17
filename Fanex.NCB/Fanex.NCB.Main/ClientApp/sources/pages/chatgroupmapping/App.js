import React, { Component } from 'react';
import { Route } from 'react-router';
//import Layout from './components/Layout';
import ChatGroupMapping from './components/ChatGroupMapping';
//import FetchData from './components/FetchData';
import { connect } from 'react-redux';
//import Login from './components/Login';

class App extends Component {
  constructor(props) {
    super(props);      

  }
 
  render() { 
    return (      
       <ChatGroupMapping />
    );
  }
}

export default connect(
  state => state.login
)(App);