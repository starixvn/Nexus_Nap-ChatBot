import React, { Component } from 'react';

export class ResponseMessage extends Component {
    constructor(props) {
        super(props);
    }

    isSuccess = () => this.props.responseData.isSuccessful;

    getClassName = () => this.isSuccess() ? 'msg-success' : 'msg-error';

    render() {
        if (!this.props.responseData || this.props.responseData.errorCode === undefined) {
            return null;
        }

        return (<div className={this.getClassName()}>{this.props.responseData.message}</div>);
    }
}