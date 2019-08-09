import React, { Component } from 'react';

export default class ResponseMessage extends Component {
    constructor(props) {
        super(props);
    }

    getClassName = () => this.props.isSuccessful ? 'msg-success' : 'msg-error';

    render() {
        return (<div className={this.getClassName()}>{this.props.message}</div>);
    }
}