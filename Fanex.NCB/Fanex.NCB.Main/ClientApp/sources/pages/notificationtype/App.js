import React, { Component } from 'react';
import NotificationType from './components/NotificationType';
import { connect } from 'react-redux';

class App extends Component {
  constructor(props) {
    super(props);      
  }
 
  render() { 
    return (      
       <NotificationType />
    );
  }
}

export default connect()(App);