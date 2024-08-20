import './AppContainer.scss';
import "react-mde/lib/styles/css/react-mde-all.css";

import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import Labels from '../../common/Labels';
import { Input, Tooltip } from 'reactstrap';
import { toast } from 'react-toastify';
import ConfirmPopup from '../../common/confirm-popup/ConfirmPopup';
import CommonFunctions from '../../common/common-functions/CommonFunctions';
import { DateTimeMillModified } from '../../constants/constants'
import DatePicker from '../../common/DatePicker/index';
var dateFormat = require("dateformat");

export class EditJobInfo extends Component {
    constructor(props) {
        super(props);
        this.state = {
            id: props.jobInfo.id,
            jobStatus: props.jobInfo.jobStatus,
            previousMaxMessageId: props.jobInfo.previousMaxMessageId,
            batchSize: props.jobInfo.batchSize,
            testingGroup: props.jobInfo.testingGroup,
            confirmUpdateJobInfo: false,
            tooltipOpen: false,
            lastVoidingHistoryCollectionId: props.jobInfo.lastVoidingHistoryCollectionId,
            lastLogId: props.jobInfo.lastLogId,
            lastMsgTargetId: props.jobInfo.lastMsgTargetId,
            lastModifiedTime: props.jobInfo.lastModifiedTime
        };

        this.haveHostPermission = document.getElementById('have-host-permission') && document.getElementById('have-host-permission').checked;
    }

    isValueExceedMaxInt = (value) => {
        const maxInt = 2147483647;
        if (value) {
            return parseInt(value) >= maxInt;
        }
        return false;
    }

    handleChangeSaveMode = () => {
        this.toggleConfirmUpdateJobInfo();
        const groupIds = this.state.testingGroup.trim().split(',');
        const { previousMaxMessageId, lastVoidingHistoryCollectionId, lastLogId, lastMsgTargetId, lastModifiedTime } = this.state;
        const idsToCheck = [previousMaxMessageId, lastVoidingHistoryCollectionId, lastLogId, lastMsgTargetId, lastModifiedTime];

        let rexegNumber = new RegExp('[^0-9]');
        if (rexegNumber.test(this.state.batchSize)) {
            toast.error('Batch Size must be integer');
        } else if (groupIds.length === 0 || groupIds.filter(item => rexegNumber.test(item)).length > 0) {
            toast.error('Testing Group must be integer and separated by \',\' for multiple group.');
        } else if (idsToCheck.some(this.isValueExceedMaxInt)) {
            toast.error('Cannot save, new Previous Max ID exceeds the Integer datatype range!');
        } else {
            let data = {
                id: this.state.id,
                jobStatus: this.state.jobStatus,
                batchSize: parseInt(this.state.batchSize),
                testingGroup: this.state.testingGroup,
                previousMaxMessageId: previousMaxMessageId ? previousMaxMessageId : 0,
                lastVoidingHistoryCollectionId: lastVoidingHistoryCollectionId ? lastVoidingHistoryCollectionId : 0,
                lastLogId: lastLogId ? lastLogId : 0,
                lastMsgTargetId: lastMsgTargetId ? lastMsgTargetId : 0,
                lastModifiedTime: this.props.jobInfo.id === 9 ? dateFormat(lastModifiedTime, DateTimeMillModified) : ""
            };

            this.props.setJobInfoData(data, () => this.props.setJobInfoFlag({ isEdit: false }));
        }
    }

    handleChange = e => {
        this.setState({
            [e.target.name]: e.target.value
        });
    }

    handleConfirmUpdateJobInfoPopup = () => {
        this.setState({ confirmUpdateJobInfo: true });
    }

    toggleConfirmUpdateJobInfo = () => {
        this.setState({ confirmUpdateJobInfo: false });
        this.props.setJobInfoFlag({ isEdit: false });
    }

    handleCancel = () => this.props.setJobInfoFlag({ isEdit: false })

    toggleTooltip = () => {
        this.setState(prevState => {
            return {
                tooltipOpen: !prevState.tooltipOpen
            };
        });
    }

    handleDateChange = value => {
        this.setState({ lastModifiedTime: value });
    };

    render() {
        const jobId = this.props.jobInfo !== null ? this.props.jobInfo.id : 0;
        const previousMaxIDTitle = CommonFunctions.renderPrivousIdTitle(jobId);

        return (
            this.props.jobInfo && <div className='job-info-container'>
                <div className='job-info-item'>
                    <p className='job-info-title'>Job Sync Status:</p>
                    <Input
                        name='jobStatus'
                        onChange={this.handleChange}
                        className={`job-info-input`}
                        defaultValue={this.props.jobInfo.jobStatus}
                    />
                </div>
                <div className='job-info-item'>
                    <p className='job-info-title'>{previousMaxIDTitle}</p>
                    {
                        jobId === 9 ? (
                            <DatePicker selected={new Date(this.state.lastModifiedTime)}
                                showTimeSelect
                                onChange={value => this.handleDateChange(value)}
                                className='picker-modifier'
                            />
                        ) :
                        (
                            <Input
                                name={CommonFunctions.renderLabelMaxMessageId(jobId)}
                                onChange={this.handleChange}
                                className='job-info-input maxId-input'
                                defaultValue={CommonFunctions.renderMaxMessageId(jobId, this.props.jobInfo)}
                            />
                        )
                    }
                </div>
                {
                    jobId === 2 && (
                        <div className='job-info-item'>
                            <p className='job-info-title'>Previous Max MsgTargetID:</p>
                            <Input
                                name='lastMsgTargetId'
                                onChange={this.handleChange}
                                className='job-info-input maxId-input'
                                defaultValue={this.props.jobInfo.lastMsgTargetId}
                            />
                        </div>
                    )
                }
                <div className='job-info-item'>
                    <p className='job-info-title'>Batch Size:</p>
                    <Input
                        name='batchSize'
                        onChange={this.handleChange}
                        className='job-info-input'
                        defaultValue={this.props.jobInfo.batchSize}
                    />
                </div>
                <div className='job-info-item'>
                    <p className='job-info-title'>Testing Group:</p>
                    <Input
                        name='testingGroup' id="testingGroup"
                        onChange={this.handleChange}
                        className='job-info-input'
                        defaultValue={this.props.jobInfo.testingGroup}
                    />
                    <Tooltip placement="auto" isOpen={this.state.tooltipOpen} target="testingGroup" toggle={this.toggleTooltip}>
                        Separated by ',' for multiple group
                    </Tooltip>
                </div>
                {
                    this.haveHostPermission &&
                    <div className='job-info-item'>
                        <button className="btn-new btn-save-job-info" onClick={this.handleConfirmUpdateJobInfoPopup}>{Labels.Save}</button>
                        <button className="btn-cancel" onClick={this.handleCancel}>{Labels.Cancel}</button>
                    </div>
                }

                <ConfirmPopup
                    isOpen={this.state.confirmUpdateJobInfo}
                    onToggle={this.toggleConfirmUpdateJobInfo}
                    onConfirm={this.handleChangeSaveMode}
                    content='Do you want to save it?'
                />
            </div>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { jobInfo, setJobInfoFlag, setJobInfoData } = state;
    return {
        jobInfo,
        setJobInfoFlag,
        setJobInfoData
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(EditJobInfo);