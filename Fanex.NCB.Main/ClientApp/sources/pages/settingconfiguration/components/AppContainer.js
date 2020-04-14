import './AppContainer.scss';
import "react-mde/lib/styles/css/react-mde-all.css";

import React, { Component } from 'react';
import { connect } from 'react-redux';
import { toast } from 'react-toastify';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { NCBBreadcrum } from '../../common/breadcrumbs/breadcrumbs';
import Filter from './Filter';
import DataGrid from './DataGrid';
import Labels from '../../common/Labels';
import NewSettingType from './NewSettingType';
import EditSettingType from './EditSettingType';
import ConfirmPopup from '../../common/confirm-popup/ConfirmPopup';
import MarkdownRules from '../../common/markdown-rules';

export class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.haveHostPermission = document.getElementById('have-host-permission') && document.getElementById('have-host-permission').checked;
    }

    componentDidMount() {
        this.props.getSettingTypes();
        console.log('mr', this.props)
    }

    handleOpenCreateSettingPopup = () => this.props.setCreateSettingPopup({ show: true })

    handleCloseCreateSettingTypePopup = () => this.props.setCreateSettingPopup({ show: false })

    handleCreateSuccess = () => {
        this.handleCloseCreateSettingTypePopup();
        this.props.setCreateSettingTypeResult(null);
        this.props.getGridData(this.props.gridParams);
    }

    handleEditSuccess = () => {
        this.props.setEditSettingTypePopup({ show: false });
        this.props.setEditSettignTypeResult(null);
        this.props.getGridData(this.props.gridParams);
    }

    // handleDeleteTemplate = () => {
    //     this.props.deleteTemplate(this.props.deleteTemplatePopup.templateId).then(data => {
    //         if (data.isSuccessful) {
    //             this.props.getGridData(this.props.gridParams);
    //             toast.success(Labels.DeleteTemplateSuccessfully);
    //         } else {
    //             toast.error(data.message);
    //         }
    //     });

    //     this.props.closeDeleteTemplatePopup();
    // }

    render() {
        return (
            <>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Setting Management" />
                <Filter />
                {this.haveHostPermission &&
                    <button className="btn-new" onClick={this.handleOpenCreateSettingPopup}>{Labels.CreateNewSetting}</button>
                }
                <DataGrid haveHostPermission={this.haveHostPermission} />
                {this.props.createSettingTypePopup.show &&
                    <NewSettingType onClose={this.handleCloseCreateSettingTypePopup} onCreateSuccess={this.handleCreateSuccess} />
                }

                {this.props.editSettingTypePopup && this.props.editSettingTypePopup.show && <EditSettingType onEditSuccess={this.handleEditSuccess} />}
                {/* {this.props.deleteTemplatePopup.show &&
                    <ConfirmPopup
                        isOpen={this.props.deleteTemplatePopup.show}
                        onToggle={this.props.closeDeleteTemplatePopup}
                        onConfirm={this.handleDeleteTemplate}
                        content=" Do you want to delete the selected notification template?"
                    />
                } */}
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { gridParams, editSettingTypePopup, createSettingTypePopup, deleteTemplatePopup } = state;
    return {
        gridParams,
        editSettingTypePopup,
        createSettingTypePopup,
        // deleteTemplatePopup,
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);