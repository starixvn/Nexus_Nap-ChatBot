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
import JobInfo from './JobInfo';
import EditJobInfo from './EditJobInfo';

export class AppContainer extends Component {
    constructor(props) {
        super(props);
        this.state = {
            isEditJobInfo: false
        }

        this.haveHostPermission = document.getElementById('have-host-permission') && document.getElementById('have-host-permission').checked;
    }

    componentDidMount() {
        this.props.getSettingTypes();
        this.props.getJobInfo();
    }

    handleOpenCreateSettingPopup = () => this.props.setCreateSettingPopup({ show: true })

    handleCloseCreateSettingTypePopup = () => {
        this.props.setCreateSettingPopup({ show: false });
        this.props.setCreateSettingTypeResult(null);
    }

    handleCreateSuccess = () => {
        this.handleCloseCreateSettingTypePopup();
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
                    this.state.isEditJobInfo? <EditJobInfo /> : <JobInfo />
                }

                {this.haveHostPermission &&
                    <button className="btn-new" onClick={this.handleOpenCreateSettingPopup}>{Labels.CreateNewMappingSetting}</button>
                }

                <DataGrid haveHostPermission={this.haveHostPermission} />
                {/* {this.props.createSettingTypePopup.show &&
                    <NewSettingType onClose={this.handleCloseCreateSettingTypePopup} onCreateSuccess={this.handleCreateSuccess} />
                } */}

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
    const { settingTypes, gridParams, jobInfo, getJobInfo, editSettingTypePopup, createSettingTypePopup, deleteSettingTypePopup } = state;
    return {
        settingTypes,
        gridParams,
        editSettingTypePopup,
        createSettingTypePopup,
        deleteSettingTypePopup,
        jobInfo,
        getJobInfo
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);