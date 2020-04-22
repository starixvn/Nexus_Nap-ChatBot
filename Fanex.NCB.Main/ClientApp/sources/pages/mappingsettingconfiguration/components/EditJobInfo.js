import './AppContainer.scss';
import "react-mde/lib/styles/css/react-mde-all.css";

import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import Labels from '../../common/Labels';
import { Input } from 'reactstrap';
import { toast } from 'react-toastify';

export class EditJobInfo extends Component {
    constructor(props) {
        super(props);
        this.state = {
            id: props.jobInfo.id,
            status: props.jobInfo.status,
            previousMaxMessageId: props.jobInfo.previousMaxMessageId,
            batchSize: props.jobInfo.batchSize,
            testingGroup: props.jobInfo.testingGroup
        };

        this.haveHostPermission = document.getElementById('have-host-permission') && document.getElementById('have-host-permission').checked;
    }

    handleChangeSaveMode = () => {
        let rexegNumber = new RegExp('[^0-9]');
        if (rexegNumber.test(this.state.batchSize)) {
            toast.error('Batch Size must be integer');
        }
        else if (rexegNumber.test(this.state.testingGroup)) {
            toast.error('Testing Group must be integer');
        }
        else {
            let data = {
                id: this.state.id,
                status: this.state.status,
                previousMaxMessageId: this.state.previousMaxMessageId,
                batchSize: parseInt(this.state.batchSize),
                testingGroup: parseInt(this.state.testingGroup)
            };

            this.props.setJobInfoData(data, () => this.props.setJobInfoFlag({ isEdit: false }));
        }
    }
    
    handleChange = e => {
        this.setState({
            [e.target.name]: e.target.value
        });
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
                        name='batchSize'
                        onChange={this.handleChange}
                        className='job-info-input'
                        defaultValue={this.props.jobInfo.batchSize}
                    />
                </div>
                <div className='job-info-item'>
                    <p className='job-info-title'>Testing Group:</p>
                    <Input
                        name='testingGroup'
                        onChange={this.handleChange}
                        className='job-info-input'
                        defaultValue={this.props.jobInfo.testingGroup}
                    />
                </div>
                {
                    this.haveHostPermission &&
                    <div className='job-info-item'>
                        <button className="btn-new" onClick={this.handleChangeSaveMode}>{Labels.Save}</button>
                    </div>
                }
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