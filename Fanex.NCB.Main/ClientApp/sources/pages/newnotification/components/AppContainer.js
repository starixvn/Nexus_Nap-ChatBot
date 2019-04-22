import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label, Input, FormFeedback } from 'reactstrap';
import { Button } from 'reactstrap';
import { NCBBreadcrum } from '../../common/breadcrumbs';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import './AppContainer.scss';
import Labels from '../../common/Labels';

export class AppContainer extends Component {
    constructor(props) {
        super(props);
    }
    

    render() {
        return (
            <div>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="New Notification" />
                <div className="content form">
                    <Form>
                        <FormGroup>
                            <Label for="notificationType" className="form-label required">Notification Type</Label>
                            <Input type="select" name="select" id="notificationType" className="notification-type" invalid>
                                <option>---Please select---</option>
                                <option>UM Licensee 1</option>
                                <option>UM Licensee 2</option>
                            </Input>
                            <FormFeedback>This field is required</FormFeedback>
                        </FormGroup>

                        <FormGroup>
                            <Label for="recipient" className="form-label required">Recipient</Label>
                            <Input invalid id="recipient" />
                            <FormFeedback>This field is required</FormFeedback>
                        </FormGroup>

                        <div className="row-has-actions">
                            <FormGroup>
                                <Label for="notificationTemplate" className="form-label required">Notification Template</Label>
                                <Input type="select" name="select" id="notificationTemplate" invalid className="notification-template">
                                    <option>---Please select---</option>
                                    <option>Weekly UM 1</option>
                                    <option>Weekly UM 2</option>
                                </Input>
                                <FormFeedback>This field is required</FormFeedback>
                            </FormGroup>
                            <div className="action" onClick={this.contentEditConfirmPopupHandler}>
                                <div className="circle orange" >
                                    <span className="icon-edit" />
                                </div>

                                <span className="text">Edit Title & Content</span>
                            </div>
                            <div className="action" onClick={this.contentResetConfirmPopupHandler}>
                                <div className="circle blue">
                                    <span className="icon-reset" />
                                </div>

                                <span className="text">Reset Title & Content</span>
                            </div>
                        </div>

                        <FormGroup>
                            <Label for="title" className="form-label required">Title</Label>
                            <Input invalid id="title" />
                            <FormFeedback>This field is required</FormFeedback>
                        </FormGroup>

                        <FormGroup>
                            <Label for="content" className="form-label content required">Content</Label>
                            <Input type="textarea" invalid id="content" className="content" value={""} />
                            <FormFeedback>This field is required</FormFeedback>
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
    const {  } = state;
    return Object.assign({}, {  });
}

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);