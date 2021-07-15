import './AppContainer.scss';

import React, { Component } from 'react'
import { connect } from 'react-redux';
import { Form, FormGroup, Label, Input } from 'reactstrap';
import { NCBBreadcrum } from '../../common/breadcrumbs/breadcrumbs';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import DataGridSystemMessage from './DataGridSystemMessage';
import DataGridRecipientList from './DataGridRecipientList';
import DataGridConversationMessage from './DataGridConversationMessage';

export class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.haveHostPermission = document.getElementById('have-host-permission') ? document.getElementById('have-host-permission').checked : null;
        this.state = {
            notificationId: '',
            show:{
                showSystemMessage: true,
                showRecipientList: false,
                showConversation: false,
            }
        }
    }

    handleChangeNotificationId = (event) => {
        const regex = /^[0-9\b]+$/;
        if (event.target.value === '' || regex.test(event.target.value)) {
            this.setState({
                notificationId: event.target.value
            });
        }      
    }

    handleSubmit = (event) => {
        event.preventDefault();
        const params = { 
            notificationId: this.state.notificationId,
            pageIndex: 1,
            pageSize: 100
        }
        this.props.getParamsPage(params);
        this.props.getSystemMessage(params);
        this.props.getRecipientList(params);
        this.props.getConversationMessage(params);
    }
    
    ShowOption = (currentState) =>{
        var keyShow = Object.keys(this.state.show);
        this.setState({
            show: {
                [keyShow[0]]: false, [keyShow[1]]: false, [keyShow[2]]: false,
                [currentState]: true
            }
        });
    }

    render() {
        return (
            <>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="View Log" />
                <Form className="form-filter-new" onSubmit={this.handleSubmit}>
                    <div className="form-row group-form">
                        <Label className="label">Notification ID</Label>                       
                        <FormGroup>
                            <Input value={this.state.notificationId} type="text" onChange={this.handleChangeNotificationId}/>
                        </FormGroup>
                        <FormGroup>
                            <button id="submit-button" className="btn-submit" type="submit">Submit</button>
                        </FormGroup>
                    </div>
                </Form>

                <div className="group-choose">
                    <div className={this.state.show.showSystemMessage ? 'choose-option active' : 'choose-option'} onClick={() => this.ShowOption('showSystemMessage')}>Notification System Message</div>
                    <div className={this.state.show.showRecipientList ? 'choose-option active' : 'choose-option'} onClick={() => this.ShowOption('showRecipientList')}>Notification Recipient List</div>
                    <div className={this.state.show.showConversation ? 'choose-option active' : 'choose-option'} onClick={() => this.ShowOption('showConversation')}>Conversation Message Log</div>
                </div>

                {this.state.show.showSystemMessage && <DataGridSystemMessage notificationId={this.state.notificationId} haveHostPermission={this.haveHostPermission}/>}
                {this.state.show.showRecipientList && <DataGridRecipientList notificationId={this.state.notificationId} haveHostPermission={this.haveHostPermission}/>}
                {this.state.show.showConversation && <DataGridConversationMessage notificationId={this.state.notificationId} haveHostPermission={this.haveHostPermission}/>}
            </>
        )
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    return {
        paramsPage: state.paramsPage,
        systemMessage: state.systemMessage,
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer)
