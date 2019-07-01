import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { NCBBreadcrum } from '../../common/breadcrumbs';
import Filter from './Filter';
import DataGrid from './DataGrid';
import UpdatePopup from './UpdatePopup';
import DeletePopup from './DeletePopup';

import './AppContainer.scss';

export class AppContainer extends Component {
    constructor(props) {
        super(props);
    }

    closeUpdatePopupHandler = () => {
        //todo
    }

    closeDeletePopupHandler = () => {
        //todo
    }

    render() {
        return (
            <div>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification Template" />
                <Filter />
                <DataGrid />
                {this.props.updatePopupData.isShowed && <UpdatePopup closeUpdatePopupHandler={this.closeUpdatePopupHandler} />}
                {this.props.deletePopupData.isShowed && <DeletePopup closeDeletePopupHandler={this.closeDeletePopupHandler} />}
            </div>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { gridParams, updatePopupData, deletePopupData } = state;
    return {
        gridParams,
        updatePopupData,
        deletePopupData
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);
