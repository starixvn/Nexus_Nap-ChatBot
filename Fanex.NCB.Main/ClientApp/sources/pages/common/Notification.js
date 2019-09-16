import React, { Component } from 'react';

export class Notification extends Component {
  constructor(props) {
    super(props);  
  }

  render() {
    let message = this.props.isSuccess ? this.props.successMsg : this.props.failedMsg;
    let clsName = this.props.isSuccess ? 'msg-success' : 'msg-error';
    
    return (
      <div className={clsName}>{message}</div>
    );
  }
}