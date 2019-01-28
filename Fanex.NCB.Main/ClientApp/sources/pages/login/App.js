import React, { Component } from 'react';
import Login from './components/Login';
import { connect } from 'react-redux';

class App extends Component {
  constructor(props) {
    super(props);      

  }
 
  render() { 
    return (      
       <Login />
    );
  }
}

export default connect(
  state => state.login
)(App);