import React, { Component } from 'react';
import NotificationTemplate from './components/NotificationTemplate';
import { connect } from 'react-redux';

class App extends Component {
  constructor(props) {
    super(props);      

  }
 
  render() { 
    return (      
       <NotificationTemplate />
    );
  }
}

export default connect(
  state => state.login
)(App);