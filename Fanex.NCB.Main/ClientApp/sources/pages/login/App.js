import React, { Component } from 'react';
import Login from './components/login';
import { connect } from 'react-redux';

export class App extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Login />
        );
    }
}

export default connect()(App);