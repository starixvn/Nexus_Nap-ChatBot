import React, { Component } from 'react';
import { connect } from 'react-redux';
import Filter from './Filter';
import DataGrid from './DataGrid';
import { NCBBreadcrum } from '../../common/breadcrumbs';
import "../../../fonts/style.css";
import './AppContainer.scss';

class AppContainer extends Component {
    constructor(props) {
        super(props);
    }

    togglePopup() {
        
    }

    render() {
        return (
            <div>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Chat Group Mapping"></NCBBreadcrum>
                <Filter />
                <DataGrid />               
            </div>
        );
    }
}

export default connect()(AppContainer);
