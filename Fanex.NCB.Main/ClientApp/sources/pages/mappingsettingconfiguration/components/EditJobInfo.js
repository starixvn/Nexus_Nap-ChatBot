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

export class EditJobInfo extends Component {
    constructor(props) {
        super(props);
        this.state = {
            id: props.jobInfo.id,
            status: props.jobInfo.status,
            previousMaxMessageId: props.jobInfo.previousMaxMessageId,
            batchSize: props.jobInfo.batchSize,
            testingGroup: props.jobInfo.testingGroup,
            confirmUpdateJobInfo: false,
            tooltipOpen: false
        };

        this.haveHostPermission = document.getElementById('have-host-permission') && document.getElementById('have-host-permission').checked;
    }

    handleChangeSaveMode = () => {
        this.toggleConfirmUpdateJobInfo();
        const groupIds = this.state.testingGroup.trim().split(',');
        let rexegNumber = new RegExp('[^0-9]');
        if (rexegNumber.test(this.state.batchSize)) {
            toast.error('Batch Size must be integer');
        } else if (groupIds.length === 0 || groupIds.filter(item => rexegNumber.test(item)).length > 0) {
            toast.error('Testing Group must be integer and separated by \',\' for multiple group.');
        } else {
            let data = {
                id: this.state.id,
                status: this.state.status,
                previousMaxMessageId: this.state.previousMaxMessageId,
                batchSize: parseInt(this.state.batchSize),
                testingGroup: this.state.testingGroup
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

    render() {
        const jobId = this.props.jobInfo !== null ? this.props.jobInfo.id : 0;
        const previousMaxIDTitle = CommonFunctions.renderPrivousIdTitle(jobId);
        return (
            this.props.jobInfo && <div className='job-info-container'>
                <div className='job-info-item'>
                    <p className='job-info-title'>Job Sync Status:</p>
                    <p className={`job-info-value ${this.props.jobInfo.status === 'Done' ? 'status-done' : 'status-running'}`}>{this.props.jobInfo.status}</p>
                </div>
                <div className='job-info-item'>
                    <p className='job-info-title'>{previousMaxIDTitle}</p>
                    <p className='job-info-value'>{this.props.jobInfo.previousMaxMessageId}</p>
                </div>
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