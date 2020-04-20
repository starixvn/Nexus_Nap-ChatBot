import './AppContainer.scss';
import "react-mde/lib/styles/css/react-mde-all.css";

import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import Labels from '../../common/Labels';
import { Input } from 'reactstrap';

export class EditJobInfo extends Component {
    constructor(props) {
        super(props);
        this.state = {
            count: 10
        };

        this.haveHostPermission = document.getElementById('have-host-permission') && document.getElementById('have-host-permission').checked;
    }
    
    handleChange = e => {
        console.log(e.taget.value)
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
                    <Input
                        onChange={this.handleChange}
                        className='job-info-input'
                        value={this.props.jobInfo.batchSize}
                    />
                </div>
                <div className='job-info-item'>
                    <p className='job-info-title'>Testing Group:</p>
                    <Input
                        onChange={this.handleChange}
                        className='job-info-input'
                        value={this.props.jobInfo.testingGroup}
                    />
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

export default connect(mapStateToProps, mapDispatchToProps)(EditJobInfo);