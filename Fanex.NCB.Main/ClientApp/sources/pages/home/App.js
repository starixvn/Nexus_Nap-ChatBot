import React, { Component } from 'react';
import Layout from './components/Layout';
import { connect } from 'react-redux';

class App extends Component {
  constructor(props) {
    super(props);      

  }
 
  render() { 
    return (      
       <Layout />
    );
  }
}

export default connect(
  state => state.login
)(App);