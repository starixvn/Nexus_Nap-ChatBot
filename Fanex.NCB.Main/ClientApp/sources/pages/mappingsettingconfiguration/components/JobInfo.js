import './AppContainer.scss';
import "react-mde/lib/styles/css/react-mde-all.css";

import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import Labels from '../../common/Labels';
import CommonFunctions from '../../common/common-functions/CommonFunctions';

export class JobInfo extends Component {
    constructor(props) {
        super(props);

        this.haveHostPermission = document.getElementById('have-host-permission') && document.getElementById('have-host-permission').checked;
    }

    handleChangeEditMode = () => {
        this.props.setJobInfoFlag({
            isEdit: true
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
                    <p className='job-info-value'>{this.props.jobInfo.batchSize}</p>
                </div>
                <div className='job-info-item'>
                    <p className='job-info-title'>Testing Group:</p>
                    <p className='job-info-value'>{this.props.jobInfo.testingGroup}</p>
                </div>
                {
                    this.haveHostPermission &&
                    <div className='job-info-item'>
                        <button className="btn-new" onClick={this.handleChangeEditMode}>{Labels.Edit}</button>
                    </div>
                }
            </div>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { jobInfo, jobInfoFlag, setJobInfoFlag } = state;
    return {
        jobInfo,
        jobInfoFlag,
        setJobInfoFlag
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(JobInfo);