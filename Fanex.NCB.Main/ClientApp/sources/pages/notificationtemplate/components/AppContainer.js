import './AppContainer.scss';

import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { NCBBreadcrum } from '../../common/breadcrumbs';
import Filter from './Filter';
import DataGrid from './DataGrid';
import Labels from '../../common/Labels';
import NewNotificationTemplate from './NewNotificationTemplate';
import EditNotificationTemplate from './EditNotificationTemplate';
import DeleteNotificationTemplate from './DeleteNotificationTemplate';
import "react-mde/lib/styles/css/react-mde-all.css";

export class AppContainer extends Component {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
        this.props.getNotificationTypes();
    }

    handleOpenCreateTemplatePopup = () => this.props.setCreateTemplatePopup({ show: true })

    handleCloseCreateTemplatePopup = () => this.props.setCreateTemplatePopup({ show: false })

    handleCreateSuccess = () => {
        this.handleCloseCreateTemplatePopup();
        this.props.setCreateTemplateResult(null);
        this.props.getGridData(this.props.gridParams);
    }

    handleEditSuccess = () => {
        this.props.setEditTemplatePopup({ show: false });
        this.props.setEditTemplateResult(null);
        this.props.getGridData(this.props.gridParams);
    }

    handleDeleteSuccess = () => {
        this.props.setDeleteTemplatePopup({ show: false });
        this.props.setDeleteTemplateResult(null);
        this.props.getGridData(this.props.gridParams);
    }

    render() {
        return (
            <>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification Template" />
                <Filter />
                <button className="btn-new" onClick={this.handleOpenCreateTemplatePopup}>{Labels.CreateNewTemplate}</button>
                <DataGrid />
                {this.props.createTemplatePopup.show &&
                    <NewNotificationTemplate onClose={this.handleCloseCreateTemplatePopup} onCreateSuccess={this.handleCreateSuccess}
                    />}

                {this.props.editTemplatePopup.show && <EditNotificationTemplate onEditSuccess={this.handleEditSuccess} />}
                {this.props.deleteTemplatePopup.show && <DeleteNotificationTemplate onDeleteSuccess={this.handleDeleteSuccess} />}

            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { gridParams, editTemplatePopup, createTemplatePopup, deleteTemplatePopup } = state;
    return {
        gridParams,
        editTemplatePopup,
        createTemplatePopup,
        deleteTemplatePopup
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);