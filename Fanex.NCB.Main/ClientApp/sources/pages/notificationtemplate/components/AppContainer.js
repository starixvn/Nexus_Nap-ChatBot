import './AppContainer.scss';

import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { NCBBreadcrum } from '../../common/breadcrumbs';
import Filter from './Filter';
import DataGrid from './DataGrid';
import UpdatePopup from './UpdatePopup';
import DeletePopup from './DeletePopup';
import Labels from '../../common/Labels';

export class AppContainer extends Component {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
        this.props.getNotificationTypes();
    }

    closeUpdatePopupHandler = () => {
        //todo
    }

    closeDeletePopupHandler = () => {
        //todo
    }

    render() {
        return (
            <>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification Template" />
                <Filter />
                <button className="btn-new">{Labels.CreateNewTemplate}</button>
                <DataGrid />
                {this.props.updatePopupData.isShowed && <UpdatePopup closeUpdatePopupHandler={this.closeUpdatePopupHandler} />}
                {this.props.deletePopupData.isShowed && <DeletePopup closeDeletePopupHandler={this.closeDeletePopupHandler} />}
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { gridParams, updatePopupData, deletePopupData, notificationTypes } = state;
    return {
        gridParams,
        updatePopupData,
        deletePopupData,
        notificationTypes
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);