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
import NewMapping from './NewMapping';
import EditSettingType from './EditSettingType';
import ConfirmPopup from '../../common/confirm-popup/ConfirmPopup';
import JobInfo from './JobInfo';
import EditJobInfo from './EditJobInfo';

export class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.haveHostPermission = document.getElementById('have-host-permission') && document.getElementById('have-host-permission').checked;
    }

    componentDidMount() {
        this.props.getSettingTypes();
        this.props.getNotificationTypes();
        this.props.getJobInfo();
    }

    handleOpenCreateMappingPopup = () => this.props.setCreateMappingPopup({ show: true })

    handleCloseCreateMappingPopup = () => {
        this.props.setCreateMappingPopup({ show: false });
        this.props.setCreateSettingTypeResult(null);
    }

    handleCreateSuccess = () => {
        this.handleCloseCreateMappingPopup();
        this.props.getGridData(this.props.gridParams);
    }

    handleEditSuccess = () => {
        this.props.setEditSettingTypePopup({ show: false });
        this.props.setEditSettingTypeResult(null);
        this.props.getGridData(this.props.gridParams);
    }

    handleDeleteSettingType = () => {
        this.props.deleteSettingType(this.props.deleteSettingTypePopup.type).then(data => {
            if (data.isSuccessful) {
                this.props.getGridData(this.props.gridParams);
                toast.success(Labels.DeleteMsgContentsTypeSuccessfully);
            } else {
                toast.error(data.message);
            }
        });

        this.props.closeDeleteSettingTypePopup();
    }

    render() {
        return (
            <>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Mapping Setting Management" />
                <Filter />
                
                {
                    this.props.jobInfoFlag.isEdit? <EditJobInfo jobInfo={this.props.jobInfo} /> : <JobInfo />
                }

                {this.haveHostPermission &&
                    <button className="btn-new" onClick={this.handleOpenCreateMappingPopup}>{Labels.CreateNewMappingSetting}</button>
                }

                <DataGrid haveHostPermission={this.haveHostPermission} />
                {this.props.createMappingPopup.show &&
                    <NewMapping onClose={this.handleCloseCreateMappingPopup} onCreateSuccess={this.handleCreateSuccess} />
                }

                {/* {this.props.editSettingTypePopup && this.props.editSettingTypePopup.show && <EditSettingType onEditSuccess={this.handleEditSuccess} />} */}

                {/* {this.props.deleteSettingTypePopup && this.props.deleteSettingTypePopup.show &&
                    <ConfirmPopup
                        isOpen={this.props.deleteSettingTypePopup.show}
                        onToggle={this.props.closeDeleteSettingTypePopup}
                        onConfirm={this.handleDeleteSettingType}
                        content={`Do you want to delete Msg Contents Type ${this.props.deleteSettingTypePopup.name}?`}
                    />
                } */}
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { settingTypes, getNotificationTypes, gridParams, jobInfoFlag, editSettingTypePopup, createMappingPopup, deleteSettingTypePopup, getRecipients } = state;
    return {
        settingTypes,
        gridParams,
        editSettingTypePopup,
        createMappingPopup,
        deleteSettingTypePopup,
        jobInfoFlag,
        getNotificationTypes,
        getRecipients
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);