import React, { Component } from 'react';
import { Route } from 'react-router';
//import Layout from './components/Layout';
import NewNotification from './components/NewNotification';
//import FetchData from './components/FetchData';
import { connect } from 'react-redux';
//import Login from './components/Login';

class App extends Component {
  constructor(props) {
    super(props);      
    this.state = {
      modal: false
    };

    this.toggle = this.toggle.bind(this);
  }

  toggle() {
    this.setState({
      modal: !this.state.modal
    });
  }
  render() { 
    return (      
       <NewNotification 
          isShowModal={this.state.modal} 
          showHideModalHandler={this.toggle} />
    );
  }
}

export default connect(
  state => state.login
)(App);