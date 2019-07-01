import React, { Component } from 'react';
import Layout from './components/layout';
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

export default connect()(App);