import './AppContainer.scss';
import "react-mde/lib/styles/css/react-mde-all.css";

import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import Labels from '../../common/Labels';

export class JobInfo extends Component {
    constructor(props) {
        super(props);
        this.state = {
            count: 10
        };

        this.haveHostPermission = document.getElementById('have-host-permission') && document.getElementById('have-host-permission').checked;
    }

    componentDidMount() {
    }

    render() {
        return (
            this.props.jobInfo && <div className='job-info-container'>
                <div className='job-info-item'>
                    <p className='job-info-title'>Job Sync Status:</p>
                    <p>{this.props.jobInfo.status}</p>
                </div>
                <div className='job-info-item'>
                    <p className='job-info-title'>Previous Max MessageID:</p>
                    <p>{this.props.jobInfo.previousMaxMessageId}</p>
                </div>
                <div className='job-info-item'>
                    <p className='job-info-title'>Batch Size:</p>
                    <p>{this.props.jobInfo.batchSize}</p>
                </div>
                <div className='job-info-item'>
                    <p className='job-info-title'>Testing Group:</p>
                    <p>{this.props.jobInfo.testingGroup}</p>
                </div>
                {
                    this.haveHostPermission &&
                    <div className='job-info-item'>
                        <button className="btn-new" onClick={this.handleOpenCreateSettingPopup}>{Labels.Edit}</button>
                    </div>
                }
            </div>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { jobInfo, setJobInfo } = state;
    return {
        jobInfo,
        setJobInfo
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(JobInfo);