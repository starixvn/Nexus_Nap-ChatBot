import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label, Input, FormFeedback } from 'reactstrap';
import { Button } from 'reactstrap';
import { NCBBreadcrum } from '../../common/breadcrumbs';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import './AppContainer.scss';
import Labels from '../../common/Labels';
import { SelectField } from '../../common/SelectField';

export class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isSummited: false
        };

        this.props.getNotificationTypes();
    }

    onChange = (fieldName, fieldValue) => this.setState({ [fieldName]: fieldValue, isChanged: true });    

    render() {
        if (!this.props.notificationTypes.length) {
            return null;
        }

        console.log(!this.props.notificationTypes);

        console.log(this.props.notificationTypes);
        return (
            <div>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="New Notification" />
                <div className="content form">
                    <Form>
                        <FormGroup>
                            <Label for="notificationType" className="form-label required">Notification Type</Label>
                            <SelectField 
                                data={this.props.notificationTypes}
                                isSummited={this.state.isSummited}
                                onChange={(notificationTypeId) => { this.onChange("notificationTypeId", notificationTypeId); }}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label">Product</Label>
                            <SelectField
                                data={this.props.notificationTypes}
                                isSummited={this.state.isSummited}
                                onChange={(notificationTypeId) => { this.onChange("notificationTypeId", notificationTypeId); }}
                            />
                        </FormGroup>

                        <Button className="btn-submit">Submit</Button>

                    </Form>
                </div>
            </div>
        );
    }
}

function mapDispatchToProps(dispatch) {
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state) {
    const { notificationTypes  } = state;
    return Object.assign({}, { notificationTypes });
}

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);