import './AppContainer.scss';

import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label, Input, TabContent, TabPane,
    Nav, NavItem, NavLink
} from 'reactstrap';
import { NCBBreadcrum } from '../../common/breadcrumbs/breadcrumbs';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import DataGridSystemMessage from './DataGridSystemMessage';
import DataGridRecipientList from './DataGridRecipientList';
import DataGridConversationMessage from './DataGridConversationMessage';

export class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            notificationId: '',
            activeTab: '1'
        }
    }

    handleChangeNotificationId = (event) => {
        const regex = /^[0-9\b]+$/;
        if (event.target.value === '' || regex.test(event.target.value)) {
            this.setState({
                notificationId: event.target.value
            });
        }      
    };

    handleSubmit = (event) => {
        event.preventDefault();
        const params = { 
            notificationId: this.state.notificationId,
            pageIndex: 1,
            pageSize: 100
        }
        this.props.getParamsPageSystemMessage(params);
        this.props.getParamsPageRecipientList(params);
        this.props.getParamsPageConversationMessage(params);

        this.props.getNotificationSystemMessage(params);
        this.props.getNotificationRecipientList(params);
        this.props.getConversationMessageLog(params);
    };

    handleToggle = tab => {
        if (this.state.activeTab !== tab) {
            this.setState({
                activeTab: tab
            });
        }
    };

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

                <Nav tabs className="group-choose">
                    <NavItem>
                        <NavLink
                            className={this.state.activeTab === '1' ? 'choose-option active' : 'choose-option'}
                            onClick={() => this.handleToggle('1')}
                        >
                            Notification System Message
                        </NavLink>
                    </NavItem>
                    <NavItem>
                        <NavLink
                            className={this.state.activeTab === '2' ? 'choose-option active' : 'choose-option'}
                            onClick={() => this.handleToggle('2')}
                        >
                            Notification Recipient List
                        </NavLink>
                    </NavItem>
                    <NavItem>
                        <NavLink
                            className={this.state.activeTab === '3' ? 'choose-option active' : 'choose-option'}
                            onClick={() => this.handleToggle('3')}
                        >
                            Conversation Message Log
                        </NavLink>
                    </NavItem>
                </Nav>
                <TabContent activeTab={this.state.activeTab}>
                    <TabPane tabId="1">
                        <DataGridSystemMessage notificationId={this.state.notificationId} />
                    </TabPane>
                    <TabPane tabId="2">
                        <DataGridRecipientList notificationId={this.state.notificationId} />
                    </TabPane>
                    <TabPane tabId="3">
                        <DataGridConversationMessage notificationId={this.state.notificationId} />
                    </TabPane>
                </TabContent>
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    return {
        paramsPageSystemMessage: state.paramsPageSystemMessage,
        paramsPageRecipientList: state.paramsPageRecipientList,
        paramsPageConversationMessage: state.paramsPageConversationMessage,
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer)
