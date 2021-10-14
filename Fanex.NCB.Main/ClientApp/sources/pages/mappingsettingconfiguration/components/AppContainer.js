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
import EditMapping from './EditMapping';
import ConfirmPopup from '../../common/confirm-popup/ConfirmPopup';
import JobInfo from './JobInfo';
import EditJobInfo from './EditJobInfo';
import { internalCSDJobId } from '../../constants/constants';

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
    }

    handleCloseEditMappingPopup = () => {
        this.props.setEditMappingPopup({ show: false });
    }

    handleCreateSuccess = () => {
        this.handleCloseCreateMappingPopup();
        this.props.getGridData(this.props.gridParams);
    }

    handleEditSuccess = () => {
        this.props.setEditMappingPopup({ show: false });
        this.props.setEditMappingResult(null);
        this.props.getGridData(this.props.gridParams);
    }

    handleDeleteMapping = () => {
        let params = {
            templateId: this.props.deleteMappingPopup.templateId,
            msgContentsType: this.props.deleteMappingPopup.msgContentsType,
            jobId: this.props.jobInfo.id
        };

        this.props.deleteMapping(params).then(data => {
            if (data.isSuccessful) {
                this.props.getGridData(this.props.gridParams, this.props.setGridParams);
                toast.success(Labels.DeleteMappingSettingSuccessfully);
            } else {
                if (data.errors && data.errors.length > 0) {
                    toast.error(data.errors[0].errorMessage);
                }
                else {
                    toast.error(data.message || Labels.UnexpectedErrorsRequestMessage);
                }
            }
        });

        this.props.closeDeleteMappingPopup();
    }

    render() {
        const isIntelnalCSDJob = this.props.jobInfo && this.props.jobInfo.id === internalCSDJobId;
        return (
            <>
                <NCBBreadcrum chatbotName={Labels.ChatbotSystem} pageTitle="Mapping Setting Management" />
                <Filter />
                
                {
                    this.props.jobInfoFlag.isEdit? <EditJobInfo jobInfo={this.props.jobInfo} /> : <JobInfo />
                }

                {
                    this.haveHostPermission &&
                    <button
                        className={`${isIntelnalCSDJob ? 'btn-disabled' : 'btn-new'}`}
                        disabled={isIntelnalCSDJob ? true : false}
                        onClick={this.handleOpenCreateMappingPopup}>{Labels.CreateNewMappingSetting}
                    </button>
                }

                <DataGrid haveHostPermission={this.haveHostPermission} isIntelnalCSDJob={isIntelnalCSDJob} />
                {this.props.createMappingPopup.show &&
                    <NewMapping onClose={this.handleCloseCreateMappingPopup} onCreateSuccess={this.handleCreateSuccess} />
                }

                {this.props.editMappingPopup && this.props.editMappingPopup.show && <EditMapping onEditSuccess={this.handleEditSuccess} onClose={this.handleCloseEditMappingPopup} />}

                {this.props.deleteMappingPopup && this.props.deleteMappingPopup.show &&
                    <ConfirmPopup
                        isOpen={this.props.deleteMappingPopup.show}
                        onToggle={this.props.closeDeleteMappingPopup}
                        onConfirm={this.handleDeleteMapping}
                        content={`Do you want to delete Mapping Setting ${this.props.deleteMappingPopup.notificationTypeName} - ${this.props.deleteMappingPopup.notificationTemplateName}?`}
                    />
                }
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const {
        settingTypes,
        getNotificationTypes,
        gridParams,
        jobInfoFlag,
        editMappingPopup,
        createMappingPopup,
        deleteMappingPopup,
        jobInfo,
        setGridParams
    } = state;

    return {
        settingTypes,
        gridParams,
        editMappingPopup,
        createMappingPopup,
        deleteMappingPopup,
        jobInfoFlag,
        getNotificationTypes,
        jobInfo,
        setGridParams
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);