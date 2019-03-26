import React, { Component } from 'react';

export class ResponseMessage extends Component {
    constructor(props) {
        super(props);       
    }    

    isSuccess = () => {
        return this.props.responseData.errorCode === 0;
    }

    getClassName = () => {
        return this.isSuccess() ? "msg-success" : "msg-error";
    }

    getMessage = () => {
        return this.isSuccess() ? this.props.successMsg : this.props.responseData.errorMsg;        
    }

    render() {
        if (!this.props.responseData || this.props.responseData.errorCode === undefined) {
            return null;
        }        

        return (<div className={this.getClassName()}>{this.getMessage()}</div>);
    }
}
