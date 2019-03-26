import React, { Component } from 'react';
import NotificationList from './components/NotificationList';
import { connect } from 'react-redux';

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
       <NotificationList
           isModal={this.state.modal}
           toggleHandler={this.toggle}
       />
    );
  }
}

export default connect(
  state => state.login
)(App);