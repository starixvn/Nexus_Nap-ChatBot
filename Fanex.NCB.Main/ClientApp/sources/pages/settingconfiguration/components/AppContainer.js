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
import NewSettingConfiguration from './NewSettingConfiguration';
import EditSettingConfiguration from './EditSettingConfiguration';
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
        // this.props.setCreateTemplateResult(null);
        this.props.getGridData(this.props.gridParams);
    }

    // handleEditSuccess = () => {
    //     this.props.setEditTemplatePopup({ show: false });
    //     this.props.setEditTemplateResult(null);
    //     this.props.getGridData(this.props.gridParams);
    // }

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
                    <NewSettingConfiguration onClose={this.handleCloseCreateSettingTypePopup} onCreateSuccess={this.handleCreateSuccess} />
                }

                {/* {this.props.editTemplatePopup.show && <EditSettingConfiguration onEditSuccess={this.handleEditSuccess} />}
                {this.props.deleteTemplatePopup.show &&
                    <ConfirmPopup
                        isOpen={this.props.deleteTemplatePopup.show}
                        onToggle={this.props.closeDeleteTemplatePopup}
                        onConfirm={this.handleDeleteTemplate}
                        content=" Do you want to delete the selected notification template?"
                    />
                }

                {this.props.markdownRulesPopup.show &&
                    <MarkdownRules show={this.props.markdownRulesPopup.show} onToggle={this.props.closeMarkdownRulesPopup} />
                } */}

            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { gridParams, editTemplatePopup, createSettingTypePopup, deleteTemplatePopup, markdownRulesPopup } = state;
    return {
        gridParams,
        // editTemplatePopup,
        createSettingTypePopup,
        // deleteTemplatePopup,
        // markdownRulesPopup
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);