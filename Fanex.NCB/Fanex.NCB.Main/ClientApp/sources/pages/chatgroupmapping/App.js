import React, { Component } from 'react';
import ChatGroupMapping from './components/ChatGroupMapping';
import { connect } from 'react-redux';

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